using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AstPIE.Api.Data;
using AstPIE.Api.Dtos;
using AstPIE.Api.Models;

namespace AstPIE.Api.Controllers
{
	// Explore: missions and mysteries
	[ApiController]
	[Route("api/entries")]
	public class EntriesController : ControllerBase
	{
		private readonly AppDbContext db;

		public EntriesController(AppDbContext db)
		{
			this.db = db;
		}

		// GET /api/entries?kind=Mission
		// Published entries, oldest first. Without kind: missions and mysteries together.
		[HttpGet]
		public async Task<ActionResult<List<EntryListItemDto>>> GetEntries(EntryKind? kind)
		{
			List<EntryListItemDto> entries = await (
				from entry in db.Entries
				where entry.IsPublished && (kind == null || entry.Kind == kind)
				orderby entry.StartDate, entry.Title
				select new EntryListItemDto
				{
					Slug = entry.Slug,
					Kind = entry.Kind,
					Title = entry.Title,
					Tagline = entry.Tagline,
					Summary = entry.Summary,
					CoverImageUrl = entry.CoverImageUrl,
					Country = entry.Country,
					Desk = entry.Desk,
					StartDate = entry.StartDate,
					EndDate = entry.EndDate,
					Agency = entry.Agency,
					MissionOutcome = entry.MissionOutcome,
					EvidenceStatus = entry.EvidenceStatus,
					StatusNote = entry.StatusNote,
					PageCount = entry.Chapters.Count()
				}).ToListAsync();

			return entries;
		}

		// GET /api/entries/sputnik-1
		// One case with its cover sheet, pages, desk items, people and cork board, or 404
		[HttpGet("{slug}")]
		public async Task<ActionResult<EntryDto>> GetEntry(string slug)
		{
			Entry entry = await (
				from e in db.Entries
				where e.Slug == slug && e.IsPublished
				select e).FirstOrDefaultAsync();

			if (entry == null)
			{
				return NotFound();
			}

			EntryDto result = new EntryDto();
			result.Slug = entry.Slug;
			result.Kind = entry.Kind;
			result.Title = entry.Title;
			result.Tagline = entry.Tagline;
			result.Summary = entry.Summary;
			result.CoverImageUrl = entry.CoverImageUrl;
			result.CoverCaption = entry.CoverCaption;
			result.CoverCredit = entry.CoverCredit;
			result.Country = entry.Country;
			result.Desk = entry.Desk;
			result.StartDate = entry.StartDate;
			result.EndDate = entry.EndDate;
			result.Agency = entry.Agency;
			result.MissionOutcome = entry.MissionOutcome;
			result.EvidenceStatus = entry.EvidenceStatus;
			result.StatusNote = entry.StatusNote;
			result.RadioMinMHz = entry.RadioMinMHz;
			result.RadioMaxMHz = entry.RadioMaxMHz;
			result.RelatedTopic = await FindTopicLink(entry.RelatedTopicId);

			result.Facts = await (
				from fact in db.EntryFacts
				where fact.EntryId == entry.Id
				orderby fact.SortOrder
				select new FactDto
				{
					Label = fact.Label,
					Value = fact.Value
				}).ToListAsync();

			result.People = await (
				from link in db.EntryPeople
				join person in db.People on link.PersonId equals person.Id
				where link.EntryId == entry.Id
				orderby link.SortOrder
				select new EntryPersonDto
				{
					PersonId = person.Id,
					FullName = person.FullName,
					Nationality = person.Nationality,
					BirthDate = person.BirthDate,
					DeathDate = person.DeathDate,
					PhotoUrl = person.PhotoUrl,
					PhotoCredit = person.PhotoCredit,
					Bio = person.Bio,
					Role = link.Role,
					IsCrew = link.IsCrew
				}).ToListAsync();

			List<Chapter> chapters = await (
				from chapter in db.Chapters
				where chapter.EntryId == entry.Id
				orderby chapter.SortOrder
				select chapter).ToListAsync();

			List<MediaItem> media = await (
				from item in db.MediaItems
				where item.EntryId == entry.Id
				orderby item.SortOrder
				select item).ToListAsync();

			// Who each photo of the case shows
			List<MediaItemPerson> shown = await (
				from link in db.MediaItemPeople
				join item in db.MediaItems on link.MediaItemId equals item.Id
				where item.EntryId == entry.Id
				select link).ToListAsync();

			List<Explanation> explanations = await (
				from explanation in db.Explanations
				where explanation.EntryId == entry.Id
				orderby explanation.SortOrder
				select explanation).ToListAsync();

			// Things without a page lie on the desk with the case summary
			foreach (MediaItem item in media)
			{
				if (item.ChapterId == null)
				{
					result.Media.Add(ToDto(item, shown));
				}
			}
			foreach (Explanation explanation in explanations)
			{
				if (explanation.ChapterId == null)
				{
					result.Explanations.Add(ToDto(explanation));
				}
			}

			foreach (Chapter chapter in chapters)
			{
				ChapterDto page = new ChapterDto();
				page.Section = chapter.Section;
				page.TabLabel = chapter.TabLabel;
				page.Title = chapter.Title;
				page.Place = chapter.Place;
				page.DateLabel = chapter.DateLabel;
				page.Summary = chapter.Summary;
				page.Body = chapter.Body;
				page.MarginNote = chapter.MarginNote;
				page.ShowsRadioHint = chapter.ShowsRadioHint;

				foreach (MediaItem item in media)
				{
					if (item.ChapterId == chapter.Id)
					{
						page.Media.Add(ToDto(item, shown));
					}
				}
				foreach (Explanation explanation in explanations)
				{
					if (explanation.ChapterId == chapter.Id)
					{
						page.Explanations.Add(ToDto(explanation));
					}
				}
				result.Chapters.Add(page);
			}

			return result;
		}

		private static MediaItemDto ToDto(MediaItem item, List<MediaItemPerson> shown)
		{
			MediaItemDto dto = new MediaItemDto();
			dto.Type = item.Type;
			dto.Url = item.Url;
			dto.Title = item.Title;
			dto.Caption = item.Caption;
			dto.Credit = item.Credit;
			dto.License = item.License;
			dto.Text = item.Text;
			dto.Author = item.Author;
			dto.DateLabel = item.DateLabel;
			dto.PosterUrl = item.PosterUrl;
			dto.LengthLabel = item.LengthLabel;
			dto.PersonId = item.PersonId;
			dto.ShownPeopleIds = (
				from link in shown
				where link.MediaItemId == item.Id
				orderby link.PersonId
				select link.PersonId).ToList();
			dto.Style = item.Style;
			dto.ChartKey = item.ChartKey;
			dto.Data = item.Data;
			dto.SortOrder = item.SortOrder;
			return dto;
		}

		private static ExplanationDto ToDto(Explanation explanation)
		{
			ExplanationDto dto = new ExplanationDto();
			dto.Title = explanation.Title;
			dto.ProposedBy = explanation.ProposedBy;
			dto.ForText = explanation.ForText;
			dto.AgainstText = explanation.AgainstText;
			dto.Verdict = explanation.Verdict;
			dto.Stamp = explanation.Stamp;
			dto.ImageUrl = explanation.ImageUrl;
			dto.ImageCredit = explanation.ImageCredit;
			dto.ImageFit = explanation.ImageFit;
			dto.SortOrder = explanation.SortOrder;
			return dto;
		}

		// The linked Learn topic, or null when there is none (or it is not published)
		private async Task<TopicLinkDto> FindTopicLink(int? topicId)
		{
			if (topicId == null)
			{
				return null;
			}

			return await (
				from topic in db.Topics
				where topic.Id == topicId && topic.IsPublished
				select new TopicLinkDto
				{
					Slug = topic.Slug,
					Title = topic.Title
				}).FirstOrDefaultAsync();
		}
	}
}
