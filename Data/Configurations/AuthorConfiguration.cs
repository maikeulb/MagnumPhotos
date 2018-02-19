using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Library.API.Entities;

namespace Library.API.Data.Configurations
{
    class AuthorConfiguration : IEntityTypeConfiguration<Author>
    {
        public void Configure(EntityTypeBuilder<Author> builder)
        {
            builder.ToTable("Authors");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id)
               .IsRequired(true);

            builder.Property(a => a.FirstName)
                .IsRequired(true)
                .HasMaxLength(50);

            builder.Property(a => a.LastName)
                .IsRequired(true)
                .HasMaxLength(50);

            builder.Property(a => a.DateOfBirth)
                .IsRequired(true);

            builder.Property(a => a.DateOfDeath);
        }
    }
}
