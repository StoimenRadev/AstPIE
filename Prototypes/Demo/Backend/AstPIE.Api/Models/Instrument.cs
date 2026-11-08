using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace AstPIE.Api.Models
{
	// A telescope or spacecraft that studied catalogue objects
	public class Instrument
	{
		public int Id { get; set; }

		// For example "Kepler space telescope"
		[Required]
		public string Name { get; set; }

		[Required]
		public string PhotoUrl { get; set; }

		public string PhotoCredit { get; set; }

		// True for a dark sky photo, which is framed differently
		public bool PhotoIsDark { get; set; }

		// A short text about it
		public string Description { get; set; }

		public List<CatalogueObjectInstrument> Objects { get; set; } = new List<CatalogueObjectInstrument>();
	}

	// Which instruments studied which object, and what each one did
	[PrimaryKey(nameof(CatalogueObjectId), nameof(InstrumentId))]
	public class CatalogueObjectInstrument
	{
		public int CatalogueObjectId { get; set; }

		public CatalogueObject CatalogueObject { get; set; }

		public int InstrumentId { get; set; }

		// An instrument can't be deleted while an object still lists it
		[DeleteBehavior(DeleteBehavior.Restrict)]
		public Instrument Instrument { get; set; }

		// For example "found it, 2014 to 2015"
		[Required]
		public string Role { get; set; }

		public int SortOrder { get; set; }
	}
}
