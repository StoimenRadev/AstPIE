using System.Collections.Generic;
using System.Linq;
using AstPIE.Api.Models;

namespace AstPIE.Api.Data
{
	// First content of the site. Runs at startup and only fills empty tables.
	// The content itself is in the JSON files in Data/Content, read by SeedContent.
	public static class SeedData
	{
		// The simulations are served locally for now. Later this becomes the CDN address.
		private const string SimulationsUrl = "http://localhost:8081/";
		private const string SourceUrl = "https://github.com/StoimenRadev/AstPIE/tree/prototypes/Prototypes/Demo/Simulations/ClassicalMechanics/";

		// Each part only fills its own tables, and only when they are empty
		public static void Run(AppDbContext db)
		{
			if (!db.KeyWords.Any())
			{
				AddKeyWords(db);
				db.SaveChanges();
			}

			if (!db.Topics.Any())
			{
				AddTopics(db);
				db.SaveChanges();
			}

			if (!db.Entries.Any())
			{
				ExploreSeedData.AddEntries(db);
				db.SaveChanges();
				LinkTopicsToCases(db);
				db.SaveChanges();
			}

			if (!db.CatalogueObjects.Any())
			{
				ExploreSeedData.AddCatalogue(db);
				db.SaveChanges();
			}
		}

		private static void AddKeyWords(AppDbContext db)
		{
			List<KeyWord> words = SeedContent.Read<List<KeyWord>>("learn", "keywords.json");
			foreach (KeyWord word in words)
			{
				db.KeyWords.Add(word);
			}
		}

		private static void AddTopics(AppDbContext db)
		{
			List<SeedContent.TopicFile> files = SeedContent.Read<List<SeedContent.TopicFile>>("learn", "topics.json");

			foreach (SeedContent.TopicFile file in files)
			{
				Topic topic = new Topic();
				topic.Slug = file.Slug;
				topic.Title = file.Title;
				topic.Question = file.Question;
				topic.Branch = file.Branch;
				topic.Summary = file.Summary;
				topic.SortOrder = file.SortOrder;
				topic.Content = file.Content;
				topic.PredictQuestion = file.PredictQuestion;
				topic.PredictOptions = file.PredictOptions;
				topic.PredictAnswer = file.PredictAnswer;
				topic.LawStatement = file.LawStatement;
				topic.LawEquation = file.LawEquation;
				topic.ReadAloud = file.ReadAloud;
				topic.RecapPoints = file.RecapPoints;
				topic.ExploreLine = file.ExploreLine;
				topic.IsPublished = true;

				int sortOrder = 1;
				foreach (TopicSymbol symbol in file.Symbols)
				{
					symbol.SortOrder = sortOrder;
					topic.Symbols.Add(symbol);
					sortOrder++;
				}

				sortOrder = 1;
				foreach (CheckQuestion question in file.CheckQuestions)
				{
					question.SortOrder = sortOrder;
					topic.CheckQuestions.Add(question);
					sortOrder++;
				}

				foreach (string slug in file.KeyWords)
				{
					TopicKeyWord link = new TopicKeyWord();
					link.KeyWord = (from word in db.KeyWords where word.Slug == slug select word).First();
					topic.KeyWords.Add(link);
				}

				topic.Simulations.Add(NewSimulation(file.Simulation));
				db.Topics.Add(topic);
			}
		}

		private static Simulation NewSimulation(SeedContent.SimulationFile file)
		{
			Simulation simulation = new Simulation();
			simulation.Slug = file.Slug;
			simulation.Title = file.Title;
			simulation.Engine = SimulationEngine.Godot2D;
			simulation.Version = "0.1";
			simulation.Status = SimulationStatus.Published;
			simulation.BuildUrl = SimulationsUrl + "?sim=" + file.Slug;
			simulation.ThumbnailUrl = SimulationsUrl + file.Slug + ".webp";
			simulation.LearningGoal = file.LearningGoal;
			simulation.SourceUrl = SourceUrl + FolderName(file.Slug);
			simulation.LabIntro = file.LabIntro;
			simulation.ModelText = file.ModelText;
			simulation.Limits = file.Limits;
			simulation.StepCode = file.StepCode;

			int sortOrder = 1;
			foreach (LabTask task in file.LabTasks)
			{
				task.SortOrder = sortOrder;
				simulation.LabTasks.Add(task);
				sortOrder++;
			}
			return simulation;
		}

		// "newtons-first-law" -> "NewtonsFirstLaw", the simulation's folder in the repository
		private static string FolderName(string slug)
		{
			string name = "";
			foreach (string part in slug.Split('-'))
			{
				name += char.ToUpper(part[0]) + part.Substring(1);
			}
			return name;
		}

		// "See it at work in Explore": set once the cases exist
		private static void LinkTopicsToCases(AppDbContext db)
		{
			List<SeedContent.TopicFile> files = SeedContent.Read<List<SeedContent.TopicFile>>("learn", "topics.json");

			foreach (SeedContent.TopicFile file in files)
			{
				Topic topic = (from t in db.Topics where t.Slug == file.Slug select t).FirstOrDefault();
				Entry entry = (from e in db.Entries where e.Slug == file.ExploreSlug select e).FirstOrDefault();
				if (topic != null && entry != null)
				{
					topic.ExploreEntry = entry;
				}
			}
		}
	}
}
