using Wissance.WebApiToolkit.TestApp.Data.Entity;
using Wissance.WebApiToolkit.TestApp.Dto;

namespace Wissance.WebApiToolkit.TestApp.Factories
{
    internal static class CodeFactory
    {
        public static CodeDto Create(CodeEntity entity)
        {
            return new CodeDto()
            {
                Id = entity.Id,
                Name = entity.Name,
                Code = entity.Code
            };
        }
    }
}