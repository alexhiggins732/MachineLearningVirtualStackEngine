using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ILEngine
{

    public enum ILOpCodeValues
    {
        /// <summary>
        /// Fills space if opcodes are patched
        /// </summary>
        Nop = 0x00,
        /// <summary>
        /// Signals CLR to inform debugger breakpoint has been hit
        /// </summary>
        Break = 0x01,
        /// <summary>
        /// Load arg at index 0 onto the stack
        /// </summary>
        Ldarg_0 = 0x02,
        /// <summary>
        /// Load arg at index 1 onto the stack
        /// </summary>
        Ldarg_1 = 0x03,
        /// <summary>
        /// Load arg at index 2 onto the stack
        /// </summary>
        Ldarg_2 = 0x04,
        /// <summary>
        /// Load arg at index 3 onto the stack
        /// </summary>
        Ldarg_3 = 0x05,
        /// <summary>
        /// Load local variable at index 0 onto the stack
        /// </summary>
        Ldloc_0 = 0x06,
        /// <summary>
        /// Load local variable at index 1 onto the stack
        /// </summary>
        Ldloc_1 = 0x07,
        /// <summary>
        /// Load local variable at index 2 onto the stack
        /// </summary>
        Ldloc_2 = 0x08,
        /// <summary>
        /// Load local variable at index 3 onto the stack
        /// </summary>
        Ldloc_3 = 0x09,
        /// <summary>
        /// Pop value on stack and store to local variable at index 0
        /// </summary>
        Stloc_0 = 0x0a,
        /// <summary>
        /// Pop value on stack and store to local variable at index 1
        /// </summary>
        Stloc_1 = 0x0b,
        /// <summary>
        /// Pop value on stack and store to local variable at index 2
        /// </summary>
        Stloc_2 = 0x0c,
        /// <summary>
        /// Pop value on stack and store to local variable at index 3
        /// </summary>
        Stloc_3 = 0x0d,
        /// <summary>
        /// Load arg at index s (short) onto the stack
        /// </summary>
        Ldarg_S = 0x0e,
        /// <summary>
        /// Load arg address s (short) onto the stack
        /// </summary>
        Ldarga_S = 0x0f,
        /// <summary>
        /// Stor arg at index s (short) onto the stack
        /// </summary>
        Starg_S = 0x10,
        /// <summary>
        /// Load local at index s (short) onto the stack
        /// </summary>
        Ldloc_S = 0x11,
        /// <summary>
        /// Load local address s (short) onto the stack
        /// </summary>
        Ldloca_S = 0x12,
        /// <summary>
        /// Pop value off stack and store at local address s (short)
        /// </summary>
        Stloc_S = 0x13,
        /// <summary>
        /// Push null onto the stack
        /// </summary>
        Ldnull = 0x14,
        /// <summary>
        /// pushes -1 onto the stack
        /// </summary>
        Ldc_I4_M1 = 0x15,
        /// <summary>
        /// pushes 0 onto the stack
        /// </summary>
        Ldc_I4_0 = 0x16,
        /// <summary>
        /// pushes 1 onto the stack
        /// </summary>
        Ldc_I4_1 = 0x17,
        /// <summary>
        /// pushes 2 onto the stack
        /// </summary>
        Ldc_I4_2 = 0x18,
        /// <summary>
        /// pushes 3 onto the stack
        /// </summary>
        Ldc_I4_3 = 0x19,
        /// <summary>
        /// pushes 4 onto the stack
        /// </summary>
        Ldc_I4_4 = 0x1a,
        /// <summary>
        /// pushes 5 onto the stack
        /// </summary>
        Ldc_I4_5 = 0x1b,
        /// <summary>
        /// pushes 6 onto the stack
        /// </summary>
        Ldc_I4_6 = 0x1c,
        /// <summary>
        /// pushes 7 onto the stack
        /// </summary>
        Ldc_I4_7 = 0x1d,
        /// <summary>
        /// pushes 8 onto the stack
        /// </summary>
        Ldc_I4_8 = 0x1e,
        /// <summary>
        /// pushes const value S specified in the argument onto the stack as int32
        /// </summary>
        Ldc_I4_S = 0x1f,
        /// <summary>
        /// pushes the const value specifiied in the argument onto the stack as an int32
        /// </summary>
        Ldc_I4 = 0x20,
        /// <summary>
        /// pushes the const value specifiied in the argument onto the stack as an int64
        /// </summary>
        Ldc_I8 = 0x21,
        Ldc_R4 = 0x22,
        Ldc_R8 = 0x23,
        Exec_MSIL_I = 0x24,
        Dup = 0x25,
        Pop = 0x26,
        Jmp = 0x27,
        Call = 0x28,
        Calli = 0x29,
        Ret = 0x2a,
        Br_S = 0x2b,
        Brfalse_S = 0x2c,
        Brtrue_S = 0x2d,
        /// <summary>
        /// Branch to the target instruction at offset target if the two values are equal.
        /// The target instruction is represented as a 1 byte signed offset from the beginning of the instruction following the current instruction.
        /// Control transfers into and out of try, catch, filter, and finally blocks cannot be performed by this instruction which must use the Leave instruction instead. 
        /// </summary>
        Beq_S = 0x2e,
        Bge_S = 0x2f,
        Bgt_S = 0x30,
        Ble_S = 0x31,
        Blt_S = 0x32,
        Bne_Un_S = 0x33,
        Bge_Un_S = 0x34,
        Bgt_Un_S = 0x35,
        Ble_Un_S = 0x36,
        Blt_Un_S = 0x37,
        Br = 0x38,
        Brfalse = 0x39,
        Brtrue = 0x3a,
        /// <summary>
        /// Branch to the target instruction at offset target if the two values are equal.
        /// The target instruction is represented as a 4-byte signed offset from the beginning of the instruction following the current instruction.
        /// Control transfers into and out of try, catch, filter, and finally blocks cannot be performed by this instruction which must use the Leave instruction instead. 
        /// </summary>
        Beq = 0x3b,
        Bge = 0x3c,
        Bgt = 0x3d,
        Ble = 0x3e,
        Blt = 0x3f,
        Bne_Un = 0x40,
        Bge_Un = 0x41,
        Bgt_Un = 0x42,
        Ble_Un = 0x43,
        Blt_Un = 0x44,
        Switch = 0x45,
        Ldind_I1 = 0x46,
        Ldind_U1 = 0x47,
        Ldind_I2 = 0x48,
        Ldind_U2 = 0x49,
        Ldind_I4 = 0x4a,
        Ldind_U4 = 0x4b,
        Ldind_I8 = 0x4c,
        Ldind_I = 0x4d,
        Ldind_R4 = 0x4e,
        Ldind_R8 = 0x4f,
        Ldind_Ref = 0x50,
        Stind_Ref = 0x51,
        Stind_I1 = 0x52,
        Stind_I2 = 0x53,
        Stind_I4 = 0x54,
        Stind_I8 = 0x55,
        Stind_R4 = 0x56,
        Stind_R8 = 0x57,
        /// <summary>
        /// Adds two numeric values, returning a new numeric value.  
        /// Overflow is not detected for integer operations (for proper overflow handling, see <see cref="Add_Ovf"/>).
        ///  Integer addition wraps, rather than saturates. For example, assuming 8-bit integers where value1 is set to 255 and value2 is set to 1, the wrapped result is 0 rather than 256.
        ///  Floating-point overflow returns +inf (PositiveInfinity) or -inf (NegativeInfinity). 
        /// </summary>

        Add = 0x58,
        Sub = 0x59,
        Mul = 0x5a,
        Div = 0x5b,
        Div_Un = 0x5c,
        Rem = 0x5d,
        Rem_Un = 0x5e,
        /// <summary>
        /// Determines the bitwise AND of two integer values. And is an integer-specific operation. 
        /// </summary>
        And = 0x5f,
        Or = 0x60,
        Xor = 0x61,
        Shl = 0x62,
        Shr = 0x63,
        Shr_Un = 0x64,
        Neg = 0x65,
        Not = 0x66,
        Conv_I1 = 0x67,
        Conv_I2 = 0x68,
        Conv_I4 = 0x69,
        Conv_I8 = 0x6a,
        Conv_R4 = 0x6b,
        Conv_R8 = 0x6c,
        Conv_U4 = 0x6d,
        Conv_U8 = 0x6e,
        Callvirt = 0x6f,
        Cpobj = 0x70,
        Ldobj = 0x71,
        Ldstr = 0x72,
        Newobj = 0x73,
        Castclass = 0x74,
        Isinst = 0x75,
        Conv_R_Un = 0x76,

        Exec_MSIL_S = 0x77,
        Unbox = 0x79,
        Throw = 0x7a,
        Ldfld = 0x7b,
        Ldflda = 0x7c,
        Stfld = 0x7d,
        Ldsfld = 0x7e,
        Ldsflda = 0x7f,
        Stsfld = 0x80,
        Stobj = 0x81,
        Conv_Ovf_I1_Un = 0x82,
        Conv_Ovf_I2_Un = 0x83,
        Conv_Ovf_I4_Un = 0x84,
        Conv_Ovf_I8_Un = 0x85,
        Conv_Ovf_U1_Un = 0x86,
        Conv_Ovf_U2_Un = 0x87,
        Conv_Ovf_U4_Un = 0x88,
        Conv_Ovf_U8_Un = 0x89,
        Conv_Ovf_I_Un = 0x8a,
        Conv_Ovf_U_Un = 0x8b,
        Box = 0x8c,
        Newarr = 0x8d,
        Ldlen = 0x8e,
        Ldelema = 0x8f,
        Ldelem_I1 = 0x90,
        Ldelem_U1 = 0x91,
        Ldelem_I2 = 0x92,
        Ldelem_U2 = 0x93,
        Ldelem_I4 = 0x94,
        Ldelem_U4 = 0x95,
        Ldelem_I8 = 0x96,
        Ldelem_I = 0x97,
        Ldelem_R4 = 0x98,
        Ldelem_R8 = 0x99,
        Ldelem_Ref = 0x9a,
        Stelem_I = 0x9b,
        Stelem_I1 = 0x9c,
        Stelem_I2 = 0x9d,
        Stelem_I4 = 0x9e,
        Stelem_I8 = 0x9f,
        Stelem_R4 = 0xa0,
        Stelem_R8 = 0xa1,
        Stelem_Ref = 0xa2,
        Ldelem = 0xa3,
        Stelem = 0xa4,
        Unbox_Any = 0xa5,
        Conv_Ovf_I1 = 0xb3,
        Conv_Ovf_U1 = 0xb4,
        Conv_Ovf_I2 = 0xb5,
        Conv_Ovf_U2 = 0xb6,
        Conv_Ovf_I4 = 0xb7,
        Conv_Ovf_U4 = 0xb8,
        Conv_Ovf_I8 = 0xb9,
        Conv_Ovf_U8 = 0xba,
        Refanyval = 0xc2,
        Ckfinite = 0xc3,
        Mkrefany = 0xc6,
        Ldtoken = 0xd0,
        Conv_U2 = 0xd1,
        Conv_U1 = 0xd2,
        Conv_I = 0xd3,
        Conv_Ovf_I = 0xd4,
        Conv_Ovf_U = 0xd5,
        /// <summary>
        /// Adds two signed integer values with an overflow check.  OverflowException is thrown if the result is not represented in the result type.
        /// You can perform this operation on signed integers. For floating-point values, use <see cref="Add"/>
        /// </summary>
        Add_Ovf = 0xd6,
        /// <summary>
        /// Adds two unsigned integer values with an overflow check.  OverflowException is thrown if the result is not represented in the result type.
        /// You can perform this operation on signed integers. For floating-point values, use <see cref="Add"/>
        /// </summary>
        Add_Ovf_Un = 0xd7,
        Mul_Ovf = 0xd8,
        Mul_Ovf_Un = 0xd9,
        Sub_Ovf = 0xda,
        Sub_Ovf_Un = 0xdb,
        Endfinally = 0xdc,
        Leave = 0xdd,
        Leave_S = 0xde,
        Stind_I = 0xdf,
        Conv_U = 0xe0,
        /// <summary>
        /// Reserved and unused.
        /// </summary>
        Prefix7 = 0xf8,
        /// <summary>
        /// Reserved and unused.
        /// </summary>
        Prefix6 = 0xf9,
        /// <summary>
        /// Reserved and unused.
        /// </summary>
        Prefix5 = 0xfa,
        /// <summary>
        /// Reserved and unused.
        ///</summary>
        Prefix4 = 0xfb,
        /// <summary>
        /// Reserved and unused.
        /// </summary>
        Prefix3 = 0xfc,
        /// <summary>
        /// Reserved and unused.
        /// </summary>
        Prefix2 = 0xfd,
        /// <summary>
        /// Used to specify the OpCode is 2 byte opcode, specified by the next byte in the instruction stream which the lower hex value of the 0xfe{XX} instructions.
        /// </summary>
        Prefix1 = 0xfe,
        Prefixref = 0xff,
        /// <summary>
        /// Unsupported in Reflection/Dynamic code. Used in statically compiled code get a native <see cref="System.TypedReference"/> to an <see cref="System.ArgIterator"/>.
        /// </summary>
        Arglist = 0xfe00,
        /// <summary>
        /// Pops two values off the stack and tests if they are equal.
        /// </summary>
        Ceq = 0xfe01,
        /// <summary>
        /// Pops two values off the stack and returns true if the signed value of the first operand is greater the the signed value of the second operand. 
        /// To compare values as unsigned values use <see cref="Cgt_Un."/>
        /// </summary>
        Cgt = 0xfe02,
        /// <summary>
        /// Pops two values off the stack and returns true if the unsigned value of the first operand is greater than the unsigned value of the second operand.
        /// To compre values as signed values use <see cref="Cgt"/>.
        /// </summary>
        Cgt_Un = 0xfe03,
        /// <summary>
        /// Pops two values off the stack and returns true if the signed value of the first operand is less than the signed value of the second operand.
        /// </summary>
        Clt = 0xfe04,
        /// <summary>
        /// Pops two values off the stack and returns true if the unsigned value of the first operand is less than the unsigned value of the second operand.
        /// </summary>
        Clt_Un = 0xfe05,
        Ldftn = 0xfe06,
        Ldvirtftn = 0xfe07,
        /// <summary>
        /// Push the arg at the index of the specified 4 byte signed integer value onto the stack
        /// </summary>
        Ldarg = 0xfe09,
        /// <summary>
        /// Pushes the address of the arg at the index of the specified 1 byte signed integer value onto the stack.
        /// </summary>
        Ldarga = 0xfe0a,
        /// <summary>
        /// Pops the a value from the top of the stack and saves it to argument at the index of the specified 4 byte signed integer value.
        /// </summary>
        Starg = 0xfe0b,
        /// <summary>
        /// Pushes the local at the index of the specified 4 byte signed interger value onto the stack.
        /// </summary>
        Ldloc = 0xfe0c,
        /// <summary>
        /// Pushes the address of the local at the index of the specified 4 byte signed integer onto the stack.
        /// </summary>
        Ldloca = 0xfe0d,
        /// <summary>
        /// Pops a value off the top of the stack and stores it to the local at the specified 4 byte signed integer.
        /// </summary>
        Stloc = 0xfe0e,
        Localloc = 0xfe0f,
        Endfilter = 0xfe11,
        Unaligned = 0xfe12,
        Volatile = 0xfe13,
        Tailcall = 0xfe14,
        Initobj = 0xfe15,
        Constrained = 0xfe16,
        Cpblk = 0xfe17,
        Initblk = 0xfe18,
        Rethrow = 0xfe1a,
        Sizeof = 0xfe1c,
        Refanytype = 0xfe1d,
        Readonly = 0xfe1e,

        // If you add more opcodes here, modify OpCode.Name to handle them correctly

    }
}
