## Wissance.WebApiToolkit

![GitHub code size in bytes](https://img.shields.io/github/languages/code-size/wissance/WebApiToolkit?style=plastic) 
![GitHub issues](https://img.shields.io/github/issues/wissance/WebApiToolkit?style=plastic)
![GitHub Release Date](https://img.shields.io/github/release-date/wissance/WebApiToolkit) 
![GitHub release (latest by date)](https://img.shields.io/github/downloads/wissance/WebApiToolkit/v1.5.0/total?style=plastic)

#### This lib helps to build `REST API` with `C#` and `AspNet` easily than writing it from scratch over and over in different projects.

![WebApiToolkit helps to build application easily](/img/cover.png)

### 1. Key Features
* `REST API Controller` with **full `CRUD`** contains ***only 20 lines*** of code (~ 10 are imports)
  - `GET` methods have ***built-in paging*** support;
  - `GET` methods have ***built-in sorting and filter*** by query parameters;
* support ***BULK operations*** with objects (Bulk `Create`, `Update` and `Delete`) on a Controller && interface level
* support to work with ***any persistent storage*** (`IModelManager` interface); Good built-in EntityFramework support (see `EfModelManager` class). See [WeatherControl App](https://github.com/Wissance/WeatherControl) which has 2 WEB API projects: 
  - `Wissance.WeatherControl.WebApi` uses `EntityFramework`;
  - `Wissance.WeatherControl.WebApi.V2` uses `EdgeDb`.
  
Key concepts:
1. `Controller` is a class that handles `HTTP-requests` to `REST Resource`.
2. `REST Resource` is equal to `Entity class / Database Table`
3. Every operation on `REST Resource` produce `JSON` with `DTO` as output. We ASSUME to use only one `DTO` class with all `REST` methods.  

### 2. API Contract
* `DTO` classes:
    - `OperationResultDto` represents result of operation that changes Data in db;
    - `PagedDataDto` represents portion (page) of same objects (any type);
* `Controllers` classes - abstract classes
    - basic read controller (`BasicReadController`) contains 2 methods:
        - `GET /api/[controller]/?[page={page}&size={size}&sort={sort}&order={order}]` to get `PagedDataDto<T>`
          now we also have possibility to send **ANY number of query params**, you just have to pass filter func to `EfModelManager` or do it in your own way like in [WeatherControl example with edgedb](https://github.com/Wissance/WeatherControl/blob/master/WeatherControl/Wissance.WeatherControl.WebApi.V2/Helpers/EqlResolver.cs). We also pass sort (column name) && order (`asc` or `desc`) to manager classes,
          `EfModelManager` allows to sort **by any column**. 
          Unfortunately here we have a ***ONE disadvantage*** - **we should override `Swagger` info to show query parameters usage!!!** 
        - `GET /api/[controller]/{id}` to get one object by `id`
    - full `CRUD` controller (`BasicCrudController`) = basic read controller (`BasicReadController`) + `Create`, `Update` and `Delete` operations :
        - `POST   /api/[controller]` - for new object creation
        - `PUT    /api/[controller]/{id}` - for edit object by id
        - `DELETE /api/[controller]/{id}` - for delete object by id
    - full `CRUD` with **Bulk** operations (operations over multiple objects at once), Base class - `BasicBulkCrudController` = basic read controller (`BasicReadController`) + `BulkCreate`, `BulkUpdate` and `BulkDelete` operations:
        - `POST   /api/bulk/[controller]` - for new objects creation
        - `PUT    /api/bulk/[controller]` - for edit objects passing in a request body
        - `DELETE /api/bulk/[controller]/{idList}` - for delete multiple objects by id.
        
  Controllers classes expects that all operation will be performed using Manager classes (each controller must have it own manager)
* Managers classes - classes that implements business logic of application
    - `IModelManager` - interface that describes basic operations
    - `EfModelManager`- is abstract class that contains implementation of `Get` and `Delete` operations
    - `EfSoftRemovableModelManager` is abstract class that contains implementation of `Get` and `Delete` operations with soft removable models (`IsDeleted = true` means model was removed)
    
Example of how faster Bulk vs Non-Bulk:
![Bulk vs Non Bulk](/img/bulk_performance.png)
```
Elapsed time in Non-Bulk REST API with EF is 0.9759984016418457 secs.
Elapsed time in Bulk API with EF is 0.004002094268798828 secs.
```
as a result we got almost ~`250 x` faster `API`.

### 3. Requirements
There is **only ONE requirement**: all Entity classes for any Persistence storage that are using with controllers & managers MUST implements `IModelIdentifiable<T>` from `Wissance.WebApiToolkit.Data.Entity`.
If this toolkit should be used with `EntityFramework` you should derive you resource manager from
`EfModelManager` it have built-in methods for:
* `get many` items
* `get one` item `by id`
* `delete` item `by id`


### 4. Toolkit usage algorithm with EntityFramework
Full example is mentioned in section 6 (see below). But if you are starting to build new `REST Resource`
`API` you should do following:
1. Create a `model` (`entity`) class implementing `IModelIdentifiable<T>` and `DTO` class for it representation (**for soft remove** also **add** `IModelSoftRemovable` implementation), i.e.:
```csharp
public class BookEntity : IModelIdentifiable<int>
{
    public int Id {get; set;}
    public string Title {get; set;}
    public string Authors {get; set;} // for simplicity
    public DateTimeOffset Created {get; set;}
    public DateTimeOffset Updated {get; set;}
}

public class BookDto
{
    public int Id {get; set;}
    public string Title {get; set;}
    public string Authors {get; set;} 
}
```
2. Create a factory function (i.e. static function of a static class) that converts `Model` to `DTO` i.e.:
```csharp
public static class BookFactory
{
    public static BookDto Create(BookEntity entity)
    {
        return new BookDto
        {
            Id = entity.Id,
            Title = entity.Title,
            Authors = entity.Authors;
        };
    }
}
```
3. Create `IModelContext` interface that has you `BookEntity` as a `DbSet` and it's implementation class that also derives from `DbContext` (**Ef abstract class**):
```csharp
public interface IModelContext
{
    DbSet<BookEntity> Books {get;set;}
}

public MoidelContext: DbContext<ModelContext>, IModelContext
{
    // todo: not mrntioned here constructor, entity mapping and so on
    public DbSet<BookEntity> Books {get; set;}
}
```
4. Configure to inject `ModelContext` as a `DbContext` via `DI` see [Startup](https://github.com/Wissance/WeatherControl/blob/master/WeatherControl/Wissance.WeatherControl/Startup.cs) class
5. Create `Controller` class and a manager class pair, i.e. consider here full `CRUD`
```csharp
[ApiController]
public class BookController : BasicCrudController<BookDto, BookEntity, int>
{
    public BookController(BookManager manager)
    {
        Manager = manager;  // this is for basic operations
        _manager = manager; // this for extended operations
    }
    
    private BookManager _manager;
}

public class BookManager : EfModelManager<BookEntity, BookDto, int>
{
    public BookManager(ModelContext modelContext, ILoggerFactory loggerFactory) : base(modelContext, BookFactory.Create, loggerFactory)
    {
        _modelContext = modelContext;
    }
    
    public override async Task<OperationResultDto<StationDto>> CreateAsync(StationDto data)
    {
        // todo: implement
    }
    
    public override async Task<OperationResultDto<StationDto>> UpdateAsync(int id, StationDto data)
    {
        // todo: implement
    }
    
    private readonly ModelContext _modelContext;
}
```
    
### 5. Nuget package
You could find nuget-package [here](https://www.nuget.org/packages/Wissance.WebApiToolkit)
    
### 6. Examples
### Here we consider only Full CRUD controllers because **Full CRUD = Read Only + Additional Operations (CREATE, UPDATE, DELETE)**, a **full example = full application** created with **Wissance.WebApiToolkit** could be found here: https://github.com/Wissance/WeatherControl

```csharp
[ApiController]
public class StationController : BasicCrudController<StationDto, StationEntity, int>
{
    public StationController(StationManager manager)
    {
        Manager = manager;  // this is for basic operations
        _manager = manager; // this for extended operations
    }
    
    private StationManager _manager;
}
```
    
```csharp
public class StationManager : EfModelManager<StationEntity, StationDto, int>
{
    public StationManager(ModelContext modelContext, ILoggerFactory loggerFactory) : base(modelContext, StationFactory.Create, loggerFactory)
    {
        _modelContext = modelContext;
    }

    public override async Task<OperationResultDto<StationDto>> CreateAsync(StationDto data)
    {
        try
        {
            StationEntity entity = StationFactory.Create(data);
            await _modelContext.Stations.AddAsync(entity);
            int result = await _modelContext.SaveChangesAsync();
            if (result >= 0)
            {
                return new OperationResultDto<StationDto>(true, (int)HttpStatusCode.Created, null, StationFactory.Create(entity));
            }
            return new OperationResultDto<StationDto>(false, (int)HttpStatusCode.InternalServerError, "An unknown error occurred during station creation", null);

            }
        catch (Exception e)
        {
            return new OperationResultDto<StationDto>(false, (int)HttpStatusCode.InternalServerError, $"An error occurred during station creation: {e.Message}", null);
        }
    }

    public override async Task<OperationResultDto<StationDto>> UpdateAsync(int id, StationDto data)
    {
        try
        {
            StationEntity entity = StationFactory.Create(data);
            StationEntity existingEntity = await _modelContext.Stations.FirstOrDefaultAsync(s => s.Id == id);
            if (existingEntity == null)
            {
                return new OperationResultDto<StationDto>(false, (int)HttpStatusCode.NotFound, $"Station with id: {id} does not exists", null);
            }

            // Copy only name, description and positions, create measurements if necessary from MeasurementsManager
            existingEntity.Name = entity.Name;
            existingEntity.Description = existingEntity.Description;
            existingEntity.Latitude = existingEntity.Latitude;
            existingEntity.Longitude = existingEntity.Longitude;
            int result = await _modelContext.SaveChangesAsync();
            if (result >= 0)
            {
                return new OperationResultDto<StationDto>(true, (int)HttpStatusCode.OK, null, StationFactory.Create(entity));
            }
            return new OperationResultDto<StationDto>(false, (int)HttpStatusCode.InternalServerError, "An unknown error occurred during station update", null);

        }
        catch (Exception e)
        {
             return new OperationResultDto<StationDto>(false, (int)HttpStatusCode.InternalServerError, $"An error occurred during station update: {e.Message}", null);
        }
            
    }

    private readonly ModelContext _modelContext;
}
```
JUST 2 VERY SIMPLE CLASSES ^^ USING WebApiToolkit

### 7. Extending API

#### 7.1 Add new methods to existing controller
Consider we would like to add method search to our controller:
```csharp
[HttpGet]
[Route("api/[controller]/search")]
public async Task<PagedDataDto<BookDto>>> SearchAsync([FromQuery]string query, [FromQuery]int page, [FromQuery]int size)
{ 
    OperationResultDto<Tuple<IList<BookDto>, long>> result = await Manager.GetAsync(page, size, query);
    if (result == null)
    {
        HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
    }

    HttpContext.Response.StatusCode = result.Status;
    return new PagedDataDto<TRes>(pageNumber, result.Data.Item2, GetTotalPages(result.Data.Item2, pageSize), result.Data.Item1);
}
```

#### 7.2 Add security to protect you API

We have [additional project](https://github.com/Wissance/Authorization) to protect `API` with `Keycloak` `OpenId-Connect`.
pass `IHttpContextAccessor` to `Manager` class and check something like this: `ClaimsPrincipal principal = _httpContext.HttpContext.User;`

### 8. Additional materials

You could see our articles about Toolkit usage:
* [Medium article about v1.0.x usage]( https://medium.com/@m-ushakov/how-to-reduce-amount-of-code-when-writing-netcore-rest-api-services-28352edcfca6)
* [Dev.to article about v1.0.x usage]( https://dev.to/wissance/dry-your-web-api-net-core-with-our-toolkit-cbb)

### 9. Contributors

<a href="https://github.com/Wissance/WebApiToolkit/graphs/contributors">
  <img src="https://contrib.rocks/image?repo=Wissance/WebApiToolkit" />
</a>
