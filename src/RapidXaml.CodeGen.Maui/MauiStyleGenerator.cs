using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using GeneratorCore;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace RapidXaml.CodeGen
{
	public class MauiStyleGenerator : Task
	{
		[Required]
		public ITaskItem[] InputFiles { get; set; } = [];

		[Required]
		public string GenerationNamespace { get; set; }

		[Required]
		public bool SupportResxGeneration { get; set; } = false;

		[Required]
		public ITaskItem[] ResxInputFiles { get; set; } = [];

		public override bool Execute()
		{
			try
			{
				List<string> GetFilesFromTaskItem(ITaskItem item, string filePattern)
				{
					List<string> result = [];

					if (item.ItemSpec.Contains("*"))
					{
						string fullFilePath = item.ItemSpec;
						string fileNamePattern = Path.GetFileName(fullFilePath);
						string sourceDirectory = fullFilePath.Replace(fileNamePattern, string.Empty);

						var searchOptions = SearchOption.TopDirectoryOnly;

						if (sourceDirectory.EndsWith("**\\"))
						{
							sourceDirectory = sourceDirectory.Substring(0, sourceDirectory.Length - 3);
							searchOptions = SearchOption.AllDirectories;
						}

						if (Directory.Exists(sourceDirectory))
						{
							if (string.IsNullOrEmpty(fileNamePattern))
							{
								fileNamePattern = filePattern;
							}

							var foundFiles = Directory.EnumerateFiles(sourceDirectory, fileNamePattern, searchOptions);

							result.AddRange(foundFiles);
						}
						else
						{
							Log.LogError($"{nameof(MauiStyleGenerator)}: Could not find directory: '{sourceDirectory}' ");
						}
					}
					else
					{
						result.Add(item.ItemSpec);
					}

					return result;
				}

				foreach (var inputFileItem in InputFiles)
				{
					//Log.LogMessage(MessageImportance.High, $"InputFile: {inputFileItem}::{inputFileItem.ItemSpec} ");

					List<string> inputFilePaths = GetFilesFromTaskItem(inputFileItem, "*.xaml");

					if (inputFilePaths.Count > 0)
					{
						foreach (var inputPath in inputFilePaths)
						{
							var inputFile = new FileInfo(inputPath);

							if (inputFile.FullName.EndsWith(".xaml", StringComparison.InvariantCultureIgnoreCase))
							{
								Log.LogMessage(MessageImportance.Normal, $"Generating types for {inputFile.FullName}");

								var outputFileName = inputFile.FullName.Substring(0, inputFile.FullName.Length - 5) + ".cs";

								var inputFileContents = File.ReadAllText(inputFile.FullName);

								// TODO: get the version from the assembly
								var generator = new MauiGeneratorLogic("RapidXaml.CodeGen.Maui.MauiStyleGenerator", version: "0.4.0.0");

								var generated = generator.GenerateCode(inputFile.Name, inputFileContents, GenerationNamespace, SupportResxGeneration);

								File.WriteAllBytes(outputFileName, generated);
							}
							else
							{
								Log.LogWarning($"{nameof(MauiStyleGenerator)}: Skipping generation of {inputFile.FullName}");
							}
						}
					}
					else
					{
						Log.LogError($"{nameof(MauiStyleGenerator)}: No files found in '{inputFileItem.ItemSpec}' ");
					}
				}

				if (SupportResxGeneration && ResxInputFiles.Length > 0)
				{
					List<string> resxFilesOfInterest = [];

					foreach (var inputResxItem in ResxInputFiles)
					{
						List<string> resxFiles = GetFilesFromTaskItem(inputResxItem, "*.resx");

						foreach (var resxFile in resxFiles)
						{
							if (!Path.GetFileNameWithoutExtension(resxFile).Contains("."))
							{
								// Neutral language resources, not locale specific ones
								resxFilesOfInterest.Add(resxFile);
							}
						}
					}

					Dictionary<string, Dictionary<string, List<SupportedProperty>>> resourcesOfInterest = [];

					const string separator = "_._";

					// TODO: ?? CHeck that there are some resxFilesOfInterest

					foreach (var resourceFile in resxFilesOfInterest)
					{
						var doc = new XmlDocument();
						doc.Load(resourceFile);

						foreach (XmlElement element in doc.GetElementsByTagName("data"))
						{
							if (element.GetAttribute("name").Contains(separator))
							{
								if (!resourcesOfInterest.ContainsKey(resourceFile))
								{
									resourcesOfInterest[resourceFile] = [];
								}
								var parts = element.GetAttribute("name").Split([separator], StringSplitOptions.RemoveEmptyEntries);

								if (parts.Length == 2)
								{
									var resourceId = parts[0];

									if (!resourcesOfInterest[resourceFile].ContainsKey(resourceId))
									{
										resourcesOfInterest[resourceFile].Add(resourceId, []);
									}

									var propertyType = GetSupportedPropertyType(parts[1]);

									if (propertyType != SupportedProperty.Unknown)
									{
										resourcesOfInterest[resourceFile][resourceId].Add(propertyType);
									}
									else
									{
										// TODO: Log unknown/unexpected value
									}
								}
								else
								{
									// TODO: Log unknown/unexpected format
								}
							}
						}
					}

					// TODO: Create ResourceId enum
					var resIdEnum = GenerateResourceIdsEnum(resourcesOfInterest);
					// TODO: work out where to save this
					//File.WriteAllBytes(outputFileName, Encoding.UTF8.GetBytes(resIdEnum));

					// TODO: Create GeneratedResourceLoader class
					var resLoaderClass = GenerateResourceLoader(resourcesOfInterest);
					// TODO: work out where to save this
					//File.WriteAllBytes(outputFileName, Encoding.UTF8.GetBytes(resLoaderClass));
				}

				return true;
			}
			catch (Exception ex)
			{
				Log.LogErrorFromException(ex, showStackTrace: true);
				return false;
			}
		}

		private SupportedProperty GetSupportedPropertyType(string formattedPropertyName)
			=> formattedPropertyName switch
			{
				"Content" => SupportedProperty.Content,
				"Placeholder" => SupportedProperty.Placeholder,
				"Text" => SupportedProperty.Text,
				"Title" => SupportedProperty.Title,
				"ToolTipProperties.Text" => SupportedProperty.ToolTipText,
				"SemanticProperties.Description" => SupportedProperty.SemanticDescription,
				"SemanticProperties.Hint" => SupportedProperty.SemanticHint,
				"AutomationProperties.Name" => SupportedProperty.AutomationName,
				"AutomationProperties.HelpText" => SupportedProperty.AutomationHelpText,
				_ => SupportedProperty.Unknown,
			};

		public string GenerateResourceIdsEnum(Dictionary<string, Dictionary<string, List<SupportedProperty>>> allResourcesOfInterest)
		{
			List<string> uniqueResources = [];

			foreach (var item in allResourcesOfInterest)
			{
				uniqueResources.AddRange(item.Value.Keys);
			}

			uniqueResources = (List<string>)uniqueResources.Distinct();

			StringBuilder sb = new();

			sb.AppendLine($"namespace {GenerationNamespace};");
			sb.AppendLine();
			sb.AppendLine("public enum ResourceId");
			sb.AppendLine("{");

			foreach (var res in uniqueResources)
			{
				sb.AppendLine($"    {res},");
			}

			sb.AppendLine("}");

			return sb.ToString();
		}

		public string GenerateResourceLoader(Dictionary<string, Dictionary<string, List<SupportedProperty>>> allResourcesOfInterest)
		{
			StringBuilder sb = new();

			sb.AppendLine($"namespace {GenerationNamespace};");
			sb.AppendLine();
			sb.AppendLine("public static class GeneratedResourceLoader");
			sb.AppendLine("{");
			sb.AppendLine("    public static void LoadResources(this StyleableElement element, ResourceId resId)");
			sb.AppendLine("    {");
			sb.AppendLine("        switch (resId)");
			sb.AppendLine("        {");

			// TODO: Review how this handles multiple resx files containing duplicate resource names
			foreach (var fileItem in allResourcesOfInterest)
			{
				var fileName = Path.GetFileNameWithoutExtension(fileItem.Key);

				foreach (var res in fileItem.Value)
				{
					sb.AppendLine($"            case ResourceId.{res.Key}:");

					foreach (var resProp in res.Value)
					{
						switch (resProp)
						{
							case SupportedProperty.Content:
								sb.AppendLine($"                if (element is RadioButton {res.Key}AsRadioButton)");
								sb.AppendLine($"                {{");
								sb.AppendLine($"                    {res.Key}AsRadioButton.Content = {fileName}.{res.Key}___Content;");
								sb.AppendLine($"                }}");
								break;
							case SupportedProperty.Placeholder:
								sb.AppendLine($"                if (element is InputView {res.Key}AsInputViewPh)");
								sb.AppendLine($"                {{");
								sb.AppendLine($"                    {res.Key}AsInputViewPh.Placeholder = {fileName}.{res.Key}___Placeholder;");
								sb.AppendLine($"                }}");
								break;
							case SupportedProperty.Text:
								sb.AppendLine($"                if (element is InputView {res.Key}AsInputViewTxt)");
								sb.AppendLine($"                {{");
								sb.AppendLine($"                    {res.Key}AsInputViewTxt.Text = {fileName}.{res.Key}___Text;");
								sb.AppendLine($"                }}");
								sb.AppendLine($"                if (element is MenuItem {res.Key}AsMenuItem)");
								sb.AppendLine($"                {{");
								sb.AppendLine($"                    {res.Key}AsMenuItem.Text = {fileName}.{res.Key}___Text;");
								sb.AppendLine($"                }}");
								sb.AppendLine($"                if (element is MenuBarItem {res.Key}AsMenuBarItem)");
								sb.AppendLine($"                {{");
								sb.AppendLine($"                    {res.Key}AsMenuBarItem.Text = {fileName}.{res.Key}___Text;");
								sb.AppendLine($"                }}");
								break;
							case SupportedProperty.Title:
								sb.AppendLine($"                if (element is Picker {res.Key}AsPicker)");
								sb.AppendLine($"                {{");
								sb.AppendLine($"                    {res.Key}AsPicker.Title = {fileName}.{res.Key}___Title;");
								sb.AppendLine($"                }}");
								break;
							case SupportedProperty.ToolTipText:
								sb.AppendLine($"                ToolTipProperties.SetText(element, {fileName}.{res.Key}___ToolTipProperties_Text);");
								break;
							case SupportedProperty.SemanticDescription:
								sb.AppendLine($"                SemanticProperties.SetDescription(element, {fileName}.{res.Key}___SemanticProperties_Description);");
								break;
							case SupportedProperty.SemanticHint:
								sb.AppendLine($"                SemanticProperties.SetHint(element, {fileName}.{res.Key}___SemanticProperties_Hint);");
								break;
							case SupportedProperty.AutomationName:
								sb.AppendLine($"                AutomationProperties.SetName(element, {fileName}.{res.Key}___AutomationProperties_Name);");
								break;
							case SupportedProperty.AutomationHelpText:
								sb.AppendLine($"                AutomationProperties.SetHelpText(element, {fileName}.{res.Key}___AutomationProperties_HelpText);");
								break;
							case SupportedProperty.Unknown:
							default:
								break;
						}
					}

					sb.AppendLine($"            break;");
				}
			}

			sb.AppendLine("            default:");
			sb.AppendLine("                break;");
			sb.AppendLine("        }");
			sb.AppendLine("    }");
			sb.AppendLine("}");

			return sb.ToString();
		}
	}
}
