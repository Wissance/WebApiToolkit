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
                
            }
        }
        
        private static void InitUsers(ModelContext context)
        {
            
        }
    }
}