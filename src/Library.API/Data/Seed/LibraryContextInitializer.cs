using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Library.API.Entities;
using Library.API.Data.Context;

namespace Library.API.Data.Seed
{
    public class LibraryContextInitializer
    {
        public static async Task Initialize(
            LibraryContext context,
            ILogger<LibraryContextInitializer> logger)
        {
            context.Database.EnsureCreated();
            
            if (!context.Authors.Any())
            {
                context.Authors.AddRange(
                    GetPreconfiguredAuthors());

                await context.SaveChangesAsync();
            }

            if (!context.Books.Any())
            {
                context.Books.AddRange(
                    GetPreconfiguredBooks());

                    await context.SaveChangesAsync();
            }

        }

        static IEnumerable<Author> GetPreconfiguredAuthors()
        {
            return new List<Author>()
            {
                new Author()
                {
                    FirstName = "Vincent", 
                    LastName = "John", 
                    DateOfBirth = new DateTimeOffset(new DateTime(1952, 3, 11)), 
                    DateOfDeath = new DateTimeOffset(new DateTime(2000, 4, 21)), 
                    Genre = "Fiction"
                },
                new Author()
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
                    AuthorId = 1
                },
                new Book()
                {
                    Title = "FarField", 
                    Description  = "Romantic", 
                    AuthorId = 1
                },
                new Book()
                {
                    Title = "Vincent", 
                    Description  = "John", 
                    AuthorId = 2
                },
            };
        }
    }
}
