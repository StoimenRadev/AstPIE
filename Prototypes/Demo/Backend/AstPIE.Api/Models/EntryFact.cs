using System.ComponentModel.DataAnnotations;

namespace AstPIE.Api.Models
{
	// One row on a case's cover sheet, for example "Rocket" | "Saturn V"
	public class EntryFact
	{
		public int Id { get; set; }

		public int EntryId { get; set; }

		public Entry Entry { get; set; }

		[Required]
		public string Label { get; set; }

		[Required]
		public string Value { get; set; }

		public int SortOrder { get; set; }
	}
}
