using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AstPIE.Api.Models
{
	public enum EntryKind
	{
		Mission,
		Mystery
	}

	public enum MissionOutcome
	{
		Success,
		PartialSuccess,
		Failure,
		Ongoing
	}

	public enum EvidenceStatus
	{
		Unexplained,
		UnderInvestigation,
		Hypothesis,
		Debunked
	}

	// Which desk a case opens on
	public enum EntryDesk
	{
		Soviet,
		American,
		Mystery
	}

	// One page in Explore: a mission or a mystery, told in chapters
	[Index(nameof(Slug), IsUnique = true)]
	public class Entry : ITimestamped
	{
		public int Id { get; set; }

		public EntryKind Kind { get; set; }

		// For example "sputnik-1"
		[Required]
		public string Slug { get; set; }

		[Required]
		public string Title { get; set; }

		// The short line on the folder and the cover sheet, for example "Two people on another world"
		public string Tagline { get; set; }

		// The case summary; <mark> highlights the key sentence
		[Required]
		public string Summary { get; set; }

		[Required]
		public string CoverImageUrl { get; set; }

		// The caption under the cover photo, and who took it
		public string CoverCaption { get; set; }

		public string CoverCredit { get; set; }

		// For example "United States"
		public string Country { get; set; }

		public EntryDesk Desk { get; set; }

		public DateOnly? StartDate { get; set; }
		public DateOnly? EndDate { get; set; }

		// Missions only, for example "NASA"
		public string Agency { get; set; }

		// Missions only
		public MissionOutcome? MissionOutcome { get; set; }

		// Mysteries only
		public EvidenceStatus? EvidenceStatus { get; set; }

		// Mysteries only: the small note under the stamp, for example "case still open"
		public string StatusNote { get; set; }

		// The radio dial, when this case needs its own range (otherwise the desk's range)
		public double? RadioMinMHz { get; set; }

		public double? RadioMaxMHz { get; set; }

		// "Physics behind it": a Learn topic. Deleting the topic only clears this link.
		public int? RelatedTopicId { get; set; }

		[DeleteBehavior(DeleteBehavior.SetNull)]
		[InverseProperty(nameof(Topic.RelatedEntries))]
		public Topic RelatedTopic { get; set; }

		public bool IsPublished { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime UpdatedAt { get; set; }

		public List<Chapter> Chapters { get; set; } = new List<Chapter>();
		public List<EntryPerson> People { get; set; } = new List<EntryPerson>();
		public List<MediaItem> Media { get; set; } = new List<MediaItem>();
		public List<EntryFact> Facts { get; set; } = new List<EntryFact>();
		public List<Explanation> Explanations { get; set; } = new List<Explanation>();

		// Topics that send readers here ("See it at work in Explore")
		[InverseProperty(nameof(Topic.ExploreEntry))]
		public List<Topic> ExploredIn { get; set; } = new List<Topic>();
	}
}
