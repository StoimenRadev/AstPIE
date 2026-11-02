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
	[Route("api/admin/people")]
	public class AdminPeopleController : AdminControllerBase
	{
		public AdminPeopleController(AppDbContext db) : base(db)
		{
		}

		// GET /api/admin/people
		[HttpGet]
		public async Task<ActionResult<List<PersonAdminDto>>> GetAll()
		{
			return await (
				from p in db.People
				orderby p.FullName
				select new PersonAdminDto
				{
					Id = p.Id,
					FullName = p.FullName,
					Nationality = p.Nationality,
					BirthDate = p.BirthDate,
					DeathDate = p.DeathDate,
					PhotoUrl = p.PhotoUrl,
					Bio = p.Bio
				}).ToListAsync();
		}

		// GET /api/admin/people/5
		[HttpGet("{id:int}")]
		public async Task<ActionResult<PersonAdminDto>> GetOne(int id)
		{
			PersonAdminDto person = await (
				from p in db.People
				where p.Id == id
				select new PersonAdminDto
				{
					Id = p.Id,
					FullName = p.FullName,
					Nationality = p.Nationality,
					BirthDate = p.BirthDate,
					DeathDate = p.DeathDate,
					PhotoUrl = p.PhotoUrl,
					Bio = p.Bio
				}).FirstOrDefaultAsync();

			if (person == null)
			{
				return NotFound();
			}
			return person;
		}

		// POST /api/admin/people
		[HttpPost]
		public async Task<ActionResult> Create(PersonInput input)
		{
			ActionResult problem = Check(input);
			if (problem != null)
			{
				return problem;
			}

			Person person = new Person();
			Apply(input, person);
			db.People.Add(person);
			await db.SaveChangesAsync();
			return Created("/api/admin/people/" + person.Id, new { id = person.Id });
		}

		// PUT /api/admin/people/5
		[HttpPut("{id:int}")]
		public async Task<ActionResult> Update(int id, PersonInput input)
		{
			Person person = await db.People.FindAsync(id);
			if (person == null)
			{
				return NotFound();
			}

			ActionResult problem = Check(input);
			if (problem != null)
			{
				return problem;
			}

			Apply(input, person);
			await db.SaveChangesAsync();
			return NoContent();
		}

		// DELETE /api/admin/people/5
		// Blocked while the person is linked to an entry
		[HttpDelete("{id:int}")]
		public async Task<ActionResult> Delete(int id)
		{
			Person person = await db.People.FindAsync(id);
			if (person == null)
			{
				return NotFound();
			}

			int links = await (from l in db.EntryPeople where l.PersonId == id select l).CountAsync();
			if (links > 0)
			{
				return ConflictMessage("This person is part of " + links + " entr" + EntryWord(links) + ". Remove them from those entries first.");
			}

			db.People.Remove(person);
			await db.SaveChangesAsync();
			return NoContent();
		}

		private static string EntryWord(int count)
		{
			if (count == 1)
			{
				return "y";
			}
			return "ies";
		}

		private ActionResult Check(PersonInput input)
		{
			if (input.BirthDate != null && input.DeathDate != null && input.DeathDate < input.BirthDate)
			{
				return FieldError("DeathDate", "The date of death can't be before the date of birth.");
			}
			return null;
		}

		private static void Apply(PersonInput input, Person person)
		{
			person.FullName = input.FullName.Trim();
			person.Nationality = input.Nationality.Trim();
			person.BirthDate = input.BirthDate;
			person.DeathDate = input.DeathDate;
			person.PhotoUrl = TextOrEmpty(input.PhotoUrl);
			person.Bio = input.Bio.Trim();
		}
	}
}
