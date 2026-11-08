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
	[Route("api/admin/topics")]
	public class AdminTopicsController : AdminControllerBase
	{
		public AdminTopicsController(AppDbContext db) : base(db)
		{
		}

		// GET /api/admin/topics - all topics, also unpublished
		[HttpGet]
		public async Task<ActionResult<List<TopicAdminDto>>> GetAll()
		{
			return await (
				from topic in db.Topics
				orderby topic.Branch, topic.SortOrder
				select new TopicAdminDto
				{
					Id = topic.Id,
					Question = topic.Question,
					PredictQuestion = topic.PredictQuestion,
					PredictOptions = topic.PredictOptions,
					PredictAnswer = topic.PredictAnswer,
					LawStatement = topic.LawStatement,
					LawEquation = topic.LawEquation,
					ReadAloud = topic.ReadAloud,
					RecapPoints = topic.RecapPoints,
					ExploreEntryId = topic.ExploreEntryId,
					ExploreLine = topic.ExploreLine,
					Slug = topic.Slug,
					Title = topic.Title,
					Branch = topic.Branch,
					Summary = topic.Summary,
					Content = topic.Content,
					SortOrder = topic.SortOrder,
					IsPublished = topic.IsPublished
				}).ToListAsync();
		}

		// GET /api/admin/topics/5
		[HttpGet("{id:int}")]
		public async Task<ActionResult<TopicAdminDto>> GetOne(int id)
		{
			TopicAdminDto topic = await (
				from t in db.Topics
				where t.Id == id
				select new TopicAdminDto
				{
					Id = t.Id,
					Question = t.Question,
					PredictQuestion = t.PredictQuestion,
					PredictOptions = t.PredictOptions,
					PredictAnswer = t.PredictAnswer,
					LawStatement = t.LawStatement,
					LawEquation = t.LawEquation,
					ReadAloud = t.ReadAloud,
					RecapPoints = t.RecapPoints,
					ExploreEntryId = t.ExploreEntryId,
					ExploreLine = t.ExploreLine,
					Slug = t.Slug,
					Title = t.Title,
					Branch = t.Branch,
					Summary = t.Summary,
					Content = t.Content,
					SortOrder = t.SortOrder,
					IsPublished = t.IsPublished
				}).FirstOrDefaultAsync();

			if (topic == null)
			{
				return NotFound();
			}
			return topic;
		}

		// POST /api/admin/topics
		[HttpPost]
		public async Task<ActionResult> Create(TopicInput input)
		{
			if (await SlugTaken(input.Slug, 0))
			{
				return ConflictMessage("Another topic already uses the slug \"" + input.Slug + "\".");
			}

			Topic topic = new Topic();
			if (input.ExploreEntryId != null && !await (from e in db.Entries where e.Id == input.ExploreEntryId select e).AnyAsync())
			{
				return FieldError("ExploreEntryId", "This case does not exist.");
			}
			Apply(input, topic);
			db.Topics.Add(topic);
			await db.SaveChangesAsync();
			return Created("/api/admin/topics/" + topic.Id, new { id = topic.Id });
		}

		// PUT /api/admin/topics/5
		[HttpPut("{id:int}")]
		public async Task<ActionResult> Update(int id, TopicInput input)
		{
			Topic topic = await db.Topics.FindAsync(id);
			if (topic == null)
			{
				return NotFound();
			}
			if (await SlugTaken(input.Slug, id))
			{
				return ConflictMessage("Another topic already uses the slug \"" + input.Slug + "\".");
			}

			if (input.ExploreEntryId != null && !await (from e in db.Entries where e.Id == input.ExploreEntryId select e).AnyAsync())
			{
				return FieldError("ExploreEntryId", "This case does not exist.");
			}
			Apply(input, topic);
			await db.SaveChangesAsync();
			return NoContent();
		}

		// DELETE /api/admin/topics/5
		// Blocked while the topic has simulations. Links from entries and catalogue objects are cleared.
		[HttpDelete("{id:int}")]
		public async Task<ActionResult> Delete(int id)
		{
			Topic topic = await db.Topics.FindAsync(id);
			if (topic == null)
			{
				return NotFound();
			}

			int simulations = await (from s in db.Simulations where s.TopicId == id select s).CountAsync();
			if (simulations > 0)
			{
				return ConflictMessage("This topic still has " + simulations + " simulation(s). Delete or move them first.");
			}

			db.Topics.Remove(topic);
			await db.SaveChangesAsync();
			return NoContent();
		}

		private async Task<bool> SlugTaken(string slug, int ownId)
		{
			return await (from t in db.Topics where t.Slug == slug && t.Id != ownId select t).AnyAsync();
		}

		private static void Apply(TopicInput input, Topic topic)
		{
			topic.Slug = input.Slug;
			topic.Title = input.Title.Trim();
			topic.Branch = input.Branch.Trim();
			topic.Summary = input.Summary.Trim();
			topic.Content = input.Content;
			topic.SortOrder = input.SortOrder;
			topic.IsPublished = input.IsPublished;
			topic.Question = input.Question.Trim();
			topic.PredictQuestion = input.PredictQuestion.Trim();
			topic.PredictOptions = CleanList(input.PredictOptions);
			topic.PredictAnswer = input.PredictAnswer;
			topic.LawStatement = input.LawStatement.Trim();
			topic.LawEquation = input.LawEquation;
			topic.ReadAloud = input.ReadAloud.Trim();
			topic.RecapPoints = CleanList(input.RecapPoints);
			topic.ExploreEntryId = input.ExploreEntryId;
			topic.ExploreLine = Optional(input.ExploreLine);
		}
	}
}
