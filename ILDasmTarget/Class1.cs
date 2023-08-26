using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ILDasmTarget
{
    public class Class1
    {
    }

    public interface IDataContract
    {
        object[] ToArray();
    }
    public class DataContract : IDataContract
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Guid Guid { get; set; }
        public DateTime Type { get; set; }

        public object[] ToArray()
        {
            return new object[4]
            {
                Id, Name, Guid, Type
            };
        }
    }
    public interface IDataContractTarget { }
    public class DataContractTarget: IDataContractTarget
    {
        public DataContractTarget() { }
        public DataContractTarget(IDataContract dataContract)
            : this(dataContract.ToArray())
        {

        }

        public DataContractTarget(object[] objects)
        {
            this.Id = (int)objects[0];
            this.Name = (string)objects[1];
            this.Guid = (Guid)objects[2];
            this.Type = (DateTime)objects[3];
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public Guid Guid { get; set; }
        public DateTime Type { get; set; }
    }
}
