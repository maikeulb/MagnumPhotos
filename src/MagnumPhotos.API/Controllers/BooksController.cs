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
        [HttpHead]
        public IActionResult GetBooksForPhotographer ([FromQuery]Guid photographerId)
        {
            if (!_magnumPhotosRepository.PhotographerExists (photographerId))
                return NotFound ();

            var booksForPhotographerFromRepo = _magnumPhotosRepository.GetBooksForPhotographer (photographerId);

            var booksForPhotographer = Mapper.Map<IEnumerable<BookDto>> (booksForPhotographerFromRepo);

            return Ok (booksForPhotographer);
        }

        [HttpGet ("{id}", Name = "GetBookForPhotographer")]
        [HttpHead]
        public IActionResult GetBookForPhotographer ([FromQuery]Guid photographerId, [FromQuery]Guid id)
        {
            if (!_magnumPhotosRepository.PhotographerExists (photographerId))
                return NotFound ();

            var bookForPhotographerFromRepo = _magnumPhotosRepository.GetBookForPhotographer (photographerId, id);

            if (bookForPhotographerFromRepo == null)
                return NotFound ();

            var bookForPhotographer = Mapper.Map<BookDto> (bookForPhotographerFromRepo);
            return Ok (bookForPhotographer);
        }

        [HttpPost (Name = "CreateBookForPhotographer")]
        public IActionResult CreateBookForPhotographer ([FromQuery]Guid photographerId, [FromBody] BookForCreationDto book)
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

            _logger.LogInformation("before adding to repository");
            _magnumPhotosRepository.AddBookForPhotographer (photographerId, bookEntity);

            _logger.LogInformation("before adding to saving");
            if (!_magnumPhotosRepository.Save ())
                throw new Exception ($"Creating a book for photographer {photographerId} failed on save.");

            var bookToReturn = Mapper.Map<BookDto> (bookEntity);

            return CreatedAtRoute ("GetBookForPhotographer",
                new { photographerId = photographerId, id = bookToReturn.Id }, bookToReturn);
        }

        [HttpDelete ("{id}", Name = "DeleteBookForPhotographer")]
        public IActionResult DeleteBookForPhotographer ([FromQuery]Guid photographerId, [FromQuery]Guid id)
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

        [HttpPut ("{id}", Name = "UpdateBookForPhotographer")]
        public IActionResult UpdateBookForPhotographer ([FromQuery]Guid photographerId, Guid id, [FromBody] BookForUpdateDto book)
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
        public IActionResult PartiallyUpdateBookForPhotographer ([FromQuery]Guid photographerId, [FromQuery]Guid id, [FromBody] JsonPatchDocument<BookForUpdateDto> patchDoc)
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

        [HttpOptions]
        public IActionResult GetBooksOptions()
        {
            Response.Headers.Add("Allow", "GET,OPTIONS,POST,PUT,PATCH");
            return Ok();
        }
    }
}
