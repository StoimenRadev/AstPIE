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
	// Who a photo shows. A link is known by its photo and its person: /api/admin/media-people/12-3
	[Route("api/admin/media-people")]
	public class AdminMediaPeopleController : AdminControllerBase
	{
		public AdminMediaPeopleController(AppDbContext db) : base(db)
		{
		}

		// GET /api/admin/media-people or /api/admin/media-people?mediaItemId=12
		[HttpGet]
		public async Task<ActionResult<List<MediaItemPersonAdminDto>>> GetAll(int? mediaItemId)
		{
			List<MediaItemPerson> links = await (
				from l in db.MediaItemPeople
				where mediaItemId == null || l.MediaItemId == mediaItemId
				orderby l.MediaItemId, l.PersonId
				select l).ToListAsync();

			List<MediaItemPersonAdminDto> result = new List<MediaItemPersonAdminDto>();
			foreach (MediaItemPerson link in links)
			{
				result.Add(ToDto(link));
			}
			return result;
		}

		// GET /api/admin/media-people/12-3
		[HttpGet("{mediaItemId:int}-{personId:int}")]
		public async Task<ActionResult<MediaItemPersonAdminDto>> GetOne(int mediaItemId, int personId)
		{
			MediaItemPerson link = await db.MediaItemPeople.FindAsync(mediaItemId, personId);
			if (link == null)
			{
				return NotFound();
			}
			return ToDto(link);
		}

		// POST /api/admin/media-people
		[HttpPost]
		public async Task<ActionResult> Create(MediaItemPersonInput input)
		{
			MediaItem photo = await db.MediaItems.FindAsync(input.MediaItemId);
			if (photo == null)
			{
				return FieldError("MediaItemId", "This photo does not exist.");
			}
			bool personExists = await (from x in db.People where x.Id == input.PersonId select x).AnyAsync();
			if (!personExists)
			{
				return FieldError("PersonId", "This person does not exist.");
			}
			// The photo folder only lists the people of the case
			bool inCase = await (from x in db.EntryPeople where x.EntryId == photo.EntryId && x.PersonId == input.PersonId select x).AnyAsync();
			if (!inCase)
			{
				return FieldError("PersonId", "This person is not one of the people of this photo's case. Add them to the case first.");
			}
			MediaItemPerson existing = await db.MediaItemPeople.FindAsync(input.MediaItemId, input.PersonId);
			if (existing != null)
			{
				return ConflictMessage("This photo already shows this person.");
			}

			MediaItemPerson link = new MediaItemPerson();
			link.MediaItemId = input.MediaItemId;
			link.PersonId = input.PersonId;
			db.MediaItemPeople.Add(link);
			await db.SaveChangesAsync();
			return Created("/api/admin/media-people/" + link.MediaItemId + "-" + link.PersonId, new { id = link.MediaItemId + "-" + link.PersonId });
		}

		// PUT /api/admin/media-people/12-3
		// The two ends of a link can't change: remove it and add a new one.
		[HttpPut("{mediaItemId:int}-{personId:int}")]
		public async Task<ActionResult> Update(int mediaItemId, int personId, MediaItemPersonInput input)
		{
			MediaItemPerson link = await db.MediaItemPeople.FindAsync(mediaItemId, personId);
			if (link == null)
			{
				return NotFound();
			}
			if (input.MediaItemId != mediaItemId || input.PersonId != personId)
			{
				return FieldError("PersonId", "The two ends of a link can't be changed. Remove this link and add a new one.");
			}
			return NoContent();
		}

		// DELETE /api/admin/media-people/12-3
		// Removes only the link: the photo and the person stay
		[HttpDelete("{mediaItemId:int}-{personId:int}")]
		public async Task<ActionResult> Delete(int mediaItemId, int personId)
		{
			MediaItemPerson link = await db.MediaItemPeople.FindAsync(mediaItemId, personId);
			if (link == null)
			{
				return NotFound();
			}

			db.MediaItemPeople.Remove(link);
			await db.SaveChangesAsync();
			return NoContent();
		}

		private static MediaItemPersonAdminDto ToDto(MediaItemPerson link)
		{
			MediaItemPersonAdminDto dto = new MediaItemPersonAdminDto();
			dto.Id = link.MediaItemId + "-" + link.PersonId;
			dto.MediaItemId = link.MediaItemId;
			dto.PersonId = link.PersonId;
			return dto;
		}
	}
}
