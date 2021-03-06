using System;
using System.Collections.Generic;
using System.Linq;
using MagnumPhotos.API.Data.Context;
using MagnumPhotos.API.Entities;
using MagnumPhotos.API.Helpers;
using MagnumPhotos.API.Models;
using MagnumPhotos.API.Services;
using MagnumPhotos.API.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace MagnumPhotos.API.Services
{
    public class MagnumPhotosRepository : IMagnumPhotosRepository
    {
        private MagnumPhotosContext _context;
        private IPropertyMappingService _propertyMappingService;
        private ILogger<MagnumPhotosRepository> _logger;

        public MagnumPhotosRepository (MagnumPhotosContext context,
            ILogger<MagnumPhotosRepository> logger,
            IPropertyMappingService propertyMappingService)
        {
            _logger = logger;
            _context = context;
            _propertyMappingService = propertyMappingService;
        }

        public PagedList<Photographer> GetPhotographers (PhotographersResourceParameters photographersResourceParameters)
        {
            var collectionBeforePaging =
                _context.Photographers.ApplySort (photographersResourceParameters.OrderBy,
                    _propertyMappingService.GetPropertyMapping<PhotographerDto, Photographer> ());

            if (!string.IsNullOrEmpty (photographersResourceParameters.Genre))
            {
                var genreForWhereClause = photographersResourceParameters.Genre
                    .Trim ().ToLowerInvariant ();
                collectionBeforePaging = collectionBeforePaging
                    .Where (a => a.Genre.ToLowerInvariant () == genreForWhereClause);
            }
            if (!string.IsNullOrEmpty (photographersResourceParameters.SearchQuery))
            {
                var searchQueryForWhereClause = photographersResourceParameters.SearchQuery
                    .Trim ().ToLowerInvariant ();
                collectionBeforePaging = collectionBeforePaging
                    .Where (a => a.Genre.ToLowerInvariant ().Contains (searchQueryForWhereClause) ||
                        a.FirstName.ToLowerInvariant ().Contains (searchQueryForWhereClause) ||
                        a.LastName.ToLowerInvariant ().Contains (searchQueryForWhereClause));
            }
            return PagedList<Photographer>.Create (collectionBeforePaging,
                photographersResourceParameters.PageNumber,
                photographersResourceParameters.PageSize);
        }

        public Photographer GetPhotographer (Guid photographerId)
        {
            return _context.Photographers
                .FirstOrDefault (a => a.Id == photographerId);
        }

        public IEnumerable<Photographer> GetPhotographers (IEnumerable<Guid> photographerIds)
        {
            return _context.Photographers
                .Where (a => photographerIds.Contains (a.Id))
                .OrderBy (a => a.FirstName)
                .OrderBy (a => a.LastName)
                .ToList ();
        }

        public Book GetBook (Guid photographerId, Guid bookId)
        {
            return _context.Books
                .Where (b => b.PhotographerId == photographerId && b.Id == bookId)
                .FirstOrDefault ();
        }

        public IEnumerable<Book> GetBooks (Guid photographerId)
        {
            return _context.Books
                .Where (b => b.PhotographerId == photographerId)
                .OrderBy (b => b.Title)
                .ToList ();
        }

        public void AddPhotographer (Photographer photographer)
        {
            photographer.Id = Guid.NewGuid ();
            _context.Photographers.Add (photographer);

            if (photographer.Books.Any ())
            {
                foreach (var book in photographer.Books)
                {
                    book.Id = Guid.NewGuid ();
                }
            }
        }

        public void AddBook (Guid photographerId, Book book)
        {
            var photographer = GetPhotographer (photographerId);
            if (photographer != null)
            {
                if (book.Id == Guid.Empty)
                    book.Id = Guid.NewGuid ();

                photographer.Books
                    .Add (book);
            }
        }

        public bool PhotographerExists (Guid photographerId)
        {
            return _context
                .Photographers.Any (a => a.Id == photographerId);
        }

        public void DeletePhotographer (Photographer photographer)
        {
            _context.Photographers
                .Remove (photographer);
        }

        public void DeleteBook (Book book)
        {
            _context.Books
                .Remove (book);
        }

        public bool Save ()
        {
            return (_context.SaveChanges () >= 0);
        }
    }
}
