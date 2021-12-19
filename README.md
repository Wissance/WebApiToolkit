## WebApiToolkit Description
A set of useful C# reusable classes and components that could be used with any Net Core Web application, contains:
* Dto clasess:
    - `OperationResultDto` represents result of operation that changes Data in db
    - `PagedDataDto` represents portion (page) of same objects (any type)
* Controllers classes - abstract classes
    - basic read controller (`BasicReadController`) contains 2 methods:
        - /api/[controller]/?page={page}&size={size} to get PagedDataDto<T>
        - /api/[controller]/{id} to get one object by id
    - full CRUD controller (`BasicCrudController`) = basic read controller (`BasicReadController`) + Create, Update and Delete operations 
  Controllers classes expects that all operation will be performed using Manager classes (each controller must have it own manager)
* Managers classes - classes that implements business logic of application
    - IModelManager - interface that describes basic operations
    - ModelManager is abstract class that contains impl of Get operations
 
  ## Examples
  ...tbc
