using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Community.VisualStudio.Toolkit;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TextTemplating.VSHost;

namespace XamlStyleTypes
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]   
    [InstalledProductRegistration(Vsix.Name, Vsix.Description, Vsix.Version)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(PackageGuids.XamlStyleTypesString)]

    [ProvideCodeGenerator(typeof(MauiStyleGenerator), MauiStyleGenerator.Name, MauiStyleGenerator.Description, true, RegisterCodeBase = true)]
    [ProvideCodeGeneratorExtension(MauiStyleGenerator.Name, ".xaml")]

    [ProvideUIContextRule(PackageGuids.CommandVisisiblityString,
     name: "XAML files",
     expression: "IsXaml",
     termNames: new[] { "IsXaml" },
     termValues: new[] { "HierSingleSelectionName:.xaml$" })]
    public sealed class VSPackage : ToolkitPackage
    {
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await this.RegisterCommandsAsync();
        }
    }
}
