using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AstPIE.Api.Data;
using AstPIE.Api.Dtos;
using AstPIE.Api.Models;

namespace AstPIE.Api.Controllers.Admin
{
	[Route("api/admin/simulations")]
	public class AdminSimulationsController : AdminControllerBase
	{
		public AdminSimulationsController(AppDbContext db) : base(db)
		{
		}

		// GET /api/admin/simulations
		[HttpGet]
		public async Task<ActionResult<List<SimulationAdminDto>>> GetAll()
		{
			return await (
				from s in db.Simulations
				orderby s.TopicId, s.Id
				select new SimulationAdminDto
				{
					Id = s.Id,
					LabIntro = s.LabIntro,
					ModelText = s.ModelText,
					Limits = s.Limits,
					StepCode = s.StepCode,
					TopicId = s.TopicId,
					Slug = s.Slug,
					Title = s.Title,
					Engine = s.Engine,
					Version = s.Version,
					Status = s.Status,
					BuildUrl = s.BuildUrl,
					ThumbnailUrl = s.ThumbnailUrl,
					LearningGoal = s.LearningGoal,
					SourceUrl = s.SourceUrl
				}).ToListAsync();
		}

		// GET /api/admin/simulations/5
		[HttpGet("{id:int}")]
		public async Task<ActionResult<SimulationAdminDto>> GetOne(int id)
		{
			SimulationAdminDto simulation = await (
				from s in db.Simulations
				where s.Id == id
				select new SimulationAdminDto
				{
					Id = s.Id,
					LabIntro = s.LabIntro,
					ModelText = s.ModelText,
					Limits = s.Limits,
					StepCode = s.StepCode,
					TopicId = s.TopicId,
					Slug = s.Slug,
					Title = s.Title,
					Engine = s.Engine,
					Version = s.Version,
					Status = s.Status,
					BuildUrl = s.BuildUrl,
					ThumbnailUrl = s.ThumbnailUrl,
					LearningGoal = s.LearningGoal,
					SourceUrl = s.SourceUrl
				}).FirstOrDefaultAsync();

			if (simulation == null)
			{
				return NotFound();
			}
			return simulation;
		}

		// POST /api/admin/simulations
		[HttpPost]
		public async Task<ActionResult> Create(SimulationInput input)
		{
			ActionResult problem = await Check(input, 0);
			if (problem != null)
			{
				return problem;
			}

			Simulation simulation = new Simulation();
			Apply(input, simulation);
			db.Simulations.Add(simulation);
			await db.SaveChangesAsync();
			return Created("/api/admin/simulations/" + simulation.Id, new { id = simulation.Id });
		}

		// PUT /api/admin/simulations/5
		[HttpPut("{id:int}")]
		public async Task<ActionResult> Update(int id, SimulationInput input)
		{
			Simulation simulation = await db.Simulations.FindAsync(id);
			if (simulation == null)
			{
				return NotFound();
			}

			ActionResult problem = await Check(input, id);
			if (problem != null)
			{
				return problem;
			}

			Apply(input, simulation);
			await db.SaveChangesAsync();
			return NoContent();
		}

		// DELETE /api/admin/simulations/5
		[HttpDelete("{id:int}")]
		public async Task<ActionResult> Delete(int id)
		{
			Simulation simulation = await db.Simulations.FindAsync(id);
			if (simulation == null)
			{
				return NotFound();
			}

			db.Simulations.Remove(simulation);
			await db.SaveChangesAsync();
			return NoContent();
		}

		// null when everything is fine
		private async Task<ActionResult> Check(SimulationInput input, int ownId)
		{
			bool topicExists = await (from t in db.Topics where t.Id == input.TopicId select t).AnyAsync();
			if (!topicExists)
			{
				return FieldError("TopicId", "This topic does not exist.");
			}

			bool slugTaken = await (from s in db.Simulations where s.Slug == input.Slug && s.Id != ownId select s).AnyAsync();
			if (slugTaken)
			{
				return ConflictMessage("Another simulation already uses the slug \"" + input.Slug + "\".");
			}
			return null;
		}

		private static void Apply(SimulationInput input, Simulation simulation)
		{
			simulation.TopicId = input.TopicId;
			simulation.Slug = input.Slug;
			simulation.Title = input.Title.Trim();
			simulation.Engine = input.Engine;
			simulation.Version = input.Version.Trim();
			simulation.Status = input.Status;
			simulation.BuildUrl = input.BuildUrl.Trim();
			simulation.ThumbnailUrl = TextOrEmpty(input.ThumbnailUrl);
			simulation.LearningGoal = input.LearningGoal.Trim();
			simulation.SourceUrl = TextOrEmpty(input.SourceUrl);
			simulation.LabIntro = Optional(input.LabIntro);
			simulation.ModelText = Optional(input.ModelText);
			simulation.Limits = CleanList(input.Limits);
			simulation.StepCode = Optional(input.StepCode);
		}
	}
}
