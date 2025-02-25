using System.Net.Http.Headers;
using RapidXaml.CodeGen;

// TODO: Remove the central test project and replace with platform specific ones
namespace RapidXaml.GeneratorCore.Tests;

[TestClass]
public class MauiStyleGeneratorTests
{
	[TestMethod]
	public void CanCreateForTesting_WithDefaultFileSystem()
	{
		var mauiGenerator = new MauiExecutionLogic(new DefaultFileSystem(), new DummyLogger());

		Assert.IsNotNull(mauiGenerator);

		//var testXaml = """
		//          <?xml version="1.0" encoding="utf-8" ?>
		//          <ResourceDictionary xmlns="http://schemas.microsoft.com/dotnet/2021/maui" xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml">

		//              <Style TargetType="Label" x:Key="PageTitle">
		//                  <Setter Property="FontAttributes" Value="Bold" />
		//                  <Setter Property="FontSize" Value="30" />
		//              </Style>

		//          </ResourceDictionary>
		//          """;

		//var bytes = mauiGenerator.GenerateCode("ignore.xaml", testXaml, "RapidXaml.Testing", includeResourceLoading: true);

		//var stringResult = Encoding.UTF8.GetString(bytes);

		//Assert.IsFalse(stringResult.StartsWith("#error"));
		//Assert.IsTrue(stringResult.Contains("public class PageTitle : Label"));
		//Assert.IsTrue(stringResult.Contains("public PageTitle(ResourceId resourceId) : this()"));
	}

	[TestMethod]
	public void CanCreateForTesting_WithTestFileSystem()
	{
		var mauiGenerator = new MauiExecutionLogic(new TestFileSystem(), new DummyLogger());

		Assert.IsNotNull(mauiGenerator);
	}

	[TestMethod]
	public void CreateOutputFromSingleResourceDictionary()
	{
		var testFs = new TestFileSystem();

		var testFileName = "TestRd.xaml";
		var testFileContents = """
            <?xml version="1.0" encoding="utf-8" ?>
            <ResourceDictionary xmlns="http://schemas.microsoft.com/dotnet/2021/maui" xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml">

                <Style TargetType="Label" x:Key="PageTitle">
                    <Setter Property="FontAttributes" Value="Bold" />
                    <Setter Property="FontSize" Value="30" />
                </Style>

            </ResourceDictionary>
            """;

		testFs.FileContents.Add(testFileName, testFileContents);

		var mauiGenerator = new MauiExecutionLogic(testFs, new DummyLogger());

		var executeResult = mauiGenerator.Execute([testFileName], "My.Test.Namespace", supportResxGeneration: false, resxInputFiles: []);

		Assert.IsTrue(executeResult);
		Assert.AreEqual(1, testFs.WrittenFiles.Count);

		Assert.IsTrue(testFs.WrittenFiles.First().Key.EndsWith("TestRd.cs"));

		var writtenFileContents = testFs.WrittenFiles.First().Value;

		Assert.IsFalse(writtenFileContents.StartsWith("#error"));
		Assert.IsTrue(writtenFileContents.Contains("public class PageTitle : Label"));
	}

	[TestMethod]
	public void CreateOutputFromSingleResourceDictionary_WithResxGeneration_ButNoResxFile()
	{
		var testFs = new TestFileSystem();

		var testFileName = "TestRd.xaml";
		var testFileContents = """
            <?xml version="1.0" encoding="utf-8" ?>
            <ResourceDictionary xmlns="http://schemas.microsoft.com/dotnet/2021/maui" xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml">

                <Style TargetType="Label" x:Key="PageTitle2">
                    <Setter Property="FontAttributes" Value="Bold" />
                    <Setter Property="FontSize" Value="30" />
                </Style>

            </ResourceDictionary>
            """;

		testFs.FileContents.Add(testFileName, testFileContents);

		var mauiGenerator = new MauiExecutionLogic(testFs, new DummyLogger());

		var executeResult = mauiGenerator.Execute([testFileName], "My.Test.Namespace", supportResxGeneration: true, resxInputFiles: []);

		Assert.IsTrue(executeResult);
		Assert.AreEqual(1, testFs.WrittenFiles.Count);

		Assert.IsTrue(testFs.WrittenFiles.First().Key.EndsWith("TestRd.cs"));

		var writtenFileContents = testFs.WrittenFiles.First().Value;

		Assert.IsFalse(writtenFileContents.StartsWith("#error"));
		Assert.IsTrue(writtenFileContents.Contains("public class PageTitle2 : Label"));
		// Note the below shouldn't be included as there were no known resx file paths provided
		Assert.IsFalse(writtenFileContents.Contains("public PageTitle2(ResourceId resourceId) : this()"));
	}

	[TestMethod]
	public void CreateOutputFromSingleResourceDictionary_WithResxGeneration_AndOneResxFile()
	{
		var testFs = new TestFileSystem();

		var testXamlFileName = "TestRd2.xaml";
		var testXamlFileContents = """
            <?xml version="1.0" encoding="utf-8" ?>
            <ResourceDictionary xmlns="http://schemas.microsoft.com/dotnet/2021/maui" xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml">

                <Style TargetType="Label" x:Key="PageTitle3">
                    <Setter Property="FontAttributes" Value="Bold" />
                    <Setter Property="FontSize" Value="30" />
                </Style>

            </ResourceDictionary>
            """;

		var testResxFileName = "TestResx.resx";
		var testResxFileContents = """
            <?xml version="1.0" encoding="utf-8" ?>
            <root>
              <data name="PageTitle3_._Text" xml:space="preserve">
                <value>This is the (potentially localized) page title</value>
              </data>
              <data name="PageTitle3_._ToolTipProperties.Text" xml:space="preserve">
                <value>This is the tooltip for the PageTitle</value>
              </data>
            </root>
            """;

		testFs.FileContents.Add(testXamlFileName, testXamlFileContents);
		testFs.FileContents.Add(testResxFileName, testResxFileContents);

		var mauiGenerator = new MauiExecutionLogic(testFs, new DummyLogger());

		var executeResult = mauiGenerator.Execute([testXamlFileName], "My.Test.Namespace", supportResxGeneration: true, resxInputFiles: [testResxFileName]);

		Assert.IsTrue(executeResult);
		Assert.AreEqual(2, testFs.WrittenFiles.Count);

		Assert.IsTrue(testFs.WrittenFiles.First().Key.EndsWith("TestRd2.cs"));

		var writtenFileContents = testFs.WrittenFiles.First().Value;

		Assert.IsFalse(writtenFileContents.StartsWith("#error"));
		Assert.IsTrue(writtenFileContents.Contains("public class PageTitle3 : Label"));
		Assert.IsTrue(writtenFileContents.Contains("public PageTitle3(ResourceId resourceId) : this()"));

		writtenFileContents = testFs.WrittenFiles.Last().Value;

		Assert.IsTrue(writtenFileContents.Contains("public enum ResourceId"));
		Assert.IsTrue(writtenFileContents.Contains("PageTitle3,"));
		Assert.IsTrue(writtenFileContents.Contains("public static class GeneratedResourceLoader"));
		Assert.IsTrue(writtenFileContents.Contains("case ResourceId.PageTitle3:"));
		Assert.IsTrue(writtenFileContents.Contains("if (element is Label PageTitle3AsLabelTxt)"));
		Assert.IsTrue(writtenFileContents.Contains("ToolTipProperties.SetText(element, "));
	}
}
