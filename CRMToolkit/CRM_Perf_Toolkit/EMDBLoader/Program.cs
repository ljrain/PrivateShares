using System;
using System.Collections.Generic;
using System.Text;
using CRM_Perf_BenchMark;

namespace EMDBLoader
{
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
                if ((System.Net.ServicePointManager.SecurityProtocol & System.Net.SecurityProtocolType.Tls12) == 0)
                {
                    System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls12;
                }
                System.Net.ServicePointManager.DefaultConnectionLimit = 100;
                System.Threading.ThreadPool.SetMinThreads(100, 100);
                System.Net.ServicePointManager.Expect100Continue = false;
                System.Net.ServicePointManager.UseNagleAlgorithm = false;


				System.DateTime Start = System.DateTime.Now;
				Console.WriteLine("Initializing Entity Manager...");
				if (args.Length == 0)
				{
					EntityManager.LoadFromCRM();
				}
				else if (args.Length == 1)
				{
					foreach (string arg in args)
					{
						switch (arg.Trim().ToLower())
						{						
							case "refresh":
								EntityManager.RefreshAllAuthTokens();
								break;
						}
					}
				}
				else
				{
					// Supported arguments:
					// SkipEntities=1;MaxRecordsPerEntity=100
					if (args[1].Contains("="))
					{
						var contextParams = args[1].Split(";".ToCharArray());
						foreach (var contextParam in contextParams)
						{
							var pair = contextParam.Split("=".ToCharArray());
							if (pair.Length == 2)
							{
								EntityManager.Context.Add(pair[0], pair[1]);
							}
						}
					}
				}
				Console.WriteLine("Entity Manager Database loading from CRM SQL Database completed successfully. Time elapsed: {0}", System.DateTime.Now.Subtract(Start).ToString());
                Console.ReadLine();
				
			}
			catch (Exception e)
			{
				string stacktrace = e.InnerException == null ? e.StackTrace : e.InnerException.StackTrace;
				Console.WriteLine("Entity Manager Database loading from CRM SQL Database failed. " + stacktrace);
                Console.ReadLine();
			}
		}
	}
}
