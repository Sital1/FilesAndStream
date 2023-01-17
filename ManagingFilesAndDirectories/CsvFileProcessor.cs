using CsvHelper.Configuration;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagingFilesAndDirectories
{
	class CsvFileProcessor
	{
		public string InputFilePath { get; }
		public string OutputFilePath { get; }

		public CsvFileProcessor(string inputFilePath, string outputFilePath)
		{
			InputFilePath = inputFilePath;
			OutputFilePath = outputFilePath;
		}

		public void Process()
		{
			using StreamReader inputReader = File.OpenText(InputFilePath);

			var csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture)
			{
				Comment = '@',
				AllowComments = true,
				TrimOptions = TrimOptions.Trim,
				IgnoreBlankLines = true, // default
				HasHeaderRecord = true, // default
				Delimiter = "," // default
			};
			using CsvReader csvReader = new CsvReader(inputReader, csvConfiguration);
			csvReader.Context.RegisterClassMap<ProcessedOrderMap>();

			IEnumerable<ProcessedOrder> records = csvReader.GetRecords<ProcessedOrder>();

			using StreamWriter output = File.CreateText(OutputFilePath);
			using var csvWriter = new CsvWriter(output, CultureInfo.InvariantCulture);

			// csvWriter.WriteRecords(records);

			csvWriter.WriteHeader<ProcessedOrder>();
			csvWriter.NextRecord();

			var recordsArray = records.ToArray();
			for (int i = 0; i < recordsArray.Length; i++)
			{
				//csvWriter.WriteRecord(recordsArray[i]);
				csvWriter.WriteField(recordsArray[i].OrderNumber);
				csvWriter.WriteField(recordsArray[i].Customer);
				csvWriter.WriteField(recordsArray[i].Amount);

				bool isLastRecord = i == recordsArray.Length - 1;

				if (!isLastRecord)
				{
					csvWriter.NextRecord();
				}
			}
		}
	}
}
