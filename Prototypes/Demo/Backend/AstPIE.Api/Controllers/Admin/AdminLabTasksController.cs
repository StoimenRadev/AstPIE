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
	// The tasks on the lab sheet of a simulation
	[Route("api/admin/lab-tasks")]
	public class AdminLabTasksController : AdminControllerBase
	{
		public AdminLabTasksController(AppDbContext db) : base(db)
		{
		}

		// GET /api/admin/lab-tasks or /api/admin/lab-tasks?simulationId=3
		[HttpGet]
		public async Task<ActionResult<List<LabTaskAdminDto>>> GetAll(int? simulationId)
		{
			List<LabTask> rows = await (
				from x in db.LabTasks
				where simulationId == null || x.SimulationId == simulationId
				orderby x.SimulationId, x.SortOrder
				select x).ToListAsync();

			List<LabTaskAdminDto> result = new List<LabTaskAdminDto>();
			foreach (LabTask row in rows)
			{
				result.Add(ToDto(row));
			}
			return result;
		}

		// GET /api/admin/lab-tasks/5
		[HttpGet("{id:int}")]
		public async Task<ActionResult<LabTaskAdminDto>> GetOne(int id)
		{
			LabTask item = await db.LabTasks.FindAsync(id);
			if (item == null)
			{
				return NotFound();
			}
			return ToDto(item);
		}

		// POST /api/admin/lab-tasks
		[HttpPost]
		public async Task<ActionResult> Create(LabTaskInput input)
		{
			if (!await ParentExists(input.SimulationId))
			{
				return FieldError("SimulationId", "This simulation does not exist.");
			}
			LabTask item = new LabTask();
			Apply(input, item);
			db.LabTasks.Add(item);
			await db.SaveChangesAsync();
			return Created("/api/admin/lab-tasks/" + item.Id, new { id = item.Id });
		}

		// PUT /api/admin/lab-tasks/5
		[HttpPut("{id:int}")]
		public async Task<ActionResult> Update(int id, LabTaskInput input)
		{
			LabTask item = await db.LabTasks.FindAsync(id);
			if (item == null)
			{
				return NotFound();
			}
			if (!await ParentExists(input.SimulationId))
			{
				return FieldError("SimulationId", "This simulation does not exist.");
			}
			Apply(input, item);
			await db.SaveChangesAsync();
			return NoContent();
		}

		// DELETE /api/admin/lab-tasks/5
		[HttpDelete("{id:int}")]
		public async Task<ActionResult> Delete(int id)
		{
			LabTask item = await db.LabTasks.FindAsync(id);
			if (item == null)
			{
				return NotFound();
			}

			db.LabTasks.Remove(item);
			await db.SaveChangesAsync();
			return NoContent();
		}

		private async Task<bool> ParentExists(int id)
		{
			return await (from p in db.Simulations where p.Id == id select p).AnyAsync();
		}

		private static LabTaskAdminDto ToDto(LabTask item)
		{
			LabTaskAdminDto dto = new LabTaskAdminDto();
			dto.Id = item.Id;
			dto.SimulationId = item.SimulationId;
			dto.Title = item.Title;
			dto.Task = item.Task;
			dto.Hint = item.Hint;
			dto.Answer = item.Answer;
			dto.AnswersPredict = item.AnswersPredict;
			dto.SortOrder = item.SortOrder;
			return dto;
		}

		private static void Apply(LabTaskInput input, LabTask item)
		{
			item.SimulationId = input.SimulationId;
			item.Title = input.Title.Trim();
			item.Task = input.Task.Trim();
			item.Hint = input.Hint.Trim();
			item.Answer = input.Answer.Trim();
			item.AnswersPredict = input.AnswersPredict;
			item.SortOrder = input.SortOrder;
		}
	}
}
