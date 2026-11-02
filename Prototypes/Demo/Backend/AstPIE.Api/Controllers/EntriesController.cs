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
					Summary = entry.Summary,
					CoverImageUrl = entry.CoverImageUrl,
					StartDate = entry.StartDate,
					EndDate = entry.EndDate,
					Agency = entry.Agency,
					MissionOutcome = entry.MissionOutcome,
					EvidenceStatus = entry.EvidenceStatus
				}).ToListAsync();

			return entries;
		}

		// GET /api/entries/sputnik-1
		// One entry with its chapters, people, media and "Physics behind it", or 404
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
			result.Summary = entry.Summary;
			result.CoverImageUrl = entry.CoverImageUrl;
			result.StartDate = entry.StartDate;
			result.EndDate = entry.EndDate;
			result.Agency = entry.Agency;
			result.MissionOutcome = entry.MissionOutcome;
			result.EvidenceStatus = entry.EvidenceStatus;
			result.RelatedTopic = await FindTopicLink(entry.RelatedTopicId);

			result.Chapters = await (
				from chapter in db.Chapters
				where chapter.EntryId == entry.Id
				orderby chapter.SortOrder
				select new ChapterDto
				{
					Title = chapter.Title,
					DateLabel = chapter.DateLabel,
					Summary = chapter.Summary,
					Body = chapter.Body
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
					PhotoUrl = person.PhotoUrl,
					Role = link.Role,
					IsCrew = link.IsCrew
				}).ToListAsync();

			result.Media = await (
				from media in db.MediaItems
				where media.EntryId == entry.Id
				orderby media.SortOrder
				select new MediaItemDto
				{
					Type = media.Type,
					Url = media.Url,
					Title = media.Title,
					Caption = media.Caption,
					Credit = media.Credit,
					License = media.License
				}).ToListAsync();

			return result;
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
