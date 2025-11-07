using Microsoft.EntityFrameworkCore;
using Wissance.WebApiToolkit.TestApp.Data;
using Wissance.WebApiToolkit.TestApp.Data.Entity;
using Wissance.WebApiToolkit.TestApp.Dto;

namespace Wissance.WebApiToolkit.TestApp.Factories
{
    public static class UserFactory
    {
        public static UserDto Create(UserEntity entity)
        {
            return new UserDto()
            {
                Id = entity.Id,
                Login = entity.Login,
                FullName = entity.FullName,
                OrganizationId = entity.OrganizationId,
                Roles = entity.Roles != null ? entity.Roles.Select(r => r.Id).ToArray() : new int[]{}
            };
        }

        public static UserEntity Create(UserDto dto, ModelContext dbContext)
        {
            UserEntity entity = new UserEntity()
            {
                Id = dto.Id,
                FullName = dto.FullName,
                Login = dto.Login,
                OrganizationId = dto.OrganizationId,
                //Roles = dto.Roles != null ? dto.Roles.Select(r => new RoleEntity(){Id = r}).ToList(): new List<RoleEntity>()
            };
            
            if (dto.Roles != null)
            {
                //entity.Roles.
            }

            return entity;
        }

        public static void Update(UserDto dto, int id, ModelContext context, UserEntity entity)
        {
            entity.Login = dto.Login;
            entity.FullName = dto.FullName;
            entity.OrganizationId = dto.OrganizationId;
        }
    }
}