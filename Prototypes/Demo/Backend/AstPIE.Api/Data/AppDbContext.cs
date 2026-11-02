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
		public DbSet<Entry> Entries { get; set; }
		public DbSet<Chapter> Chapters { get; set; }
		public DbSet<Person> People { get; set; }
		public DbSet<EntryPerson> EntryPeople { get; set; }
		public DbSet<MediaItem> MediaItems { get; set; }
		public DbSet<CatalogueObject> CatalogueObjects { get; set; }

		protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
		{
			// Store enums as text ("Godot2D"), not numbers, so the tables are readable
			configurationBuilder.Properties<SimulationEngine>().HaveConversion<string>();
			configurationBuilder.Properties<SimulationStatus>().HaveConversion<string>();
			configurationBuilder.Properties<EntryKind>().HaveConversion<string>();
			configurationBuilder.Properties<MissionOutcome>().HaveConversion<string>();
			configurationBuilder.Properties<EvidenceStatus>().HaveConversion<string>();
			configurationBuilder.Properties<MediaType>().HaveConversion<string>();
			configurationBuilder.Properties<CatalogueObjectType>().HaveConversion<string>();
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
