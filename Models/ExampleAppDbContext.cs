using Microsoft.EntityFrameworkCore;

namespace ASPNetVueTemplate.Models;

/*
 * 
 * This is your database context class. Generally it should be named [YourAppName]DbContext
 * 
 * How to enable migrations:
 * 
 * 1. Open Package Manager Console
 * 2. Type: Enable-Migrations
 * 3. Type: Add-Migration GiveYourMigrationADescriptiveName
 * 4. Type: Update-Database
 * 
 * Number 4 applies the changes to the database. This also gets done programatically in program.cs so we do not have to worry about production databases being updated as we deploy.
 * 
 */
public class ExampleAppDbContext : DbContext
{
	public DbSet<ExampleTable> ExampleTables { get; set; } //Example table; be sure to delete before running migrations

	public ExampleAppDbContext(DbContextOptions options) : base(options)
	{
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);
		modelBuilder.ApplyConfiguration(new ExampleTableConfiguration()); //Example table configuration; be sure to delete before running migrations
	}
}
