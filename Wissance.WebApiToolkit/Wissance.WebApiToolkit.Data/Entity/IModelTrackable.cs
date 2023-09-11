using System;

namespace Wissance.WebApiToolkit.Data.Entity
{
    public interface IModelTrackable
    {
        DateTimeOffset Created { get; set; }
        DateTimeOffset Modified { get; set; }
    }
}