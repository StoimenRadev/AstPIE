using System;
using System.ComponentModel.DataAnnotations;
using AstPIE.Api.Models;

namespace AstPIE.Api.Dtos
{
	// What the admin pages send (…Input) and get back (…AdminDto = the same fields + Id).
	// Wrong values are refused with 400 before anything is saved.

	public static class Rules
	{
		// lower-case words joined by dashes, for example "newtons-first-law"
		public const string SlugPattern = "^[a-z0-9]+(-[a-z0-9]+)*$";
		public const string SlugMessage = "Use lower-case letters, numbers and dashes, for example newtons-first-law.";
	}

	// ---------- Topics ----------

	public class TopicInput
	{
		[Required, RegularExpression(Rules.SlugPattern, ErrorMessage = Rules.SlugMessage)]
		public string Slug { get; set; }

		[Required]
		public string Title { get; set; }

		[Required]
		public string Branch { get; set; }

		[Required]
		public string Summary { get; set; }

		[Required]
		public string Content { get; set; }

		public int SortOrder { get; set; }
		public bool IsPublished { get; set; }
	}

	public class TopicAdminDto : TopicInput
	{
		public int Id { get; set; }
	}

	// ---------- Simulations ----------

	public class SimulationInput
	{
		[Range(1, int.MaxValue, ErrorMessage = "Choose a topic.")]
		public int TopicId { get; set; }

		[Required, RegularExpression(Rules.SlugPattern, ErrorMessage = Rules.SlugMessage)]
		public string Slug { get; set; }

		[Required]
		public string Title { get; set; }

		public SimulationEngine Engine { get; set; }

		[Required]
		public string Version { get; set; }

		public SimulationStatus Status { get; set; }

		[Required]
		public string BuildUrl { get; set; }

		public string ThumbnailUrl { get; set; }

		[Required]
		public string LearningGoal { get; set; }

		public string SourceUrl { get; set; }
	}

	public class SimulationAdminDto : SimulationInput
	{
		public int Id { get; set; }
	}

	// ---------- Entries (missions and mysteries) ----------

	public class EntryInput
	{
		public EntryKind Kind { get; set; }

		[Required, RegularExpression(Rules.SlugPattern, ErrorMessage = Rules.SlugMessage)]
		public string Slug { get; set; }

		[Required]
		public string Title { get; set; }

		[Required]
		public string Summary { get; set; }

		public string CoverImageUrl { get; set; }
		public DateOnly? StartDate { get; set; }
		public DateOnly? EndDate { get; set; }
		public string Agency { get; set; }
		public MissionOutcome? MissionOutcome { get; set; }
		public EvidenceStatus? EvidenceStatus { get; set; }
		public int? RelatedTopicId { get; set; }
		public bool IsPublished { get; set; }
	}

	public class EntryAdminDto : EntryInput
	{
		public int Id { get; set; }
	}

	// ---------- Chapters ----------

	public class ChapterInput
	{
		[Range(1, int.MaxValue, ErrorMessage = "Choose an entry.")]
		public int EntryId { get; set; }

		[Required]
		public string Title { get; set; }

		public string DateLabel { get; set; }

		[Required]
		public string Summary { get; set; }

		[Required]
		public string Body { get; set; }

		public int SortOrder { get; set; }
	}

	public class ChapterAdminDto : ChapterInput
	{
		public int Id { get; set; }
	}

	// ---------- People ----------

	public class PersonInput
	{
		[Required]
		public string FullName { get; set; }

		[Required]
		public string Nationality { get; set; }

		public DateOnly? BirthDate { get; set; }
		public DateOnly? DeathDate { get; set; }
		public string PhotoUrl { get; set; }

		[Required]
		public string Bio { get; set; }
	}

	public class PersonAdminDto : PersonInput
	{
		public int Id { get; set; }
	}

	// ---------- People in entries ----------

	public class EntryPersonInput
	{
		[Range(1, int.MaxValue, ErrorMessage = "Choose an entry.")]
		public int EntryId { get; set; }

		[Range(1, int.MaxValue, ErrorMessage = "Choose a person.")]
		public int PersonId { get; set; }

		[Required]
		public string Role { get; set; }

		public bool IsCrew { get; set; }
		public int SortOrder { get; set; }
	}

	public class EntryPersonAdminDto : EntryPersonInput
	{
		// The link has no number of its own: it is known by its entry and person, "3-7"
		public string Id { get; set; }
	}

	// ---------- Media ----------

	public class MediaItemInput
	{
		[Range(1, int.MaxValue, ErrorMessage = "Choose an entry.")]
		public int EntryId { get; set; }

		public MediaType Type { get; set; }

		[Required]
		public string Url { get; set; }

		[Required]
		public string Title { get; set; }

		public string Caption { get; set; }
		public string Credit { get; set; }
		public string License { get; set; }
		public int SortOrder { get; set; }
	}

	public class MediaItemAdminDto : MediaItemInput
	{
		public int Id { get; set; }
	}

	// ---------- Catalogue ----------

	public class CatalogueObjectInput
	{
		[Required, RegularExpression(Rules.SlugPattern, ErrorMessage = Rules.SlugMessage)]
		public string Slug { get; set; }

		[Required]
		public string Name { get; set; }

		public string OtherNames { get; set; }
		public CatalogueObjectType Type { get; set; }
		public string Constellation { get; set; }

		[Range(0, double.MaxValue, ErrorMessage = "The distance can't be negative.")]
		public double? DistanceLy { get; set; }

		[Range(0, double.MaxValue, ErrorMessage = "The mass can't be negative.")]
		public double? MassKg { get; set; }

		[Range(0, double.MaxValue, ErrorMessage = "The radius can't be negative.")]
		public double? RadiusKm { get; set; }

		public double? ApparentMagnitude { get; set; }
		public string DiscoveredBy { get; set; }
		public int? DiscoveryYear { get; set; }

		[Required]
		public string Summary { get; set; }

		[Required]
		public string Description { get; set; }

		public string ImageUrl { get; set; }
		public int? RelatedTopicId { get; set; }
		public bool IsPublished { get; set; }
	}

	public class CatalogueObjectAdminDto : CatalogueObjectInput
	{
		public int Id { get; set; }
	}
}
