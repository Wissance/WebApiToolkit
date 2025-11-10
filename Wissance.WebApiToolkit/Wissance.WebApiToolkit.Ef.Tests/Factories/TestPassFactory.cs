using System.Security.Cryptography;
using Wissance.WebApiToolkit.Data.Entity;
using Wissance.WebApiToolkit.Ef.Factories;

namespace Wissance.WebApiToolkit.Ef.Tests.Factories
{
    public class User : IModelIdentifiable<Guid>
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
        public virtual IList<Profile> Profiles { get; set; }
        public Guid OrganizationId { get; set; }
        public virtual Organization Organization {get; set; }
    }

    public class Profile : IModelIdentifiable<Guid>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public virtual IList<User> Users { get; set; }
    }

    public class Organization : IModelIdentifiable<Guid>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public virtual IList<User> Users { get; set; }
    }

    public class TestPassFactory
    {
        [Fact]
        public void TestCreateWithCyclingDepsRemove()
        {
            Organization org = new Organization()
            {
                Id = Guid.NewGuid(),
                Name = "test_org"
            };
            User user1 = new User()
            {
                Id = Guid.NewGuid(),
                Name = "Ivan Ivanov",
                OrganizationId = org.Id,
                Organization = org
            };
            User user2 = new User()
            {
                Id = Guid.NewGuid(),
                Name = "Petr Petrov",
                OrganizationId = org.Id,
                Organization = org
            };
            Profile regularProfile = new Profile()
            {
                Id = Guid.NewGuid(),
                Name = "User",
                Users = new List<User>() {user1, user2}
            };
            Profile adminProfile = new Profile()
            {
                Id = Guid.NewGuid(),
                Name = "Admin",
                Users = new List<User>() {user2}
            };
            user1.Profiles = new List<Profile>() {regularProfile};
            user2.Profiles = new List<Profile>() {regularProfile, adminProfile};
            
            org.Users = new List<User>() {user1, user2};

            User user1Repr = PassFactory.CreateRes(user1);
            Assert.Null(user1Repr.Profiles);
            Assert.Null(user1Repr.Organization);
            User user2Repr = PassFactory.CreateRes(user2);
            Assert.Null(user2Repr.Profiles);
            Assert.Null(user2Repr.Organization);
        }
    }
}