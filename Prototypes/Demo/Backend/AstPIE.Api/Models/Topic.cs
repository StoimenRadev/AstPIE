using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace AstPIE.Api.Models
{
	// One educational page in Learn, for example "Newton's First Law"
	[Index(nameof(Slug), IsUnique = true)]
	public class Topic : ITimestamped
	{
		public int Id { get; set; }

		[Required]
		public string Slug { get; set; }

		[Required]
		public string Title { get; set; }

		// For example "Classical mechanics"
		[Required]
		public string Branch { get; set; }

		[Required]
		public string Summary { get; set; }

		// Markdown, with formulas
		[Required]
		public string Content { get; set; }

		public int SortOrder { get; set; }
		public bool IsPublished { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime UpdatedAt { get; set; }

		public List<Simulation> Simulations { get; set; } = new List<Simulation>();
	}
}
