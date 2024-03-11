using System.Text;
using GeneratorCore;

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

                <x:Int16 x:Key="anint16">10</x:Int16>
                <x:Int32 x:Key="anint32">10</x:Int32>
                <x:Int64 x:Key="anint64">10</x:Int64>
                <x:Decimal x:Key="someDecimal">10.2</x:Decimal>

                <Thickness x:Key="StandardItemPadding1">10</Thickness>
                <Thickness x:Key="StandardItemPadding2">20,30</Thickness>
                <Thickness x:Key="StandardItemPadding4">40, 50,60, 70</Thickness>
            

            </ResourceDictionary>
            """;

        var bytes = mauiGenerator.GenerateCode("ignore.xaml", testXaml, "RapidXaml.Testing");

        var stringResult = Encoding.UTF8.GetString(bytes);

        Assert.IsFalse(stringResult.StartsWith("#error"));
        Assert.IsTrue(stringResult.Contains("InternalSpacing"));
        Assert.IsTrue(stringResult.Contains("anint16"));
        Assert.IsTrue(stringResult.Contains("anint32"));
        Assert.IsTrue(stringResult.Contains("anint64"));
        Assert.IsTrue(stringResult.Contains("StandardItemPadding1"));
        Assert.IsTrue(stringResult.Contains("StandardItemPadding2"));
        Assert.IsTrue(stringResult.Contains("StandardItemPadding4"));
    }
}


