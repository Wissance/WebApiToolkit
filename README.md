## WebApiToolkit Description
#### This lib helps to build `REST API` with `C#` and `AspNet` easily than writing it from scratch over and over in different projects.

### 1. Key Features
* `REST API Controller` with **full `CRUD`** contains ***only 20 lines*** of code (~ 10 are imports)
  - `GET` methods have ***built-in paging*** support;
  - `GET` methods have ***built-in sorting and filter*** by query params (coming soon, ***MORE STARS -> sooner RELEASE***);
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
        - `GET /api/[controller]/?page={page}&size={size}` to get PagedDataDto<T>
        - `GET /api/[controller]/{id}` to get one object by id
    - full `CRUD` controller (`BasicCrudController`) = basic read controller (`BasicReadController`) + `Create`, `Update` and `Delete` operations :
        - `POST   /api/[controller]` - for new object creation
        - `PUT    /api/[controller]/{id}` - for edit object by id
        - `DELETE /api/[controller]/{id}` - for delete object by id
        
  Controllers classes expects that all operation will be performed using Manager classes (each controller must have it own manager)
* Managers classes - classes that implements business logic of application
    - `IModelManager` - interface that describes basic operations
    - `EfModelManager`- is abstract class that contains implementation of `Get` and `Delete` operations

### 3. Requirements
There is **only ONE requirement**: all Entity classes for any Persistence storage that are using with controllers & managers MUST implements `IModelIdentifiable<T>` from `Wissance.WebApiToolkit.Data.Entity`.
If this toolkit should be used with `EntityFramework` you should derive you resource manager from
`EfModelManager` it have built-in methods for:
* get many
* get one
* delete


### 4. Toolkit usage algorithm with EntityFramework
Full example mentioned in chapter 6 (see below). But if you are starting to build new `REST Resource`
`API` you should do following:
1. aaa
2. sss
3. ddd
    
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

#### 7.2 Add security to protect you API

Using WebApiToolkit we could also:
* extend `controller` and `manager` classes
* use authorization by passing `IHttpContextAccessor` to Manager and check somthing like this: `ClaimsPrincipal principal = _httpContext.HttpContext.User;`
