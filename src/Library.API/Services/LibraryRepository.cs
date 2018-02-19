using Library.API.Services.Interfaces;
using Library.API.Entities;
using Library.API.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Library.API.Services
{
    public class LibraryRepository : ILibraryRepository
    {
        private LibraryContext _context;

        public LibraryRepository(LibraryContext context)
        {
            _context = context;
        }

        public void AddAuthor(Author author)
        {
            _context.Authors.Add(author);
        }

        public void AddBookForAuthor(int authorId, Book book)
        {
            var author = GetAuthor(authorId);
            if (author != null)
                author.Books.Add(book);
        }

        public bool AuthorExists(int authorId)
        {
            return _context.Authors.Any(a => a.Id == authorId);
        }

        public void DeleteAuthor(Author author)
        {
            _context.Authors.Remove(author);
        }

        public void DeleteBook(Book book)
        {
            _context.Books.Remove(book);
        }

        public Author GetAuthor(int authorId)
        {
            return _context.Authors.FirstOrDefault(a => a.Id == authorId);
        }

        /* public PagedList<Author> GetAuthors( */
        /*     AuthorsResourceParameters authorsResourceParameters) */
        /* { */
        /*     var collectionBeforePaging = _context.Authors */
        /*         .OrderBy(a => a.FirstName) */
        /*         .ThenBy(a => a.LastName).AsQueryable(); */

        /*     var collectionBeforePaging = */
        /*         _context.Authors.ApplySort(authorsResourceParameters.OrderBy, */
        /*         _propertyMappingService.GetPropertyMapping<AuthorDto, Author>()); */

        /*     if (!string.IsNullOrEmpty(authorsResourceParameters.Genre)) */
        /*     { */
        /*         var genreForWhereClause = authorsResourceParameters.Genre */
        /*             .Trim().ToLowerInvariant(); */
        /*         collectionBeforePaging = collectionBeforePaging */
        /*             .Where(a => a.Genre.ToLowerInvariant() == genreForWhereClause); */
        /*     } */

        /*     if (!string.IsNullOrEmpty(authorsResourceParameters.SearchQuery)) */
        /*     { */
        /*         var searchQueryForWhereClause = authorsResourceParameters.SearchQuery */
        /*             .Trim().ToLowerInvariant(); */

        /*         collectionBeforePaging = collectionBeforePaging */
        /*             .Where(a => a.Genre.ToLowerInvariant().Contains(searchQueryForWhereClause) */
        /*             || a.FirstName.ToLowerInvariant().Contains(searchQueryForWhereClause) */
        /*             || a.LastName.ToLowerInvariant().Contains(searchQueryForWhereClause)); */
        /*     } */

        /*     return PagedList<Author>.Create(collectionBeforePaging, */
        /*         authorsResourceParameters.PageNumber, */
        /*         authorsResourceParameters.PageSize); */               
        /* } */

        public IEnumerable<Author> GetAuthors()
        {
            return _context.Authors.OrderBy(a => a.FirstName).ThenBy(a => a.LastName);
        }

        public IEnumerable<Author> GetAuthors(IEnumerable<int> authorIds)
        {
            return _context.Authors.Where(a => authorIds.Contains(a.Id))
                .OrderBy(a => a.FirstName)
                .OrderBy(a => a.LastName)
                .ToList();
        }

        public void UpdateAuthor(Author author)
        {
        
        }

        public Book GetBookForAuthor(int authorId, int bookId)
        {
            return _context.Books
              .Where(b => b.AuthorId == authorId && b.Id == bookId).FirstOrDefault();
        }

        public IEnumerable<Book> GetBooksForAuthor(int authorId)
        {
            return _context.Books
                        .Where(b => b.AuthorId == authorId).OrderBy(b => b.Title).ToList();
        }

        public void UpdateBookForAuthor(Book book)
        {

        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }
    }
}
