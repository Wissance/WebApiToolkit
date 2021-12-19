## WebApiToolkit Description
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
 
## Examples
### Create Controllers for readonly (GET operation REST resource):
    
This Toolkit ***significantly reduces amount*** of code that should be written to implement method 4 Get REST resource with paging and size

```c#
[ApiController]
public class GroupController : BasicReadController<GroupDto, GroupEntity, int>
{
    public TaxSchemeController(GroupManager manager)
    {
        _manager = manager;   // this one if we need to perform specific operation that were not defined in IModelManager
        Manager = _manager;
    }

    private readonly GroupManager _manager;
}
```
```c#
public class GroupManager : ModelManager<GroupEntity, GroupDto, int>
{
    public GroupManager(ModelContext context, ILoggerFactory loggerFactory)
        :base(loggerFactory)
    {
        _context = context;
    }

    public override async Task<OperationResultDto<IList<GroupDto>>> GetAsync(int page, int size)
    {
        return await GetAsync<int>(_context.Groupd, page, size, null, null, GroupFactory.Create);
    }

    public override async Task<OperationResultDto<GroupDto>> GetByIdAsync(int id)
    {
        return await GetAsync(_context.Groups, id, GroupFactory.Create);
    }

     private readonly IModelContext _context;
}
```
JUST 2 VERY SIMPLE CLASSES ^^ USING WebApiToolkit

### Create Controllers for full CRUD operations
Full CRUD with additional method, see example below
    
```c#
[Route("api/[controller]")]
public class UserController : BasicPagedDataController
{
    public UserController(UserManager manager)
    {
         _manager = manager;
    }

    [HttpGet]
    [Route("api/[controller]/search")]
    public async Task<IList<UserSuggestionDto>> SearchInLdapAsync([FromQuery]string query, [FromQuery]int page)
    {
         IList<UserSuggestionDto> result = await _manager.SearchInLdapAsync(query, page);
         if (result == null)
         {
             HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
         }

         return result;
    }

    private readonly MoneyMoveManager _manager;
}
```
