using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIR
{
    public class UIRMachine
    {
        /*
         Instructions:

	Get: (Alternately Load)
	Set: (Alternately Store)
	
	DefineType: <- Should this be member of Type <TypeBuilder>
	RemoveTypeDefintion:  <- Should this be member of Type <TypeBuilder>
	newarray
	new <Type>
		newarray <type>

	Type:
		GetMember:
		SetMember:
		GetMethod:
		SetMethod:
         */
        public static void Test()
        {
            var moves = new[] { "Set", "Get", "New<Type>", "NewArray<Type>", "CreateType", "ReadType", "UpdateType", "DeleteType" };
            dynamic machine = new UIRMachine();

            UIRTypeBuilder typeBuilder = machine.Execute("CreateType"); // returns TypeBuilder
            var idField = typeBuilder.CreateField("Id", "int");
            var nameField = typeBuilder.CreateField("Name", "string");

            var voidMethod = typeBuilder.CreateMethod("SomeVoid", "void");
            var funcMethod = typeBuilder.CreateMethod("SomeFunction", "int");

        }
    }
    public class UIRTypeBuilder
    {

        public UIRType Type;
        public UIRTypeBuilder(string typeName)
        {
            this.Type = new UIRType(typeName);
        }

        #region Field CRUD methods
        public UIRField CreateField(string FieldName, string FieldTypeName)
        {
            var field = UIRFieldBuilder.Create(FieldName, FieldTypeName);
            field.DeclaringType = Type;
            Type.Fields.Add(field);
            return field;
        }

        public UIRField ReadField(string fieldName) => Type.Fields.Single(x => x.FieldName == fieldName);
        public void UpdateField(UIRField field)
        {
            var existing = Type.Fields.Single(x => x.FieldId == field.FieldId);
            var index = Type.Fields.IndexOf(existing);
            Type.Fields[index] = field;
            field.DeclaringType = Type;
        }
        public void DeleteField(UIRField field)
        {
            var existing = Type.Fields.Single(x => x.FieldId == field.FieldId);
            var index = Type.Fields.IndexOf(existing);
            Type.Fields.RemoveAt(index);
        }
        #endregion

        #region Method CRUD

        public UIRMethod CreateMethod(string MethodName, string MethodTypeName)
        {
            var method = UIRMethodBuilder.Create(MethodName, MethodTypeName);
            method.DeclaringType = Type;
            Type.Methods.Add(method);
            return method;
        }

        public UIRMethod ReadMethod(string methodName) => Type.Methods.Single(x => x.MethodName == methodName);
        public void UpdateMethod(UIRMethod method)
        {
            var existing = Type.Methods.Single(x => x.MethodId == method.MethodId);
            var index = Type.Methods.IndexOf(existing);
            Type.Methods[index] = method;
            method.DeclaringType = Type;
        }
        public void DeleteMethod(UIRMethod method)
        {
            var existing = Type.Methods.Single(x => x.MethodId == method.MethodId);
            var index = Type.Methods.IndexOf(existing);
            Type.Methods.RemoveAt(index);
        }

        #endregion
    }
    public class UIRType
    {
        public List<UIRField> Fields;
        public List<UIRMethod> Methods;
        public string TypeName;

        public UIRType(string typeName)
        {
            this.TypeName = typeName;
        }
    }
    public class UIRFieldBuilder
    {
        public static UIRField Create(string fieldName, string fieldTypeName)
        {
            return new UIRField(fieldName, fieldTypeName);
        }
    }
    public class UIRMethodBuilder
    {
        public static UIRMethod Create(string methodName, string methodTypeName)
        {
            return new UIRMethod(methodName, methodTypeName);
        }
    }
    public class UIRField: IEquatable<UIRField>
    {
        public readonly Guid FieldId;
        public string FieldName;
        private string TypeName;

        public UIRField(string fieldName, string typeName) : this(Guid.NewGuid(), fieldName, typeName) { }
        public UIRField(Guid fieldId, string fieldName, string typeName)
        {
            this.FieldId = fieldId;
            this.FieldName = fieldName;
            this.TypeName = typeName;
        }

        public UIRType DeclaringType { get; internal set; }
        public override int GetHashCode() => FieldId.GetHashCode();
        public override bool Equals(object obj) => Equals(obj as UIRField);
        public bool Equals(UIRField other) => !(other is null) && FieldId == other.FieldId;
    }
    public class UIRMethod : IEquatable<UIRMethod>
    {
        public readonly Guid MethodId;
        public string MethodName;
        private string TypeName;

        public UIRMethod(string methodName, string typeName) : this(Guid.NewGuid(), methodName, typeName) { }
        public UIRMethod(Guid methodId, string methodName, string typeName)
        {
            this.MethodId = methodId;
            this.MethodName = methodName;
            this.TypeName = typeName;
        }

        public UIRType DeclaringType { get; internal set; }
        public override int GetHashCode() => MethodId.GetHashCode();
        public override bool Equals(object obj) => Equals(obj as UIRMethod);
        public bool Equals(UIRMethod other) => !(other is null) && MethodId == other.MethodId;
    }
}
