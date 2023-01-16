using System;
using System.IO;
using static System.Console;

namespace ManagingFilesAndDirectories
{
	class FileProcessor
	{
		private const string BackupDirectoryName = "backup";
		private const string InProgressDirectoryName = "processing";
		private const string CompletedDirectoryName = "complete";

		public string InputFilePath { get; }

		public FileProcessor(string filePath) => InputFilePath = filePath;

		public void Process()
		{
			WriteLine($"Begin process of {InputFilePath}");

			// Check if file exists
			if (!File.Exists(InputFilePath))
			{
				WriteLine($"ERROR: file {InputFilePath} does not exist.");
				return;
			}

			string rootDirectoryPath = new DirectoryInfo(InputFilePath).Parent.Parent.FullName;
			WriteLine($"Root data path is {rootDirectoryPath}");

			// Check if backup dir exists
			string backupDirectoryPath = Path.Combine(rootDirectoryPath, BackupDirectoryName);

			//if (!Directory.Exists(backupDirectoryPath))
			//{
			WriteLine($"Attempting to create {backupDirectoryPath}");
			Directory.CreateDirectory(backupDirectoryPath);
			//}

			// Copy file to backup dir
			string inputFileName = Path.GetFileName(InputFilePath);
			string backupFilePath = Path.Combine(backupDirectoryPath, inputFileName);
			WriteLine($"Copying {InputFilePath} to {backupFilePath}");
			File.Copy(InputFilePath, backupFilePath, true);

			// Move to in progress dir
			Directory.CreateDirectory(Path.Combine(rootDirectoryPath, InProgressDirectoryName));
			string inProgressFilePath =
				Path.Combine(rootDirectoryPath, InProgressDirectoryName, inputFileName);

			if (File.Exists(inProgressFilePath))
			{
				WriteLine($"ERROR: a file with the name {inProgressFilePath} is already being processed.");
				return;
			}

			WriteLine($"Moving {InputFilePath} to {inProgressFilePath}");
			File.Move(InputFilePath, inProgressFilePath);


			// Determine type of file
			string extension = Path.GetExtension(InputFilePath);

			switch (extension)
			{
				case ".txt":
					ProcessTextFile(inProgressFilePath);
					break;
				default:
					WriteLine($"{extension} is an unsupported file type.");
					break;
			}

			// Move file after processing is complete
			string completedDirectoryPath = Path.Combine(rootDirectoryPath, CompletedDirectoryName);
			Directory.CreateDirectory(completedDirectoryPath);
			WriteLine($"Moving {inProgressFilePath} to {completedDirectoryPath}");
			//File.Move(inProgressFilePath, Path.Combine(completedDirectoryPath, inputFileName));

			string completedFileName =
				$"{Path.GetFileNameWithoutExtension(InputFilePath)}-{Guid.NewGuid()}{extension}";

			//completedFileName = Path.ChangeExtension(completedFileName, ".complete");

			var completedFilePath = Path.Combine(completedDirectoryPath, completedFileName);

			File.Move(inProgressFilePath, completedFilePath);

			string inProgressDirectoryPath = Path.GetDirectoryName(inProgressFilePath);

			// this may cause error when processing multiple files at the same time.
			// one file process deletes this directory which means, the other cannot acess it
			//Directory.Delete(inProgressDirectoryPath, true);
		}

		private void ProcessTextFile(string inProgressFilePath)
		{
			WriteLine($"Processing text file {inProgressFilePath}");
			// Read in and process
		}
	}
}
