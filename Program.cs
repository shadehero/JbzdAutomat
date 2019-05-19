using System;
using HtmlAgilityPack;
using Console = Colorful.Console;
using System.Net;
using System.Drawing;
using System.Web;
using System.Text;
using System.Collections.Generic;
using System.IO;

namespace JbzdAutomat
{
	class MainClass
	{
		const string CORE_LINK = @"https://jbzdy.pl/str/";
		const string PICTURE_XPATH = @"//div/div/a/img";

		const string PICS_FOLDER = "Memy";
		const string LINKS_FOLDER = "Linki";

		private static int Page = 1;

		private static int Down_Pics = 0;
		private static int Down_Pages = 0;

		public static void Main(string[] args)
		{
			if (!Directory.Exists(PICS_FOLDER)) { Directory.CreateDirectory(PICS_FOLDER); }

			while (true)
			{
				Console.Clear();
				string link = CORE_LINK + Page;

				Console.Write(" Przeszukane Strony: ",Color.Tomato);
				Console.Write(Down_Pages,Color.Yellow);
				Console.WriteLine();
				Console.Write(" Pobrane Obrazki: ",Color.Tomato);
				Console.Write(Down_Pics, Color.Yellow);
				Console.WriteLine();

				List<Mem> MemList = new List<Mem>();
				HtmlWeb web = new HtmlWeb();
				Console.Write(" Pobieram Stronę: ",Color.Tomato);
				Console.Write(link, Color.Yellow);
				Console.WriteLine();
				HtmlDocument doc = web.Load(link);
				Console.Write(" Znalezione Memy: ", Color.Tomato);
				var nodes = doc.DocumentNode.SelectNodes(PICTURE_XPATH);

				if (nodes != null)
				{
					Console.Write(nodes.Count, Color.Yellow);
					Console.WriteLine();
					Console.WriteLine();
					Console.WriteLine(" Rozpoczynam Pobieranie:", Color.GreenYellow);

					foreach (var node in nodes)
					{
						using (WebClient client = new WebClient())
						{
							string src = node.Attributes["src"].Value;
							string title = node.Attributes["alt"].Value;
							string name = title + Path.GetExtension(src);
							string size = GetFileSize(new Uri(src));

							try
							{
								Console.Write(" [" + name + "]", Color.Gold);
								Console.Write(" [" + size + "]", Color.LimeGreen);
								Console.WriteLine();
								client.DownloadFile(src, PICS_FOLDER + "/" + name);
								Down_Pics++;
							}
							catch
							{
								Console.WriteLine(" Błąd podczas pobierania! ", Color.IndianRed);
							}
						}
					}
					Page++;
					Down_Pages++;
				}
			}
		}

		// Pobieranie Rozmiaru Pliku
		private static string GetFileSize(Uri uriPath)
		{
			var webRequest = HttpWebRequest.Create(uriPath);
			webRequest.Method = "HEAD";

			try
			{
				using (var webResponse = webRequest.GetResponse())
				{
					var fileSize = webResponse.Headers.Get("Content-Length");
					var fileSizeInMegaByte = Math.Round(Convert.ToDouble(fileSize) / 1024.0 / 1024.0, 2);
					return fileSizeInMegaByte + " MB";
				}
			}
			catch
			{
				return "Błąd!";
			}
		}
	}
}
