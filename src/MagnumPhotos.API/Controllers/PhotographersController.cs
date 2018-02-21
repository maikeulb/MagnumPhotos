using MagnumPhotos.API.Services.Interfaces;
using MagnumPhotos.API.Models;
using MagnumPhotos.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MagnumPhotos.API.Helpers;
using AutoMapper;
using MagnumPhotos.API.Entities;
using Microsoft.AspNetCore.Http;

namespace MagnumPhotos.API.Controllers
{
    [Route("api/photographers")]
    public class PhotographersController : Controller
    {
        private IMagnumPhotosRepository _magnumPhotosRepository;
        private IUrlHelper _urlHelper;
        private IPropertyMappingService _propertyMappingService;

        public PhotographersController(IMagnumPhotosRepository magnumPhotosRepository,
            IUrlHelper urlHelper,
            IPropertyMappingService propertyMappingService)
        {
            _magnumPhotosRepository = magnumPhotosRepository;
            _urlHelper = urlHelper;
            _propertyMappingService = propertyMappingService;
        }

        [HttpGet(Name = "GetPhotographers")]
        [HttpHead]
        public IActionResult GetPhotographers([FromQuery] PhotographersResourceParameters photographersResourceParameters)
        {
            if (!_propertyMappingService.ValidMappingExistsFor<PhotographerDto, Photographer>
               (photographersResourceParameters.OrderBy))
                return BadRequest();

            var photographersFromRepo = _magnumPhotosRepository.GetPhotographers(photographersResourceParameters);

            var previousPageLink = photographersFromRepo.HasPrevious ?
                CreatePhotographersResourceUri(photographersResourceParameters,
                ResourceUriType.PreviousPage) : null;

            var nextPageLink = photographersFromRepo.HasNext ? 
                CreatePhotographersResourceUri(photographersResourceParameters,
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

            Response.Headers.Add("X-Pagination",
                Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));

            var photographers = Mapper.Map<IEnumerable<PhotographerDto>>(photographersFromRepo);
            return Ok(photographers);
        }

        private string CreatePhotographersResourceUri(
            [FromQuery] PhotographersResourceParameters photographersResourceParameters,
            ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return _urlHelper.Link("GetPhotographers",
                      new
                      {
                          orderBy = photographersResourceParameters.OrderBy,
                          searchQuery = photographersResourceParameters.SearchQuery,
                          genre = photographersResourceParameters.Genre,
                          pageNumber = photographersResourceParameters.PageNumber - 1,
                          pageSize = photographersResourceParameters.PageSize
                      });
                case ResourceUriType.NextPage:
                    return _urlHelper.Link("GetPhotographers",
                      new
                      {
                          orderBy = photographersResourceParameters.OrderBy,
                          searchQuery = photographersResourceParameters.SearchQuery,
                          genre = photographersResourceParameters.Genre,
                          pageNumber = photographersResourceParameters.PageNumber + 1,
                          pageSize = photographersResourceParameters.PageSize
                      });

                default:
                    return _urlHelper.Link("GetPhotographers",
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

        [HttpGet("{id}", Name ="GetPhotographer")]
        [HttpHead]
        public IActionResult GetPhotographer([FromQuery] Guid id)
        {
            var photographerFromRepo = _magnumPhotosRepository.GetPhotographer(id);

            if (photographerFromRepo == null)
                return NotFound();

            var photographer = Mapper.Map<PhotographerDto>(photographerFromRepo);
            return Ok(photographer);
        }

        [HttpPost]
        public IActionResult CreatePhotographer([FromBody] PhotographerForCreationDto photographer)
        {
            if (photographer == null)
                return BadRequest();

            var photographerEntity = Mapper.Map<Photographer>(photographer);

            _magnumPhotosRepository.AddPhotographer(photographerEntity);

            if (!_magnumPhotosRepository.Save())
                throw new Exception("Creating an photographer failed on save.");

            var photographerToReturn = Mapper.Map<PhotographerDto>(photographerEntity);

            return CreatedAtRoute("GetPhotographer",
                new { id = photographerToReturn.Id },
                photographerToReturn);
        }

        [HttpPost("{id}")]
        public IActionResult BlockPhotographerCreation([FromQuery] Guid id)
        {
            if (_magnumPhotosRepository.PhotographerExists(id))
                return new StatusCodeResult(StatusCodes.Status409Conflict);

            return NotFound();
        }

        [HttpDelete("{id}")]
        public IActionResult DeletePhotographer([FromQuery] Guid id)
        {
            var photographerFromRepo = _magnumPhotosRepository.GetPhotographer(id);
            if (photographerFromRepo == null)
                return NotFound();

            _magnumPhotosRepository.DeletePhotographer(photographerFromRepo);

            if (!_magnumPhotosRepository.Save())
                throw new Exception($"Deleting photographer {id} failed on save.");

            return NoContent();
        }

        [HttpOptions]
        public IActionResult GetPhotographersOptions()
        {
            Response.Headers.Add("Allow", "GET,OPTIONS,POST");
            return Ok();
        }
    }
}
