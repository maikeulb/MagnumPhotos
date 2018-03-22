using System;
using System.Collections.Generic;
using System.Linq;
using MagnumPhotos.API.Entities;
using MagnumPhotos.API.Models;
using MagnumPhotos.API.Services.Interfaces;

namespace MagnumPhotos.API.Services
{
    public class PropertyMappingService : IPropertyMappingService
    {
        private Dictionary<string, PropertyMappingValue> _photographerPropertyMapping =
            new Dictionary<string, PropertyMappingValue> (StringComparer.OrdinalIgnoreCase)
            { { "Id", new PropertyMappingValue (new List<string> () { "Id" }) }, { "Genre", new PropertyMappingValue (new List<string> () { "Genre" }) }, { "Age", new PropertyMappingValue (new List<string> () { "DateOfBirth" }, true) }, { "Name", new PropertyMappingValue (new List<string> () { "FirstName", "LastName" }) }
            };

        private IList<IPropertyMapping> propertyMappings = new List<IPropertyMapping> ();

        public PropertyMappingService ()
        {
            propertyMappings.Add (new PropertyMapping<PhotographerDto, Photographer> (_photographerPropertyMapping));
        }
        public Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination> ()
            {
                var matchingMapping = propertyMappings.OfType<PropertyMapping<TSource, TDestination>> ();

                if (matchingMapping.Count () == 1)
                {
                    return matchingMapping.First ()._mappingDictionary;
                }

                throw new Exception ($"Cannot find exact property mapping instance for <{typeof(TSource)},{typeof(TDestination)}");
            }

        public bool ValidMappingExistsFor<TSource, TDestination> (string fields)
        {
            var propertyMapping = GetPropertyMapping<TSource, TDestination> ();

            if (string.IsNullOrWhiteSpace (fields))
            {
                return true;
            }

            var fieldsAfterSplit = fields.Split (',');

            foreach (var field in fieldsAfterSplit)
            {
                var trimmedField = field.Trim ();

                var indexOfFirstSpace = trimmedField.IndexOf (" ");
                var propertyName = indexOfFirstSpace == -1 ?
                    trimmedField : trimmedField.Remove (indexOfFirstSpace);

                if (!propertyMapping.ContainsKey (propertyName))
                {
                    return false;
                }
            }
            return true;

        }

    }
}