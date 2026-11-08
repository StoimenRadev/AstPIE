using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using AstPIE.Api.Models;

namespace AstPIE.Api.Data
{
	// Reads the first content of the site from the JSON files in Data/Content.
	// The files are copied next to the program when it is built (see the .csproj).
	//   learn/topics.json, learn/keywords.json   the Learn topics and their key words
	//   explore/<slug>.json, explore/people.json  one file per case, and everyone in them
	//   catalogue/<slug>.json                     one file per catalogue object
	public static class SeedContent
	{
		private static readonly JsonSerializerOptions Options = CreateOptions();

		private static JsonSerializerOptions CreateOptions()
		{
			JsonSerializerOptions options = new JsonSerializerOptions();
			options.PropertyNameCaseInsensitive = true;
			options.Converters.Add(new JsonStringEnumConverter());
			return options;
		}

		// One file, read into the given class
		public static T Read<T>(string folder, string fileName)
		{
			string path = Path.Combine(AppContext.BaseDirectory, "Data", "Content", folder, fileName);
			string text = File.ReadAllText(path).Replace("\r\n", "\n");
			return JsonSerializer.Deserialize<T>(text, Options);
		}

		// The names of all files in a folder that end in .json, sorted
		public static List<string> FileNames(string folder)
		{
			List<string> names = new List<string>();
			string path = Path.Combine(AppContext.BaseDirectory, "Data", "Content", folder);
			foreach (string file in Directory.GetFiles(path, "*.json"))
			{
				names.Add(Path.GetFileName(file));
			}
			names.Sort();
			return names;
		}

		// ---------- Learn ----------

		public class TopicFile
		{
			public string Slug { get; set; }
			public string Title { get; set; }
			public string Question { get; set; }
			public string Branch { get; set; }
			public string Summary { get; set; }
			public int SortOrder { get; set; }
			public string Content { get; set; }
			public string PredictQuestion { get; set; }
			public List<string> PredictOptions { get; set; }
			public int PredictAnswer { get; set; }
			public string LawStatement { get; set; }
			public string LawEquation { get; set; }
			public string ReadAloud { get; set; }
			public List<string> RecapPoints { get; set; }
			public string ExploreSlug { get; set; }
			public string ExploreLine { get; set; }
			public List<string> KeyWords { get; set; }
			public List<TopicSymbol> Symbols { get; set; }
			public List<CheckQuestion> CheckQuestions { get; set; }
			public SimulationFile Simulation { get; set; }
		}

		public class SimulationFile
		{
			public string Slug { get; set; }
			public string Title { get; set; }
			public string LearningGoal { get; set; }
			public string LabIntro { get; set; }
			public string ModelText { get; set; }
			public List<string> Limits { get; set; }
			public string StepCode { get; set; }
			public List<LabTask> LabTasks { get; set; }
		}

		// ---------- Explore ----------

		public class PersonFile
		{
			// The name the cases use for this person, for example "korolev"
			public string Key { get; set; }
			public string FullName { get; set; }
			public string Nationality { get; set; }
			public DateOnly? BirthDate { get; set; }
			public DateOnly? DeathDate { get; set; }
			public string PhotoUrl { get; set; }
			public string PhotoCredit { get; set; }
			public string Bio { get; set; }
		}

		public class EntryFile
		{
			public string Slug { get; set; }
			public EntryKind Kind { get; set; }
			public string Title { get; set; }
			public string Tagline { get; set; }
			public string Summary { get; set; }
			public string CoverImageUrl { get; set; }
			public string CoverCaption { get; set; }
			public string CoverCredit { get; set; }
			public string Country { get; set; }
			public EntryDesk Desk { get; set; }
			public DateOnly? StartDate { get; set; }
			public DateOnly? EndDate { get; set; }
			public string Agency { get; set; }
			public MissionOutcome? MissionOutcome { get; set; }
			public EvidenceStatus? EvidenceStatus { get; set; }
			public string StatusNote { get; set; }
			public double? RadioMinMHz { get; set; }
			public double? RadioMaxMHz { get; set; }
			// The Learn topic whose simulation opens in "Try it yourself"
			public string RelatedTopic { get; set; }
			public List<EntryFact> Facts { get; set; }
			public List<EntryPersonFile> People { get; set; }
			public List<MediaFile> Media { get; set; }
			public List<Explanation> Explanations { get; set; }
			public List<ChapterFile> Chapters { get; set; }
		}

		public class EntryPersonFile
		{
			public string Person { get; set; }
			public string Role { get; set; }
			public bool IsCrew { get; set; }
			public int SortOrder { get; set; }
		}

		public class ChapterFile
		{
			public string Section { get; set; }
			public string TabLabel { get; set; }
			public string Title { get; set; }
			public string Place { get; set; }
			public string DateLabel { get; set; }
			public string Summary { get; set; }
			public string Body { get; set; }
			public string MarginNote { get; set; }
			public bool ShowsRadioHint { get; set; }
			public int SortOrder { get; set; }
			public List<MediaFile> Media { get; set; }
			public List<Explanation> Explanations { get; set; }
		}

		public class MediaFile
		{
			public MediaType Type { get; set; }
			public string Url { get; set; }
			public string Title { get; set; }
			public string Caption { get; set; }
			public string Credit { get; set; }
			public string License { get; set; }
			public string Text { get; set; }
			public string Author { get; set; }
			public string DateLabel { get; set; }
			public string PosterUrl { get; set; }
			public string LengthLabel { get; set; }
			// A personnel file: the person's key in people.json
			public string Person { get; set; }
			// A photo: the keys of the people it shows
			public List<string> People { get; set; }
			public string Style { get; set; }
			public string ChartKey { get; set; }
			public JsonNode Data { get; set; }
			public int SortOrder { get; set; }
		}

		// ---------- Catalogue ----------

		public class CatalogueFile
		{
			public string Slug { get; set; }
			public string Name { get; set; }
			public string OtherNames { get; set; }
			public CatalogueObjectType Type { get; set; }
			public string TypeLabel { get; set; }
			public string Constellation { get; set; }
			public double? RightAscension { get; set; }
			public double? Declination { get; set; }
			public double? DistanceLy { get; set; }
			public double? MassKg { get; set; }
			public double? RadiusKm { get; set; }
			public double? ApparentMagnitude { get; set; }
			public string DiscoveredBy { get; set; }
			public int? DiscoveryYear { get; set; }
			public string Summary { get; set; }
			public string Description { get; set; }
			public string ImageUrl { get; set; }
			public string ImageCaption { get; set; }
			public string ImageCredit { get; set; }
			public bool ImageIsDark { get; set; }
			public List<CatalogueFact> Facts { get; set; }
			public List<ObservationLogEntry> LogEntries { get; set; }
			public List<InstrumentFile> Instruments { get; set; }
		}

		public class InstrumentFile
		{
			public string Name { get; set; }
			public string PhotoUrl { get; set; }
			public string PhotoCredit { get; set; }
			public bool PhotoIsDark { get; set; }
			public string Description { get; set; }
			public string Role { get; set; }
			public int SortOrder { get; set; }
		}
	}
}
