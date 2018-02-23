using System.Collections.Generic;

namespace ScemaTest
{
    public class DataModel
    {
        public string Name { get; set; }
        public Dictionary<string, Dictionary<string, object>> Properties { get; set; }
        public string PrimaryKey { get; set; }
        public string[][] SelectBy { get; set; }
    }
}