using System;
using System.Collections.Generic;
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

		// The line above the lab sheet, for example "One box, the forces you add, no friction."
		public string LabIntro { get; set; }

		// "How the simulation works": what it shows and leaves out, its limits, and the C# of one step
		public string ModelText { get; set; }

		public List<string> Limits { get; set; } = new List<string>();

		public string StepCode { get; set; }

		public DateTime CreatedAt { get; set; }
		public DateTime UpdatedAt { get; set; }

		// The tasks on the lab sheet
		public List<LabTask> LabTasks { get; set; } = new List<LabTask>();
	}
}
