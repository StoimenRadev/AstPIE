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
					Question = topic.Question,
					Branch = topic.Branch,
					Summary = topic.Summary,
					SortOrder = topic.SortOrder
				}).ToListAsync();

			return topics;
		}

		// GET /api/topics/newtons-first-law
		// One topic with everything on its page, or 404
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

			TopicDto result = new TopicDto();
			result.Slug = topic.Slug;
			result.Title = topic.Title;
			result.Question = topic.Question;
			result.Branch = topic.Branch;
			result.Summary = topic.Summary;
			result.Content = topic.Content;
			result.PredictQuestion = topic.PredictQuestion;
			result.PredictOptions = topic.PredictOptions;
			result.PredictAnswer = topic.PredictAnswer;
			result.LawStatement = topic.LawStatement;
			result.LawEquation = topic.LawEquation;
			result.ReadAloud = topic.ReadAloud;
			result.RecapPoints = topic.RecapPoints;

			if (topic.ExploreEntryId != null)
			{
				result.Explore = await (
					from entry in db.Entries
					where entry.Id == topic.ExploreEntryId && entry.IsPublished
					select new ExploreLinkDto
					{
						Slug = entry.Slug,
						Kind = entry.Kind,
						Title = entry.Title,
						Line = topic.ExploreLine
					}).FirstOrDefaultAsync();
			}

			result.Symbols = await (
				from symbol in db.TopicSymbols
				where symbol.TopicId == topic.Id
				orderby symbol.SortOrder
				select new SymbolDto
				{
					Symbol = symbol.Symbol,
					Name = symbol.Name,
					Meaning = symbol.Meaning,
					Unit = symbol.Unit,
					EquationPart = symbol.EquationPart
				}).ToListAsync();

			result.CheckQuestions = await (
				from question in db.CheckQuestions
				where question.TopicId == topic.Id
				orderby question.SortOrder
				select new CheckQuestionDto
				{
					AfterSection = question.AfterSection,
					Question = question.Question,
					Answer = question.Answer
				}).ToListAsync();

			result.KeyWords = await (
				from link in db.TopicKeyWords
				join word in db.KeyWords on link.KeyWordId equals word.Id
				where link.TopicId == topic.Id
				orderby word.Name
				select new KeyWordDto
				{
					Slug = word.Slug,
					Name = word.Name,
					OtherForms = word.OtherForms,
					Definition = word.Definition,
					Symbol = word.Symbol,
					Unit = word.Unit
				}).ToListAsync();

			result.Simulations = await (
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
					SourceUrl = simulation.SourceUrl,
					LabIntro = simulation.LabIntro,
					ModelText = simulation.ModelText,
					Limits = simulation.Limits,
					StepCode = simulation.StepCode,
					LabTasks = (
						from task in simulation.LabTasks
						orderby task.SortOrder
						select new LabTaskDto
						{
							Title = task.Title,
							Task = task.Task,
							Hint = task.Hint,
							Answer = task.Answer,
							AnswersPredict = task.AnswersPredict
						}).ToList()
				}).ToListAsync();

			return result;
		}
	}
}
