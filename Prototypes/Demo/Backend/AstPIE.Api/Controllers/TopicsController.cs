using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AstPIE.Api.Data;
using AstPIE.Api.Dtos;
using AstPIE.Api.Models;

namespace AstPIE.Api.Controllers
{
	[ApiController]
	[Route("api/topics")]
	public class TopicsController : ControllerBase
	{
		private readonly AppDbContext db;

		public TopicsController(AppDbContext db)
		{
			this.db = db;
		}

		// GET /api/topics
		// All published topics, grouped by branch and sorted
		[HttpGet]
		public async Task<ActionResult<List<TopicListItemDto>>> GetTopics()
		{
			List<TopicListItemDto> topics = await (
				from topic in db.Topics
				where topic.IsPublished
				orderby topic.Branch, topic.SortOrder
				select new TopicListItemDto
				{
					Slug = topic.Slug,
					Title = topic.Title,
					Branch = topic.Branch,
					Summary = topic.Summary
				}).ToListAsync();

			return topics;
		}

		// GET /api/topics/newtons-first-law
		// One topic with its published simulations, or 404
		[HttpGet("{slug}")]
		public async Task<ActionResult<TopicDto>> GetTopic(string slug)
		{
			Topic topic = await (
				from t in db.Topics
				where t.Slug == slug && t.IsPublished
				select t).FirstOrDefaultAsync();

			if (topic == null)
			{
				return NotFound();
			}

			List<SimulationDto> simulations = await (
				from simulation in db.Simulations
				where simulation.TopicId == topic.Id && simulation.Status == SimulationStatus.Published
				orderby simulation.Id
				select new SimulationDto
				{
					Slug = simulation.Slug,
					Title = simulation.Title,
					Engine = simulation.Engine.ToString(),
					Version = simulation.Version,
					BuildUrl = simulation.BuildUrl,
					ThumbnailUrl = simulation.ThumbnailUrl,
					LearningGoal = simulation.LearningGoal,
					SourceUrl = simulation.SourceUrl
				}).ToListAsync();

			TopicDto result = new TopicDto();
			result.Slug = topic.Slug;
			result.Title = topic.Title;
			result.Branch = topic.Branch;
			result.Summary = topic.Summary;
			result.Content = topic.Content;
			result.Simulations = simulations;

			return result;
		}
	}
}
