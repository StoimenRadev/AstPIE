using System.Collections.Generic;

namespace AstPIE.Api.Dtos
{
	// What the API sends out. The database classes in Models stay inside the backend.

	// One row in the list of topics
	public class TopicListItemDto
	{
		public string Slug { get; set; }
		public string Title { get; set; }
		public string Branch { get; set; }
		public string Summary { get; set; }
	}

	// A whole topic page
	public class TopicDto
	{
		public string Slug { get; set; }
		public string Title { get; set; }
		public string Branch { get; set; }
		public string Summary { get; set; }
		public string Content { get; set; }
		public List<SimulationDto> Simulations { get; set; } = new List<SimulationDto>();
	}

	public class SimulationDto
	{
		public string Slug { get; set; }
		public string Title { get; set; }
		public string Engine { get; set; }
		public string Version { get; set; }
		public string BuildUrl { get; set; }
		public string ThumbnailUrl { get; set; }
		public string LearningGoal { get; set; }
		public string SourceUrl { get; set; }
	}
}
