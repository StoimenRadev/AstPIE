using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AstPIE.Api.Models
{
	// A person who took part in one or more entries (crew, designers, scientists).
	// Shown as a centred card, not a page of their own.
	public class Person
	{
		public int Id { get; set; }

		[Required]
		public string FullName { get; set; }

		[Required]
		public string Nationality { get; set; }

		public DateOnly? BirthDate { get; set; }
		public DateOnly? DeathDate { get; set; }

		[Required]
		public string PhotoUrl { get; set; }

		[Required]
		public string Bio { get; set; }

		public List<EntryPerson> Entries { get; set; } = new List<EntryPerson>();
	}
}
