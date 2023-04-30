using System;
using System.Collections.Generic;
using System.IO;
using GeneratorCore;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace RapidXaml.Tasks
{
    public class MauiStyleGenerator : Task
    {
        [Required]
        public ITaskItem[] InputFiles { get; set; } = Array.Empty<ITaskItem>();

        [Required]
        public string GenerationNamespace { get; set; }

        // TODO: Support overriding namespace
        // TODO: support specifying types to ignore
        // TODO: support specifying additional namespaces to include

        public override bool Execute()
        {
            try
            {
                foreach (var inputFileItem in InputFiles)
                {
                    //Log.LogError(inputFileItem.ToString());
                    //Log.LogError(inputFileItem.ItemSpec);
                    //Log.LogMessage(MessageImportance.High, $"InputFile: {inputFileItem}::{inputFileItem.ItemSpec} ");

                    List<string> inputFilePaths = new();

                    if (inputFileItem.ItemSpec.Contains("*"))
                    {
                        string fullFilePath = inputFileItem.ItemSpec;
                        string fileNamePattern = Path.GetFileName(fullFilePath);
                        string sourceDirectory = fullFilePath.Replace(fileNamePattern, string.Empty);

                        var foundFiles = Directory.EnumerateFiles(sourceDirectory, fileNamePattern);

                        inputFilePaths.AddRange(foundFiles);
                    }
                    else
                    {
                        inputFilePaths.Add(inputFileItem.ItemSpec);
                    }

                    foreach (var inputPath in inputFilePaths)
                    {
                        var inputFile = new FileInfo(inputPath);

                        if (inputFile.FullName.EndsWith(".xaml", StringComparison.InvariantCultureIgnoreCase))
                        {
                            Log.LogMessage(MessageImportance.Normal, $"Generating types for {inputFile.FullName}");

                            var outputFileName = inputFile.FullName.Substring(0, inputFile.FullName.Length - 5) + ".cs";

                            var inputFileContents = File.ReadAllText(inputFile.FullName);

                            // TODO: review adding "Task" to the name here
                            var generator = new MauiGeneratorLogic(nameof(MauiStyleGenerator));

                            var generated = generator.GenerateCode(inputFile.Name, inputFileContents, GenerationNamespace);

                            File.WriteAllBytes(outputFileName, generated);
                        }
                        else
                        {
                            Log.LogWarning($"Skipping generation of {inputFile.FullName}");
                        }
                    }
                }

                return true;

            }
            catch (Exception ex)
            {
                // This logging helper method is designed to capture and display information
                // from arbitrary exceptions in a standard way.
                Log.LogErrorFromException(ex, showStackTrace: true);
                return false;
            }
        }
    }
}
