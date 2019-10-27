using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using static ILEngine.OpCodeConstructorFlags;
namespace ILEngine
{
    /// <summary>
    /// Dynamically injected <see cref="System.Reflection.Emit.OpCode"/> to support runtime execution of MSIL instructions.
    /// </summary>
    public class ILDynamicOpcode
    {
        /// <summary>
        /// Execute runtime generated MSIL instructions for a dynamic instance method
        /// </summary>
        public static readonly OpCode ExecMsilInstanceMethod;
        /// <summary>
        /// Execute runtime generated MSIL instructions for a dynamic static method
        /// </summary>
        public static readonly OpCode ExecMsilStaticMethod;

        /// <summary>
        /// Create and inject <see cref="System.Reflection.Emit.OpCodes"/> for <see cref="ExecMsilInstanceMethod"/> and <see cref="ExecMsilInstanceMethod"/>.
        /// </summary>
        static ILDynamicOpcode()
        {

            var msilInstruction = "exec.msil.i";
            var msilByteCode = (byte)ILOpCodeValues.Exec_MSIL_I;// (byte)available[max++];
            ExecMsilInstanceMethod = MakeDynamicOpCode(msilInstruction, msilByteCode);

            msilInstruction = "exec.msil.s";
            msilByteCode = (byte)ILOpCodeValues.Exec_MSIL_S; // (byte)available[max++];
            ExecMsilStaticMethod = MakeDynamicOpCode(msilInstruction, msilByteCode);
        }

        /// <summary>
        /// Create a dynamic <see cref="System.Reflection.Emit.OpCode"/> and inject it into the <see cref="System.Reflection.Emit.OpCodes"/> cache.
        /// </summary>
        /// <param name="msilInstruction"></param>
        /// <param name="msilByteCode"></param>
        /// <returns></returns>
        public static OpCode MakeDynamicOpCode(string msilInstruction, short msilByteCode)
        {
            var t = typeof(OpCode);

            // Initialize the OpCode name cache by accessing the Name property on existing opcode.
            var opCallName = System.Reflection.Emit.OpCodes.Call.Name;
            var nameCacheField = t.GetField("g_nameCache", BindingFlags.Static | BindingFlags.NonPublic);

            //inject our OpCode name into the name cache table;
            var gNameCache = nameCacheField.GetValue(null);
            ((string[])gNameCache)[msilByteCode] = msilInstruction;
            nameCacheField.SetValue(null, gNameCache);





            //The constructor takes the following integer flags
            // Build constructor flags matching OpCodes.Call
            var ctorFlags =
                    ((int)OperandType.InlineMethod) |
                    ((int)FlowControl.Call << FlowControlShift) |
                    ((int)OpCodeType.Primitive << OpCodeTypeShift) |
                    ((int)StackBehaviour.Varpop << StackBehaviourPopShift) |
                    ((int)StackBehaviour.Varpush << StackBehaviourPushShift) |
                    (1 << SizeShift) |
                    (0 << StackChangeShift);

            return OpCode(msilByteCode, ctorFlags);


        }
        /// <summary>
        /// <see cref="System.Reflection.Emit.OpCode"/> RuntimeType 
        /// </summary>
        private static System.Type opCodeType = typeof(OpCode);

        /// <summary>
        /// Internal <see cref="System.Reflection.Emit.OpCode"/> constuctor
        /// </summary>
        private static ConstructorInfo OpCodeConstructor = opCodeType.GetTypeInfo().DeclaredConstructors.First();

        /// <summary>
        /// Create a new instance of a <see cref="System.Reflection.Emit.OpCode"/>
        /// </summary>
        /// <param name="opCodeValue">Short value representing an internal <see cref="System.Reflection.Emit.OpCodeValues"/></param>
        /// <param name="opCodeFlags">Constructor flags as defined in <see cref="OpCodeConstructorFlags"/></param>
        /// <returns></returns>
        public static OpCode OpCode(short opCodeValue, int opCodeFlags) => (OpCode)OpCodeConstructor.Invoke(new object[] { opCodeValue, opCodeFlags });


        /// <summary>
        /// Create a new instance of a <see cref="System.Reflection.Emit.OpCode"/>
        /// </summary>
        /// <param name="opCodeValue">Short value representing an internal <see cref="System.Reflection.Emit.OpCodeValues"/></param>
        /// <param name="opCodeFlags">Constructor flags as defined in <see cref="OpCodeConstructorFlags"/></param>
        /// <returns></returns>
        public static OpCode OpCode(int opCodeValue, int opCodeFlags) => (OpCode)OpCodeConstructor.Invoke(new object[] { opCodeValue, opCodeFlags });


        /// <summary>
        /// Create a new instance of a <see cref="System.Reflection.Emit.OpCode"/>
        /// </summary>
        /// <param name="opCodeValue"><see cref="ILOpCodeValues"/> value representing an internal <see cref="System.Reflection.Emit.OpCodeValues"/></param>
        /// <param name="opCodeFlags"></param>
        /// <returns></returns>
        public static OpCode OpCode(ILOpCodeValues opCodeValue, int opCodeFlags) => (OpCode)OpCodeConstructor.Invoke(new object[] { (short)opCodeValue, opCodeFlags });
    }
}
