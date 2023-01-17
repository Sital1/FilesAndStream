using System;
using System.IO;
using System.IO.Abstractions;


namespace ManagingFilesAndDirectories
{
    public class BinaryFileProcessor
    {
        private readonly IFileSystem _fileSystem;
        public string InputFilePath { get; }
        public string OutputFilePath { get; }

        public BinaryFileProcessor(string inputFilePath, 
                                   string outputFilePath, 
                                   IFileSystem fileSystem)
        {
            InputFilePath = inputFilePath;
            OutputFilePath = outputFilePath;
            _fileSystem = fileSystem;
        }

        public BinaryFileProcessor(string inputFilePath, string outputFilePath)
            : this(inputFilePath, outputFilePath, new FileSystem()) { }

        public void Process()
        {
            using Stream inputFileStream = _fileSystem.File.Open(InputFilePath, FileMode.Open, FileAccess.Read);
            using BinaryReader binaryReader = new BinaryReader(inputFileStream);

            using Stream outputFileStream = _fileSystem.File.Create(OutputFilePath);
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

            binaryWriter.Write(largest); // writing a .NET int here
        }
    }
}
