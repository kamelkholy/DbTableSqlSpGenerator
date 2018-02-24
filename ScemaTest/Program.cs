using System.IO;
using Newtonsoft.Json;

namespace ScemaTest
{
    internal static class Program
    {
        public static void Main()
        {
            var json = File.ReadAllText(@"jsonpath...");
            var dbTableInformation = JsonConvert.DeserializeObject<DataModel>(json);

            JsonDbInitializer dbInitializer = new JsonDbInitializer(dbTableInformation);
            dbInitializer.Initialize();
        }
    }
}
