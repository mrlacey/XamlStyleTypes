using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace XamlStyleTypes
{
	public class OutputPane
	{
		private static Guid xstPaneGuid = new Guid("14EF4121-68BF-4BC7-AE29-D95FA115241F");

		private static OutputPane instance;

		private readonly IVsOutputWindowPane pane;

		private OutputPane()
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			if (ServiceProvider.GlobalProvider.GetService(typeof(SVsOutputWindow)) is IVsOutputWindow outWindow
			 && (ErrorHandler.Failed(outWindow.GetPane(ref xstPaneGuid, out pane)) || pane == null))
			{
				if (ErrorHandler.Failed(outWindow.CreatePane(ref xstPaneGuid, Vsix.Name, 1, 0)))
				{
					System.Diagnostics.Debug.WriteLine("Failed to create the Output window pane.");
					return;
				}

				if (ErrorHandler.Failed(outWindow.GetPane(ref xstPaneGuid, out pane)) || (pane == null))
				{
					System.Diagnostics.Debug.WriteLine("Failed to get access to the Output window pane.");
				}
			}
		}

		public static OutputPane Instance => instance ??= new OutputPane();

		public async Task ActivateAsync()
		{
			await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(CancellationToken.None);

			pane?.Activate();
		}

		public async Task WriteAsync(string message)
		{
			await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(CancellationToken.None);

			_ = (pane?.OutputStringThreadSafe($"{message}{Environment.NewLine}"));
		}
	}
}
