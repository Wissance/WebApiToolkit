using Wissance.WebApiToolkit.TestApp.Data;
using Wissance.WebApiToolkit.TestApp.Data.Entity;

namespace Wissance.WebApiToolkit.TestApp
{
    public static class DataInitializer
    {
        public static void Init(ModelContext context)
        {
            InitCodes(context);
            InitOrganizations(context);
            InitRoles(context);
            InitUsers(context);
        }

        private static void InitCodes(ModelContext context)
        {
            IList<CodeEntity> codes = new List<CodeEntity>()
            {
                new CodeEntity()
                {
                    Id = 1,
                    Code = "1",
                    Name = "Software development"
                },
                new CodeEntity()
                {
                    Id = 2,
                    Code = "2",
                    Name = "Hardware development"
                },
                new CodeEntity()
                {
                    Id = 3,
                    Code = "3",
                    Name = "Researching"
                }
            };
            context.AddRange(codes);
            context.SaveChanges();
        }

        private static void InitOrganizations(ModelContext context)
        {
            IList<CodeEntity> codes = context.Codes.ToList();
            for (int i = 0; i < 10; i++)
            {
                OrganizationEntity organization = new OrganizationEntity()
                {
                    Name = $"LLC Organization {i}",
                    ShortName = $"Organization {i}",
                    TaxNumber = $"9{i}8{i}765{i}10",
                    Codes = new List<CodeEntity>()
                };
                if (i % 2 == 0)
                {
                    organization.Codes.Add(codes[0]);
                    organization.Codes.Add(codes[1]);
                }
                else
                {
                    organization.Codes.Add(codes[1]);
                    organization.Codes.Add(codes[2]);
                }

                context.Organizations.Add(organization);
            }

            context.SaveChanges();
        }
        
        private static void InitUsers(ModelContext context)
        {
            IList<OrganizationEntity> organizations = context.Organizations.ToList();
            int count = 1;
            foreach (OrganizationEntity organization in organizations)
            {
                IList<int> roles = new List<int>();
                if (count >= 4 && count < 6)
                {
                    roles.Add(AdminRoleId);
                    roles.Add(ManagerRoleId);
                }
                else
                {
                    if (count % 2 == 0)
                        roles.Add(ManagerRoleId);
                    else roles.Add(CorporateSlaveRoleId);
                }

                UserEntity user = new UserEntity()
                {
                    Login = $"user_{organization.Id}",
                    OrganizationId = organization.Id,
                    FullName = $"Demo Demo {organization.Id}",
                    Roles = context.Roles.Where(r => roles.Contains(r.Id)).ToList()
                };
                context.Users.Add(user);
                count++;
            }

            context.SaveChanges();
        }

        private static void InitRoles(ModelContext context)
        {
            context.Roles.Add(new RoleEntity()
            {
                Id = AdminRoleId,
                Name = "Administrator"
            });
            context.Roles.Add(new RoleEntity()
            {
                Id = ManagerRoleId,
                Name = "Office manager"
            });
            context.Roles.Add(new RoleEntity()
            {
                Id = CorporateSlaveRoleId,
                Name = "Corporation slave"
            });

            context.SaveChanges();
        }

        private const int AdminRoleId = 1;
        private const int ManagerRoleId = 2;
        private const int CorporateSlaveRoleId = 3;
    }
}