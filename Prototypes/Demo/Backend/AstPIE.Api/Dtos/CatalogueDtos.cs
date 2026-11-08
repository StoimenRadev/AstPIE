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
		public string TypeLabel { get; set; }
		public string Constellation { get; set; }
		public double? DistanceLy { get; set; }
		public string ImageUrl { get; set; }
		public string Summary { get; set; }
		// How many entries its logbook has (shown on its card)
		public int LogCount { get; set; }
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
		public string ImageCaption { get; set; }
		public string ImageCredit { get; set; }
		public bool ImageIsDark { get; set; }
		public string TypeLabel { get; set; }
		public double? RightAscension { get; set; }
		public double? Declination { get; set; }
		public TopicLinkDto RelatedTopic { get; set; }
		public List<FactDto> Facts { get; set; } = new List<FactDto>();
		public List<LogEntryDto> LogEntries { get; set; } = new List<LogEntryDto>();
		public List<InstrumentDto> Instruments { get; set; } = new List<InstrumentDto>();
	}

	// One entry of the observation log
	public class LogEntryDto
	{
		public string DateLabel { get; set; }
		public string Title { get; set; }
		public string Summary { get; set; }
		public string Place { get; set; }
		public List<string> Body { get; set; } = new List<string>();
		public List<string> Facts { get; set; } = new List<string>();
		public string Note { get; set; }
		public PlateKind PlateKind { get; set; }
		public string PlateUrl { get; set; }
		public string PlateChartKey { get; set; }
		public string PlatePosterUrl { get; set; }
		public string PlateTitle { get; set; }
		public string PlateCaption { get; set; }
		public string PlateCredit { get; set; }
		public bool PlateIsDark { get; set; }
	}

	// A telescope or spacecraft, and what it did for this object
	public class InstrumentDto
	{
		public string Name { get; set; }
		public string PhotoUrl { get; set; }
		public string PhotoCredit { get; set; }
		public bool PhotoIsDark { get; set; }
		public string Description { get; set; }
		public string Role { get; set; }
	}

	// Values for the filter dropdowns: only what exists in the catalogue
	public class CatalogueFiltersDto
	{
		public List<CatalogueObjectType> Types { get; set; } = new List<CatalogueObjectType>();
		public List<string> Constellations { get; set; } = new List<string>();
	}
}
