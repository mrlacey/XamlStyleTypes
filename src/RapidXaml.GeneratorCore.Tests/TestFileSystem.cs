using System.Text;
using RapidXaml.CodeGen;

namespace RapidXaml.GeneratorCore.Tests;

public class TestFileSystem : IFileSystem
{
	public Dictionary<string, bool> ExistingDirectories { get; set; } = [];

	public Dictionary<string, List<string>> EnumerableFiles { get; set; } = [];

	public Dictionary<string, string> FileContents { get; set; } = [];

	public Dictionary<string, string> WrittenFiles { get; set; } = [];

	public IEnumerable<string> DirectoryEnumerateFiles(string path, string searchPattern, SearchOption searchOption)
	{
		return EnumerableFiles[path];
	}

	public bool DirectoryExists(string path)
	{
		return ExistingDirectories.ContainsKey(path);
	}

	public string FileReadAllText(string path)
	{
		if (FileContents.TryGetValue(path, out string? value))
		{
			return value;
		}

		foreach (var item in FileContents)
		{
			if (path.EndsWith(item.Key))
			{
				return item.Value;
			}
		}

		return string.Empty;
	}

	public void FileWriteAllBytes(string path, byte[] bytes)
	{
		WrittenFiles.Add(path, Encoding.UTF8.GetString(bytes));
	}

	public string GetFileName(string path)
		=> path;

	public string GetFilePath(string path)
		=> path;

	public string PathChangeExtension(string path, string extension)
	{
		return Path.ChangeExtension(path, extension);
	}

	public string PathGetFileName(string path)
	{
		return Path.GetFileName(path);
	}

	public string PathGetFileNameWithoutExtension(string path)
	{
		return Path.GetFileNameWithoutExtension(path);
	}
}
