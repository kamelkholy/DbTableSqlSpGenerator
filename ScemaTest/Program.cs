using System.IO;
using Newtonsoft.Json;

namespace ScemaTest
{
    internal static class Program
    {
        public static void Main()
        {
            var json = File.ReadAllText(@"C:\Users\el7li\RiderProjects\ScemaTest\ScemaTest\tableSchema.json");
            var dbTableInformation = JsonConvert.DeserializeObject<DataModel>(json);

            JsonDbInitializer dbInitializer = new JsonDbInitializer(dbTableInformation);
            dbInitializer.Initialize();
        }
    }
}