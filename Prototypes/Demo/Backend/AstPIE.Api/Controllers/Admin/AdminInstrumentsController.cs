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
	// Telescopes and spacecraft that studied catalogue objects
	[Route("api/admin/instruments")]
	public class AdminInstrumentsController : AdminControllerBase
	{
		public AdminInstrumentsController(AppDbContext db) : base(db)
		{
		}

		// GET /api/admin/instruments
		[HttpGet]
		public async Task<ActionResult<List<InstrumentAdminDto>>> GetAll()
		{
			List<Instrument> rows = await (
				from x in db.Instruments
				orderby x.Name
				select x).ToListAsync();

			List<InstrumentAdminDto> result = new List<InstrumentAdminDto>();
			foreach (Instrument row in rows)
			{
				result.Add(ToDto(row));
			}
			return result;
		}

		// GET /api/admin/instruments/5
		[HttpGet("{id:int}")]
		public async Task<ActionResult<InstrumentAdminDto>> GetOne(int id)
		{
			Instrument item = await db.Instruments.FindAsync(id);
			if (item == null)
			{
				return NotFound();
			}
			return ToDto(item);
		}

		// POST /api/admin/instruments
		[HttpPost]
		public async Task<ActionResult> Create(InstrumentInput input)
		{
			Instrument item = new Instrument();
			Apply(input, item);
			db.Instruments.Add(item);
			await db.SaveChangesAsync();
			return Created("/api/admin/instruments/" + item.Id, new { id = item.Id });
		}

		// PUT /api/admin/instruments/5
		[HttpPut("{id:int}")]
		public async Task<ActionResult> Update(int id, InstrumentInput input)
		{
			Instrument item = await db.Instruments.FindAsync(id);
			if (item == null)
			{
				return NotFound();
			}
			Apply(input, item);
			await db.SaveChangesAsync();
			return NoContent();
		}

		// DELETE /api/admin/instruments/5
		[HttpDelete("{id:int}")]
		public async Task<ActionResult> Delete(int id)
		{
			Instrument item = await db.Instruments.FindAsync(id);
			if (item == null)
			{
				return NotFound();
			}
			bool inUse = await (from l in db.CatalogueObjectInstruments where l.InstrumentId == id select l).AnyAsync();
			if (inUse)
			{
				return ConflictMessage("This instrument is still listed on a catalogue object. Remove it there first.");
			}

			db.Instruments.Remove(item);
			await db.SaveChangesAsync();
			return NoContent();
		}

		private static InstrumentAdminDto ToDto(Instrument item)
		{
			InstrumentAdminDto dto = new InstrumentAdminDto();
			dto.Id = item.Id;
			dto.Name = item.Name;
			dto.PhotoUrl = item.PhotoUrl;
			dto.PhotoCredit = item.PhotoCredit;
			dto.PhotoIsDark = item.PhotoIsDark;
			dto.Description = item.Description;
			return dto;
		}

		private static void Apply(InstrumentInput input, Instrument item)
		{
			item.Name = input.Name.Trim();
			item.PhotoUrl = input.PhotoUrl;
			item.PhotoCredit = Optional(input.PhotoCredit);
			item.PhotoIsDark = input.PhotoIsDark;
			item.Description = Optional(input.Description);
		}
	}
}
