using RapidXaml.CodeGen;

// TODO: Remove the central test project and replace with platform specific ones
namespace RapidXaml.GeneratorCore.Tests;

public partial class DummyLogger : ITaskLogWrapper
{
	public void LogError(string message)
	{
		// NOOP
	}

	public void LogErrorFromException(Exception ex, bool showStackTrace)
	{
		// NOOP
	}

	public void LogImportantMessage(string message)
	{
		// NOOP
	}

	public void LogNormalMessage(string message)
	{
		// NOOP
	}

	public void LogWarning(string message)
	{
		// NOOP
	}
}
