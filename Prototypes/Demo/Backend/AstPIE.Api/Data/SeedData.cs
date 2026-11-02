using System.Linq;
using AstPIE.Api.Models;

namespace AstPIE.Api.Data
{
	// First content of the site. Runs at startup and only fills empty tables.
	public static class SeedData
	{
		// The simulations are served locally for now. Later this becomes the CDN address.
		private const string SimulationsUrl = "http://localhost:8081/";
		private const string SourceUrl = "https://github.com/StoimenRadev/AstPIE/tree/prototypes/Prototypes/Demo/Simulations/ClassicalMechanics/";

		// Each part only fills its own tables, and only when they are empty
		public static void Run(AppDbContext db)
		{
			if (!db.Topics.Any())
			{
				db.Topics.Add(FirstLaw());
				db.Topics.Add(SecondLaw());
				db.Topics.Add(ThirdLaw());
				db.SaveChanges();
			}

			if (!db.Entries.Any())
			{
				ExploreSeedData.AddEntries(db);
				db.SaveChanges();
			}

			if (!db.CatalogueObjects.Any())
			{
				ExploreSeedData.AddCatalogue(db);
				db.SaveChanges();
			}
		}

		private static Topic FirstLaw()
		{
			Topic topic = new Topic();
			topic.Slug = "newtons-first-law";
			topic.Title = "Newton's First Law";
			topic.Branch = "Classical mechanics";
			topic.Summary = "Without a net force, a body keeps moving the way it already moves.";
			topic.SortOrder = 1;
			topic.IsPublished = true;
			topic.Content = "The text of this topic will be here.";

			Simulation simulation = new Simulation();
			simulation.Slug = "newtons-first-law";
			simulation.Title = "A box on a frictionless field";
			simulation.Engine = SimulationEngine.Godot2D;
			simulation.Version = "0.1";
			simulation.Status = SimulationStatus.Published;
			simulation.BuildUrl = SimulationsUrl + "?sim=newtons-first-law";
			simulation.ThumbnailUrl = SimulationsUrl + simulation.Slug + ".webp";
			simulation.LearningGoal = "See that with zero net force the velocity does not change.";
			simulation.SourceUrl = SourceUrl + "NewtonsFirstLaw";
			topic.Simulations.Add(simulation);

			return topic;
		}

		private static Topic SecondLaw()
		{
			Topic topic = new Topic();
			topic.Slug = "newtons-second-law";
			topic.Title = "Newton's Second Law";
			topic.Branch = "Classical mechanics";
			topic.Summary = "The same force speeds up a light body more than a heavy one.";
			topic.SortOrder = 2;
			topic.IsPublished = true;
			topic.Content = "The text of this topic will be here.";

			Simulation simulation = new Simulation();
			simulation.Slug = "newtons-second-law";
			simulation.Title = "Two boxes, the same force";
			simulation.Engine = SimulationEngine.Godot2D;
			simulation.Version = "0.1";
			simulation.Status = SimulationStatus.Published;
			simulation.BuildUrl = SimulationsUrl + "?sim=newtons-second-law";
			simulation.ThumbnailUrl = SimulationsUrl + simulation.Slug + ".webp";
			simulation.LearningGoal = "Compare how the same force accelerates a light and a heavy body.";
			simulation.SourceUrl = SourceUrl + "NewtonsSecondLaw";
			topic.Simulations.Add(simulation);

			return topic;
		}

		private static Topic ThirdLaw()
		{
			Topic topic = new Topic();
			topic.Slug = "newtons-third-law";
			topic.Title = "Newton's Third Law";
			topic.Branch = "Classical mechanics";
			topic.Summary = "When one body pushes another, it is pushed back just as hard.";
			topic.SortOrder = 3;
			topic.IsPublished = true;
			topic.Content = "The text of this topic will be here.";

			Simulation simulation = new Simulation();
			simulation.Slug = "newtons-third-law";
			simulation.Title = "Two boxes pushing apart";
			simulation.Engine = SimulationEngine.Godot2D;
			simulation.Version = "0.1";
			simulation.Status = SimulationStatus.Published;
			simulation.BuildUrl = SimulationsUrl + "?sim=newtons-third-law";
			simulation.ThumbnailUrl = SimulationsUrl + simulation.Slug + ".webp";
			simulation.LearningGoal = "See that the push and the push back are equal, and that the total momentum is kept.";
			simulation.SourceUrl = SourceUrl + "NewtonsThirdLaw";
			topic.Simulations.Add(simulation);

			return topic;
		}
	}
}
