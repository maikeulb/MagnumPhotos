using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MagnumPhotos.API.Entities;

namespace MagnumPhotos.API.Data.Configurations
{
    class PhotographerConfiguration : IEntityTypeConfiguration<Photographer>
    {
        public void Configure(EntityTypeBuilder<Photographer> builder)
        {
            builder.ToTable("Photographers");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.FirstName)
                .IsRequired(true)
                .HasMaxLength(50);

            builder.Property(a => a.LastName)
                .IsRequired(true)
                .HasMaxLength(50);

            builder.Property(a => a.Genre)
                .IsRequired(true)
                .HasMaxLength(50);

            builder.Property(a => a.DateOfBirth)
                .IsRequired(true);

            builder.Property(a => a.DateOfDeath);
        }
    }
}
