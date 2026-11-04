using System;
using System.Collections.Generic;
using System.IO;

namespace AstPIE.Api.Data
{
	// Reads the long texts of the first content from Data/Content.
	// The files are copied next to the program when it is built (see the .csproj).
	//   topics/<slug>.md      the text of a Learn topic
	//   entries/<slug>.html   the parts of a mission or mystery
	//   catalogue/<slug>.md   the description of a catalogue object
	public static class SeedContent
	{
		// One part of a mission or mystery
		public class ChapterText
		{
			public string Title { get; set; }
			// Empty in the file means no date
			public string DateLabel { get; set; }
			public string Summary { get; set; }
			public string Body { get; set; }
		}

		// A whole file as text, with the same line endings on every computer
		public static string Read(string folder, string fileName)
		{
			string path = Path.Combine(AppContext.BaseDirectory, "Data", "Content", folder, fileName);
			return File.ReadAllText(path).Replace("\r\n", "\n");
		}

		// The parts of a mission or mystery. Each part starts with a line
		// "=== Title | Date | One-line summary", and its HTML follows until the next one.
		public static List<ChapterText> ReadChapters(string slug)
		{
			List<ChapterText> chapters = new List<ChapterText>();
			ChapterText current = null;
			List<string> body = new List<string>();

			foreach (string line in Read("entries", slug + ".html").Split('\n'))
			{
				if (line.StartsWith("=== "))
				{
					if (current != null)
					{
						current.Body = string.Join("\n", body).Trim();
						chapters.Add(current);
					}
					string[] head = line.Substring(4).Split('|');
					current = new ChapterText();
					current.Title = head[0].Trim();
					current.DateLabel = head[1].Trim();
					if (current.DateLabel == "")
					{
						current.DateLabel = null;
					}
					current.Summary = head[2].Trim();
					body = new List<string>();
				}
				else if (current != null)
				{
					body.Add(line);
				}
			}

			if (current != null)
			{
				current.Body = string.Join("\n", body).Trim();
				chapters.Add(current);
			}
			return chapters;
		}
	}
}
