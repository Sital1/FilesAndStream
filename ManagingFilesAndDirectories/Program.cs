using System;
using static System.Console;

namespace ManagingFilesAndDirectories
{
	public class Program
	{
		static void Main(string[] args)
		{
			WriteLine("Parsing command line options");
			var command = args[0];

			if (command == "--file")
			{
				var filePath = args[1];

				// check if the file path is absolute
				if (!Path.IsPathFullyQualified(filePath))
				{
					WriteLine($"Error: path '{filePath}' must be fully qualified");
					ReadLine();
					return;
				}

				WriteLine($"Single file {filePath} selected");
				ProcessFiles(filePath);
			}
			else if (command == "--dir")
			{
				var directoryPath = args[1];
				var fileType = args[2];
				WriteLine($"Directory {directoryPath} selected for {fileType} files");
				ProcessDirectory(directoryPath, fileType);
			}
			else
			{
				WriteLine("Invalid Command Line options");
			}

			WriteLine("Press Enter to quit");
			ReadLine();
		}

		private static void ProcessDirectory(string directoryPath, string fileType)
		{
			switch (fileType)
			{
				case "TEXT":
					string[] textFiles = Directory.GetFiles(directoryPath, "*.txt");
					foreach (string textFilePath in textFiles)
					{
						new FileProcessor(textFilePath).Process();
					}
					break;
				default:
					WriteLine($"Error: {fileType} is not supported");
					return;
			}


		}
		private static void ProcessFiles(string filePath)
		{
			var fileProcessor = new FileProcessor(filePath);
			fileProcessor.Process();
		}
	}
}
