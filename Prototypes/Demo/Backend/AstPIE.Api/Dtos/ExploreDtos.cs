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
		public string Tagline { get; set; }
		public string Summary { get; set; }
		public string CoverImageUrl { get; set; }
		public string Country { get; set; }
		public EntryDesk Desk { get; set; }
		public string StatusNote { get; set; }
		// How many pages the case has (shown on its folder)
		public int PageCount { get; set; }
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
		public string Tagline { get; set; }
		public string Summary { get; set; }
		public string CoverImageUrl { get; set; }
		public string CoverCaption { get; set; }
		public string CoverCredit { get; set; }
		public string Country { get; set; }
		public EntryDesk Desk { get; set; }
		public string StatusNote { get; set; }
		public double? RadioMinMHz { get; set; }
		public double? RadioMaxMHz { get; set; }
		public DateOnly? StartDate { get; set; }
		public DateOnly? EndDate { get; set; }
		public string Agency { get; set; }
		public MissionOutcome? MissionOutcome { get; set; }
		public EvidenceStatus? EvidenceStatus { get; set; }
		public TopicLinkDto RelatedTopic { get; set; }
		public List<FactDto> Facts { get; set; } = new List<FactDto>();
		public List<ChapterDto> Chapters { get; set; } = new List<ChapterDto>();
		public List<EntryPersonDto> People { get; set; } = new List<EntryPersonDto>();
		// What lies on the desk with the case summary (ChapterId empty)
		public List<MediaItemDto> Media { get; set; } = new List<MediaItemDto>();
		public List<ExplanationDto> Explanations { get; set; } = new List<ExplanationDto>();
	}

	// One row of a cover sheet or logbook page
	public class FactDto
	{
		public string Label { get; set; }
		public string Value { get; set; }
	}

	public class ChapterDto
	{
		public string Section { get; set; }
		public string TabLabel { get; set; }
		public string Title { get; set; }
		public string Place { get; set; }
		public string DateLabel { get; set; }
		public string Summary { get; set; }
		public string Body { get; set; }
		public string MarginNote { get; set; }
		public bool ShowsRadioHint { get; set; }
		public List<MediaItemDto> Media { get; set; } = new List<MediaItemDto>();
		public List<ExplanationDto> Explanations { get; set; } = new List<ExplanationDto>();
	}

	// A person as listed on an entry page. The full card comes from /api/people/{id}.
	public class EntryPersonDto
	{
		public int PersonId { get; set; }
		public string FullName { get; set; }
		public string Nationality { get; set; }
		public DateOnly? BirthDate { get; set; }
		public DateOnly? DeathDate { get; set; }
		public string PhotoUrl { get; set; }
		public string PhotoCredit { get; set; }
		public string Bio { get; set; }
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
		public string Text { get; set; }
		public string Author { get; set; }
		public string DateLabel { get; set; }
		public string PosterUrl { get; set; }
		public string LengthLabel { get; set; }
		// A personnel file: whose it is
		public int? PersonId { get; set; }
		// A photo: the people it shows
		public List<int> ShownPeopleIds { get; set; } = new List<int>();
		public string Style { get; set; }
		public string ChartKey { get; set; }
		// JSON text, read by the page
		public string Data { get; set; }
		public int SortOrder { get; set; }
	}

	// A card on a mystery's cork board
	public class ExplanationDto
	{
		public string Title { get; set; }
		public string ProposedBy { get; set; }
		public string ForText { get; set; }
		public string AgainstText { get; set; }
		public string Verdict { get; set; }
		public string Stamp { get; set; }
		public string ImageUrl { get; set; }
		public string ImageCredit { get; set; }
		public string ImageFit { get; set; }
		public int SortOrder { get; set; }
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
