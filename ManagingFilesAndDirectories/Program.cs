using System;
using System.Collections.Concurrent;
using System.Runtime.Caching;
using System.Timers;
using static System.Console;

namespace ManagingFilesAndDirectories
{
	public class Program
	{
		//// A thread safe dictionary
		//private static ConcurrentDictionary<string, string> FilesToProcess = new ConcurrentDictionary<string, string>();
		private static MemoryCache FilesToProcess = MemoryCache.Default;

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

				ProcessExistingFiles(directoryToWatch);

				// make sure to use using statement
				using var inputFileWatcher = new FileSystemWatcher(directoryToWatch);
				//using var timer = new System.Threading.Timer(ProcessFiles, null, 0, 1000);

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
			AddToCache(e.FullPath);
		}


		private static void FileChanged(object sender, FileSystemEventArgs e)
		{
			WriteLine($"* File changed {e.Name} - type: {e.ChangeType}");
			AddToCache(e.FullPath);
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





		private static void AddToCache(string fullPath)
		{
			var item = new CacheItem(fullPath, fullPath);

			// cachitem policy
			// Call back when removed from cache. 
			// expiration => If cacheIetem not updated for 2 seconds the ProcessFile method will be called.
			var policy = new CacheItemPolicy
			{
				RemovedCallback = ProcessFile,
				SlidingExpiration = TimeSpan.FromSeconds(2)
			};

			FilesToProcess.Add(item, policy);
		}

		/// <summary>
		/// Will be called when the CacheItem expires
		/// </summary>
		/// <param name="arguments">Access to the cache item property</param>

		private static void ProcessFile(CacheEntryRemovedArguments arguments)
		{
			WriteLine($"* Cache item removed: {arguments.CacheItem.Key} because {arguments.RemovedReason}");

			// if expiration was the reason for removal, then we had no duplicate events in the
			// last two seconds for the filename, safe to process
			if (arguments.RemovedReason == CacheEntryRemovedReason.Expired)
			{
				var fileProcessor = new FileProcessor(arguments.CacheItem.Key);
				fileProcessor.Process();
			}
			else
			{
				WriteLine($"Warning: {arguments.CacheItem.Key} was removed unexpectedly and may not be processed because {arguments.RemovedReason}");
			}
		}



		private static void ProcessExistingFiles(string inputDirectory)
		{
			WriteLine($"Checking {inputDirectory} for existing files");

			// add the items to the cache.
			// will be automatically handled when the item expires.
			foreach (var filePath in Directory.EnumerateFiles(inputDirectory))
			{
				WriteLine($" - Found {filePath}");
				AddToCache(filePath);
			}
		}




		//private static void ProcessFiles(object stateInfo)
		//{
		//	foreach (var fileName in FilesToProcess.Keys)
		//	{
		//		if (FilesToProcess.TryRemove(fileName, out _))
		//		{
		//			new FileProcessor(fileName).Process();
		//		}
		//	}
		//}


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
