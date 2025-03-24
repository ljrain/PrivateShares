using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace CRM_Perf_BenchMark
{
	public class FileWriter
	{
		private static FileWriter m_instance;
		private StreamWriter m_Writer;
		private TextWriter writer;
		private static string startTime;
		private FileStream fs;

		public static FileWriter Instance
		{
			get
			{
				return m_instance;
			}
		}

		public static string StartTime
		{
			get
			{
				return startTime;
			}
		}

		static FileWriter()
		{
			m_instance = new FileWriter();
		}

		public void CreateFileWriter(string fileName)
		{
			string name = fileName.Substring(0, fileName.LastIndexOf('.'));
			fs = File.Open(name + startTime + ".txt", FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
			m_Writer = new StreamWriter(fs);
			m_Writer.AutoFlush = true;
			writer = StreamWriter.Synchronized(m_Writer);
		}

		public FileWriter()
		{
			startTime = System.DateTime.Now.ToString("dd-MM-yy") + "-" + System.DateTime.Now.Ticks.ToString();
			string name = "CRM_Perf_Trace_";
			fs = File.Open(name + startTime + ".txt", FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
			m_Writer = new StreamWriter(fs);
			m_Writer.AutoFlush = true;
			writer = StreamWriter.Synchronized(m_Writer);
            //create a file to keep track of the trx file name
            var testResultsFolder = Directory.GetParent(Directory.GetCurrentDirectory());
            var trxFileName = testResultsFolder.Name;
            System.IO.File.WriteAllText(Path.Combine(Directory.GetParent(testResultsFolder.ToString()).ToString(), "LatestTrxFile.txt"), trxFileName);
		}

		public void Close()
		{
			writer.Close();
			m_Writer.Close();
			fs.Close();
		}

		//
		// writing function to write to log file.
		//
		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.Synchronized)]
		public void WriteToFile(string output)
		{
			bool lockTaken = false;
			System.Threading.Monitor.TryEnter(m_instance, ref lockTaken);
			if (false == lockTaken)
			{
				System.Threading.Monitor.Enter(m_instance, ref lockTaken);
			}
			try
			{
				writer.WriteLine(output);
				writer.Flush();
			}
			finally
			{
				if (lockTaken)
					System.Threading.Monitor.Exit(m_instance);
			}
		}
	}
}
