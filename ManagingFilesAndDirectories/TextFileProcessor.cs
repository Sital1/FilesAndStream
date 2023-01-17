using System;
using System.IO;
using System.IO.Abstractions;


namespace ManagingFilesAndDirectories
{
	public class TextFileProcessor
	{
		private readonly IFileSystem _fileSystem;
		public string InputFilePath { get; }
		public string OutputFilePath { get; }

		public TextFileProcessor(string inputFilePath, 
			                     string outputFilePath, 
								 IFileSystem fileSystem)
		{
			InputFilePath = inputFilePath;
			OutputFilePath = outputFilePath;
			_fileSystem = fileSystem;
		}

		public TextFileProcessor(string inputFilePath, string outputFilePath)
			: this(inputFilePath, outputFilePath, new FileSystem()) { }

		public void Process()
		{
			//using var inputFileStream = new FileStream(InputFilePath, FileMode.Open);
			using StreamReader inputStreamReader = _fileSystem.File.OpenText(InputFilePath);

			//using var outputFileStream = new FileStream(OutputFilePath, FileMode.CreateNew);
			//using var outputStreamWriter = new StreamWriter(OutputFilePath);
			using var outputStreamWriter = _fileSystem.File.CreateText(OutputFilePath);

			var currentLineNumber = 1;
			while (!inputStreamReader.EndOfStream)
            {
				string inputLine = inputStreamReader.ReadLine();
				
				if (currentLineNumber == 2)
                {
					inputLine = inputLine.ToUpperInvariant();
				}

				bool isLastLine = inputStreamReader.EndOfStream;

				if (isLastLine)
				{
					outputStreamWriter.Write(inputLine);
				}
				else
				{
					outputStreamWriter.WriteLine(inputLine);
				}

				currentLineNumber++;
			}

		}
	}
}
