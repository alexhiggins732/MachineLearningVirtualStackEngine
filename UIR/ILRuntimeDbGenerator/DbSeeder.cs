using Dapper;
using ILEngine;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace UIR.ILRuntimeDbGenerator
{
    public class DbSeeder
    {
        public static string DbServerName = ".";
        public static string DbDatabaseName = "IlRunTime";
        static string connectionString => $"server={DbServerName};initial catalog={DbDatabaseName};trusted_connection=yes;";
        private static void SaveOpCodesToDb()
        {
            var flowContolIds = SaveToDb<FlowControl>();
            var opCodeTypeIds = SaveToDb<OpCodeType>();
            var operandTypeIds = SaveToDb<OperandType>();
            var stackBehaviourIds = SaveToDb<StackBehaviour>();
            using (var conn = new SqlConnection(connectionString))
            {

                string tableName = "OpCode";
                string query = "";
                var tblCount = conn.QueryFirst<int>("select count(0) from sys.Tables t where t.name=@tableName", new { tableName });
                if (tblCount == 0)
                {
                    query = $@"Create table [{tableName}] 
(
    Id int identity(1, 1) constraint PK_{tableName} Primary Key,
    ClrName varchar(100) not null constraint UN_{ tableName }_CLRName Unique,
    Name varchar(100) not null constraint UN_{ tableName }_Name Unique,
    Value int not null constraint UN_{ tableName }_Value Unique,
    Size int not null,
    FlowControlId int not null constraint FK_{ tableName }_FlowControlId Foreign Key References FlowControl(Id),
    OpCodeTypeId int not null constraint FK_{ tableName }_OpCodeTypeId Foreign Key References OpCodeType(Id),
    OperandTypeId int not null constraint FK_{ tableName }_OperandTypeId Foreign Key References OperandType(Id),
    StackBehaviourPopId int not null constraint FK_{ tableName }_StackBehaviourPopId Foreign Key References StackBehaviour(Id),
    StackBehaviourPushId int not null constraint FK_{ tableName }_StackBehaviourPushId Foreign Key References StackBehaviour(Id),
    Description varchar(500) not null
)";
                    conn.Execute(query);
                }


                var clrOpCodes = OpCodeLookup.OpCodesByName;

                var dbCount = conn.QueryFirst<int>($"select count(0) from [{tableName}]");
                if (dbCount == clrOpCodes.Count) return;

                query = $@"insert into [{tableName}](ClrName, Name,Value,Size,FlowControlId,OpCodeTypeId,OperandTypeId,StackBehaviourPopId,StackBehaviourPushId, Description)
values
(@ClrName, @Name,@Value,@Size,@FlowControlId,@OpCodeTypeId,@OperandTypeId,@StackBehaviourPopId,@StackBehaviourPushId,'')

";
                foreach (var opc in clrOpCodes)
                {
                    var ClrName = opc.Key;
                    var opcode = opc.Value;
                    conn.Execute(query, new
                    {
                        ClrName,
                        opcode.Name,
                        opcode.Value,
                        opcode.Size,
                        FlowControlId = flowContolIds[opcode.FlowControl],
                        OpCodeTypeId = opCodeTypeIds[opcode.OpCodeType],
                        OperandTypeId = operandTypeIds[opcode.OperandType],
                        StackBehaviourPopId = stackBehaviourIds[opcode.StackBehaviourPop],
                        StackBehaviourPushId = stackBehaviourIds[opcode.StackBehaviourPush],

                    });
                }



            }
        }
        private static Dictionary<EnumType, int> SaveToDb<EnumType>()
        {

            var t = typeof(EnumType);
            var fields = t.GetMembers(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            var d = fields.ToDictionary(x => x.Name, x => (int)((FieldInfo)x).GetValue(null));
            var d2 = fields.ToDictionary(x => x.Name, x => (EnumType)((FieldInfo)x).GetValue(null));
            var result = new Dictionary<EnumType, int>();
            var tableName = t.Name;


            var query = $@"Create table [{tableName}] 
(
    Id int identity(1, 1) constraint PK_{tableName} Primary Key,
    Name varchar(100) not null constraint UN_{ tableName }_Name Unique,
    Value int not null,
    Description varchar(500) not null
)";

            using (var conn = new SqlConnection(connectionString))
            {
                var exists = conn.QueryFirstOrDefault<String>("select top 1 name from sys.tables where name=@TableName", new { tableName });
                if (exists == null)
                {
                    conn.Execute(query);
                    query = $@"Insert into [{ tableName }] (name, value, description) values (@Key, @Value, '')
                    select @@Identity";
                    foreach (var kvp in d)
                    {
                        var id = conn.QueryFirst<int>(query, new { kvp.Key, kvp.Value });
                        result.Add(d2[kvp.Key], id);
                    }
                }
                else
                {
                    query = $"select top 1 id from [{ tableName }] where [name]=@Key";
                    foreach (var kvp in d)
                    {
                        var id = conn.QueryFirst<int>(query, new { kvp.Key, kvp.Value });
                        result.Add(d2[kvp.Key], id);
                    }
                }


            }
            return result;
        }

        internal static void TestValueMasks()
        {
            SaveOpCodesToDb();
            List<short> allOpCodes = null;
            using (var conn = new SqlConnection(connectionString))
            {
                allOpCodes = conn.Query<short>("select value from opcode").ToList();
            }
            var negativeOpCodes = allOpCodes.Where(x => x < 0).ToList();
            var postiveOpCodes = allOpCodes.Where(x => x >= 0).ToList();
            var negativeUnsigned = negativeOpCodes.Select(opcode => unchecked((ushort)opcode)).ToList();
            var negativeLLSB = negativeOpCodes.Select(opcode => unchecked((ushort)opcode) & 255).ToList();

            var negativeUnsignedNotted = negativeOpCodes.Select(opcode => opcode & (255)).ToList();
            var negativeUnsignedNottedShifted = negativeUnsigned.Select(opcode => opcode >> 8).ToList();
            //for 2 byte opcodes, first byte == 254.
            // when ready if byte==254 )

            var postiveMatches = postiveOpCodes.Where(x => negativeOpCodes.TrueForAll(n => (n & x) == n)).ToList();
            var shortPrefix = unchecked((short)((ushort)254 << 8));
            var postiveCount = postiveMatches.Count;
        }

        static void seedDescriptions((string, string) source, string nameColumn)
        {
            var tableName = source.Item1;
           
            var query = $"update [{tableName}] set description=@description where [{nameColumn}]=@name";
            var lines = source.Item2.Split("\r\n".ToCharArray());
            bool found = false;
            bool readComment = false;
            bool readSummary = false;
            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                if (found == false)
                {
                    found = line.IndexOf("public enum") > -1 || line.IndexOf("public class") > -1;
                    readComment = true;
                }
                else
                {
                    if (!readComment)
                    {
                        readComment = line.IndexOf("//") > -1;
                    }
                    else if (!readSummary)
                    {
                        readSummary = line.IndexOf("// Summary") > -1;
                    }
                    else
                    {
                        List<string> summaryLines = new List<string>();
                        while (string.IsNullOrEmpty(line) || line.IndexOf("//") > -1)
                        {
                            if (!string.IsNullOrEmpty(line))
                            {
                                var descriptionToken = line.Substring(line.IndexOf("//") + 2).Trim();
                                summaryLines.Add(descriptionToken);
                            }
                            line = lines[++i];
                        }
                        var nameLine = line.Trim();
                        var name = nameLine.Split(' ')[0];
                        if (name == "public")
                        {
                            nameLine = nameLine.Trim().Replace("public ", "");
                            name = nameLine.Split(' ')[0];
                        }
                        if (name == "static")
                        {
                            nameLine = nameLine.Trim().Replace("static ", "");
                            name = nameLine.Split(' ')[0];
                        }
                        if (name == "readonly")
                        {
                            nameLine = nameLine.Trim().Replace("readonly ", "");
                            name = nameLine.Split(' ')[0];
                        }
                        if(string.Compare(name, tableName, true) == 0)
                        {
                            nameLine = nameLine.Substring(tableName.Length + 1);
                            name = nameLine.Split(' ')[0];
                        }
                        name = name.Trim(';');
                        string description = string.Join(" ", summaryLines);
                        using (var conn = new SqlConnection(connectionString))
                        {
                            conn.Execute(query, new { name, description });
                        }
                        readSummary = false;

                    }
                }
            }
        }
        public static void SeedOpCodeEnumDocs()
        {
            List<(string, string)> sources = null;
            using (var conn = new SqlConnection(connectionString))
            {
                sources = conn.Query<(string, string)>("select Name, SourceCode from MetaSourceCode").ToList();
            }
            foreach (var source in sources)
            {
                seedDescriptions(source, "name");
            }
        }
        public static void SeedOpCodesDocs()
        {
            string sourceCode = File.ReadAllText(@"C:\Users\alexander.higgins\AppData\Local\Temp\MetadataAsSource\52c6ecf3227b47888e6faaa2361d581e\2392982a7a6f49308cf6c04b4f09a729\opcodes.cs");
            (string, string) source = ("opcode",sourceCode);
            seedDescriptions(source, "ClrName");
        }
        public static void SeedOpCodeEnumSourceCode()
        {
            var types = new Dictionary<dynamic, string>
            {
                { EnumOf<FlowControl>(),
                    @"C:\Users\alexander.higgins\AppData\Local\Temp\MetadataAsSource\52c6ecf3227b47888e6faaa2361d581e\b5dc90b510d84fc496cfc393f550b7e0" },
                { EnumOf<OpCodeType>(), @"C:\Users\alexander.higgins\AppData\Local\Temp\MetadataAsSource\52c6ecf3227b47888e6faaa2361d581e\26ff7e67de8a4cb8a8e489af3658c3c9" },
                { EnumOf<OperandType>(), @"C:\Users\alexander.higgins\AppData\Local\Temp\MetadataAsSource\52c6ecf3227b47888e6faaa2361d581e\a2c529b556674214a19e8f4ba41e0d4e\" },
                { EnumOf<StackBehaviour>(),  @"C:\Users\alexander.higgins\AppData\Local\Temp\MetadataAsSource\52c6ecf3227b47888e6faaa2361d581e\e14792693f684a35846842b589ffa450\" },
            };
            using (var conn = new SqlConnection(connectionString))
            {
                var tableName = "MetaSourceCode";
                var query = $@"Create table [{tableName}](
    Id int identity(1,1) not null constraint PK_{tableName} Primary Key,
    Name varchar(100) not null,
    AssemblyQualifiedName varchar(300) not null,
    CodeBase varchar(500) not null,
    SourceCode varchar(max) not null
)";
                var tableCount = conn.QueryFirst<int>("select count(0) from sys.tables where name=@tableName", new { tableName });
                if (tableCount == 0)
                {
                    conn.Execute(query);
                }

                query = "Insert into MetaSourceCode(Name, AssemblyQualifiedName,CodeBase, SourceCode) values(@Name, @AssemblyQualifiedName, @CodeBase, @SourceCode)";
                foreach (var kvp in types)
                {
                    var folder = new DirectoryInfo(kvp.Value);
                    var file = folder.GetFiles("*.cs").First();
                    var sourceCode = File.ReadAllText(file.FullName);
                    Type type = kvp.Key.TType;
                    var assmeblyPath = type.Assembly.CodeBase;
                    string TypeName = type.Name;



                    conn.Execute(query, new { type.Name, type.AssemblyQualifiedName, type.Assembly.CodeBase, sourceCode });
                }
            }
        }
        public static EnumOfWrapper<T> EnumOf<T>() where T : Enum
        {
            return new EnumOfWrapper<T>();
        }
    }
    public class EnumOfWrapper<T> where T : Enum
    {
        public T Value;
        public Type TType = typeof(T);
        public string TypeName = typeof(T).Name;
    }
}
