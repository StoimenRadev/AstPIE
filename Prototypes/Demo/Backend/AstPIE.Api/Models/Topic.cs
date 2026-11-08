using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AstPIE.Api.Models
{
	// One concept page in Learn, for example "Newton's first law"
	[Index(nameof(Slug), IsUnique = true)]
	public class Topic : ITimestamped
	{
		public int Id { get; set; }

		[Required]
		public string Slug { get; set; }

		// The law's name, for example "Newton's first law"
		[Required]
		public string Title { get; set; }

		// The question that is the page title, for example "Does moving need a push?"
		[Required]
		public string Question { get; set; }

		// For example "Classical mechanics"
		[Required]
		public string Branch { get; set; }

		// The one-sentence lead under the title
		[Required]
		public string Summary { get; set; }

		// The reading text: "## Title" starts a numbered section, the rest is HTML.
		// Lines [[law]], [[figure]] and [[graph]] mark where those parts go.
		[Required]
		public string Content { get; set; }

		// The question asked before reading, its three choices and the right one (0, 1 or 2)
		[Required]
		public string PredictQuestion { get; set; }

		public List<string> PredictOptions { get; set; } = new List<string>();

		public int PredictAnswer { get; set; }

		// The law in one sentence, as an equation (HTML with marked symbols), and said in words
		[Required]
		public string LawStatement { get; set; }

		[Required]
		public string LawEquation { get; set; }

		[Required]
		public string ReadAloud { get; set; }

		// The numbered points of "In short"
		public List<string> RecapPoints { get; set; } = new List<string>();

		// "See it at work in Explore": the case and the sentence after the link
		public int? ExploreEntryId { get; set; }

		[DeleteBehavior(DeleteBehavior.SetNull)]
		public Entry ExploreEntry { get; set; }

		public string ExploreLine { get; set; }

		public int SortOrder { get; set; }

		public bool IsPublished { get; set; }

		public DateTime CreatedAt { get; set; }

		public DateTime UpdatedAt { get; set; }

		public List<Simulation> Simulations { get; set; } = new List<Simulation>();

		public List<TopicSymbol> Symbols { get; set; } = new List<TopicSymbol>();

		public List<CheckQuestion> CheckQuestions { get; set; } = new List<CheckQuestion>();

		public List<TopicKeyWord> KeyWords { get; set; } = new List<TopicKeyWord>();

		// Cases whose "Try it yourself" opens this topic's simulation
		[InverseProperty(nameof(Entry.RelatedTopic))]
		public List<Entry> RelatedEntries { get; set; } = new List<Entry>();
	}
}
