using System.Collections.Generic;
using AstPIE.Api.Models;

namespace AstPIE.Api.Dtos
{
	// What the API sends out. The database classes in Models stay inside the backend.

	// One concept in the list on the Learn page
	public class TopicListItemDto
	{
		public string Slug { get; set; }
		public string Title { get; set; }
		public string Question { get; set; }
		public string Branch { get; set; }
		public string Summary { get; set; }
		public int SortOrder { get; set; }
	}

	// A whole concept page
	public class TopicDto
	{
		public string Slug { get; set; }
		public string Title { get; set; }
		public string Question { get; set; }
		public string Branch { get; set; }
		public string Summary { get; set; }
		public string Content { get; set; }
		public string PredictQuestion { get; set; }
		public List<string> PredictOptions { get; set; } = new List<string>();
		public int PredictAnswer { get; set; }
		public string LawStatement { get; set; }
		public string LawEquation { get; set; }
		public string ReadAloud { get; set; }
		public List<string> RecapPoints { get; set; } = new List<string>();
		public ExploreLinkDto Explore { get; set; }
		public List<SymbolDto> Symbols { get; set; } = new List<SymbolDto>();
		public List<CheckQuestionDto> CheckQuestions { get; set; } = new List<CheckQuestionDto>();
		public List<KeyWordDto> KeyWords { get; set; } = new List<KeyWordDto>();
		public List<SimulationDto> Simulations { get; set; } = new List<SimulationDto>();
	}

	// "See it at work in Explore"
	public class ExploreLinkDto
	{
		public string Slug { get; set; }
		public EntryKind Kind { get; set; }
		public string Title { get; set; }
		public string Line { get; set; }
	}

	public class SymbolDto
	{
		public string Symbol { get; set; }
		public string Name { get; set; }
		public string Meaning { get; set; }
		public string Unit { get; set; }
		public string EquationPart { get; set; }
	}

	public class CheckQuestionDto
	{
		public int AfterSection { get; set; }
		public string Question { get; set; }
		public string Answer { get; set; }
	}

	public class KeyWordDto
	{
		public string Slug { get; set; }
		public string Name { get; set; }
		public List<string> OtherForms { get; set; } = new List<string>();
		public string Definition { get; set; }
		public string Symbol { get; set; }
		public string Unit { get; set; }
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
		public string LabIntro { get; set; }
		public string ModelText { get; set; }
		public List<string> Limits { get; set; } = new List<string>();
		public string StepCode { get; set; }
		public List<LabTaskDto> LabTasks { get; set; } = new List<LabTaskDto>();
	}

	public class LabTaskDto
	{
		public string Title { get; set; }
		public string Task { get; set; }
		public string Hint { get; set; }
		public string Answer { get; set; }
		public bool AnswersPredict { get; set; }
	}
}
