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
	// The rows on the left page of an object's logbook
	[Route("api/admin/catalogue-facts")]
	public class AdminCatalogueFactsController : AdminControllerBase
	{
		public AdminCatalogueFactsController(AppDbContext db) : base(db)
		{
		}

		// GET /api/admin/catalogue-facts or /api/admin/catalogue-facts?catalogueObjectId=3
		[HttpGet]
		public async Task<ActionResult<List<CatalogueFactAdminDto>>> GetAll(int? catalogueObjectId)
		{
			List<CatalogueFact> rows = await (
				from x in db.CatalogueFacts
				where catalogueObjectId == null || x.CatalogueObjectId == catalogueObjectId
				orderby x.CatalogueObjectId, x.SortOrder
				select x).ToListAsync();

			List<CatalogueFactAdminDto> result = new List<CatalogueFactAdminDto>();
			foreach (CatalogueFact row in rows)
			{
				result.Add(ToDto(row));
			}
			return result;
		}

		// GET /api/admin/catalogue-facts/5
		[HttpGet("{id:int}")]
		public async Task<ActionResult<CatalogueFactAdminDto>> GetOne(int id)
		{
			CatalogueFact item = await db.CatalogueFacts.FindAsync(id);
			if (item == null)
			{
				return NotFound();
			}
			return ToDto(item);
		}

		// POST /api/admin/catalogue-facts
		[HttpPost]
		public async Task<ActionResult> Create(CatalogueFactInput input)
		{
			if (!await ParentExists(input.CatalogueObjectId))
			{
				return FieldError("CatalogueObjectId", "This catalogue object does not exist.");
			}
			CatalogueFact item = new CatalogueFact();
			Apply(input, item);
			db.CatalogueFacts.Add(item);
			await db.SaveChangesAsync();
			return Created("/api/admin/catalogue-facts/" + item.Id, new { id = item.Id });
		}

		// PUT /api/admin/catalogue-facts/5
		[HttpPut("{id:int}")]
		public async Task<ActionResult> Update(int id, CatalogueFactInput input)
		{
			CatalogueFact item = await db.CatalogueFacts.FindAsync(id);
			if (item == null)
			{
				return NotFound();
			}
			if (!await ParentExists(input.CatalogueObjectId))
			{
				return FieldError("CatalogueObjectId", "This catalogue object does not exist.");
			}
			Apply(input, item);
			await db.SaveChangesAsync();
			return NoContent();
		}

		// DELETE /api/admin/catalogue-facts/5
		[HttpDelete("{id:int}")]
		public async Task<ActionResult> Delete(int id)
		{
			CatalogueFact item = await db.CatalogueFacts.FindAsync(id);
			if (item == null)
			{
				return NotFound();
			}

			db.CatalogueFacts.Remove(item);
			await db.SaveChangesAsync();
			return NoContent();
		}

		private async Task<bool> ParentExists(int id)
		{
			return await (from p in db.CatalogueObjects where p.Id == id select p).AnyAsync();
		}

		private static CatalogueFactAdminDto ToDto(CatalogueFact item)
		{
			CatalogueFactAdminDto dto = new CatalogueFactAdminDto();
			dto.Id = item.Id;
			dto.CatalogueObjectId = item.CatalogueObjectId;
			dto.Label = item.Label;
			dto.Value = item.Value;
			dto.SortOrder = item.SortOrder;
			return dto;
		}

		private static void Apply(CatalogueFactInput input, CatalogueFact item)
		{
			item.CatalogueObjectId = input.CatalogueObjectId;
			item.Label = input.Label.Trim();
			item.Value = input.Value.Trim();
			item.SortOrder = input.SortOrder;
		}
	}
}
