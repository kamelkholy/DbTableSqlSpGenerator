namespace ScemaTest
{
    public class StoredProcedureParmeters
    {
        public string StoredProcedureName { get; set; }
        public string[] Parameters { get; set; } = {};
        public bool IncludePrimaryKey { get; set; } = false;
        public bool IncludeAllParameters { get; set; } = false;
    }
}