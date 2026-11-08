using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace AstPIE.Api.Models
{
	// A physics word explained in the margin of the topics that use it, for example "inertia"
	[Index(nameof(Slug), IsUnique = true)]
	public class KeyWord
	{
		public int Id { get; set; }

		[Required]
		public string Slug { get; set; }

		// The word as shown, for example "net force"
		[Required]
		public string Name { get; set; }

		// Other spellings to find in the text, for example "forces"
		public List<string> OtherForms { get; set; } = new List<string>();

		// One to three sentences
		[Required]
		public string Definition { get; set; }

		// For example "ΣF"; empty when it has no symbol
		public string Symbol { get; set; }

		// For example "newtons (N)"; empty when it has no unit
		public string Unit { get; set; }

		public List<TopicKeyWord> Topics { get; set; } = new List<TopicKeyWord>();
	}

	// Which key words a topic marks in its text
	[PrimaryKey(nameof(TopicId), nameof(KeyWordId))]
	public class TopicKeyWord
	{
		public int TopicId { get; set; }

		public Topic Topic { get; set; }

		public int KeyWordId { get; set; }

		public KeyWord KeyWord { get; set; }
	}
}
