using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AstPIE.Api.Data;
using AstPIE.Api.Dtos;
using AstPIE.Api.Models;

namespace AstPIE.Api.Controllers.Admin
{
	// Which key words a topic marks in its text. A link is known by its topic and key word: /api/admin/topic-key-words/1-4
	[Route("api/admin/topic-key-words")]
	public class AdminTopicKeyWordsController : AdminControllerBase
	{
		public AdminTopicKeyWordsController(AppDbContext db) : base(db)
		{
		}

		// GET /api/admin/topic-key-words or /api/admin/topic-key-words?topicId=3
		[HttpGet]
		public async Task<ActionResult<List<TopicKeyWordAdminDto>>> GetAll(int? topicId)
		{
			List<TopicKeyWord> links = await (
				from l in db.TopicKeyWords
				where topicId == null || l.TopicId == topicId
				orderby l.TopicId, l.KeyWordId
				select l).ToListAsync();

			List<TopicKeyWordAdminDto> result = new List<TopicKeyWordAdminDto>();
			foreach (TopicKeyWord link in links)
			{
				result.Add(ToDto(link));
			}
			return result;
		}

		// GET /api/admin/topic-key-words/1-4
		[HttpGet("{topicId:int}-{keyWordId:int}")]
		public async Task<ActionResult<TopicKeyWordAdminDto>> GetOne(int topicId, int keyWordId)
		{
			TopicKeyWord link = await db.TopicKeyWords.FindAsync(topicId, keyWordId);
			if (link == null)
			{
				return NotFound();
			}
			return ToDto(link);
		}

		// POST /api/admin/topic-key-words
		[HttpPost]
		public async Task<ActionResult> Create(TopicKeyWordInput input)
		{
			bool firstExists = await (from x in db.Topics where x.Id == input.TopicId select x).AnyAsync();
			if (!firstExists)
			{
				return FieldError("TopicId", "This topic does not exist.");
			}
			bool secondExists = await (from x in db.KeyWords where x.Id == input.KeyWordId select x).AnyAsync();
			if (!secondExists)
			{
				return FieldError("KeyWordId", "This key word does not exist.");
			}
			TopicKeyWord existing = await db.TopicKeyWords.FindAsync(input.TopicId, input.KeyWordId);
			if (existing != null)
			{
				return ConflictMessage("This topic already uses this key word.");
			}

			TopicKeyWord link = new TopicKeyWord();
			link.TopicId = input.TopicId;
			link.KeyWordId = input.KeyWordId;
			Apply(input, link);
			db.TopicKeyWords.Add(link);
			await db.SaveChangesAsync();
			return Created("/api/admin/topic-key-words/" + link.TopicId + "-" + link.KeyWordId, new { id = link.TopicId + "-" + link.KeyWordId });
		}

		// PUT /api/admin/topic-key-words/1-4
		// The two ends of a link can't change: remove it and add a new one.
		[HttpPut("{topicId:int}-{keyWordId:int}")]
		public async Task<ActionResult> Update(int topicId, int keyWordId, TopicKeyWordInput input)
		{
			TopicKeyWord link = await db.TopicKeyWords.FindAsync(topicId, keyWordId);
			if (link == null)
			{
				return NotFound();
			}
			if (input.TopicId != topicId || input.KeyWordId != keyWordId)
			{
				return FieldError("KeyWordId", "The two ends of a link can't be changed. Remove this link and add a new one.");
			}

			Apply(input, link);
			await db.SaveChangesAsync();
			return NoContent();
		}

		// DELETE /api/admin/topic-key-words/1-4
		// Removes only the link: both ends stay
		[HttpDelete("{topicId:int}-{keyWordId:int}")]
		public async Task<ActionResult> Delete(int topicId, int keyWordId)
		{
			TopicKeyWord link = await db.TopicKeyWords.FindAsync(topicId, keyWordId);
			if (link == null)
			{
				return NotFound();
			}

			db.TopicKeyWords.Remove(link);
			await db.SaveChangesAsync();
			return NoContent();
		}

		private static TopicKeyWordAdminDto ToDto(TopicKeyWord link)
		{
			TopicKeyWordAdminDto dto = new TopicKeyWordAdminDto();
			dto.Id = link.TopicId + "-" + link.KeyWordId;
			dto.TopicId = link.TopicId;
			dto.KeyWordId = link.KeyWordId;
			return dto;
		}

		private static void Apply(TopicKeyWordInput input, TopicKeyWord link)
		{
		}
	}
}
