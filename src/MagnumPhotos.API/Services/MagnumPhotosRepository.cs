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

        public void AddPhotographer(Photographer author)
        {
            _context.Photographers.Add(author);
        }

        public void AddBookForPhotographer(int authorId, Book book)
        {
            var author = GetPhotographer(authorId);
            if (author != null)
                author.Books.Add(book);
        }

        public bool PhotographerExists(int authorId)
        {
            return _context.Photographers.Any(a => a.Id == authorId);
        }

        public void DeletePhotographer(Photographer author)
        {
            _context.Photographers.Remove(author);
        }

        public void DeleteBook(Book book)
        {
            _context.Books.Remove(book);
        }

        public Photographer GetPhotographer(int authorId)
        {
            return _context.Photographers.FirstOrDefault(a => a.Id == authorId);
        }

        public PagedList<Photographer> GetPhotographers(
            PhotographersResourceParameters authorsResourceParameters)
        {
            var collectionBeforePaging =
                _context.Photographers.ApplySort(authorsResourceParameters.OrderBy,
                _propertyMappingService.GetPropertyMapping<PhotographerDto, Photographer>());

            if (!string.IsNullOrEmpty(authorsResourceParameters.Genre))
            {
                var genreForWhereClause = authorsResourceParameters.Genre
                    .Trim().ToLowerInvariant();
                collectionBeforePaging = collectionBeforePaging
                    .Where(a => a.Genre.ToLowerInvariant() == genreForWhereClause);
            }

            if (!string.IsNullOrEmpty(authorsResourceParameters.SearchQuery))
            {
                var searchQueryForWhereClause = authorsResourceParameters.SearchQuery
                    .Trim().ToLowerInvariant();

                collectionBeforePaging = collectionBeforePaging
                    .Where(a => a.Genre.ToLowerInvariant().Contains(searchQueryForWhereClause)
                    || a.FirstName.ToLowerInvariant().Contains(searchQueryForWhereClause)
                    || a.LastName.ToLowerInvariant().Contains(searchQueryForWhereClause));
            }

            return PagedList<Photographer>.Create(collectionBeforePaging,
                authorsResourceParameters.PageNumber,
                authorsResourceParameters.PageSize);               
        }

        public IEnumerable<Photographer> GetPhotographers()
        {
            return _context.Photographers.OrderBy(a => a.FirstName).ThenBy(a => a.LastName);
        }

        public IEnumerable<Photographer> GetPhotographers(IEnumerable<int> authorIds)
        {
            return _context.Photographers.Where(a => authorIds.Contains(a.Id))
                .OrderBy(a => a.FirstName)
                .OrderBy(a => a.LastName)
                .ToList();
        }

        public Book GetBookForPhotographer(int authorId, int bookId)
        {
            return _context.Books
              .Where(b => b.PhotographerId == authorId && b.Id == bookId).FirstOrDefault();
        }

        public IEnumerable<Book> GetBooksForPhotographer(int authorId)
        {
            return _context.Books
                        .Where(b => b.PhotographerId == authorId).OrderBy(b => b.Title).ToList();
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }
    }
}
