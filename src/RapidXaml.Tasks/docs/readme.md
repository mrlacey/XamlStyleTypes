# RapidXaml.Tasks

MSBuild tasks from the Rapid XAML Toolkit.

Included in this version:

## MauiStyleGenerator

This task is used to generate classes from a `ResourceDictionary` of styles in a .NET MAUI app.

To use:

1. Add a reference to the NuGet package `RapidXaml.Tasks`.

e.g. (in your .csproj file)
```xml
<ItemGroup>
	<PackageReference Include="RapidXaml.Tasks" Version="0.0.6-prerelease" />
</ItemGroup>
```

**That's it!**

By default, it will look for ResourceDictionaries in `.xaml` files in the `Resources` folder of your project and generate appropriate C# files.

## Configuration

There are two ways to configure what is generated.

You can use  one or both of these.

### Per file Configuration

You can do [per file customizations in the same way as the VSIX extension](https://github.com/mrlacey/XamlStyleTypes#configuration).

### Per project Configuration

Project level configuration is done by specifying additional MSBuild Properties.

#### Configure files to include

Do this by specifying **XamlStyleInputFiles**

You can specify an **exact directory**, a **directory and all sub-directories**, or a **specific file**.

You can also specify multiple values by spearating with a semicolon '`;`'.

```xml
<PropertyGroup>
	<!-- A specific directory -->
	<XamlStyleInputFiles>Resources\Styles\*.xaml</XamlStyleInputFiles>
	<!-- A directory and any intermediary sub-directories -->
	<XamlStyleInputFiles>Resources\**\*.xaml</XamlStyleInputFiles>
	<!-- Two specific files -->
	<XamlStyleInputFiles>Resources\Styles\Colors.xaml;Resources\Styles\Styles.xaml</XamlStyleInputFiles>
</PropertyGroup>
```

#### Configure a different default namespace

Do this by specifying **XamlStyleGenerationNamespace**

```xml
<PropertyGroup>
	<XamlStyleGenerationNamespace>MyCoolApp.Resources.Generated</XamlStyleGenerationNamespace>
</PropertyGroup
```

Any per file configuration will override this value. If not specified, the default will be the `RootNamespace` of the project.
