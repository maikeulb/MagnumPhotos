# Magnum Photos

Restful API for [Magnum Photo Agency](https://www.magnumphotos.com/)
(unofficial). The resources are the agency photographers and their photography
books. Additional features include hypermedia links, pagination,
search/filter/order, rate-limiting, cache headers, file logging (NLog), and
swagger. 

Technology
----------
* ASP.NET Core
* PostgreSQL (with Entity Framework)

Endpoints
---------

| Method     | URI                                   | Action                                      |
|------------|---------------------------------------|---------------------------------------------|
| `GET`      | `/api/photographers`                       | `Retrieve all photographers`<sup>1</sup>         |
| `GET`      | `/api/photographers/{pid}`                 | `Retrieve photographer`                          |
| `POST`     | `/api/photographers`                       | `Create photographer`                            |
| `PUT`      | `/api/photographers/{pid}`                 | `Update photographer`<sup>2</sup>                            |
| `PATCH`    | `/api/photographers/{pid}`                 | `Partially update photographer`<sup>3</sup>                  |
| `DELETE`   | `/api/photographers/{pid}`                 | `Delete photographer`                            |
| `GET`      | `/api/photographers/{pid}/books`       | `Retrieve all photographer books`|
| `GET`      | `/api/photographers/{pid}/books/{id}`       | `Retrieve photographer books`                 |
| `POST`     | `/api/photographers/{pid}/books`       | `Create photographer books`                   |
| `PUT`      | `/api/photographers/{pid}/books/{id}`  | `Update photographer books`<sup>2</sup>                   |
| `PATCH`    | `/api/photographers/{pid}/books/{id}`  | `Partially update photographer books`<sup>3</sup>         |
| `DELETE`   | `/api/photographers/{pid}/books/{id}`  | `Delete photographer's books`                 |

1. Optional query parameters: genre, searchQuery, orderBy
2. Supports upserts
3. Implemented with JSON Patch Documents

Sample Responses
---------------

`http get http://localhost:5000/api/photographers`

```
"links": [
        {
            "href": null, 
            "method": "GET", 
            "rel": "self"
        }
    ], 
    "value": [
        {
            "age": 74, 
            "genre": "PhotoJournalism", 
            "id": "16053df4-6687-4353-8937-b45556748abe", 
            "links": [], 
            "name": "Bruce Davidson"
        }, 
        {
            "age": 89, 
            "genre": "Street", 
            "id": "06053df4-6687-4353-8937-b45556748abe", 
            "links": [], 
            "name": "Elliot Erwitt"
        }, 
...
```

`http get http://localhost:5000/api/photographers/16053df4-6687-4353-8937-b45556748abe/books`
```
{
    "links": [
        {
            "href": "http://localhost:5000/api/photographers/16053df4-6687-4353-8937-b45556748abe/books", 
            "method": "GET", 
            "rel": "self"
        }
    ], 
    "value": [
        {
            "copyrightDate": "1998-04-21T00:00:00", 
            "description": "A 25-year-old Bruce Davidson investigatesateenageganginBrooklyn capturing the spirit of post-war youth culture in New York", 
            "id": "edebd45a-ba75-4dd6-b41c-6290caa1165d", 
            "links": [
                {
                    "href": "http://localhost:5000/api/photographers/16053df4-6687-4353-8937-b45556748abe/books/edebd45a-ba75-4dd6-b41c-6290caa1165d", 
                    "method": "GET", 
                    "rel": "self"
                }, 
                {
                    "href": "http://localhost:5000/api/photographers/16053df4-6687-4353-8937-b45556748abe/books/edebd45a-ba75-4dd6-b41c-6290caa1165d", 
                    "method": "DELETE", 
                    "rel": "delete_book"
                }, 
                {
                    "href": "http://localhost:5000/api/photographers/16053df4-6687-4353-8937-b45556748abe/books/edebd45a-ba75-4dd6-b41c-6290caa1165d", 
                    "method": "PUT", 
                    "rel": "update_book"
                }, 
                {
                    "href": "http://localhost:5000/api/photographers/16053df4-6687-4353-8937-b45556748abe/books/edebd45a-ba75-4dd6-b41c-6290caa1165d", 
                    "method": "PATCH", 
                    "rel": "partially_update_book"
                }
            ], 
            "photographerId": "16053df4-6687-4353-8937-b45556748abe", 
            "title": "Brooklyn Gang"
        }
    ]
}
```
Run
---
If you have docker installed,
```
docker-compose build
docker-compose up
Go to http://localhost:5000 and visit one of the above endpoints (or /swagger)
```

Otherwise you will need the .NET Core 2.0 SDK. If you have the SDK installed,
then open `appsettings.json` and point the connection string to your server,
then run:
```
dotnet restore
dotnet ef database update
dotnet run
Go to http://localhost:5000 and visit one of the above endpoints
```
