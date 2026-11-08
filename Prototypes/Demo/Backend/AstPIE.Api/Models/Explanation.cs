using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace AstPIE.Api.Models
{
	// One explanation of a mystery, pinned on the cork board as a card with a stamp
	public class Explanation
	{
		public int Id { get; set; }

		public int EntryId { get; set; }

		public Entry Entry { get; set; }

		// The page it is pinned on; empty means the case summary
		public int? ChapterId { get; set; }

		// Deleting the page deletes what lands on it
		[DeleteBehavior(DeleteBehavior.Cascade)]
		public Chapter Chapter { get; set; }

		// The explanation in a few words
		[Required]
		public string Title { get; set; }

		// Who suggested it
		public string ProposedBy { get; set; }

		[Required]
		public string ForText { get; set; }

		[Required]
		public string AgainstText { get; set; }

		[Required]
		public string Verdict { get; set; }

		// The stamp on the card, for example "RULED OUT"
		[Required]
		public string Stamp { get; set; }

		// A picture pinned with it; empty for none
		public string ImageUrl { get; set; }

		public string ImageCredit { get; set; }

		// How the picture fills its frame: "cover" (cropped) or "contain" (whole picture)
		public string ImageFit { get; set; }

		// Its place among the other things on the page
		public int SortOrder { get; set; }
	}
}
