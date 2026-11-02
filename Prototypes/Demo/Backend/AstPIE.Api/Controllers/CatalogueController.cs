using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AstPIE.Api.Data;
using AstPIE.Api.Dtos;
using AstPIE.Api.Models;

namespace AstPIE.Api.Controllers
{
	// The Catalogue: stars, planets, galaxies... with search, filters and pages
	[ApiController]
	[Route("api/catalogue")]
	public class CatalogueController : ControllerBase
	{
		private const int DefaultPageSize = 12;
		private const int MaxPageSize = 50;

		private readonly AppDbContext db;

		public CatalogueController(AppDbContext db)
		{
			this.db = db;
		}

		// GET /api/catalogue?search=sgr&type=BlackHole&constellation=Sagittarius&minDistance=0&maxDistance=30000&page=1&pageSize=12
		// Every parameter is optional. Search and constellation ignore capital letters.
		[HttpGet]
		public async Task<ActionResult<CataloguePageDto>> GetObjects(
			string search,
			CatalogueObjectType? type,
			string constellation,
			double? minDistance,
			double? maxDistance,
			int page = 1,
			int pageSize = DefaultPageSize)
		{
			if (page < 1)
			{
				ModelState.AddModelError("page", "page must be 1 or more.");
			}
			if (pageSize < 1 || pageSize > MaxPageSize)
			{
				ModelState.AddModelError("pageSize", "pageSize must be between 1 and " + MaxPageSize + ".");
			}
			if (minDistance < 0 || maxDistance < 0)
			{
				ModelState.AddModelError("distance", "Distances can't be negative.");
			}
			if (minDistance > maxDistance)
			{
				ModelState.AddModelError("distance", "minDistance can't be larger than maxDistance.");
			}
			if (!ModelState.IsValid)
			{
				return ValidationProblem(ModelState);
			}

			IQueryable<CatalogueObject> query =
				from item in db.CatalogueObjects
				where item.IsPublished
				select item;

			if (!string.IsNullOrWhiteSpace(search))
			{
				// ILIKE: "sgr", "SGR" and "Sgr" all match. Looks in the name and the other names.
				string pattern = "%" + EscapeLike(search.Trim()) + "%";
				query =
					from item in query
					where EF.Functions.ILike(item.Name, pattern)
						|| (item.OtherNames != null && EF.Functions.ILike(item.OtherNames, pattern))
					select item;
			}

			if (type != null)
			{
				query =
					from item in query
					where item.Type == type
					select item;
			}

			if (!string.IsNullOrWhiteSpace(constellation))
			{
				string exact = EscapeLike(constellation.Trim());
				query =
					from item in query
					where item.Constellation != null && EF.Functions.ILike(item.Constellation, exact)
					select item;
			}

			if (minDistance != null)
			{
				query =
					from item in query
					where item.DistanceLy != null && item.DistanceLy >= minDistance
					select item;
			}

			if (maxDistance != null)
			{
				query =
					from item in query
					where item.DistanceLy != null && item.DistanceLy <= maxDistance
					select item;
			}

			int totalCount = await query.CountAsync();

			IQueryable<CatalogueListItemDto> sorted =
				from item in query
				orderby item.Name
				select new CatalogueListItemDto
				{
					Slug = item.Slug,
					Name = item.Name,
					Type = item.Type,
					Constellation = item.Constellation,
					DistanceLy = item.DistanceLy,
					ImageUrl = item.ImageUrl,
					Summary = item.Summary
				};

			CataloguePageDto result = new CataloguePageDto();
			result.Items = await sorted.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
			result.Page = page;
			result.PageSize = pageSize;
			result.TotalCount = totalCount;
			result.TotalPages = (totalCount + pageSize - 1) / pageSize;
			return result;
		}

		// GET /api/catalogue/filters
		// The types and constellations that exist, for the filter dropdowns
		[HttpGet("filters")]
		public async Task<ActionResult<CatalogueFiltersDto>> GetFilters()
		{
			CatalogueFiltersDto result = new CatalogueFiltersDto();

			IQueryable<CatalogueObjectType> types = (
				from item in db.CatalogueObjects
				where item.IsPublished
				select item.Type).Distinct();
			result.Types = await (
				from type in types
				orderby type
				select type).ToListAsync();

			IQueryable<string> constellations = (
				from item in db.CatalogueObjects
				where item.IsPublished && item.Constellation != null
				select item.Constellation).Distinct();
			result.Constellations = await (
				from name in constellations
				orderby name
				select name).ToListAsync();

			return result;
		}

		// GET /api/catalogue/sagittarius-a-star
		// One object, or 404
		[HttpGet("{slug}")]
		public async Task<ActionResult<CatalogueObjectDto>> GetObject(string slug)
		{
			CatalogueObject item = await (
				from o in db.CatalogueObjects
				where o.Slug == slug && o.IsPublished
				select o).FirstOrDefaultAsync();

			if (item == null)
			{
				return NotFound();
			}

			CatalogueObjectDto result = new CatalogueObjectDto();
			result.Slug = item.Slug;
			result.Name = item.Name;
			result.OtherNames = item.OtherNames;
			result.Type = item.Type;
			result.Constellation = item.Constellation;
			result.DistanceLy = item.DistanceLy;
			result.MassKg = item.MassKg;
			result.RadiusKm = item.RadiusKm;
			result.ApparentMagnitude = item.ApparentMagnitude;
			result.DiscoveredBy = item.DiscoveredBy;
			result.DiscoveryYear = item.DiscoveryYear;
			result.Summary = item.Summary;
			result.Description = item.Description;
			result.ImageUrl = item.ImageUrl;

			if (item.RelatedTopicId != null)
			{
				result.RelatedTopic = await (
					from topic in db.Topics
					where topic.Id == item.RelatedTopicId && topic.IsPublished
					select new TopicLinkDto
					{
						Slug = topic.Slug,
						Title = topic.Title
					}).FirstOrDefaultAsync();
			}

			return result;
		}

		// % and _ are wildcards in LIKE; a backslash makes them normal characters
		private static string EscapeLike(string text)
		{
			return text.Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_");
		}
	}
}
