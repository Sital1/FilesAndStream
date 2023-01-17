using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagingFilesAndDirectories
{
	public class BinaryFileProcessor
	{
		public string InputFilePath { get; }
		public string OutputFilePath { get; }

		public BinaryFileProcessor(string inputFilePath, string outputFilePath)
		{
			InputFilePath = inputFilePath;
			OutputFilePath = outputFilePath;
		}

		public void Process()
		{

			using FileStream inputFileStream = File.Open(InputFilePath, FileMode.Open, FileAccess.Read);
			using BinaryReader binaryReader = new BinaryReader(inputFileStream);

			using FileStream outputFileStream = File.OpenWrite(OutputFilePath);
			using BinaryWriter binaryWriter = new BinaryWriter(outputFileStream);

			int largest = 0;

			while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
			{
				int currentByte = binaryReader.ReadByte();

				binaryWriter.Write(currentByte); // writing a .NET int here

				if (currentByte > largest)
				{
					largest = currentByte;
				}
			}

			binaryWriter.Write(largest); // writing a .NET int => 4 bytes

			//const int endOfStream = -1;

			//int largestByte = 0;

			//int currentByte = inputFileStream.ReadByte();
			//while (currentByte != endOfStream)
			//{
			//	outputFileStream.WriteByte((byte)currentByte);

			//	if (currentByte > largestByte)
			//	{
			//		largestByte = currentByte;
			//	}

			//	currentByte = inputFileStream.ReadByte();
			//}
			//outputFileStream.WriteByte((byte)largestByte);

			//byte[] data = File.ReadAllBytes(InputFilePath);
			//byte largest = data.Max();

			//byte[] newData = new byte[data.Length + 1];
			//Array.Copy(data, newData, data.Length);
			//newData[^1] = largest;

			//File.WriteAllBytes(OutputFilePath, newData);
		}
	}
}
