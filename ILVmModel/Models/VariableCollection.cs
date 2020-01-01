using System.Collections.Generic;
using System.Linq;

namespace ILVmModel.Models
{
    public class VariableCollection //: Dictionary<string, dynamic>
    {
        Dictionary<string, dynamic> variables;
        public VariableCollection()
        {
            variables = new Dictionary<string, dynamic>();
        }
        public void Add(string argumentName, dynamic value)
        {
            this.variables.Add(argumentName, value);
        }
        public void Add(dynamic value)
        {
            this.variables.Add($"arg_{variables.Count}", value);
        }
        public dynamic[] ValuesArray => variables.Values.ToArray();
        public string[] KeysArray => variables.Keys.ToArray();
        public KeyValuePair<string, dynamic>[] Items => variables.ToArray();
        public int Count => variables.Count;
    }
}
