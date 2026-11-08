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
	[Route("api/admin/catalogue")]
	public class AdminCatalogueController : AdminControllerBase
	{
		public AdminCatalogueController(AppDbContext db) : base(db)
		{
		}

		// GET /api/admin/catalogue - all objects, also unpublished
		[HttpGet]
		public async Task<ActionResult<List<CatalogueObjectAdminDto>>> GetAll()
		{
			return await (
				from o in db.CatalogueObjects
				orderby o.Name
				select new CatalogueObjectAdminDto
				{
					Id = o.Id,
					TypeLabel = o.TypeLabel,
					RightAscension = o.RightAscension,
					Declination = o.Declination,
					ImageCaption = o.ImageCaption,
					ImageCredit = o.ImageCredit,
					ImageIsDark = o.ImageIsDark,
					Slug = o.Slug,
					Name = o.Name,
					OtherNames = o.OtherNames,
					Type = o.Type,
					Constellation = o.Constellation,
					DistanceLy = o.DistanceLy,
					MassKg = o.MassKg,
					RadiusKm = o.RadiusKm,
					ApparentMagnitude = o.ApparentMagnitude,
					DiscoveredBy = o.DiscoveredBy,
					DiscoveryYear = o.DiscoveryYear,
					Summary = o.Summary,
					Description = o.Description,
					ImageUrl = o.ImageUrl,
					RelatedTopicId = o.RelatedTopicId,
					IsPublished = o.IsPublished
				}).ToListAsync();
		}

		// GET /api/admin/catalogue/5
		[HttpGet("{id:int}")]
		public async Task<ActionResult<CatalogueObjectAdminDto>> GetOne(int id)
		{
			CatalogueObjectAdminDto item = await (
				from o in db.CatalogueObjects
				where o.Id == id
				select new CatalogueObjectAdminDto
				{
					Id = o.Id,
					TypeLabel = o.TypeLabel,
					RightAscension = o.RightAscension,
					Declination = o.Declination,
					ImageCaption = o.ImageCaption,
					ImageCredit = o.ImageCredit,
					ImageIsDark = o.ImageIsDark,
					Slug = o.Slug,
					Name = o.Name,
					OtherNames = o.OtherNames,
					Type = o.Type,
					Constellation = o.Constellation,
					DistanceLy = o.DistanceLy,
					MassKg = o.MassKg,
					RadiusKm = o.RadiusKm,
					ApparentMagnitude = o.ApparentMagnitude,
					DiscoveredBy = o.DiscoveredBy,
					DiscoveryYear = o.DiscoveryYear,
					Summary = o.Summary,
					Description = o.Description,
					ImageUrl = o.ImageUrl,
					RelatedTopicId = o.RelatedTopicId,
					IsPublished = o.IsPublished
				}).FirstOrDefaultAsync();

			if (item == null)
			{
				return NotFound();
			}
			return item;
		}

		// POST /api/admin/catalogue
		[HttpPost]
		public async Task<ActionResult> Create(CatalogueObjectInput input)
		{
			ActionResult problem = await Check(input, 0);
			if (problem != null)
			{
				return problem;
			}

			CatalogueObject item = new CatalogueObject();
			Apply(input, item);
			db.CatalogueObjects.Add(item);
			await db.SaveChangesAsync();
			return Created("/api/admin/catalogue/" + item.Id, new { id = item.Id });
		}

		// PUT /api/admin/catalogue/5
		[HttpPut("{id:int}")]
		public async Task<ActionResult> Update(int id, CatalogueObjectInput input)
		{
			CatalogueObject item = await db.CatalogueObjects.FindAsync(id);
			if (item == null)
			{
				return NotFound();
			}

			ActionResult problem = await Check(input, id);
			if (problem != null)
			{
				return problem;
			}

			Apply(input, item);
			await db.SaveChangesAsync();
			return NoContent();
		}

		// DELETE /api/admin/catalogue/5
		[HttpDelete("{id:int}")]
		public async Task<ActionResult> Delete(int id)
		{
			CatalogueObject item = await db.CatalogueObjects.FindAsync(id);
			if (item == null)
			{
				return NotFound();
			}

			db.CatalogueObjects.Remove(item);
			await db.SaveChangesAsync();
			return NoContent();
		}

		// null when everything is fine
		private async Task<ActionResult> Check(CatalogueObjectInput input, int ownId)
		{
			if (input.RelatedTopicId != null)
			{
				bool topicExists = await (from t in db.Topics where t.Id == input.RelatedTopicId select t).AnyAsync();
				if (!topicExists)
				{
					return FieldError("RelatedTopicId", "This topic does not exist.");
				}
			}

			bool slugTaken = await (from o in db.CatalogueObjects where o.Slug == input.Slug && o.Id != ownId select o).AnyAsync();
			if (slugTaken)
			{
				return ConflictMessage("Another catalogue object already uses the slug \"" + input.Slug + "\".");
			}
			return null;
		}

		private static void Apply(CatalogueObjectInput input, CatalogueObject item)
		{
			item.Slug = input.Slug;
			item.Name = input.Name.Trim();
			item.OtherNames = Optional(input.OtherNames);
			item.Type = input.Type;
			item.Constellation = Optional(input.Constellation);
			item.DistanceLy = input.DistanceLy;
			item.MassKg = input.MassKg;
			item.RadiusKm = input.RadiusKm;
			item.ApparentMagnitude = input.ApparentMagnitude;
			item.DiscoveredBy = Optional(input.DiscoveredBy);
			item.DiscoveryYear = input.DiscoveryYear;
			item.Summary = input.Summary.Trim();
			item.Description = input.Description;
			item.ImageUrl = TextOrEmpty(input.ImageUrl);
			item.RelatedTopicId = input.RelatedTopicId;
			item.IsPublished = input.IsPublished;
			item.TypeLabel = Optional(input.TypeLabel);
			item.RightAscension = input.RightAscension;
			item.Declination = input.Declination;
			item.ImageCaption = Optional(input.ImageCaption);
			item.ImageCredit = Optional(input.ImageCredit);
			item.ImageIsDark = input.ImageIsDark;
		}
	}
}
