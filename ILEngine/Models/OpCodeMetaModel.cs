using Dapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILEngine.Models
{
    public class OpCodeMetaModel
    {
        public static List<OpCodeMetaModel> OpCodeMetas;
        static OpCodeMetaModel()
        {
            var json = System.IO.File.ReadAllText(@"Models\OpCodeMetaModel.json");
            OpCodeMetas = JsonConvert.DeserializeObject<List<OpCodeMetaModel>>(json);
        }
        public static void UpdateModelFromDb(string[] outputPaths, string connection= "server=.;Initial Catalog=ILRuntime;Integrated Security=true;")
        {

            List<OpCodeMetaModel> metaModels;

            using (var conn = new SqlConnection(connection))
            {
                metaModels = conn.Query<OpCodeMetaModel>("select * from [vwOpCodeFullMeta]").ToList();

            }
            var json = JsonConvert.SerializeObject(metaModels, Formatting.Indented);
            foreach (var path in outputPaths)
            {
                File.WriteAllText(path, json);
            }

        }
        public string ClrName { get; set; }
        public string OpCode { get; set; }
        public int OpCodeValue { get; set; }
        public int OpCodeSize { get; set; }
        public string FlowControl { get; set; }
        public int FlowControlValue { get; set; }
        public string OpCodeType { get; set; }
        public int OpCodeTypeValue { get; set; }
        public string OperandType { get; set; }
        public int OperandTypeValue { get; set; }
        public int OperandTypeBitSize { get; set; }
        public int OperandTypeByteSize { get; set; }
        public bool OperandTypeIsFloatingPoint { get; set; }
        public string OperandTypeSystemType { get; set; }
        public string StackBehaviourPop { get; set; }
        public int StackBehaviourPopValue { get; set; }
        public int StackBehaviourPopCount { get; set; }
        public string StackBehaviourPopType0 { get; set; }
        public string StackBehaviourPopType1 { get; set; }
        public string StackBehaviourPopType2 { get; set; }
        public string StackBehaviourPush { get; set; }
        public int StackBehaviourPushValue { get; set; }
        public int StackBehaviourPushCount { get; set; }
        public string StackBehaviourPushType0 { get; set; }
        public string StackBehaviourPushType1 { get; set; }
        public string OpCodeDescription { get; set; }
        public string FlowControlDescription { get; set; }
        public string OpCodeTypeDescription { get; set; }
        public string OperandTypeDescription { get; set; }
        public string StackBehaviourPopDescription { get; set; }
        public string StackBehaviourPushDescription { get; set; }
        public int OpCodeId { get; set; }
        public int FlowControlId { get; set; }
        public int OpCodeTypeId { get; set; }
        public int OperandTypeId { get; set; }
        public int StackBehaviourPopId { get; set; }
        public int StackBehaviourPushId { get; set; }

        public override string ToString()
        {
            return $"{ClrName}: {FlowControl} {OperandType} ({OperandTypeBitSize} bits) {OpCodeType} Push: {StackBehaviourPush} Pop: {StackBehaviourPop}";
        }
    }
}
