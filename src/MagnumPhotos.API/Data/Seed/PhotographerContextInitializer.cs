using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using MagnumPhotos.API.Entities;
using MagnumPhotos.API.Data.Context;

namespace MagnumPhotos.API.Data.Seed
{
    public class MagnumPhotosContextInitializer
    {
        public static async Task Initialize(
            MagnumPhotosContext context,
            ILogger<MagnumPhotosContextInitializer> logger)
        {
            context.Database.EnsureCreated();
            
            if (!context.Photographers.Any())
            {
                context.Photographers.AddRange(
                    GetPreconfiguredPhotographers());

                await context.SaveChangesAsync();
            }

            if (!context.Books.Any())
            {
                context.Books.AddRange(
                    GetPreconfiguredBooks());

                    await context.SaveChangesAsync();
            }

        }

        static IEnumerable<Photographer> GetPreconfiguredPhotographers()
        {
            return new List<Photographer>()
            {
                new Photographer()
                {
                    FirstName = "Vincent", 
                    LastName = "John", 
                    DateOfBirth = new DateTimeOffset(new DateTime(1952, 3, 11)), 
                    DateOfDeath = new DateTimeOffset(new DateTime(2000, 4, 21)), 
                    Genre = "Fiction"
                },
                new Photographer()
                {
                    FirstName = "Smoh", 
                    LastName = "Rick", 
                    DateOfBirth = new DateTimeOffset(new DateTime(1943, 4, 21)), 
                    DateOfDeath= new DateTimeOffset(new DateTime(2000, 4, 21)), 
                    Genre = "NonFiction"
                },
            };
        }

        static IEnumerable<Book> GetPreconfiguredBooks()
        {
            return new List<Book>()
            {
                new Book()
                {
                    Title = "Vincent", 
                    Description  = "Not that fun", 
                    CopyrightDate = new DateTime(1943, 4, 21), 
                    PhotographerId = 1
                },
                new Book()
                {
                    Title = "FarField", 
                    Description  = "Romantic", 
                    CopyrightDate = new DateTime(1943, 4, 21),
                    PhotographerId = 1
                },
                new Book()
                {
                    Title = "Vincent", 
                    Description  = "John", 
                    CopyrightDate = new DateTimeOffset(new DateTime(1943, 4, 21)), 
                    PhotographerId = 2
                },
            };
        }
    }
}
