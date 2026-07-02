using Wissance.WebApiToolkit.TestApp.Data;
using Wissance.WebApiToolkit.TestApp.Data.Entity;
using Wissance.WebApiToolkit.TestApp.Dto;

namespace Wissance.WebApiToolkit.TestApp.Factories
{
    public static class ProfileFactory
    {
        public static ProfileDto Create(ProfileEntity entity)
        {
            return new ProfileDto()
            {
                Id = entity.Id,
                Name = entity.Name,
                Address = entity.Address,
                Bio = entity.Bio,
                Photo = entity.Photo,
                UserId = entity.UserId
            };
        }
        
        public static ProfileEntity Create(ProfileDto dto, ModelContext dbContext)
        {
            return new ProfileEntity()
            {
                Id = dto.Id,
                Name = dto.Name,
                UserId = dto.UserId,
                Address = dto.Address,
                Bio = dto.Bio,
                Photo = dto.Photo
            };
        }
        
        public static void Update(ProfileDto dto, int id, ModelContext dbContext, ProfileEntity entity)
        {
            entity.Name = dto.Name;
            entity.Address = dto.Address;
            entity.Bio = dto.Bio;
            entity.Photo = dto.Photo;
        }
    }
}