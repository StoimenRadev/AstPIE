using System.Collections.Generic;
using AstPIE.Api.Models;

namespace AstPIE.Api.Dtos
{
	// One row in the catalogue list
	public class CatalogueListItemDto
	{
		public string Slug { get; set; }
		public string Name { get; set; }
		public CatalogueObjectType Type { get; set; }
		public string Constellation { get; set; }
		public double? DistanceLy { get; set; }
		public string ImageUrl { get; set; }
		public string Summary { get; set; }
	}

	// One page of the catalogue list (page 1, 2, 3...)
	public class CataloguePageDto
	{
		public List<CatalogueListItemDto> Items { get; set; } = new List<CatalogueListItemDto>();
		public int Page { get; set; }
		public int PageSize { get; set; }
		public int TotalCount { get; set; }
		public int TotalPages { get; set; }
	}

	// A whole catalogue object page
	public class CatalogueObjectDto
	{
		public string Slug { get; set; }
		public string Name { get; set; }
		public string OtherNames { get; set; }
		public CatalogueObjectType Type { get; set; }
		public string Constellation { get; set; }
		public double? DistanceLy { get; set; }
		public double? MassKg { get; set; }
		public double? RadiusKm { get; set; }
		public double? ApparentMagnitude { get; set; }
		public string DiscoveredBy { get; set; }
		public int? DiscoveryYear { get; set; }
		public string Summary { get; set; }
		public string Description { get; set; }
		public string ImageUrl { get; set; }
		public TopicLinkDto RelatedTopic { get; set; }
	}

	// Values for the filter dropdowns: only what exists in the catalogue
	public class CatalogueFiltersDto
	{
		public List<CatalogueObjectType> Types { get; set; } = new List<CatalogueObjectType>();
		public List<string> Constellations { get; set; } = new List<string>();
	}
}
