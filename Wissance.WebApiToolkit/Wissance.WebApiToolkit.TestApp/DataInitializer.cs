using Wissance.WebApiToolkit.TestApp.Data;
using Wissance.WebApiToolkit.TestApp.Data.Entity;

namespace Wissance.WebApiToolkit.TestApp
{
    public static class DataInitializer
    {
        public static void Init(ModelContext context)
        {
            InitCodes(context);
        }

        private static void InitCodes(ModelContext context)
        {
            IList<CodeEntity> codes = new List<CodeEntity>()
            {
                new CodeEntity()
                {
                    Code = "1",
                    Name = "Software development"
                },
                new CodeEntity()
                {
                    Code = "2",
                    Name = "Hardware development"
                },
                new CodeEntity()
                {
                    Code = "3",
                    Name = "Researching"
                }
            };
            context.AddRange(codes);
            context.SaveChanges();
        }
    }
}