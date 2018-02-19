using MagnumPhotos.API.Services.Interfaces;
using MagnumPhotos.API.Entities;
using MagnumPhotos.API.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MagnumPhotos.API.Services
{
    public class MagnumPhotosRepository : IMagnumPhotosRepository
    {
        private MagnumPhotosContext _context;

        public MagnumPhotosRepository(MagnumPhotosContext context)
        {
            _context = context;
        }

        public void AddPhotographer(Photographer photographer)
        {
            _context.Photographers.Add(photographer);
        }

        public void AddBookForPhotographer(int photographerId, Book book)
        {
            var photographer = GetPhotographer(photographerId);
            if (photographer != null)
                photographer.Books.Add(book);
        }

        public bool PhotographerExists(int photographerId)
        {
            return _context.Photographers.Any(a => a.Id == photographerId);
        }

        public void DeletePhotographer(Photographer photographer)
        {
            _context.Photographers.Remove(photographer);
        }

        public void DeleteBook(Book book)
        {
            _context.Books.Remove(book);
        }

        public Photographer GetPhotographer(int photographerId)
        {
            return _context.Photographers.FirstOrDefault(a => a.Id == photographerId);
        }

        public PagedList<Photographer> GetPhotographers(
            PhotographersResourceParameters photographersResourceParameters)
        {
            var collectionBeforePaging =
                _context.Photographers.ApplySort(photographersResourceParameters.OrderBy,
                _propertyMappingService.GetPropertyMapping<PhotographerDto, Photographer>());

            if (!string.IsNullOrEmpty(photographersResourceParameters.Genre))
            {
                var genreForWhereClause = photographersResourceParameters.Genre
                    .Trim().ToLowerInvariant();
                collectionBeforePaging = collectionBeforePaging
                    .Where(a => a.Genre.ToLowerInvariant() == genreForWhereClause);
            }

            if (!string.IsNullOrEmpty(photographersResourceParameters.SearchQuery))
            {
                var searchQueryForWhereClause = photographersResourceParameters.SearchQuery
                    .Trim().ToLowerInvariant();

                collectionBeforePaging = collectionBeforePaging
                    .Where(a => a.Genre.ToLowerInvariant().Contains(searchQueryForWhereClause)
                    || a.FirstName.ToLowerInvariant().Contains(searchQueryForWhereClause)
                    || a.LastName.ToLowerInvariant().Contains(searchQueryForWhereClause));
            }

            return PagedList<Photographer>.Create(collectionBeforePaging,
                photographersResourceParameters.PageNumber,
                photographersResourceParameters.PageSize);               
        }

        public IEnumerable<Photographer> GetPhotographers()
        {
            return _context.Photographers.OrderBy(a => a.FirstName).ThenBy(a => a.LastName);
        }

        public IEnumerable<Photographer> GetPhotographers(IEnumerable<int> photographerIds)
        {
            return _context.Photographers.Where(a => photographerIds.Contains(a.Id))
                .OrderBy(a => a.FirstName)
                .OrderBy(a => a.LastName)
                .ToList();
        }

        public Book GetBookForPhotographer(int photographerId, int bookId)
        {
            return _context.Books
              .Where(b => b.PhotographerId == photographerId && b.Id == bookId).FirstOrDefault();
        }

        public IEnumerable<Book> GetBooksForPhotographer(int photographerId)
        {
            return _context.Books
                        .Where(b => b.PhotographerId == photographerId).OrderBy(b => b.Title).ToList();
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }
    }
}
