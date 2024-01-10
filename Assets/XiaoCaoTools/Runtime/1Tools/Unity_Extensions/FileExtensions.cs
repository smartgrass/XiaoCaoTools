using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace GG.Extensions
{
    public static class FileExtensions
    {
        public static bool IsFileInUse(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                stream?.Close();
            }
            return false; 
        }

        /// <summary>
        /// Copy a whole directory with option to copy all sub folders
        /// </summary>
        /// <param name="sourceDirName"></param>
        /// <param name="destDirName"></param>
        /// <param name="copySubDirs"></param>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }
        
        public static string RemoveFileExtension(string filePath)
        {
            return Path.ChangeExtension(filePath, null);
        }
        
        /// <summary>
	/// Determine whether a given path is a directory.
	/// </summary>
	public static bool PathIsDirectory (string absolutePath)
	{
		FileAttributes attr = File.GetAttributes(absolutePath);
		if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
			return true;
		else
			return false;
	}


	/// <summary>
	/// Given an absolute path, return a path rooted at the Assets folder.
	/// </summary>
	/// <remarks>
	/// Asset relative paths can only be used in the editor. They will break in builds.
	/// </remarks>
	/// <example>
	/// /Folder/UnityProject/Assets/resources/music returns Assets/resources/music
	/// </example>
	public static string AssetsRelativePath (string absolutePath)
	{
		if (absolutePath.StartsWith(Application.dataPath)) {
			return "Assets" + absolutePath.Substring(Application.dataPath.Length);
		}
		else {
			throw new System.ArgumentException("Full path does not contain the current project's Assets folder", "absolutePath");
		}
	}

	
	/// <summary>
	/// Get all available Resources directory paths within the current project.
	/// </summary>
	public static string[] GetResourcesDirectories ()
	{
		List<string> result = new List<string>();
		
		foreach (string dir in Directory.GetDirectories(Application.dataPath, "*", SearchOption.AllDirectories)) 
		{
			if (Path.GetFileName(dir).Equals("Resources")) 
			{
				// If one of the found directories is a Resources dir, add it to the result
				result.Add(dir);
			}
		}
		return result.ToArray();
	}
    }
}
