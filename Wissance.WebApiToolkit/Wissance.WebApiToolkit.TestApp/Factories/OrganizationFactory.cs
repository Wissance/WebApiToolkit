using Wissance.WebApiToolkit.TestApp.Data.Entity;
using Wissance.WebApiToolkit.TestApp.Dto;

namespace Wissance.WebApiToolkit.TestApp.Factories
{
    internal static class OrganizationFactory
    {
        public static OrganizationDto Create(OrganizationEntity entity)
        {
            return new OrganizationDto()
            {
                Id = entity.Id,
                Name = entity.Name,
                ShortName = entity.ShortName,
                TaxNumber = entity.TaxNumber,
                Codes = entity.Codes != null ? entity.Codes.Select(c => c.Id).ToList() : new List<int>()
            };
        }
    }
}