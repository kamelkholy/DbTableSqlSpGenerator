using System;
using System.Collections.Generic;
using System.Text;

namespace ScemaTest
{
    public class JsonDbInitializer
    {
        private static DataModel _tableInformation;

        public JsonDbInitializer(DataModel tableInformation)
        {
            _tableInformation = tableInformation;
        }
        
        private void CreateDbTable()
        {
            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.AppendFormat("CREATE TABLE {0} (", _tableInformation.Name);
            foreach (var colom in _tableInformation.Properties)
                queryBuilder.AppendFormat("{0} {1},", colom.Key, MapType(colom.Value));
            queryBuilder.AppendFormat("{0} INT NOT NULL IDENTITY(1, 1), ", _tableInformation.PrimaryKey);
            queryBuilder.AppendFormat("PRIMARY KEY({0}));", _tableInformation.PrimaryKey);

            Console.WriteLine(queryBuilder);
        }

        private void CreateInsertSp()
        {
            StringBuilder queryBuilder = GenerateSpHeader(new StoredProcedureParmeters()
            {
                StoredProcedureName = "sp_" + _tableInformation.Name + "_insert",
                IncludeAllParameters = true
            });
            queryBuilder.AppendFormat("INSERT INTO {0} VALUES (", _tableInformation.Name);
            AppendParameters(queryBuilder);
            queryBuilder.Append(");");

            Console.WriteLine(queryBuilder);
        }

        private void CreateUpdateSp()
        {
            StringBuilder queryBuilder = GenerateSpHeader(new StoredProcedureParmeters()
            {
                StoredProcedureName = "sp_" + _tableInformation.Name + "_update",
                IncludeAllParameters = true,
                IncludePrimaryKey = true
            });
            queryBuilder.AppendFormat("UPDATE {0} SET ", _tableInformation.Name);
            AppendColomsWithValues(queryBuilder);
            queryBuilder.AppendFormat(" WHERE {0} = @{0}", _tableInformation.PrimaryKey);

            Console.WriteLine(queryBuilder);
        }

        private void CreateDeleteSp()
        {
            StringBuilder queryBuilder = GenerateSpHeader(new StoredProcedureParmeters()
            {
                StoredProcedureName = "sp_" + _tableInformation.Name + "_delete",
                IncludePrimaryKey = true
            });
            queryBuilder.AppendFormat("DELETE FROM {0} WHERE {1} = @{1}", _tableInformation.Name,
                _tableInformation.PrimaryKey);

            Console.WriteLine(queryBuilder);
        }

        private void CreateSelectSp()
        {
            foreach (var selctor in _tableInformation.SelectBy)
            {
                if (selctor.Length > 1)
                    CreateMultipleSelectSp(selctor);
                else
                    CreateSingleSelectSp(selctor);
            }
        }

        private void CreateSingleSelectSp(string[] selector)
        {
            StringBuilder queryBuilder = GenerateSpHeader(new StoredProcedureParmeters()
            {
                StoredProcedureName = "sp_" + _tableInformation.Name + "_select_" + selector[0],
                Parameters = selector
            });
            queryBuilder.AppendFormat("SELECT * FROM {0} WHERE {1} = @{1};", _tableInformation.Name, selector[0]);

            Console.WriteLine(queryBuilder);
        }

        private void CreateMultipleSelectSp(string[] selector)
        {
            var spName = new StringBuilder("sp_" + _tableInformation.Name + "_select");
            foreach (var s in selector)
                spName.Append("_" + s);
            
            StringBuilder queryBuilder = GenerateSpHeader(new StoredProcedureParmeters()
            {
                StoredProcedureName = spName.ToString(),
                Parameters = selector
            });
            queryBuilder.AppendFormat("SELECT * FROM {0} WHERE ", _tableInformation.Name);
            foreach (var s in selector)
                queryBuilder.AppendFormat(" {0} = @{0} AND", s);
            queryBuilder.Length -= 3;

            Console.WriteLine(queryBuilder);
        }

        private static string MapType(IDictionary<string, object> property)
        {
            return property["type"].ToString().ToLower() == "string" ? 
                "NVARCHAR(" + property["maxLength"] + ")" :
                property["type"].ToString();
        }

        private static void AppendParameters(StringBuilder queryBuilder)
        {
            foreach (var property in _tableInformation.Properties)
                queryBuilder.AppendFormat("@{0},", property.Key);
            queryBuilder.Length--;
        }

        private static void AppendColomsWithValues(StringBuilder queryBuilder)
        {
            foreach (var property in _tableInformation.Properties)
                queryBuilder.AppendFormat("{0} = @{0},", property.Key);
            queryBuilder.Length--;
        }

        private static StringBuilder GenerateSpHeader(StoredProcedureParmeters storedProcedureParmeters)
        {
            var queryHeader = new StringBuilder();
            queryHeader.Append("CREATE PROCEDURE " + storedProcedureParmeters.StoredProcedureName);
            
            if (storedProcedureParmeters.IncludePrimaryKey)
                IncludePk(queryHeader);

            if (storedProcedureParmeters.IncludeAllParameters) IncludeAllParameters(queryHeader);

            if (storedProcedureParmeters.Parameters.Length > 0) IncludeCustomParameters(storedProcedureParmeters, queryHeader);

            queryHeader.Length--;
            queryHeader.Append(" AS ");
            
            return queryHeader;
        }

        private static void IncludeCustomParameters(StoredProcedureParmeters storedProcedureParmeters,
            StringBuilder queryHeader)
        {
            foreach (var parameter in storedProcedureParmeters.Parameters)
                queryHeader.AppendFormat(" @{0} {1},", parameter,
                    MapType(_tableInformation.Properties[parameter]));
        }

        private static void IncludeAllParameters(StringBuilder queryHeader)
        {
            foreach (var property in _tableInformation.Properties)
                queryHeader.AppendFormat(" @{0} {1},", property.Key, MapType(property.Value));
        }

        private static void IncludePk(StringBuilder queryHeader)
        {
            queryHeader.AppendFormat(" @{0} {1},", _tableInformation.PrimaryKey, "int");
        }

        public void Initialize()
        {
            CreateDbTable();
            CreateInsertSp();
            CreateDeleteSp();
            CreateUpdateSp();
            CreateSelectSp();
        }
    }
}