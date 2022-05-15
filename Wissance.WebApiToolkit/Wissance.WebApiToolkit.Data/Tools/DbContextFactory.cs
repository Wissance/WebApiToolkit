using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks.Dataflow;
using Newtonsoft.Json.Linq;

namespace Wissance.WebApiToolkit.Data.Tools
{
    public class DbContextFactory
    {
        /// <summary>
        ///    This function return connecting string from project json config file using path to navigate properties
        /// </summary>
        /// <param name="project"></param>
        /// <param name="jsonConfigFile"></param>
        /// <param name="connStringPath"></param>
        /// <returns></returns>
        public string GetConnStrFromJsonConfig(string project, string jsonConfigFile, string connStringPath)
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
    }
}
