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
            
        }
    }
}