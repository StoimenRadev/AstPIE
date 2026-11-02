using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AstPIE.Api.Data;
using AstPIE.Api.Dtos;
using AstPIE.Api.Models;

namespace AstPIE.Api.Controllers
{
	// The people shown as a centred card on entry pages
	[ApiController]
	[Route("api/people")]
	public class PeopleController : ControllerBase
	{
		private readonly AppDbContext db;

		public PeopleController(AppDbContext db)
		{
			this.db = db;
		}

		// GET /api/people/3
		// One person with the published entries they were part of, or 404
		[HttpGet("{id:int}")]
		public async Task<ActionResult<PersonDto>> GetPerson(int id)
		{
			Person person = await (
				from p in db.People
				where p.Id == id
				select p).FirstOrDefaultAsync();

			if (person == null)
			{
				return NotFound();
			}

			PersonDto result = new PersonDto();
			result.Id = person.Id;
			result.FullName = person.FullName;
			result.Nationality = person.Nationality;
			result.BirthDate = person.BirthDate;
			result.DeathDate = person.DeathDate;
			result.PhotoUrl = person.PhotoUrl;
			result.Bio = person.Bio;

			result.Entries = await (
				from link in db.EntryPeople
				join entry in db.Entries on link.EntryId equals entry.Id
				where link.PersonId == person.Id && entry.IsPublished
				orderby entry.StartDate
				select new PersonEntryDto
				{
					Slug = entry.Slug,
					Kind = entry.Kind,
					Title = entry.Title,
					Role = link.Role
				}).ToListAsync();

			return result;
		}
	}
}
