using Microsoft.EntityFrameworkCore;
using MagnumPhotos.API.Entities;
using MagnumPhotos.API.Data.Configurations;

namespace MagnumPhotos.API.Data.Context
{
    public class MagnumPhotosContext : DbContext
    {
        public MagnumPhotosContext(DbContextOptions<MagnumPhotosContext> options) : base(options) {}

        public DbSet<Photographer> Photographers { get; set; }
        public DbSet<Book> Books { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new PhotographerConfiguration());
            builder.ApplyConfiguration(new BookConfiguration());
        }
    }
}
