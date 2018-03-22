using System.Collections.Generic;

namespace MagnumPhotos.API.Services.Interfaces
{
    public interface IPropertyMappingService
    {
        bool ValidMappingExistsFor<TSource, TDestination> (string fields);

        Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination> ();
    }
}