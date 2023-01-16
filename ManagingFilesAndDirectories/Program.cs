using System;
using System.Collections.Concurrent;
using System.Timers;
using static System.Console;

namespace ManagingFilesAndDirectories
{
	public class Program
	{
		// A thread safe dictionary
		private static ConcurrentDictionary<string, string> FilesToProcess = new ConcurrentDictionary<string, string>();
		static void Main(string[] args)
		{
			WriteLine("Parsing command line options");

			var directoryToWatch = args[1];
			if (!Directory.Exists(directoryToWatch))
			{
				WriteLine($"Error: {directoryToWatch} does not exist");
			}
			else
			{
				WriteLine($"Watching directory {directoryToWatch} for changes");

				// make sure to use using statement
				using var inputFileWatcher = new FileSystemWatcher(directoryToWatch);
				using var timer = new System.Threading.Timer(ProcessFiles, null, 0, 1000);

				// configure
				inputFileWatcher.IncludeSubdirectories = false;
				inputFileWatcher.InternalBufferSize = 32768; //32 kb
				inputFileWatcher.Filter = "*.*"; // default

				// LastWrite => Only get events raised by the FileSystemWatcher if it notices
				// changes to the last modified time . Can combine filters
				inputFileWatcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;

				inputFileWatcher.Created += FileCreated;
				inputFileWatcher.Changed += FileChanged;
				inputFileWatcher.Deleted += FileDeleted;
				inputFileWatcher.Renamed += FileRenamed;
				inputFileWatcher.Error += WatcherError;


				// need to se EnableRaisingEvents to turn on
				inputFileWatcher.EnableRaisingEvents = true;
				WriteLine("Press Enter to Quit.");
				ReadLine();

			}

			//var command = args[0];

			//if (command == "--file")
			//{
			//	var filePath = args[1];

			//	// check if the file path is absolute
			//	if (!Path.IsPathFullyQualified(filePath))
			//	{
			//		WriteLine($"Error: path '{filePath}' must be fully qualified");
			//		ReadLine();
			//		return;
			//	}

			//	WriteLine($"Single file {filePath} selected");
			//	ProcessFiles(filePath);
			//}
			//else if (command == "--dir")
			//{
			//	var directoryPath = args[1];
			//	var fileType = args[2];
			//	WriteLine($"Directory {directoryPath} selected for {fileType} files");
			//	ProcessDirectory(directoryPath, fileType);
			//}
			//else
			//{
			//	WriteLine("Invalid Command Line options");
			//}

			//WriteLine("Press Enter to quit");
			//ReadLine();
		}

		private static void FileCreated(object sender, FileSystemEventArgs e)
		{
			WriteLine($"* File created {e.Name} - type: {e.ChangeType}");
			FilesToProcess.TryAdd(e.FullPath, e.FullPath);
		}
		private static void FileChanged(object sender, FileSystemEventArgs e)
		{
			WriteLine($"* File changed {e.Name} - type: {e.ChangeType}");
			FilesToProcess.TryAdd(e.FullPath, e.FullPath);
		}

		private static void FileDeleted(object sender, FileSystemEventArgs e)
		{
			WriteLine($"* File deleted {e.Name} - type: {e.ChangeType}");
		}

		// This takes RenamedEventArgs
		private static void FileRenamed(object sender, RenamedEventArgs e)
		{
			WriteLine($"* File renamed {e.OldName} to {e.Name} - type: {e.ChangeType}");
		}

		// excpects ErrorEvents Args
		private static void WatcherError(object sender, ErrorEventArgs e)
		{
			WriteLine($"Error: file system watcher may no longer be active: {e.GetException()}");
		}

		private static void ProcessFiles(object stateInfo)
		{
			foreach (var fileName in FilesToProcess.Keys)
			{
				if (FilesToProcess.TryRemove(fileName, out _))
				{
					new FileProcessor(fileName).Process();
				}
			}
		}


		//private static void ProcessDirectory(string directoryPath, string fileType)
		//{
		//	switch (fileType)
		//	{
		//		case "TEXT":
		//			string[] textFiles = Directory.GetFiles(directoryPath, "*.txt");
		//			foreach (string textFilePath in textFiles)
		//			{
		//				new FileProcessor(textFilePath).Process();
		//			}
		//			break;
		//		default:
		//			WriteLine($"Error: {fileType} is not supported");
		//			return;
		//	}


		//}
		//private static void ProcessSingleFile(string filePath)
		//{
		//	var fileProcessor = new FileProcessor(filePath);
		//	fileProcessor.Process();
		//}
	}
}
