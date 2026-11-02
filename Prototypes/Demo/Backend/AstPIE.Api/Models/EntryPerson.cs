using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace AstPIE.Api.Models
{
	// Who was part of which entry, and as what. The same person can be in several entries.
	[PrimaryKey(nameof(EntryId), nameof(PersonId))]
	public class EntryPerson
	{
		// Deleting the entry removes this link
		public int EntryId { get; set; }
		public Entry Entry { get; set; }

		// A person can't be deleted while linked to an entry
		public int PersonId { get; set; }

		[DeleteBehavior(DeleteBehavior.Restrict)]
		public Person Person { get; set; }

		// For example "Commander", "Chief designer"
		[Required]
		public string Role { get; set; }

		// True: flew on it. False: worked on the ground.
		public bool IsCrew { get; set; }

		public int SortOrder { get; set; }
	}
}
