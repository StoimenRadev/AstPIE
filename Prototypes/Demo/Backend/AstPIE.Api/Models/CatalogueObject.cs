using System;
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

		public string Constellation { get; set; }

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

		// Markdown
		[Required]
		public string Description { get; set; }

		[Required]
		public string ImageUrl { get; set; }

		// "Physics behind it": a Learn topic. Deleting the topic only clears this link.
		public int? RelatedTopicId { get; set; }

		[DeleteBehavior(DeleteBehavior.SetNull)]
		public Topic RelatedTopic { get; set; }

		public bool IsPublished { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime UpdatedAt { get; set; }
	}
}
