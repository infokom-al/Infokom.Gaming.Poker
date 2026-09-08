using System;
using System.IO;

namespace BinarySerializer;

internal static class Files
{
	public static FileStream OpenFileOverwrite(this string file)
	{
		file.CreateParentDirectoryIfAbsent().CreateEmptyFileOnMacOsIfNeeded();
		return File.Open(file, FileMode.Create);
	}

	public static string CreateParentDirectoryIfAbsent(this string path)
	{
		path.GetParentDirectory().CreateDirectoryIfAbsent();
		return path;
	}

	public static string GetParentDirectory(this string path)
	{
		return Directory.GetParent(path)?.FullName ?? throw new InvalidOperationException("No parent directory for path " + path);
	}

	public static void CreateEmptyFileOnMacOsIfNeeded(this string file)
	{
		if (!OperatingSystem.IsWindows() && !file.FileExists())
		{
			file.CreateEmptyFile();
		}
	}

	public static bool FileExists(this string path)
	{
		return File.Exists(path);
	}

	public static string CreateEmptyFile(this string file)
	{
		if (!OperatingSystem.IsWindows())
		{
			Path.GetDirectoryName(file)?.CreateDirectoryIfAbsent();
		}
		using (File.Create(file))
		{
			return file;
		}
	}

	public static string CreateDirectoryIfAbsent(this string directory)
	{
		if (!directory.DirectoryExists())
		{
			Directory.CreateDirectory(directory);
		}
		return directory;
	}

	public static bool DirectoryExists(this string directory)
	{
		return Directory.Exists(directory);
	}

	public static string VerifyFileExists(this string file)
	{
		if (!file.FileExists())
		{
			throw new FileNotFoundException("File " + file.Quoted() + " not found");
		}
		return file;
	}
}
