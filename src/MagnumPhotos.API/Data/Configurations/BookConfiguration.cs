using MagnumPhotos.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MagnumPhotos.API.Data.Configurations
{
    class BookConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure (EntityTypeBuilder<Book> builder)
        {
            builder.ToTable ("Books");

            builder.HasKey (b => b.Id);

            builder.Property (b => b.Title)
                .IsRequired (true)
                .HasMaxLength (100);

            builder.Property (b => b.Description)
                .IsRequired (false)
                .HasMaxLength (500);

            builder.Property (b => b.CopyrightDate);

            builder.HasOne (p => p.Photographer)
                .WithMany (p => p.Books)
                .HasForeignKey (b => b.PhotographerId);
        }
    }
}