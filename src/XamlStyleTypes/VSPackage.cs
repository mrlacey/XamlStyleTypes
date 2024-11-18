using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Community.VisualStudio.Toolkit;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
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

			await TrackBasicUsageAnalyticsAsync();
		}

		private static async Task TrackBasicUsageAnalyticsAsync()
		{
			try
			{
#if !DEBUG
				if (string.IsNullOrWhiteSpace(AnalyticsConfig.TelemetryConnectionString))
				{
					return;
				}

				var config = new TelemetryConfiguration
				{
					ConnectionString = AnalyticsConfig.TelemetryConnectionString,
				};

				var client = new TelemetryClient(config);

				var properties = new Dictionary<string, string>
				{
					{ "VsixVersion", Vsix.Version },
					{ "VsVersion", Microsoft.VisualStudio.Telemetry.TelemetryService.DefaultSession?.GetSharedProperty("VS.Core.ExeVersion") },
				};

				client.TrackEvent(Vsix.Name, properties);
#endif
			}
			catch (Exception exc)
			{
				System.Diagnostics.Debug.WriteLine(exc);
				await OutputPane.Instance.WriteAsync("Error tracking usage analytics: " + exc.Message);
			}
		}
	}
}
