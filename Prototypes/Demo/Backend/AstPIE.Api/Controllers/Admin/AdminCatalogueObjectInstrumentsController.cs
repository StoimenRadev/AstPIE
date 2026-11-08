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
	// Which instruments studied which object. A link is known by its object and instrument: /api/admin/catalogue-instruments/2-5
	[Route("api/admin/catalogue-instruments")]
	public class AdminCatalogueObjectInstrumentsController : AdminControllerBase
	{
		public AdminCatalogueObjectInstrumentsController(AppDbContext db) : base(db)
		{
		}

		// GET /api/admin/catalogue-instruments or /api/admin/catalogue-instruments?catalogueObjectId=3
		[HttpGet]
		public async Task<ActionResult<List<CatalogueObjectInstrumentAdminDto>>> GetAll(int? catalogueObjectId)
		{
			List<CatalogueObjectInstrument> links = await (
				from l in db.CatalogueObjectInstruments
				where catalogueObjectId == null || l.CatalogueObjectId == catalogueObjectId
				orderby l.CatalogueObjectId, l.InstrumentId
				select l).ToListAsync();

			List<CatalogueObjectInstrumentAdminDto> result = new List<CatalogueObjectInstrumentAdminDto>();
			foreach (CatalogueObjectInstrument link in links)
			{
				result.Add(ToDto(link));
			}
			return result;
		}

		// GET /api/admin/catalogue-instruments/1-4
		[HttpGet("{catalogueObjectId:int}-{instrumentId:int}")]
		public async Task<ActionResult<CatalogueObjectInstrumentAdminDto>> GetOne(int catalogueObjectId, int instrumentId)
		{
			CatalogueObjectInstrument link = await db.CatalogueObjectInstruments.FindAsync(catalogueObjectId, instrumentId);
			if (link == null)
			{
				return NotFound();
			}
			return ToDto(link);
		}

		// POST /api/admin/catalogue-instruments
		[HttpPost]
		public async Task<ActionResult> Create(CatalogueObjectInstrumentInput input)
		{
			bool firstExists = await (from x in db.CatalogueObjects where x.Id == input.CatalogueObjectId select x).AnyAsync();
			if (!firstExists)
			{
				return FieldError("CatalogueObjectId", "This catalogue object does not exist.");
			}
			bool secondExists = await (from x in db.Instruments where x.Id == input.InstrumentId select x).AnyAsync();
			if (!secondExists)
			{
				return FieldError("InstrumentId", "This instrument does not exist.");
			}
			CatalogueObjectInstrument existing = await db.CatalogueObjectInstruments.FindAsync(input.CatalogueObjectId, input.InstrumentId);
			if (existing != null)
			{
				return ConflictMessage("This instrument is already listed on this object.");
			}

			CatalogueObjectInstrument link = new CatalogueObjectInstrument();
			link.CatalogueObjectId = input.CatalogueObjectId;
			link.InstrumentId = input.InstrumentId;
			Apply(input, link);
			db.CatalogueObjectInstruments.Add(link);
			await db.SaveChangesAsync();
			return Created("/api/admin/catalogue-instruments/" + link.CatalogueObjectId + "-" + link.InstrumentId, new { id = link.CatalogueObjectId + "-" + link.InstrumentId });
		}

		// PUT /api/admin/catalogue-instruments/1-4
		// The two ends of a link can't change: remove it and add a new one.
		[HttpPut("{catalogueObjectId:int}-{instrumentId:int}")]
		public async Task<ActionResult> Update(int catalogueObjectId, int instrumentId, CatalogueObjectInstrumentInput input)
		{
			CatalogueObjectInstrument link = await db.CatalogueObjectInstruments.FindAsync(catalogueObjectId, instrumentId);
			if (link == null)
			{
				return NotFound();
			}
			if (input.CatalogueObjectId != catalogueObjectId || input.InstrumentId != instrumentId)
			{
				return FieldError("InstrumentId", "The two ends of a link can't be changed. Remove this link and add a new one.");
			}

			Apply(input, link);
			await db.SaveChangesAsync();
			return NoContent();
		}

		// DELETE /api/admin/catalogue-instruments/1-4
		// Removes only the link: both ends stay
		[HttpDelete("{catalogueObjectId:int}-{instrumentId:int}")]
		public async Task<ActionResult> Delete(int catalogueObjectId, int instrumentId)
		{
			CatalogueObjectInstrument link = await db.CatalogueObjectInstruments.FindAsync(catalogueObjectId, instrumentId);
			if (link == null)
			{
				return NotFound();
			}

			db.CatalogueObjectInstruments.Remove(link);
			await db.SaveChangesAsync();
			return NoContent();
		}

		private static CatalogueObjectInstrumentAdminDto ToDto(CatalogueObjectInstrument link)
		{
			CatalogueObjectInstrumentAdminDto dto = new CatalogueObjectInstrumentAdminDto();
			dto.Id = link.CatalogueObjectId + "-" + link.InstrumentId;
			dto.CatalogueObjectId = link.CatalogueObjectId;
			dto.InstrumentId = link.InstrumentId;
			dto.Role = link.Role;
			dto.SortOrder = link.SortOrder;
			return dto;
		}

		private static void Apply(CatalogueObjectInstrumentInput input, CatalogueObjectInstrument link)
		{
			link.Role = input.Role.Trim();
			link.SortOrder = input.SortOrder;
		}
	}
}
