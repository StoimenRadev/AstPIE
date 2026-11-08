using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace AstPIE.Api.Models
{
	public enum CatalogueObjectType
	{
		Star,
		Planet,
		Moon,
		DwarfPlanet,
		Galaxy,
		Nebula,
		BlackHole,
		Cluster,
		Other
	}

	// One object in the Catalogue. Values that are not known stay empty (null)
	// instead of holding a made-up number.
	[Index(nameof(Slug), IsUnique = true)]
	public class CatalogueObject : ITimestamped
	{
		public int Id { get; set; }

		// For example "sagittarius-a-star"
		[Required]
		public string Slug { get; set; }

		[Required]
		public string Name { get; set; }

		// Other names, also searched. For example "Sgr A*"
		public string OtherNames { get; set; }

		public CatalogueObjectType Type { get; set; }

		// The type as written in the logbook, for example "Planet (a sub-Neptune)"
		public string TypeLabel { get; set; }

		public string Constellation { get; set; }

		// Where it is in the sky: right ascension in hours, declination in degrees
		public double? RightAscension { get; set; }
		public double? Declination { get; set; }

		// Stored in light-years, kilograms and kilometres. The page shows nicer units.
		public double? DistanceLy { get; set; }
		public double? MassKg { get; set; }
		public double? RadiusKm { get; set; }

		// How bright it looks from Earth (smaller = brighter)
		public double? ApparentMagnitude { get; set; }

		public string DiscoveredBy { get; set; }
		public int? DiscoveryYear { get; set; }

		[Required]
		public string Summary { get; set; }

		// The text on the left page of the logbook (HTML paragraphs)
		[Required]
		public string Description { get; set; }

		[Required]
		public string ImageUrl { get; set; }

		// The caption under the main picture, who made it, and whether it is a dark sky image
		public string ImageCaption { get; set; }

		public string ImageCredit { get; set; }

		public bool ImageIsDark { get; set; }

		// "Physics behind it": a Learn topic. Deleting the topic only clears this link.
		public int? RelatedTopicId { get; set; }

		[DeleteBehavior(DeleteBehavior.SetNull)]
		public Topic RelatedTopic { get; set; }

		public bool IsPublished { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime UpdatedAt { get; set; }

		public List<CatalogueFact> Facts { get; set; } = new List<CatalogueFact>();
		public List<ObservationLogEntry> LogEntries { get; set; } = new List<ObservationLogEntry>();
		public List<CatalogueObjectInstrument> Instruments { get; set; } = new List<CatalogueObjectInstrument>();
	}
}
