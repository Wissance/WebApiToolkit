using Wissance.WebApiToolkit.TestApp.Data.Entity;
using Wissance.WebApiToolkit.TestApp.Dto;

namespace Wissance.WebApiToolkit.TestApp.Factories
{
    public static class RoleFactory
    {
        public static RoleDto Create(RoleEntity entity)
        {
            return new RoleDto()
            {
                Id = entity.Id,
                Name = entity.Name,
                Users = entity.Users != null ? entity.Users.Select(u => u.Id).ToList() : new List<int>()
            };
        }

        public static RoleEntity Create(RoleDto dto)
        {
            return new RoleEntity()
            {
                Id = dto.Id,
                Name = dto.Name
            };
        }

        public static void Update(RoleDto dto, int id, RoleEntity entity)
        {
            entity.Name = dto.Name;
        }
    }
}