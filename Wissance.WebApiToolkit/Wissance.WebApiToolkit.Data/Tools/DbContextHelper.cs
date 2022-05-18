using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks.Dataflow;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace Wissance.WebApiToolkit.Data.Tools
{
    /// <summary>
    ///    This class is a set of tools that helps us working with EntityFramework especially for migration generation
    ///    in code first approach
    /// </summary>
    public class DbContextHelper
    {
        /// <summary>
        ///    This function return connecting string from project json config file using path to navigate properties
        ///    Example we have following JSON with name - migration.setting.json:
        ///    {
        ///        "AppSettings": {
        ///            "Db" : {
        ///                "ConnStr": ""Server=(localdb)\\mssqllocaldb;Database=MyApp;Trusted_Connection=True;"
        ///            }
        ///        }
        ///    }
        ///    consider that this settings file is located in project MyApp/MyApp.Data/migration.setting.json
        ///    where MyApp is solution name, MyApp.Data - project name
        ///    therefore for these values we should call this method to get conn str:
        ///    GetConnStrFromJsonConfig("MyApp.Data", "migration.setting.json", "AppSettings.Db.ConnStr")
        /// </summary>
        /// <param name="project"></param>
        /// <param name="jsonConfigFile">name of json config path or path relative to project</param>
        /// <param name="connStringPath">path to property that points to connection string</param>
        /// <returns></returns>
        public string GetConnStrFromJsonConfig(string project, string jsonConfigFile, string connStringPath)
        {
            try
            {
                string solutionDir = new DirectoryInfo(Environment.CurrentDirectory)?.Parent?.FullName;
                if (solutionDir == null)
                    throw new InvalidOperationException("Solution dir can't be null");
                Console.WriteLine($"Solution directory is: {solutionDir}");
                string startupPath = Path.GetFullPath(Path.Combine(solutionDir, project));
                Console.WriteLine($"Startup path is: {startupPath}");
                string configFullPath = Path.Combine(startupPath, jsonConfigFile);
                string jsonData = File.ReadAllText(configFullPath);
                JObject json = JObject.Parse(jsonData);
                string connectionString = (string)json.SelectToken(connStringPath);
                Console.WriteLine($"Connection string is: {connectionString}");
                return connectionString;
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occurred during getting connection string from JSON config file: {e.Message}");
                return null;
            }
        }

        /// <summary>
        ///    This function helps us to create instance of DbContext. It is more complicated that probably should because
        ///    DbContext depends on option but options depends on Extensions like .UseMySql(), .UseSqlServer(), pseudo code,
        ///    Consider that we are having ModelContext class derived from DbContext:
        ///    {
        ///         DbContextHelper helper = new DbContextHelper();
        ///         // string connStr = "server=127.0.0.1;database=my_app_db;uid=my_app_user;password=myPWD;SslMode=preferred;"
        ///         string connStr = helper.GetConnStrFromJsonConfig("MyApp.Data", "migration.setting.json", "AppSettings.Db.ConnStr")
        ///         DbContextOptionsBuilder<ModelContext> optionsBuilder = new DbContextOptionsBuilder<ModelContext>().UseMySql(connStr, ServerVersion.AutoDetect(connStr));
        ///         DbContextOptions<ModelContext> options = optionsBuilder.Options;
        ///         Func<DbContextOptions<ModelContext>, ModelContext> constructor = opts => new ModelContext(opts);
        ///         ModelContext context = helper.Create<ModelContext>("some-conn-str", options, constructor);
        ///         // do other things ....
        ///    }
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="options"></param>
        /// <param name="constructor"></param>
        /// <returns></returns>
        public T Create<T>(DbContextOptions<T> options, Func<DbContextOptions<T>, T> constructor) where T: DbContext, new()
        {
            return constructor(options);
        }
    }
}
