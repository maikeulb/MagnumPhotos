using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MagnumPhotos.API.Data.Context;
using MagnumPhotos.API.Entities;
using Microsoft.Extensions.Logging;

namespace MagnumPhotos.API.Data.Seed
{
    public class MagnumPhotosContextInitializer
    {
        public static async Task Initialize (
            MagnumPhotosContext context,
            ILogger<MagnumPhotosContextInitializer> logger)
        {
            context.Database.EnsureCreated ();

            if (!context.Photographers.Any ())
            {
                context.Photographers.AddRange (
                    GetPreconfiguredPhotographers ());

                await context.SaveChangesAsync ();
            }

            if (!context.Books.Any ())
            {
                context.Books.AddRange (
                    GetPreconfiguredBooks ());

                await context.SaveChangesAsync ();
            }

        }

        static IEnumerable<Photographer> GetPreconfiguredPhotographers ()
        {
            return new List<Photographer> ()
            {
                new Photographer ()
                    {
                        Id = new Guid ("25320c5e-f58a-4b1f-b63a-8ee07a840bdf"),
                            FirstName = "Henre",
                            LastName = "Cartier",
                            DateOfBirth = new DateTimeOffset (new DateTime (1908, 3, 11)),
                            DateOfDeath = new DateTimeOffset (new DateTime (2004, 4, 21)),
                            Genre = "PhotoJournalism"
                    },
                    new Photographer ()
                    {
                        Id = new Guid ("76053df4-6687-4353-8937-b45556748abe"),
                            FirstName = "Robert",
                            LastName = "Capa",
                            DateOfBirth = new DateTimeOffset (new DateTime (1928, 4, 21)),
                            Genre = "War"
                    },

                    new Photographer ()
                    {
                        Id = new Guid ("86053df4-6687-4353-8937-b45556748abe"),
                            FirstName = "Eve",
                            LastName = "Arnold",
                            DateOfBirth = new DateTimeOffset (new DateTime (1943, 4, 21)),
                            DateOfDeath = new DateTimeOffset (new DateTime (2000, 4, 21)),
                            Genre = "Portraits"
                    },
                    new Photographer ()
                    {
                        Id = new Guid ("96053df4-6687-4353-8937-b45556748abe"),
                            FirstName = "Steve",
                            LastName = "McCurry",
                            DateOfBirth = new DateTimeOffset (new DateTime (1950, 4, 21)),
                            DateOfDeath = new DateTimeOffset (new DateTime (2000, 4, 21)),
                            Genre = "PhotoJournalism"
                    },

                    new Photographer ()
                    {
                        Id = new Guid ("06053df4-6687-4353-8937-b45556748abe"),
                            FirstName = "Elliot",
                            LastName = "Erwitt",
                            DateOfBirth = new DateTimeOffset (new DateTime (1928, 4, 21)),
                            Genre = "Street"
                    },

                    new Photographer ()
                    {
                        Id = new Guid ("16053df4-6687-4353-8937-b45556748abe"),
                            FirstName = "Bruce",
                            LastName = "Davidson",
                            DateOfBirth = new DateTimeOffset (new DateTime (1943, 4, 21)),
                            DateOfDeath = new DateTimeOffset (new DateTime (2000, 4, 21)),
                            Genre = "PhotoJournalism"
                    },

            };
        }

        static IEnumerable<Book> GetPreconfiguredBooks ()
        {
            return new List<Book> ()
            {
                new Book ()
                    {
                        Title = "Brooklyn Gang",
                            Description = "A 25-year-old Bruce Davidson investigatesateenageganginBrooklyn capturing the spirit of post-war youth culture in New York",
                            CopyrightDate = new DateTime (1998, 4, 21),
                            PhotographerId = new Guid ("16053df4-6687-4353-8937-b45556748abe"),
                    },
                    new Book ()
                    {
                        Title = "Personal Exposures",
                            Description = "Seeing what few others see, and capting it for all ofus, is the essence of Personal Exposures",
                            CopyrightDate = new DateTime (1988, 4, 21),
                            PhotographerId = new Guid ("06053df4-6687-4353-8937-b45556748abe"),
                    },

                    new Book ()
                    {
                        Title = "The Decisive Moment",
                            Description = "Within the canon of European photography books it would be difficult to find one more famous, revered and influential as Henri Cariter-Bresson's The Decisive Moment",
                            CopyrightDate = new DateTime (2015, 4, 21),
                            PhotographerId = new Guid ("25320c5e-f58a-4b1f-b63a-8ee07a840bdf"),
                    },
                    new Book ()
                    {
                        Title = "Henri Cartier-Bresson: Photographer",
                            Description = "For more than 45 years Henri Cartier-Bresson's camera has glorified the decisive moment in images of unique beauty and lyrical compassion.",
                            CopyrightDate = new DateTime (1992, 4, 21),
                            PhotographerId = new Guid ("25320c5e-f58a-4b1f-b63a-8ee07a840bdf"),
                    },
            };
        }
    }
}