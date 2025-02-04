using System.Text;
using GeneratorCore;

// TODO: Remove the central test project and replace with platform specific ones
namespace RapidXaml.GeneratorCore.Tests;

[TestClass]
public class BasicManualVerificationChecks
{
	[TestMethod]
	public void SimpleSizesAreAllInOutput()
	{
		var mauiGenerator = new MauiGeneratorLogic(nameof(SimpleSizesAreAllInOutput));

		var testXaml = """
            <?xml version="1.0" encoding="utf-8" ?>
            <ResourceDictionary xmlns="http://schemas.microsoft.com/dotnet/2021/maui" xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml">

                <x:Double x:Key="LargeSquareImageSize">160</x:Double>

                <x:Double x:Key="SmallSquareImageSize">125</x:Double>

                <x:Double x:Key="InternalSpacing">10</x:Double>

                <x:Int16 x:Key="anInt16">10</x:Int16>
                <x:Int32 x:Key="anInt32">10</x:Int32>
                <x:Int64 x:Key="anInt64">10</x:Int64>
                <x:Decimal x:Key="someDecimal">10.2</x:Decimal>

                <Thickness x:Key="StandardItemPadding1">10</Thickness>
                <Thickness x:Key="StandardItemPadding2">20,30</Thickness>
                <Thickness x:Key="StandardItemPadding4">40, 50,60, 70</Thickness>
            

            </ResourceDictionary>
            """;

		var bytes = mauiGenerator.GenerateCode("ignore.xaml", testXaml, "RapidXaml.Testing", includeResourceLoading: false);

		var stringResult = Encoding.UTF8.GetString(bytes);

		Assert.IsFalse(stringResult.StartsWith("#error"));
		Assert.IsTrue(stringResult.Contains("InternalSpacing"));
		Assert.IsTrue(stringResult.Contains("anInt16"));
		Assert.IsTrue(stringResult.Contains("anInt32"));
		Assert.IsTrue(stringResult.Contains("anInt64"));
		Assert.IsTrue(stringResult.Contains("StandardItemPadding1"));
		Assert.IsTrue(stringResult.Contains("StandardItemPadding2"));
		Assert.IsTrue(stringResult.Contains("StandardItemPadding4"));
	}
}


