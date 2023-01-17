﻿using ManagingFilesAndDirectories;
using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagingFiles.Tests
{
	public class BinaryFileProcessorShould
	{
		[Fact]
		public void AddLargestNumber()
		{
			var mockInputFile = new MockFileData(new byte[] { 0xFF, 0x34, 0x56, 0xD2 });
			// set mock file system starting state
			var mockFileSystem = new MockFileSystem();
			mockFileSystem.AddFile(@"c:\root\in\myfile.data", mockInputFile);
			mockFileSystem.AddDirectory(@"c:\root\out");

			var sut = new BinaryFileProcessor(@"c:\root\in\myfile.data", @"c:\root\out\myfile.data", mockFileSystem);

			sut.Process();

			Assert.True(mockFileSystem.FileExists(@"c:\root\out\myfile.data"));

			MockFileData processedFile = mockFileSystem.GetFile(@"c:\root\out\myfile.data");

			byte[] data = processedFile.Contents;

			Assert.Equal(5, data.Length);
			Assert.Equal(0xFF, data[4]);
		}
	}
}
