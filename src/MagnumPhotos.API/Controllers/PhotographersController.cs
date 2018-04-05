using System;
using System.Collections.Generic;
using AutoMapper;
using MagnumPhotos.API.Entities;
using MagnumPhotos.API.Helpers;
using MagnumPhotos.API.Models;
using MagnumPhotos.API.Services.Interfaces;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MagnumPhotos.API.Controllers
{
    [Route ("api/photographers")]
    public class PhotographersController : Controller
    {
        private IMagnumPhotosRepository _magnumPhotosRepository;
        private IUrlHelper _urlHelper;
        private ILogger<PhotographersController> _logger;
        private IPropertyMappingService _propertyMappingService;

        public PhotographersController (IMagnumPhotosRepository magnumPhotosRepository,
            IUrlHelper urlHelper,
            ILogger<PhotographersController> logger,
            IPropertyMappingService propertyMappingService)
        {
            _magnumPhotosRepository = magnumPhotosRepository;
            _logger = logger;
            _urlHelper = urlHelper;
            _propertyMappingService = propertyMappingService;
        }

        [HttpGet (Name = "GetPhotographers")]
        public IActionResult GetPhotographers (PhotographersResourceParameters photographersResourceParameters)
        {
            if (!_propertyMappingService.ValidMappingExistsFor<PhotographerDto, Photographer>
                (photographersResourceParameters.OrderBy))
                return BadRequest ();
            var photographersFromRepo = _magnumPhotosRepository.GetPhotographers (photographersResourceParameters);

            var previousPageLink = photographersFromRepo.HasPrevious ?
                CreatePhotographersResourceUri (photographersResourceParameters,
                    ResourceUriType.PreviousPage) : null;

            var nextPageLink = photographersFromRepo.HasNext ?
                CreatePhotographersResourceUri (photographersResourceParameters,
                    ResourceUriType.NextPage) : null;

            var paginationMetadata = new
            {
                totalCount = photographersFromRepo.TotalCount,
                pageSize = photographersFromRepo.PageSize,
                currentPage = photographersFromRepo.CurrentPage,
                totalPages = photographersFromRepo.TotalPages,
                previousPageLink = previousPageLink,
                nextPageLink = nextPageLink
            };

            Response.Headers.Add ("X-Pagination",
                Newtonsoft.Json.JsonConvert.SerializeObject (paginationMetadata));

            var photographers = Mapper.Map<IEnumerable<PhotographerDto>> (photographersFromRepo);

            var wrapper = new LinkedCollectionResourceWrapper<PhotographerDto> (photographers);

            return Ok (CreateLinksForPhotographers (wrapper));
        }

        [HttpGet ("{id}", Name = "GetPhotographer")]
        public IActionResult GetPhotographer (Guid id)
        {
            var photographerFromRepo = _magnumPhotosRepository.GetPhotographer (id);

            if (photographerFromRepo == null)
                return NotFound ();

            var photographer = Mapper.Map<PhotographerDto> (photographerFromRepo);

            return Ok (CreateLinksForPhotographer (photographer));
        }

        [HttpPost (Name = "CreatePhotographer")]
        public IActionResult CreatePhotographer ([FromBody] PhotographerForCreationDto photographer)
        {
            if (photographer == null)
                return BadRequest ();

            var photographerEntity = Mapper.Map<Photographer> (photographer);

            _magnumPhotosRepository.AddPhotographer (photographerEntity);

            if (!_magnumPhotosRepository.Save ())
                throw new Exception ("Creating an photographer failed on save.");

            var photographerToReturn = Mapper.Map<PhotographerDto> (photographerEntity);
            return CreatedAtRoute ("GetPhotographer",
                new { id = photographerToReturn.Id },
                photographerToReturn);
        }

        [HttpPut ("{id}", Name = "UpdatePhotographer")]
        public IActionResult UpdatePhotographer (Guid id, [FromBody] PhotographerForUpdateDto photographer)
        {
            if (photographer == null)
                return BadRequest ();

            if (!ModelState.IsValid)
                return new UnprocessableEntityObjectResult (ModelState);

            if (!_magnumPhotosRepository.PhotographerExists (id))
                return NotFound ();

            var photographerFromRepo = _magnumPhotosRepository.GetPhotographer (id);

            if (photographerFromRepo == null)
            {
                var photographerToAdd = Mapper.Map<Photographer> (photographer);
                photographerToAdd.Id = id;

                _magnumPhotosRepository.AddPhotographer (photographerToAdd);

                if (!_magnumPhotosRepository.Save ())
                    throw new Exception ($"Upserting photographer {id} failed on save.");

                var photographerToReturn = Mapper.Map<Photographer> (photographerToAdd);

                return CreatedAtRoute ("GetPhotographer",
                    new { id = photographerToReturn.Id },
                    photographerToReturn);
            }

            Mapper.Map (photographer, photographerFromRepo);

            if (!_magnumPhotosRepository.Save ())
                throw new Exception ($"Updating photographer {id} failed on save.");

            return NoContent ();
        }

        [HttpPatch ("{id}", Name = "PartiallyUpdatePhotographer")]
        public IActionResult PartiallyUpdatePhotographer (Guid id, [FromBody] JsonPatchDocument<PhotographerForUpdateDto> patchDoc)
        {
            if (patchDoc == null)
                return BadRequest ();

            if (!_magnumPhotosRepository.PhotographerExists (id))
                return NotFound ();

            var photographerFromRepo = _magnumPhotosRepository.GetPhotographer (id);

            if (photographerFromRepo == null)
            {
                var photographerDto = new PhotographerForUpdateDto ();
                patchDoc.ApplyTo (photographerDto, ModelState);

                TryValidateModel (photographerDto);

                if (!ModelState.IsValid)
                    return new UnprocessableEntityObjectResult (ModelState);

                var photographerToAdd = Mapper.Map<Photographer> (photographerDto);
                photographerToAdd.Id = id;

                _magnumPhotosRepository.AddPhotographer (photographerToAdd);

                if (!_magnumPhotosRepository.Save ())
                    throw new Exception ($"Upserting photographer {id} failed on save.");

                var photographerToReturn = Mapper.Map<Photographer> (photographerToAdd);
                return CreatedAtRoute ("GetPhotographer",
                    new { id = photographerToReturn.Id },
                    photographerToReturn);
            }

            var photographerToPatch = Mapper.Map<PhotographerForUpdateDto> (photographerFromRepo);

            patchDoc.ApplyTo (photographerToPatch, ModelState);

            TryValidateModel (photographerToPatch);

            if (!ModelState.IsValid)
                return new UnprocessableEntityObjectResult (ModelState);

            Mapper.Map (photographerToPatch, photographerFromRepo);

            if (!_magnumPhotosRepository.Save ())
                throw new Exception ($"Patching photographer {id} failed on save.");

            return NoContent ();
        }

        [HttpDelete ("{id}", Name = "DeletePhotographer")]
        public IActionResult DeletePhotographer (Guid id)
        {
            var photographerFromRepo = _magnumPhotosRepository.GetPhotographer (id);

            if (photographerFromRepo == null)
                return NotFound ();

            _magnumPhotosRepository.DeletePhotographer (photographerFromRepo);

            if (!_magnumPhotosRepository.Save ())
                throw new Exception ($"Deleting photographer {id} failed on save.");

            return NoContent ();
        }

        private PhotographerDto CreateLinksForPhotographer (PhotographerDto photographer)
        {
            photographer.Links.Add (new Link (_urlHelper.Link ("GetPhotographer",
                    new { id = photographer.Id }),
                "self",
                "GET"));

            photographer.Links.Add (
                new Link (_urlHelper.Link ("DeletePhotographer",
                        new { id = photographer.Id }),
                    "delete_photographer",
                    "DELETE"));

            return photographer;
        }

        private LinkedCollectionResourceWrapper<PhotographerDto> CreateLinksForPhotographers (
            LinkedCollectionResourceWrapper<PhotographerDto> photographersWrapper)
        {
            photographersWrapper.Links.Add (
                new Link (_urlHelper.Link ("GetPhotographer", new { }),
                    "self",
                    "GET"));

            return photographersWrapper;
        }

        private string CreatePhotographersResourceUri (
            PhotographersResourceParameters photographersResourceParameters,
            ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return _urlHelper.Link ("GetPhotographers",
                        new
                        {
                            orderBy = photographersResourceParameters.OrderBy,
                                searchQuery = photographersResourceParameters.SearchQuery,
                                genre = photographersResourceParameters.Genre,
                                pageNumber = photographersResourceParameters.PageNumber - 1,
                                pageSize = photographersResourceParameters.PageSize
                        });
                case ResourceUriType.NextPage:
                    return _urlHelper.Link ("GetPhotographers",
                        new
                        {
                            orderBy = photographersResourceParameters.OrderBy,
                                searchQuery = photographersResourceParameters.SearchQuery,
                                genre = photographersResourceParameters.Genre,
                                pageNumber = photographersResourceParameters.PageNumber + 1,
                                pageSize = photographersResourceParameters.PageSize
                        });
                case ResourceUriType.Current:
                default:
                    return _urlHelper.Link ("GetPhotographers",
                        new
                        {
                            orderBy = photographersResourceParameters.OrderBy,
                                searchQuery = photographersResourceParameters.SearchQuery,
                                genre = photographersResourceParameters.Genre,
                                pageNumber = photographersResourceParameters.PageNumber,
                                pageSize = photographersResourceParameters.PageSize
                        });
            }
        }
    }
}
