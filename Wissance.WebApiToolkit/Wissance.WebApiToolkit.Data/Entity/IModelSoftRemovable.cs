namespace Wissance.WebApiToolkit.Data.Entity
{
    public interface IModelSoftRemovable
    {
        bool IsDeleted { get; set; }
    }
}