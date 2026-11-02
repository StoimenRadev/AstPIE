using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

		[Required]
		public string Summary { get; set; }

		[Required]
		public string CoverImageUrl { get; set; }

		public DateOnly? StartDate { get; set; }
		public DateOnly? EndDate { get; set; }

		// Missions only, for example "NASA"
		public string Agency { get; set; }

		// Missions only
		public MissionOutcome? MissionOutcome { get; set; }

		// Mysteries only
		public EvidenceStatus? EvidenceStatus { get; set; }

		// "Physics behind it": a Learn topic. Deleting the topic only clears this link.
		public int? RelatedTopicId { get; set; }

		[DeleteBehavior(DeleteBehavior.SetNull)]
		public Topic RelatedTopic { get; set; }

		public bool IsPublished { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime UpdatedAt { get; set; }

		public List<Chapter> Chapters { get; set; } = new List<Chapter>();
		public List<EntryPerson> People { get; set; } = new List<EntryPerson>();
		public List<MediaItem> Media { get; set; } = new List<MediaItem>();
	}
}
