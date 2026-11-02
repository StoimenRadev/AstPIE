using System;

namespace AstPIE.Api.Models
{
	// Tables with CreatedAt and UpdatedAt. AppDbContext fills them in on save.
	public interface ITimestamped
	{
		DateTime CreatedAt { get; set; }
		DateTime UpdatedAt { get; set; }
	}
}
