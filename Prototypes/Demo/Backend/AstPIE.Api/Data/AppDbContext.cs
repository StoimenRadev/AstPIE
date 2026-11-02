using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using AstPIE.Api.Models;

namespace AstPIE.Api.Data
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
		{
		}

		public DbSet<Topic> Topics { get; set; }
		public DbSet<Simulation> Simulations { get; set; }

		protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
		{
			// Store enums as text ("Godot2D"), not numbers, so the tables are readable
			configurationBuilder.Properties<SimulationEngine>().HaveConversion<string>();
			configurationBuilder.Properties<SimulationStatus>().HaveConversion<string>();
		}

		public override int SaveChanges()
		{
			SetTimestamps();
			return base.SaveChanges();
		}

		public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
		{
			SetTimestamps();
			return base.SaveChangesAsync(cancellationToken);
		}

		// Fills in CreatedAt for new rows and UpdatedAt for new and changed rows
		private void SetTimestamps()
		{
			DateTime now = DateTime.UtcNow;

			foreach (EntityEntry<ITimestamped> entry in ChangeTracker.Entries<ITimestamped>())
			{
				if (entry.State == EntityState.Added)
				{
					entry.Entity.CreatedAt = now;
					entry.Entity.UpdatedAt = now;
				}
				else if (entry.State == EntityState.Modified)
				{
					entry.Entity.UpdatedAt = now;
				}
			}
		}
	}
}
