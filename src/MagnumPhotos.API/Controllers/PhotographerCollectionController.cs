/* using System; */
/* using System.Collections.Generic; */
/* using System.Linq; */
/* using AutoMapper; */
/* using MagnumPhotos.API.Entities; */
/* using MagnumPhotos.API.Helpers; */
/* using MagnumPhotos.API.Models; */
/* using MagnumPhotos.API.Services.Interfaces; */
/* using Microsoft.AspNetCore.Mvc; */

/* namespace MagnumPhotos.API.Controllers */
/* { */
/*     [Route ("api/photographercollections")] */
/*     public class PhotographerCollectionsController : Controller */
/*     { */
/*         private IMagnumPhotosRepository _magnumPhotosRepository; */

        /* public PhotographerCollectionsController (IMagnumPhotosRepository magnumPhotosRepository) */
        /* { */
        /*     _magnumPhotosRepository = magnumPhotosRepository; */
        /* } */

        /* [HttpGet ("({ids})", Name = "GetPhotographerCollection")] */
        /* [HttpHead] */
        /* public IActionResult GetPhotographerCollection ( */
        /*     [ModelBinder (BinderType = typeof (ArrayModelBinder))] IEnumerable<Guid> ids) */
        /* { */
        /*     if (ids == null) */
        /*         return BadRequest (); */

            /* var photographerEntities = _magnumPhotosRepository.GetPhotographers (ids); */

/*             if (ids.Count () != photographerEntities.Count ()) */
/*                 return NotFound (); */

            /* var photographersToReturn = Mapper.Map<IEnumerable<PhotographerDto>> (photographerEntities); */
            /* return Ok (photographersToReturn); */
        /* } */

        /* [HttpPost] */
        /* public IActionResult CreatePhotographerCollection ( */
        /*     [FromBody] IEnumerable<PhotographerForCreationDto> photographerCollection) */
        /* { */
        /*     if (photographerCollection == null) */
        /*     { */
        /*         return BadRequest (); */
        /*     } */

/*             var photographerEntities = Mapper.Map<IEnumerable<Photographer>> (photographerCollection); */

/*             foreach (var photographer in photographerEntities) */
/*             { */
/*                 _magnumPhotosRepository.AddPhotographer (photographer); */
/*             } */

/*             if (!_magnumPhotosRepository.Save ()) */
/*             { */
/*                 throw new Exception ("Creating an photographer collection failed on save."); */
/*             } */

            /* var photographerCollectionToReturn = Mapper.Map<IEnumerable<PhotographerDto>> (photographerEntities); */
            /* var idsAsString = string.Join (",", */
            /*     photographerCollectionToReturn.Select (a => a.Id)); */

            /* return CreatedAtRoute ("GetPhotographerCollection", */
            /*     new { ids = idsAsString }, */
            /*     photographerCollectionToReturn); */
        /* } */

/*         [HttpOptions] */
/*         public IActionResult GetPhotographerCollectionOptions () */
/*         { */
/*             Response.Headers.Add ("Allow", "GET,OPTIONS,POST"); */
/*             return Ok (); */
/*         } */
/*     } */
/* } */
