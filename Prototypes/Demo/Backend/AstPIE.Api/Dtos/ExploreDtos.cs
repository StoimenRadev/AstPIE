using System;
using System.Collections.Generic;
using AstPIE.Api.Models;

namespace AstPIE.Api.Dtos
{
	// What the API sends out for Explore. Enums are sent as text ("Mission").

	// A link to a Learn topic ("Physics behind it")
	public class TopicLinkDto
	{
		public string Slug { get; set; }
		public string Title { get; set; }
	}

	// One card in the list of missions or mysteries
	public class EntryListItemDto
	{
		public string Slug { get; set; }
		public EntryKind Kind { get; set; }
		public string Title { get; set; }
		public string Summary { get; set; }
		public string CoverImageUrl { get; set; }
		public DateOnly? StartDate { get; set; }
		public DateOnly? EndDate { get; set; }
		public string Agency { get; set; }
		public MissionOutcome? MissionOutcome { get; set; }
		public EvidenceStatus? EvidenceStatus { get; set; }
	}

	// A whole entry page
	public class EntryDto
	{
		public string Slug { get; set; }
		public EntryKind Kind { get; set; }
		public string Title { get; set; }
		public string Summary { get; set; }
		public string CoverImageUrl { get; set; }
		public DateOnly? StartDate { get; set; }
		public DateOnly? EndDate { get; set; }
		public string Agency { get; set; }
		public MissionOutcome? MissionOutcome { get; set; }
		public EvidenceStatus? EvidenceStatus { get; set; }
		public TopicLinkDto RelatedTopic { get; set; }
		public List<ChapterDto> Chapters { get; set; } = new List<ChapterDto>();
		public List<EntryPersonDto> People { get; set; } = new List<EntryPersonDto>();
		public List<MediaItemDto> Media { get; set; } = new List<MediaItemDto>();
	}

	public class ChapterDto
	{
		public string Title { get; set; }
		public string DateLabel { get; set; }
		public string Summary { get; set; }
		public string Body { get; set; }
	}

	// A person as listed on an entry page. The full card comes from /api/people/{id}.
	public class EntryPersonDto
	{
		public int PersonId { get; set; }
		public string FullName { get; set; }
		public string PhotoUrl { get; set; }
		public string Role { get; set; }
		public bool IsCrew { get; set; }
	}

	public class MediaItemDto
	{
		public MediaType Type { get; set; }
		public string Url { get; set; }
		public string Title { get; set; }
		public string Caption { get; set; }
		public string Credit { get; set; }
		public string License { get; set; }
	}

	// The centred card of a person
	public class PersonDto
	{
		public int Id { get; set; }
		public string FullName { get; set; }
		public string Nationality { get; set; }
		public DateOnly? BirthDate { get; set; }
		public DateOnly? DeathDate { get; set; }
		public string PhotoUrl { get; set; }
		public string Bio { get; set; }
		public List<PersonEntryDto> Entries { get; set; } = new List<PersonEntryDto>();
	}

	// An entry a person was part of, and as what
	public class PersonEntryDto
	{
		public string Slug { get; set; }
		public EntryKind Kind { get; set; }
		public string Title { get; set; }
		public string Role { get; set; }
	}
}
