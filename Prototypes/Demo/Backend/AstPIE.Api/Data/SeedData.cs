using System.Linq;
using AstPIE.Api.Models;

namespace AstPIE.Api.Data
{
	// First content of the site. Runs at startup and only fills empty tables.
	// The long texts are files in Data/Content, read by SeedContent.
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
			topic.Summary = "Things keep doing what they are already doing. Why a puck keeps sliding on ice, why you lurch forward when a bus brakes, and how Voyager 1 has travelled for decades without fuel.";
			topic.SortOrder = 1;
			topic.IsPublished = true;
			topic.Content = SeedContent.Read("topics", "newtons-first-law.md");

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
			topic.Summary = "The harder you push, the faster it speeds up. Why the same push sends a ball flying but barely moves a boulder, and how engineers work out the force a rocket needs to leave the ground.";
			topic.SortOrder = 2;
			topic.IsPublished = true;
			topic.Content = SeedContent.Read("topics", "newtons-second-law.md");

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
			topic.Summary = "Every push pushes back. Why two astronauts drift apart when they push each other, and how a rocket moves forward in empty space by throwing gas backwards.";
			topic.SortOrder = 3;
			topic.IsPublished = true;
			topic.Content = SeedContent.Read("topics", "newtons-third-law.md");

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
