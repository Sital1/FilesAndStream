using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagingFilesAndDirectories
{
	public class TextFileProcessor
	{
		public string InputFilePath { get; }
		public string OutputFilePath { get; }

		public TextFileProcessor(string inputFilePath, string outputFilePath)
		{
			InputFilePath = inputFilePath;
			OutputFilePath = outputFilePath;
		}

		public void Process()
		{
			// using stream with readers and writers
			//using var inputFileStream = new FileStream(InputFilePath, FileMode.Open);
			using StreamReader inputStreamReader = File.OpenText(InputFilePath);

			using var outputFileStream = new FileStream(OutputFilePath, FileMode.CreateNew);
			using var outputStreamWriter = new StreamWriter(outputFileStream);

			while (!inputStreamReader.EndOfStream)
			{
				string inputLine = inputStreamReader.ReadLine();
				string processedLine = inputLine.ToUpperInvariant();
				outputStreamWriter.WriteLine(processedLine);
			}


			//// Using read all text to read in a single string
			////string originalText = File.ReadAllText(InputFilePath);
			////string processedText = originalText.ToUpperInvariant();
			////File.WriteAllText(OutputFilePath, processedText);

			////reading as an array of string
			//string[] lines = File.ReadAllLines(InputFilePath);
			//lines[1] = lines[1].ToUpperInvariant();
			//File.WriteAllLines(OutputFilePath, lines);
		}
	}
}
