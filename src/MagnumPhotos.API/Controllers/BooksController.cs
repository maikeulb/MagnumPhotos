using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MagnumPhotos.API.Entities;
using MagnumPhotos.API.Helpers;
using MagnumPhotos.API.Models;
using MagnumPhotos.API.Services;
using MagnumPhotos.API.Services.Interfaces;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MagnumPhotos.API.Controllers
{
    [Route ("api/photographers/{photographerId}/books")]
    public class BooksController : Controller
    {
        private IMagnumPhotosRepository _magnumPhotosRepository;
        private ILogger<BooksController> _logger;
        private IUrlHelper _urlHelper;

        public BooksController (IMagnumPhotosRepository magnumPhotosRepository,
            ILogger<BooksController> logger,
            IUrlHelper urlHelper)
        {
            _logger = logger;
            _magnumPhotosRepository = magnumPhotosRepository;
            _urlHelper = urlHelper;
        }

        [HttpGet (Name = "GetBooksForPhotographer")]
        public IActionResult GetBooksForPhotographer (Guid photographerId)
        {
            if (!_magnumPhotosRepository.PhotographerExists (photographerId))
                return NotFound ();

            var booksForPhotographerFromRepo = _magnumPhotosRepository.GetBooksForPhotographer (photographerId);

            var booksForPhotographer = Mapper.Map<IEnumerable<BookDto>> (booksForPhotographerFromRepo);

            booksForPhotographer = booksForPhotographer.Select (book =>
            {
                book = CreateLinksForBook (book);
                return book;
            });

            var wrapper = new LinkedCollectionResourceWrapper<BookDto> (booksForPhotographer);

            return Ok (CreateLinksForBooks (wrapper));
        }

        [HttpGet ("{id}", Name = "GetBookForPhotographer")]
        public IActionResult GetBookForPhotographer (Guid photographerId, Guid id)
        {
            if (!_magnumPhotosRepository.PhotographerExists (photographerId))
                return NotFound ();

            var bookForPhotographerFromRepo = _magnumPhotosRepository.GetBookForPhotographer (photographerId, id);

            if (bookForPhotographerFromRepo == null)
                return NotFound ();

            var bookForPhotographer = Mapper.Map<BookDto> (bookForPhotographerFromRepo);
            return Ok (CreateLinksForBook (bookForPhotographer));
        }

        [HttpPost (Name = "CreateBookForPhotographer")]
        public IActionResult CreateBookForPhotographer (Guid photographerId, [FromBody] BookForCreationDto book)
        {
            if (book == null)
                return BadRequest ();

            if (book.Description == book.Title)
                ModelState.AddModelError (nameof (BookForCreationDto),
                    "The provided description should be different from the title.");

            if (!ModelState.IsValid)
                return new UnprocessableEntityObjectResult (ModelState);

            if (!_magnumPhotosRepository.PhotographerExists (photographerId))
                return NotFound ();

            var bookEntity = Mapper.Map<Book> (book);

            _magnumPhotosRepository.AddBookForPhotographer (photographerId, bookEntity);

            if (!_magnumPhotosRepository.Save ())
                throw new Exception ($"Creating a book for photographer {photographerId} failed on save.");

            var bookToReturn = Mapper.Map<BookDto> (bookEntity);

            return CreatedAtRoute ("GetBookForPhotographer",
                new { photographerId = photographerId, id = bookToReturn.Id },
                CreateLinksForBook (bookToReturn));
        }

        [HttpPut ("{id}", Name = "UpdateBookForPhotographer")]
        public IActionResult UpdateBookForPhotographer (Guid photographerId, Guid id, [FromBody] BookForUpdateDto book)
        {
            if (book == null)
                return BadRequest ();

            if (book.Description == book.Title)
                ModelState.AddModelError (nameof (BookForUpdateDto),
                    "The provided description should be different from the title.");

            if (!ModelState.IsValid)
                return new UnprocessableEntityObjectResult (ModelState);

            if (!_magnumPhotosRepository.PhotographerExists (photographerId))
                return NotFound ();

            var bookForPhotographerFromRepo = _magnumPhotosRepository.GetBookForPhotographer (photographerId, id);

            if (bookForPhotographerFromRepo == null)
            {
                var bookToAdd = Mapper.Map<Book> (book);
                bookToAdd.Id = id;

                _magnumPhotosRepository.AddBookForPhotographer (photographerId, bookToAdd);

                if (!_magnumPhotosRepository.Save ())
                    throw new Exception ($"Upserting book {id} for photographer {photographerId} failed on save.");

                var bookToReturn = Mapper.Map<BookDto> (bookToAdd);

                return CreatedAtRoute ("GetBookForPhotographer",
                    new { photographerId = photographerId, id = bookToReturn.Id },
                    bookToReturn);
            }

            Mapper.Map (book, bookForPhotographerFromRepo);

            if (!_magnumPhotosRepository.Save ())
                throw new Exception ($"Updating book {id} for photographer {photographerId} failed on save.");

            return NoContent ();
        }

        [HttpPatch ("{id}", Name = "PartiallyUpdateBookForPhotographer")]
        public IActionResult PartiallyUpdateBookForPhotographer (Guid photographerId, Guid id, [FromBody] JsonPatchDocument<BookForUpdateDto> patchDoc)
        {
            if (patchDoc == null)
                return BadRequest ();

            if (!_magnumPhotosRepository.PhotographerExists (photographerId))
                return NotFound ();

            var bookForPhotographerFromRepo = _magnumPhotosRepository.GetBookForPhotographer (photographerId, id);

            if (bookForPhotographerFromRepo == null)
            {
                var bookDto = new BookForUpdateDto ();
                patchDoc.ApplyTo (bookDto, ModelState);

                if (bookDto.Description == bookDto.Title)
                    ModelState.AddModelError (nameof (BookForUpdateDto),
                        "The provided description should be different from the title.");

                TryValidateModel (bookDto);

                if (!ModelState.IsValid)
                    return new UnprocessableEntityObjectResult (ModelState);

                var bookToAdd = Mapper.Map<Book> (bookDto);
                bookToAdd.Id = id;

                _magnumPhotosRepository.AddBookForPhotographer (photographerId, bookToAdd);

                if (!_magnumPhotosRepository.Save ())
                    throw new Exception ($"Upserting book {id} for photographer {photographerId} failed on save.");

                var bookToReturn = Mapper.Map<BookDto> (bookToAdd);
                return CreatedAtRoute ("GetBookForPhotographer",
                    new { photographerId = photographerId, id = bookToReturn.Id },
                    bookToReturn);
            }

            var bookToPatch = Mapper.Map<BookForUpdateDto> (bookForPhotographerFromRepo);

            patchDoc.ApplyTo (bookToPatch, ModelState);

            if (bookToPatch.Description == bookToPatch.Title)
                ModelState.AddModelError (nameof (BookForUpdateDto),
                    "The provided description should be different from the title.");

            TryValidateModel (bookToPatch);

            if (!ModelState.IsValid)
                return new UnprocessableEntityObjectResult (ModelState);

            Mapper.Map (bookToPatch, bookForPhotographerFromRepo);

            if (!_magnumPhotosRepository.Save ())
                throw new Exception ($"Patching book {id} for photographer {photographerId} failed on save.");

            return NoContent ();
        }

        [HttpDelete ("{id}", Name = "DeleteBookForPhotographer")]
        public IActionResult DeleteBookForPhotographer (Guid photographerId, Guid id)
        {
            if (!_magnumPhotosRepository.PhotographerExists (photographerId))
                return NotFound ();

            var bookForPhotographerFromRepo = _magnumPhotosRepository.GetBookForPhotographer (photographerId, id);
            if (bookForPhotographerFromRepo == null)
                return NotFound ();

            _magnumPhotosRepository.DeleteBook (bookForPhotographerFromRepo);

            if (!_magnumPhotosRepository.Save ())
                throw new Exception ($"Deleting book {id} for photographer {photographerId} failed on save.");

            _logger.LogInformation (100, $"Book {id} for photographer {photographerId} was deleted.");

            return NoContent ();
        }

        private BookDto CreateLinksForBook (BookDto book)
        {
            book.Links.Add (new Link (_urlHelper.Link ("GetBookForPhotographer",
                    new { id = book.Id }),
                "self",
                "GET"));

            book.Links.Add (
                new Link (_urlHelper.Link ("DeleteBookForPhotographer",
                        new { id = book.Id }),
                    "delete_book",
                    "DELETE"));

            book.Links.Add (
                new Link (_urlHelper.Link ("UpdateBookForPhotographer",
                        new { id = book.Id }),
                    "update_book",
                    "PUT"));

            book.Links.Add (
                new Link (_urlHelper.Link ("PartiallyUpdateBookForPhotographer",
                        new { id = book.Id }),
                    "partially_update_book",
                    "PATCH"));

            return book;
        }

        private LinkedCollectionResourceWrapper<BookDto> CreateLinksForBooks (
            LinkedCollectionResourceWrapper<BookDto> booksWrapper)
        {
            booksWrapper.Links.Add (
                new Link (_urlHelper.Link ("GetBooksForPhotographer", new { }),
                    "self",
                    "GET"));

            return booksWrapper;
        }
    }
}
