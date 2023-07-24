using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace ASPNetVueTemplate.Models;

public class ExampleTable
{
	public int Id { get; set; }
	public string Name { get; set; }
	public string Description { get; set; }
}

public class ExampleTableConfiguration : IEntityTypeConfiguration<ExampleTable>
{
	public void Configure(EntityTypeBuilder<ExampleTable> builder)
	{
		builder.HasKey(e => e.Id);
		builder.Property(e => e.Id).ValueGeneratedOnAdd(); //This creates an auto-number ID as primary key. I recommend using this on most tables, unless it is not needed due to foreign key relationship.
		builder.Property(e => e.Name).HasMaxLength(255).IsRequired(); //Set a max length and make the column non-nullable
	}
}
