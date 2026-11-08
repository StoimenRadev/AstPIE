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
	// "Question 1, 2, 3" on a topic page, with their answers
	[Route("api/admin/check-questions")]
	public class AdminCheckQuestionsController : AdminControllerBase
	{
		public AdminCheckQuestionsController(AppDbContext db) : base(db)
		{
		}

		// GET /api/admin/check-questions or /api/admin/check-questions?topicId=3
		[HttpGet]
		public async Task<ActionResult<List<CheckQuestionAdminDto>>> GetAll(int? topicId)
		{
			List<CheckQuestion> rows = await (
				from x in db.CheckQuestions
				where topicId == null || x.TopicId == topicId
				orderby x.TopicId, x.SortOrder
				select x).ToListAsync();

			List<CheckQuestionAdminDto> result = new List<CheckQuestionAdminDto>();
			foreach (CheckQuestion row in rows)
			{
				result.Add(ToDto(row));
			}
			return result;
		}

		// GET /api/admin/check-questions/5
		[HttpGet("{id:int}")]
		public async Task<ActionResult<CheckQuestionAdminDto>> GetOne(int id)
		{
			CheckQuestion item = await db.CheckQuestions.FindAsync(id);
			if (item == null)
			{
				return NotFound();
			}
			return ToDto(item);
		}

		// POST /api/admin/check-questions
		[HttpPost]
		public async Task<ActionResult> Create(CheckQuestionInput input)
		{
			if (!await ParentExists(input.TopicId))
			{
				return FieldError("TopicId", "This topic does not exist.");
			}
			CheckQuestion item = new CheckQuestion();
			Apply(input, item);
			db.CheckQuestions.Add(item);
			await db.SaveChangesAsync();
			return Created("/api/admin/check-questions/" + item.Id, new { id = item.Id });
		}

		// PUT /api/admin/check-questions/5
		[HttpPut("{id:int}")]
		public async Task<ActionResult> Update(int id, CheckQuestionInput input)
		{
			CheckQuestion item = await db.CheckQuestions.FindAsync(id);
			if (item == null)
			{
				return NotFound();
			}
			if (!await ParentExists(input.TopicId))
			{
				return FieldError("TopicId", "This topic does not exist.");
			}
			Apply(input, item);
			await db.SaveChangesAsync();
			return NoContent();
		}

		// DELETE /api/admin/check-questions/5
		[HttpDelete("{id:int}")]
		public async Task<ActionResult> Delete(int id)
		{
			CheckQuestion item = await db.CheckQuestions.FindAsync(id);
			if (item == null)
			{
				return NotFound();
			}

			db.CheckQuestions.Remove(item);
			await db.SaveChangesAsync();
			return NoContent();
		}

		private async Task<bool> ParentExists(int id)
		{
			return await (from p in db.Topics where p.Id == id select p).AnyAsync();
		}

		private static CheckQuestionAdminDto ToDto(CheckQuestion item)
		{
			CheckQuestionAdminDto dto = new CheckQuestionAdminDto();
			dto.Id = item.Id;
			dto.TopicId = item.TopicId;
			dto.AfterSection = item.AfterSection;
			dto.Question = item.Question;
			dto.Answer = item.Answer;
			dto.SortOrder = item.SortOrder;
			return dto;
		}

		private static void Apply(CheckQuestionInput input, CheckQuestion item)
		{
			item.TopicId = input.TopicId;
			item.AfterSection = input.AfterSection;
			item.Question = input.Question.Trim();
			item.Answer = input.Answer.Trim();
			item.SortOrder = input.SortOrder;
		}
	}
}
