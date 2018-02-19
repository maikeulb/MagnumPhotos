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
        void AddPhotographer(Photographer photographer);
        void DeletePhotographer(Photographer photographer);
        bool PhotographerExists(Guid photographerId);
        IEnumerable<Book> GetBooksForPhotographer(Guid photographerId);
        Book GetBookForPhotographer(Guid photographerId, Guid bookId);
        void AddBookForPhotographer(Guid photographerId, Book book);
        void DeleteBook(Book book);
        bool Save();
    }
}
