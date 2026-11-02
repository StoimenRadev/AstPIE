using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace AstPIE.Api.Models
{
	public enum SimulationEngine
	{
		Godot2D,
		ThreeJs
	}

	public enum SimulationStatus
	{
		Draft,
		Published
	}

	// A simulation shown on a topic page. The simulation itself runs in the browser,
	// the database only says where it is and what it is about.
	[Index(nameof(Slug), IsUnique = true)]
	public class Simulation : ITimestamped
	{
		public int Id { get; set; }

		public int TopicId { get; set; }

		// A topic can't be deleted while it still has simulations
		[DeleteBehavior(DeleteBehavior.Restrict)]
		public Topic Topic { get; set; }

		// Same as the ?sim= value in the Godot project, for example "newtons-first-law"
		[Required]
		public string Slug { get; set; }

		[Required]
		public string Title { get; set; }

		public SimulationEngine Engine { get; set; }

		// Only the published version. The full history is in Git and Versions.md.
		[Required]
		public string Version { get; set; }

		public SimulationStatus Status { get; set; }

		[Required]
		public string BuildUrl { get; set; }

		[Required]
		public string ThumbnailUrl { get; set; }

		[Required]
		public string LearningGoal { get; set; }

		[Required]
		public string SourceUrl { get; set; }

		public DateTime CreatedAt { get; set; }
		public DateTime UpdatedAt { get; set; }
	}
}
