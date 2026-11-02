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
	// Who was part of which entry. A link is known by its entry and person: /api/admin/entry-people/3-7
	[Route("api/admin/entry-people")]
	public class AdminEntryPeopleController : AdminControllerBase
	{
		public AdminEntryPeopleController(AppDbContext db) : base(db)
		{
		}

		// GET /api/admin/entry-people or /api/admin/entry-people?entryId=3
		[HttpGet]
		public async Task<ActionResult<List<EntryPersonAdminDto>>> GetAll(int? entryId)
		{
			List<EntryPerson> links = await (
				from l in db.EntryPeople
				where entryId == null || l.EntryId == entryId
				orderby l.EntryId, l.SortOrder
				select l).ToListAsync();

			List<EntryPersonAdminDto> result = new List<EntryPersonAdminDto>();
			foreach (EntryPerson link in links)
			{
				result.Add(ToDto(link));
			}
			return result;
		}

		// GET /api/admin/entry-people/3-7
		[HttpGet("{entryId:int}-{personId:int}")]
		public async Task<ActionResult<EntryPersonAdminDto>> GetOne(int entryId, int personId)
		{
			EntryPerson link = await db.EntryPeople.FindAsync(entryId, personId);
			if (link == null)
			{
				return NotFound();
			}
			return ToDto(link);
		}

		// POST /api/admin/entry-people
		[HttpPost]
		public async Task<ActionResult> Create(EntryPersonInput input)
		{
			bool entryExists = await (from e in db.Entries where e.Id == input.EntryId select e).AnyAsync();
			if (!entryExists)
			{
				return FieldError("EntryId", "This entry does not exist.");
			}
			bool personExists = await (from p in db.People where p.Id == input.PersonId select p).AnyAsync();
			if (!personExists)
			{
				return FieldError("PersonId", "This person does not exist.");
			}
			EntryPerson existing = await db.EntryPeople.FindAsync(input.EntryId, input.PersonId);
			if (existing != null)
			{
				return ConflictMessage("This person is already part of this entry. Edit that link instead.");
			}

			EntryPerson link = new EntryPerson();
			link.EntryId = input.EntryId;
			link.PersonId = input.PersonId;
			Apply(input, link);
			db.EntryPeople.Add(link);
			await db.SaveChangesAsync();
			return Created("/api/admin/entry-people/" + link.EntryId + "-" + link.PersonId, new { id = link.EntryId + "-" + link.PersonId });
		}

		// PUT /api/admin/entry-people/3-7
		// Changes the role, crew flag and order. To link another person, remove this link and add a new one.
		[HttpPut("{entryId:int}-{personId:int}")]
		public async Task<ActionResult> Update(int entryId, int personId, EntryPersonInput input)
		{
			EntryPerson link = await db.EntryPeople.FindAsync(entryId, personId);
			if (link == null)
			{
				return NotFound();
			}
			if (input.EntryId != entryId || input.PersonId != personId)
			{
				return FieldError("PersonId", "The entry and person of a link can't be changed. Remove this link and add a new one.");
			}

			Apply(input, link);
			await db.SaveChangesAsync();
			return NoContent();
		}

		// DELETE /api/admin/entry-people/3-7
		// Removes only the link: the entry and the person stay
		[HttpDelete("{entryId:int}-{personId:int}")]
		public async Task<ActionResult> Delete(int entryId, int personId)
		{
			EntryPerson link = await db.EntryPeople.FindAsync(entryId, personId);
			if (link == null)
			{
				return NotFound();
			}

			db.EntryPeople.Remove(link);
			await db.SaveChangesAsync();
			return NoContent();
		}

		private static EntryPersonAdminDto ToDto(EntryPerson link)
		{
			EntryPersonAdminDto dto = new EntryPersonAdminDto();
			dto.Id = link.EntryId + "-" + link.PersonId;
			dto.EntryId = link.EntryId;
			dto.PersonId = link.PersonId;
			dto.Role = link.Role;
			dto.IsCrew = link.IsCrew;
			dto.SortOrder = link.SortOrder;
			return dto;
		}

		private static void Apply(EntryPersonInput input, EntryPerson link)
		{
			link.Role = input.Role.Trim();
			link.IsCrew = input.IsCrew;
			link.SortOrder = input.SortOrder;
		}
	}
}
