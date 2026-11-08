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
	// The rows of "Reading it symbol by symbol" under a topic's equation
	[Route("api/admin/topic-symbols")]
	public class AdminTopicSymbolsController : AdminControllerBase
	{
		public AdminTopicSymbolsController(AppDbContext db) : base(db)
		{
		}

		// GET /api/admin/topic-symbols or /api/admin/topic-symbols?topicId=3
		[HttpGet]
		public async Task<ActionResult<List<TopicSymbolAdminDto>>> GetAll(int? topicId)
		{
			List<TopicSymbol> rows = await (
				from x in db.TopicSymbols
				where topicId == null || x.TopicId == topicId
				orderby x.TopicId, x.SortOrder
				select x).ToListAsync();

			List<TopicSymbolAdminDto> result = new List<TopicSymbolAdminDto>();
			foreach (TopicSymbol row in rows)
			{
				result.Add(ToDto(row));
			}
			return result;
		}

		// GET /api/admin/topic-symbols/5
		[HttpGet("{id:int}")]
		public async Task<ActionResult<TopicSymbolAdminDto>> GetOne(int id)
		{
			TopicSymbol item = await db.TopicSymbols.FindAsync(id);
			if (item == null)
			{
				return NotFound();
			}
			return ToDto(item);
		}

		// POST /api/admin/topic-symbols
		[HttpPost]
		public async Task<ActionResult> Create(TopicSymbolInput input)
		{
			if (!await ParentExists(input.TopicId))
			{
				return FieldError("TopicId", "This topic does not exist.");
			}
			TopicSymbol item = new TopicSymbol();
			Apply(input, item);
			db.TopicSymbols.Add(item);
			await db.SaveChangesAsync();
			return Created("/api/admin/topic-symbols/" + item.Id, new { id = item.Id });
		}

		// PUT /api/admin/topic-symbols/5
		[HttpPut("{id:int}")]
		public async Task<ActionResult> Update(int id, TopicSymbolInput input)
		{
			TopicSymbol item = await db.TopicSymbols.FindAsync(id);
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

		// DELETE /api/admin/topic-symbols/5
		[HttpDelete("{id:int}")]
		public async Task<ActionResult> Delete(int id)
		{
			TopicSymbol item = await db.TopicSymbols.FindAsync(id);
			if (item == null)
			{
				return NotFound();
			}

			db.TopicSymbols.Remove(item);
			await db.SaveChangesAsync();
			return NoContent();
		}

		private async Task<bool> ParentExists(int id)
		{
			return await (from p in db.Topics where p.Id == id select p).AnyAsync();
		}

		private static TopicSymbolAdminDto ToDto(TopicSymbol item)
		{
			TopicSymbolAdminDto dto = new TopicSymbolAdminDto();
			dto.Id = item.Id;
			dto.TopicId = item.TopicId;
			dto.Symbol = item.Symbol;
			dto.Name = item.Name;
			dto.Meaning = item.Meaning;
			dto.Unit = item.Unit;
			dto.EquationPart = item.EquationPart;
			dto.SortOrder = item.SortOrder;
			return dto;
		}

		private static void Apply(TopicSymbolInput input, TopicSymbol item)
		{
			item.TopicId = input.TopicId;
			item.Symbol = input.Symbol.Trim();
			item.Name = input.Name.Trim();
			item.Meaning = input.Meaning.Trim();
			item.Unit = Optional(input.Unit);
			item.EquationPart = Optional(input.EquationPart);
			item.SortOrder = input.SortOrder;
		}
	}
}
