namespace Wissance.WebApiToolkit.Data.Entity
{
    public interface IModelIdentifiable<TId>
    {
        TId Id { get; }
    }
}
