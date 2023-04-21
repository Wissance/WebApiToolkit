## WebApiToolkit Description

This lib helps to build `REST API` with `C#` and `AspNet` easily then writing it from scratch.
Key Features:
* `REST API Controller` with **full `CRUD`** contains ***only 20 lines*** of code (~ 10 are imports)
  - `GET` methods have builtin paging support;
  - `GET` methods have sorting and filter by query params (coming soon, ***MORE STARS -> sooner RELEASE***);
* support to work with **any persistent storage** (`IModelManager` interface); Good builtin EntityFramework support (see `EfModelManager` class). See [WeatherControl App](https://github.com/Wissance/WeatherControl) it has 2 WEB API projects: 
  - `Wissance.WeatherControl.WebApi` uses `EntityFramework`;
  - `Wissance.WeatherControl.WebApi.V2` uses `EdgeDb`.

A set of useful C# reusable classes and components that could be used with any Net Core Web application, contains:
* Dto clasess:
    - `OperationResultDto` represents result of operation that changes Data in db
    - `PagedDataDto` represents portion (page) of same objects (any type)
* Controllers classes - abstract classes
    - basic read controller (`BasicReadController`) contains 2 methods:
        - GET /api/[controller]/?page={page}&size={size} to get PagedDataDto<T>
        - GET /api/[controller]/{id} to get one object by id
    - full CRUD controller (`BasicCrudController`) = basic read controller (`BasicReadController`) + Create, Update and Delete operations :
        - POST   /api/[controller] - for new object creation
        - PUT    /api/[controller]/{id} - for object with id edit
        - DELETE /api/[controller]/{id} - for object with id delete
        
  Controllers classes expects that all operation will be performed using Manager classes (each controller must have it own manager)
* Managers classes - classes that implements business logic of application
    - `IModelManager` - interface that describes basic operations
    - `ModelManager` is abstract class that contains impl of Get operations

## Requirements
There is only ONE requirement: all Entity classes that are using with controllers & managers derived from this library base classes MUST work
with Entities that IMPLEMENTS interface `IModelIdentifiable<T>` from `Wissance.WebApiToolkit.Data.Entity`
    
## Nuget
NUGET PACKAGE: https://www.nuget.org/packages/Wissance.WebApiToolkit/1.0.0
    
## Examples
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
 public class StationManager : ModelManager<StationEntity, StationDto, int>
 {
     public StationManager(ModelContext modelContext, ILoggerFactory loggerFactory) : base(loggerFactory)
     {
        _modelContext = modelContext;
     }

     public override async Task<OperationResultDto<IList<StationDto>>> GetAsync(int page, int size)
     {
         return await GetAsync<int>(_modelContext.Stations, page, size, null, null, StationFactory.Create);
     }

     public override async Task<OperationResultDto<StationDto>> GetByIdAsync(int id)
     {
         return await GetAsync(_modelContext.Stations, id, StationFactory.Create);
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

     public override async Task<OperationResultDto<bool>> DeleteAsync(int id)
     {
         return await DeleteAsync(_modelContext, _modelContext.Stations, id);
     }

     private readonly ModelContext _modelContext;
}
```
JUST 2 VERY SIMPLE CLASSES ^^ USING WebApiToolkit

### Create Controllers for full CRUD operations
Full CRUD with additional method, see example below

Using WebApiToolkit we could also:
* extend `controller` and `manager` classes
* use authorization by passing `IHttpContextAccessor` to Manager and check somthing like this: `ClaimsPrincipal principal = _httpContext.HttpContext.User;`
