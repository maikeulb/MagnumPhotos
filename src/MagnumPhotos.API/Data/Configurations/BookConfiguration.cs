using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MagnumPhotos.API.Entities;

namespace MagnumPhotos.API.Data.Configurations
{
    class BookConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.ToTable("Books");

            builder.HasKey(b => b.Id);

            builder.Property(b => b.Id)
               .IsRequired(true);

            builder.Property(b => b.Title)
                .IsRequired(true)
                .HasMaxLength(100);

            builder.Property(b => b.Description)
                .IsRequired(false)
                .HasMaxLength(500);

            builder.Property(b => b.CopyrightDate)
               .IsRequired();

            builder.HasOne(b => b.Photographer)
                .WithMany()
                .HasForeignKey(b => b.PhotographerId);
        }
    }
}
