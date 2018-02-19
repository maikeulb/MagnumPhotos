using MagnumPhotos.API.Entities;
using MagnumPhotos.API.Helpers;
using System;
using System.Collections.Generic;

namespace MagnumPhotos.API.Services.Interfaces
{
    public interface IMagnumPhotosRepository
    {
        PagedList<Photographer> GetPhotographers(PhotographersResourceParameters photographersResourceParameters);
        Photographer GetPhotographer(Guid photographerId);
        IEnumerable<Photographer> GetPhotographers(IEnumerable<Guid> photographerIds);
        Book GetBookForPhotographer(Guid photographerId, Guid bookId);
        IEnumerable<Book> GetBooksForPhotographer(Guid photographerId);
        void AddPhotographer(Photographer photographer);
        void AddBookForPhotographer(Guid photographerId, Book book);
        void DeletePhotographer(Photographer photographer);
        void DeleteBook(Book book);
        bool PhotographerExists(Guid photographerId);
        bool Save();
    }
}
