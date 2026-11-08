using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AstPIE.Api.Models;

namespace AstPIE.Api.Dtos
{
	// What the admin pages send and get back for the parts of topic pages, case desks and logbooks

	// ---------- TopicSymbols ----------

	public class TopicSymbolInput
	{
		[Range(1, int.MaxValue, ErrorMessage = "Choose a topic.")]
		public int TopicId { get; set; }

		[Required]
		public string Symbol { get; set; }

		[Required]
		public string Name { get; set; }

		[Required]
		public string Meaning { get; set; }

		public string Unit { get; set; }

		public string EquationPart { get; set; }

		public int SortOrder { get; set; }
	}

	public class TopicSymbolAdminDto : TopicSymbolInput
	{
		public int Id { get; set; }
	}

	// ---------- CheckQuestions ----------

	public class CheckQuestionInput
	{
		[Range(1, int.MaxValue, ErrorMessage = "Choose a topic.")]
		public int TopicId { get; set; }

		public int AfterSection { get; set; }

		[Required]
		public string Question { get; set; }

		[Required]
		public string Answer { get; set; }

		public int SortOrder { get; set; }
	}

	public class CheckQuestionAdminDto : CheckQuestionInput
	{
		public int Id { get; set; }
	}

	// ---------- LabTasks ----------

	public class LabTaskInput
	{
		[Range(1, int.MaxValue, ErrorMessage = "Choose a simulation.")]
		public int SimulationId { get; set; }

		[Required]
		public string Title { get; set; }

		[Required]
		public string Task { get; set; }

		[Required]
		public string Hint { get; set; }

		[Required]
		public string Answer { get; set; }

		public bool AnswersPredict { get; set; }

		public int SortOrder { get; set; }
	}

	public class LabTaskAdminDto : LabTaskInput
	{
		public int Id { get; set; }
	}

	// ---------- KeyWords ----------

	public class KeyWordInput
	{
		[Required, RegularExpression(Rules.SlugPattern, ErrorMessage = Rules.SlugMessage)]
		public string Slug { get; set; }

		[Required]
		public string Name { get; set; }

		public List<string> OtherForms { get; set; } = new List<string>();

		[Required]
		public string Definition { get; set; }

		public string Symbol { get; set; }

		public string Unit { get; set; }
	}

	public class KeyWordAdminDto : KeyWordInput
	{
		public int Id { get; set; }
	}

	// ---------- EntryFacts ----------

	public class EntryFactInput
	{
		[Range(1, int.MaxValue, ErrorMessage = "Choose a entry.")]
		public int EntryId { get; set; }

		[Required]
		public string Label { get; set; }

		[Required]
		public string Value { get; set; }

		public int SortOrder { get; set; }
	}

	public class EntryFactAdminDto : EntryFactInput
	{
		public int Id { get; set; }
	}

	// ---------- Explanations ----------

	public class ExplanationInput
	{
		[Range(1, int.MaxValue, ErrorMessage = "Choose a entry.")]
		public int EntryId { get; set; }

		public int? ChapterId { get; set; }

		[Required]
		public string Title { get; set; }

		public string ProposedBy { get; set; }

		[Required]
		public string ForText { get; set; }

		[Required]
		public string AgainstText { get; set; }

		[Required]
		public string Verdict { get; set; }

		[Required]
		public string Stamp { get; set; }

		public string ImageUrl { get; set; }

		public string ImageCredit { get; set; }

		public string ImageFit { get; set; }

		public int SortOrder { get; set; }
	}

	public class ExplanationAdminDto : ExplanationInput
	{
		public int Id { get; set; }
	}

	// ---------- CatalogueFacts ----------

	public class CatalogueFactInput
	{
		[Range(1, int.MaxValue, ErrorMessage = "Choose a catalogue object.")]
		public int CatalogueObjectId { get; set; }

		[Required]
		public string Label { get; set; }

		[Required]
		public string Value { get; set; }

		public int SortOrder { get; set; }
	}

	public class CatalogueFactAdminDto : CatalogueFactInput
	{
		public int Id { get; set; }
	}

	// ---------- ObservationLogEntries ----------

	public class LogEntryInput
	{
		[Range(1, int.MaxValue, ErrorMessage = "Choose a catalogue object.")]
		public int CatalogueObjectId { get; set; }

		[Required]
		public string DateLabel { get; set; }

		[Required]
		public string Title { get; set; }

		[Required]
		public string Summary { get; set; }

		public string Place { get; set; }

		public List<string> Body { get; set; } = new List<string>();

		public List<string> Facts { get; set; } = new List<string>();

		public string Note { get; set; }

		public PlateKind PlateKind { get; set; }

		public string PlateUrl { get; set; }

		public string PlateChartKey { get; set; }

		public string PlatePosterUrl { get; set; }

		public string PlateTitle { get; set; }

		public string PlateCaption { get; set; }

		public string PlateCredit { get; set; }

		public bool PlateIsDark { get; set; }

		public int SortOrder { get; set; }
	}

	public class LogEntryAdminDto : LogEntryInput
	{
		public int Id { get; set; }
	}

	// ---------- Instruments ----------

	public class InstrumentInput
	{
		[Required]
		public string Name { get; set; }

		[Required]
		public string PhotoUrl { get; set; }

		public string PhotoCredit { get; set; }

		public bool PhotoIsDark { get; set; }

		public string Description { get; set; }
	}

	public class InstrumentAdminDto : InstrumentInput
	{
		public int Id { get; set; }
	}

	// ---------- TopicKeyWords ----------

	public class TopicKeyWordInput
	{
		[Range(1, int.MaxValue, ErrorMessage = "Choose a topic.")]
		public int TopicId { get; set; }

		[Range(1, int.MaxValue, ErrorMessage = "Choose a key word.")]
		public int KeyWordId { get; set; }
	}

	public class TopicKeyWordAdminDto : TopicKeyWordInput
	{
		// "topicId-keyWordId"
		public string Id { get; set; }
	}

	// ---------- MediaItemPeople ----------

	public class MediaItemPersonInput
	{
		[Range(1, int.MaxValue, ErrorMessage = "Choose a photo.")]
		public int MediaItemId { get; set; }

		[Range(1, int.MaxValue, ErrorMessage = "Choose a person.")]
		public int PersonId { get; set; }
	}

	public class MediaItemPersonAdminDto : MediaItemPersonInput
	{
		// "mediaItemId-personId"
		public string Id { get; set; }
	}

	// ---------- CatalogueObjectInstruments ----------

	public class CatalogueObjectInstrumentInput
	{
		[Range(1, int.MaxValue, ErrorMessage = "Choose a catalogue object.")]
		public int CatalogueObjectId { get; set; }

		[Range(1, int.MaxValue, ErrorMessage = "Choose a instrument.")]
		public int InstrumentId { get; set; }

		[Required]
		public string Role { get; set; }

		public int SortOrder { get; set; }
	}

	public class CatalogueObjectInstrumentAdminDto : CatalogueObjectInstrumentInput
	{
		// "catalogueObjectId-instrumentId"
		public string Id { get; set; }
	}
}
