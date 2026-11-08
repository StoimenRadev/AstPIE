using System.ComponentModel.DataAnnotations;

namespace AstPIE.Api.Models
{
	// One row on the left page of an object's logbook, for example "Year" | "33 days"
	public class CatalogueFact
	{
		public int Id { get; set; }

		public int CatalogueObjectId { get; set; }

		public CatalogueObject CatalogueObject { get; set; }

		[Required]
		public string Label { get; set; }

		[Required]
		public string Value { get; set; }

		public int SortOrder { get; set; }
	}
}
