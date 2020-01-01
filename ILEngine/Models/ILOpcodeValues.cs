using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ILEngine
{

    public enum ILOpCodeValues
    {
        /// <summary>
        /// Fills space if opcodes are patched. No meaningful operation is performed although a processing cycle can be consumed.
        /// </summary>
        Nop = 0x00,

        /// <summary>
        /// Signals the Common Language Infrastructure (CLI) to inform the debugger that a break point has been tripped.
        /// </summary>
        Break = 0x01,

        /// <summary>
        /// Loads the argument at index 0 onto the evaluation stack.
        /// </summary>
        Ldarg_0 = 0x02,

        /// <summary>
        /// Loads the argument at index 1 onto the evaluation stack.
        /// </summary>
        Ldarg_1 = 0x03,

        /// <summary>
        /// Loads the argument at index 2 onto the evaluation stack.
        /// </summary>
        Ldarg_2 = 0x04,

        /// <summary>
        /// Loads the argument at index 3 onto the evaluation stack.
        /// </summary>
        Ldarg_3 = 0x05,

        /// <summary>
        /// Loads the local variable at index 0 onto the evaluation stack.
        /// </summary>
        Ldloc_0 = 0x06,

        /// <summary>
        /// Loads the local variable at index 1 onto the evaluation stack.
        /// </summary>
        Ldloc_1 = 0x07,

        /// <summary>
        /// Loads the local variable at index 2 onto the evaluation stack.
        /// </summary>
        Ldloc_2 = 0x08,

        /// <summary>
        /// Loads the local variable at index 3 onto the evaluation stack.
        /// </summary>
        Ldloc_3 = 0x09,

        /// <summary>
        /// Pops the current value from the top of the evaluation stack and stores it in a the local variable list at index 0.
        /// </summary>
        Stloc_0 = 0x0a,

        /// <summary>
        /// Pops the current value from the top of the evaluation stack and stores it in a the local variable list at index 1.
        /// </summary>
        Stloc_1 = 0x0b,

        /// <summary>
        /// Pops the current value from the top of the evaluation stack and stores it in a the local variable list at index 2.
        /// </summary>
        Stloc_2 = 0x0c,

        /// <summary>
        /// Pops the current value from the top of the evaluation stack and stores it in a the local variable list at index 3.
        /// </summary>
        Stloc_3 = 0x0d,

        /// <summary>
        /// Loads the argument (referenced by a specified short form index) onto the evaluation stack.
        /// </summary>
        Ldarg_S = 0x0e,

        /// <summary>
        /// Load an argument address, in short form, onto the evaluation stack.
        /// </summary>
        Ldarga_S = 0x0f,

        /// <summary>
        /// Stores the value on top of the evaluation stack in the argument slot at a specified index, short form.
        /// </summary>
        Starg_S = 0x10,

        /// <summary>
        /// Loads the local variable at a specific index onto the evaluation stack, short form.
        /// </summary>
        Ldloc_S = 0x11,

        /// <summary>
        /// Loads the address of the local variable at a specific index onto the evaluation stack, short form.
        /// </summary>
        Ldloca_S = 0x12,

        /// <summary>
        /// Pops the current value from the top of the evaluation stack and stores it in a the local variable list at index (short form).
        /// </summary>
        Stloc_S = 0x13,

        /// <summary>
        /// Pushes a null reference (type O) onto the evaluation stack.
        /// </summary>
        Ldnull = 0x14,

        /// <summary>
        /// Pushes the integer value of -1 onto the evaluation stack as an int32.
        /// </summary>
        Ldc_I4_M1 = 0x15,

        /// <summary>
        /// Pushes the integer value of 0 onto the evaluation stack as an int32.
        /// </summary>
        Ldc_I4_0 = 0x16,

        /// <summary>
        /// Pushes the integer value of 1 onto the evaluation stack as an int32.
        /// </summary>
        Ldc_I4_1 = 0x17,

        /// <summary>
        /// Pushes the integer value of 2 onto the evaluation stack as an int32.
        /// </summary>
        Ldc_I4_2 = 0x18,

        /// <summary>
        /// Pushes the integer value of 3 onto the evaluation stack as an int32.
        /// </summary>
        Ldc_I4_3 = 0x19,

        /// <summary>
        /// Pushes the integer value of 4 onto the evaluation stack as an int32.
        /// </summary>
        Ldc_I4_4 = 0x1a,

        /// <summary>
        /// Pushes the integer value of 5 onto the evaluation stack as an int32.
        /// </summary>
        Ldc_I4_5 = 0x1b,

        /// <summary>
        /// Pushes the integer value of 6 onto the evaluation stack as an int32.
        /// </summary>
        Ldc_I4_6 = 0x1c,

        /// <summary>
        /// Pushes the integer value of 7 onto the evaluation stack as an int32.
        /// </summary>
        Ldc_I4_7 = 0x1d,

        /// <summary>
        /// Pushes the integer value of 8 onto the evaluation stack as an int32.
        /// </summary>
        Ldc_I4_8 = 0x1e,

        /// <summary>
        /// Pushes the supplied int8 value onto the evaluation stack as an int32, short form.
        /// </summary>
        Ldc_I4_S = 0x1f,

        /// <summary>
        /// Pushes a supplied value of type int32 onto the evaluation stack as an int32.
        /// </summary>
        Ldc_I4 = 0x20,

        /// <summary>
        /// Pushes a supplied value of type int64 onto the evaluation stack as an int64.
        /// </summary>
        Ldc_I8 = 0x21,

        /// <summary>
        /// Pushes a supplied value of type float32 onto the evaluation stack as type F (float).
        /// </summary>
        Ldc_R4 = 0x22,

        /// <summary>
        /// Pushes a supplied value of type float64 onto the evaluation stack as type F (float).
        /// </summary>
        Ldc_R8 = 0x23,

        /// <summary>
        /// Copies the current topmost value on the evaluation stack, and then pushes the copy onto the evaluation stack.
        /// </summary>
        Dup = 0x25,

        /// <summary>
        /// Removes the value currently on top of the evaluation stack.
        /// </summary>
        Pop = 0x26,

        /// <summary>
        /// Exits current method and jumps to specified method.
        /// </summary>
        Jmp = 0x27,

        /// <summary>
        /// Calls the method indicated by the passed method descriptor.
        /// </summary>
        Call = 0x28,

        /// <summary>
        /// Calls the method indicated on the evaluation stack (as a pointer to an entry point) with arguments described by a calling convention.
        /// </summary>
        Calli = 0x29,

        /// <summary>
        /// Returns from the current method, pushing a return value (if present) from the callee's evaluation stack onto the caller's evaluation stack.
        /// </summary>
        Ret = 0x2a,

        /// <summary>
        /// Unconditionally transfers control to a target instruction (short form).
        /// </summary>
        Br_S = 0x2b,

        /// <summary>
        /// Transfers control to a target instruction if value is false, a null reference, or zero.
        /// </summary>
        Brfalse_S = 0x2c,

        /// <summary>
        /// Transfers control to a target instruction (short form) if value is true, not null, or non-zero.
        /// </summary>
        Brtrue_S = 0x2d,

        /// <summary>
        /// Transfers control to a target instruction (short form) if two values are equal.
        /// The target instruction is represented as a 1 byte signed offset from the beginning of the instruction following the current instruction.
        /// Control transfers into and out of try, catch, filter, and finally blocks cannot be performed by this instruction which must use the Leave instruction instead. 
        /// </summary>
        Beq_S = 0x2e,

        /// <summary>
        /// Transfers control to a target instruction (short form) if the first value is greater than or equal to the second value.
        /// </summary>
        Bge_S = 0x2f,

        /// <summary>
        /// Transfers control to a target instruction (short form) if the first value is greater than the second value.
        /// </summary>
        Bgt_S = 0x30,

        /// <summary>
        /// Transfers control to a target instruction (short form) if the first value is less than or equal to the second value.
        /// </summary>
        Ble_S = 0x31,

        /// <summary>
        /// Transfers control to a target instruction (short form) if the first value is less than the second value.
        /// </summary>
        Blt_S = 0x32,

        /// <summary>
        /// Transfers control to a target instruction (short form) when two unsigned integer values or unordered float values are not equal.
        /// </summary>
        Bne_Un_S = 0x33,

        /// <summary>
        /// Transfers control to a target instruction (short form) if the first value is greater than the second value, when comparing unsigned integer values or unordered float values.
        /// </summary>
        Bge_Un_S = 0x34,

        /// <summary>
        /// Transfers control to a target instruction (short form) if the first value is greater than the second value, when comparing unsigned integer values or unordered float values.
        /// </summary>
        Bgt_Un_S = 0x35,

        /// <summary>
        /// Transfers control to a target instruction (short form) if the first value is less than or equal to the second value, when comparing unsigned integer values or unordered float values.
        /// </summary>
        Ble_Un_S = 0x36,

        /// <summary>
        /// Transfers control to a target instruction (short form) if the first value is less than the second value, when comparing unsigned integer values or unordered float values.
        /// </summary>
        Blt_Un_S = 0x37,

        /// <summary>
        /// Unconditionally transfers control to a target instruction.
        /// </summary>
        Br = 0x38,

        /// <summary>
        /// Transfers control to a target instruction if value is false, a null reference (Nothing in Visual Basic), or zero.
        /// </summary>
        Brfalse = 0x39,

        /// <summary>
        /// Transfers control to a target instruction if value is true, not null, or non-zero.
        /// </summary>
        Brtrue = 0x3a,

        /// <summary>
        /// Transfers control to a target instruction if two values are equal.
        /// The target instruction is represented as a 4-byte signed offset from the beginning of the instruction following the current instruction.
        /// Control transfers into and out of try, catch, filter, and finally blocks cannot be performed by this instruction which must use the Leave instruction instead. 
        /// </summary>
        Beq = 0x3b,

        /// <summary>
        /// Transfers control to a target instruction if the first value is greater than or equal to the second value.
        /// </summary>
        Bge = 0x3c,

        /// <summary>
        /// Transfers control to a target instruction if the first value is greater than the second value.
        /// </summary>
        Bgt = 0x3d,

        /// <summary>
        /// Transfers control to a target instruction if the first value is less than or equal to the second value.
        /// </summary>
        Ble = 0x3e,

        /// <summary>
        /// Transfers control to a target instruction if the first value is less than the second value.
        /// </summary>
        Blt = 0x3f,

        /// <summary>
        /// Transfers control to a target instruction when two unsigned integer values or unordered float values are not equal.
        /// </summary>
        Bne_Un = 0x40,

        /// <summary>
        /// Transfers control to a target instruction if the first value is greater than the second value, when comparing unsigned integer values or unordered float values.
        /// </summary>
        Bge_Un = 0x41,

        /// <summary>
        /// Transfers control to a target instruction if the first value is greater than the second value, when comparing unsigned integer values or unordered float values.
        /// </summary>
        Bgt_Un = 0x42,

        /// <summary>
        /// Transfers control to a target instruction if the first value is less than or equal to the second value, when comparing unsigned integer values or unordered float values.
        /// </summary>
        Ble_Un = 0x43,

        /// <summary>
        /// Transfers control to a target instruction if the first value is less than the second value, when comparing unsigned integer values or unordered float values.
        /// </summary>
        Blt_Un = 0x44,

        /// <summary>
        /// Implements a jump table.
        /// </summary>
        Switch = 0x45,

        /// <summary>
        /// Loads a value of type int8 as an int32 onto the evaluation stack indirectly.
        /// </summary>
        Ldind_I1 = 0x46,

        /// <summary>
        /// Loads a value of type unsigned int8 as an int32 onto the evaluation stack indirectly.
        /// </summary>
        Ldind_U1 = 0x47,

        /// <summary>
        /// Loads a value of type int16 as an int32 onto the evaluation stack indirectly.
        /// </summary>
        Ldind_I2 = 0x48,

        /// <summary>
        /// Loads a value of type unsigned int16 as an int32 onto the evaluation stack indirectly.
        /// </summary>
        Ldind_U2 = 0x49,

        /// <summary>
        /// Loads a value of type int32 as an int32 onto the evaluation stack indirectly.
        /// </summary>
        Ldind_I4 = 0x4a,

        /// <summary>
        /// Loads a value of type unsigned int32 as an int32 onto the evaluation stack indirectly.
        /// </summary>
        Ldind_U4 = 0x4b,

        /// <summary>
        /// Loads a value of type int64 as an int64 onto the evaluation stack indirectly.
        /// </summary>
        Ldind_I8 = 0x4c,

        /// <summary>
        /// Loads a value of type native int as a native int onto the evaluation stack indirectly.
        /// </summary>
        Ldind_I = 0x4d,

        /// <summary>
        /// Loads a value of type float32 as a type F (float) onto the evaluation stack indirectly.
        /// </summary>
        Ldind_R4 = 0x4e,

        /// <summary>
        /// Loads a value of type float64 as a type F (float) onto the evaluation stack indirectly.
        /// </summary>
        Ldind_R8 = 0x4f,

        /// <summary>
        /// Loads an object reference as a type O (object reference) onto the evaluation stack indirectly.
        /// </summary>
        Ldind_Ref = 0x50,

        /// <summary>
        /// Stores a object reference value at a supplied address.
        /// </summary>
        Stind_Ref = 0x51,

        /// <summary>
        /// Stores a value of type int8 at a supplied address.
        /// </summary>
        Stind_I1 = 0x52,

        /// <summary>
        /// Stores a value of type int16 at a supplied address.
        /// </summary>
        Stind_I2 = 0x53,

        /// <summary>
        /// Stores a value of type int32 at a supplied address.
        /// </summary>
        Stind_I4 = 0x54,

        /// <summary>
        /// Stores a value of type int64 at a supplied address.
        /// </summary>
        Stind_I8 = 0x55,

        /// <summary>
        /// Stores a value of type float32 at a supplied address.
        /// </summary>
        Stind_R4 = 0x56,

        /// <summary>
        /// Stores a value of type float64 at a supplied address.
        /// </summary>
        Stind_R8 = 0x57,

        /// <summary>
        /// Adds two values and pushes the result onto the evaluation stack.
        /// Overflow is not detected for integer operations (for proper overflow handling, see <see cref="Add_Ovf"/>).
        ///  Integer addition wraps, rather than saturates. For example, assuming 8-bit integers where value1 is set to 255 and value2 is set to 1, the wrapped result is 0 rather than 256.
        ///  Floating-point overflow returns +inf (PositiveInfinity) or -inf (NegativeInfinity). 
        /// </summary>
        Add = 0x58,

        /// <summary>
        /// Subtracts one value from another and pushes the result onto the evaluation stack.
        /// </summary>
        Sub = 0x59,

        /// <summary>
        /// Multiplies two values and pushes the result on the evaluation stack.
        /// </summary>
        Mul = 0x5a,

        /// <summary>
        /// Divides two values and pushes the result as a floating-point (type F) or quotient (type int32) onto the evaluation stack.
        /// </summary>
        Div = 0x5b,

        /// <summary>
        /// Divides two unsigned integer values and pushes the result (int32) onto the evaluation stack.
        /// </summary>
        Div_Un = 0x5c,

        /// <summary>
        /// Divides two values and pushes the remainder onto the evaluation stack.
        /// </summary>
        Rem = 0x5d,

        /// <summary>
        /// Divides two unsigned values and pushes the remainder onto the evaluation stack.
        /// </summary>
        Rem_Un = 0x5e,

        /// <summary>
        /// Computes the bitwise AND of two values and pushes the result onto the evaluation stack.
        /// </summary>
        And = 0x5f,

        /// <summary>
        /// Compute the bitwise complement of the two integer values on top of the stack and pushes the result onto the evaluation stack.
        /// </summary>
        Or = 0x60,

        /// <summary>
        /// Computes the bitwise XOR of the top two values on the evaluation stack, pushing the result onto the evaluation stack.
        /// </summary>
        Xor = 0x61,

        /// <summary>
        /// Shifts an integer value to the left (in zeroes) by a specified number of bits, pushing the result onto the evaluation stack.
        /// </summary>
        Shl = 0x62,

        /// <summary>
        /// Shifts an integer value (in sign) to the right by a specified number of bits, pushing the result onto the evaluation stack.
        /// </summary>
        Shr = 0x63,

        /// <summary>
        /// Shifts an unsigned integer value (in zeroes) to the right by a specified number of bits, pushing the result onto the evaluation stack.
        /// </summary>
        Shr_Un = 0x64,

        /// <summary>
        /// Negates a value and pushes the result onto the evaluation stack.
        /// </summary>
        Neg = 0x65,

        /// <summary>
        /// Computes the bitwise complement of the integer value on top of the stack and pushes the result onto the evaluation stack as the same type.
        /// </summary>
        Not = 0x66,

        /// <summary>
        /// Converts the value on top of the evaluation stack to int8, then extends (pads) it to int32.
        /// </summary>
        Conv_I1 = 0x67,

        /// <summary>
        /// Converts the value on top of the evaluation stack to int16, then extends (pads) it to int32.
        /// </summary>
        Conv_I2 = 0x68,

        /// <summary>
        /// Converts the value on top of the evaluation stack to int32.
        /// </summary>
        Conv_I4 = 0x69,

        /// <summary>
        /// Converts the value on top of the evaluation stack to int64.
        /// </summary>
        Conv_I8 = 0x6a,

        /// <summary>
        /// Converts the value on top of the evaluation stack to float32.
        /// </summary>
        Conv_R4 = 0x6b,

        /// <summary>
        /// Converts the value on top of the evaluation stack to float64.
        /// </summary>
        Conv_R8 = 0x6c,

        /// <summary>
        /// Converts the value on top of the evaluation stack to unsigned int32, and extends it to int32.
        /// </summary>
        Conv_U4 = 0x6d,

        /// <summary>
        /// Converts the value on top of the evaluation stack to unsigned int64, and extends it to int64.
        /// </summary>
        Conv_U8 = 0x6e,

        /// <summary>
        /// Calls a late-bound method on an object, pushing the return value onto the evaluation stack.
        /// </summary>
        Callvirt = 0x6f,

        /// <summary>
        /// Copies the value type located at the address of an object (type &, * or native int) to the address of the destination object (type &, * or native int).
        /// </summary>
        Cpobj = 0x70,

        /// <summary>
        /// Copies the value type object pointed to by an address to the top of the evaluation stack.
        /// </summary>
        Ldobj = 0x71,

        /// <summary>
        /// Pushes a new object reference to a string literal stored in the metadata.
        /// </summary>
        Ldstr = 0x72,

        /// <summary>
        /// Creates a new object or a new instance of a value type, pushing an object reference (type O) onto the evaluation stack.
        /// </summary>
        Newobj = 0x73,

        /// <summary>
        /// 
        /// </summary>
        Castclass = 0x74,

        /// <summary>
        /// Tests whether an object reference (type O) is an instance of a particular class.
        /// </summary>
        Isinst = 0x75,

        /// <summary>
        /// Converts the unsigned integer value on top of the evaluation stack to float32.
        /// </summary>
        Conv_R_Un = 0x76,

        /// <summary>
        /// Converts the boxed representation of a value type to its unboxed form.
        /// </summary>
        Unbox = 0x79,

        /// <summary>
        /// Throws the exception object currently on the evaluation stack.
        /// </summary>
        Throw = 0x7a,

        /// <summary>
        /// Finds the value of a field in the object whose reference is currently on the evaluation stack.
        /// </summary>
        Ldfld = 0x7b,

        /// <summary>
        /// Finds the address of a field in the object whose reference is currently on the evaluation stack.
        /// </summary>
        Ldflda = 0x7c,

        /// <summary>
        /// Replaces the value stored in the field of an object reference or pointer with a new value.
        /// </summary>
        Stfld = 0x7d,

        /// <summary>
        /// Pushes the value of a static field onto the evaluation stack.
        /// </summary>
        Ldsfld = 0x7e,

        /// <summary>
        /// Pushes the address of a static field onto the evaluation stack.
        /// </summary>
        Ldsflda = 0x7f,

        /// <summary>
        /// Replaces the value of a static field with a value from the evaluation stack.
        /// </summary>
        Stsfld = 0x80,

        /// <summary>
        /// Copies a value of a specified type from the evaluation stack into a supplied memory address.
        /// </summary>
        Stobj = 0x81,

        /// <summary>
        /// Converts the unsigned value on top of the evaluation stack to signed int8 and extends it to int32, throwing System.OverflowException on overflow.
        /// </summary>
        Conv_Ovf_I1_Un = 0x82,

        /// <summary>
        /// Converts the unsigned value on top of the evaluation stack to signed int16 and extends it to int32, throwing System.OverflowException on overflow.
        /// </summary>
        Conv_Ovf_I2_Un = 0x83,

        /// <summary>
        /// Converts the unsigned value on top of the evaluation stack to signed int32, throwing System.OverflowException on overflow.
        /// </summary>
        Conv_Ovf_I4_Un = 0x84,

        /// <summary>
        /// Converts the unsigned value on top of the evaluation stack to signed int64, throwing System.OverflowException on overflow.
        /// </summary>
        Conv_Ovf_I8_Un = 0x85,

        /// <summary>
        /// Converts the unsigned value on top of the evaluation stack to unsigned int8 and extends it to int32, throwing System.OverflowException on overflow.
        /// </summary>
        Conv_Ovf_U1_Un = 0x86,

        /// <summary>
        /// Converts the unsigned value on top of the evaluation stack to unsigned int16 and extends it to int32, throwing System.OverflowException on overflow.
        /// </summary>
        Conv_Ovf_U2_Un = 0x87,

        /// <summary>
        /// Converts the unsigned value on top of the evaluation stack to unsigned int32, throwing System.OverflowException on overflow.
        /// </summary>
        Conv_Ovf_U4_Un = 0x88,

        /// <summary>
        /// Converts the unsigned value on top of the evaluation stack to unsigned int64, throwing System.OverflowException on overflow.
        /// </summary>
        Conv_Ovf_U8_Un = 0x89,

        /// <summary>
        /// Converts the unsigned value on top of the evaluation stack to signed native int, throwing System.OverflowException on overflow.
        /// </summary>
        Conv_Ovf_I_Un = 0x8a,

        /// <summary>
        /// Converts the unsigned value on top of the evaluation stack to unsigned native int, throwing System.OverflowException on overflow.
        /// </summary>
        Conv_Ovf_U_Un = 0x8b,

        /// <summary>
        /// Converts a value type to an object reference (type O).
        /// </summary>
        Box = 0x8c,

        /// <summary>
        /// Pushes an object reference to a new zero-based, one-dimensional array whose elements are of a specific type onto the evaluation stack.
        /// </summary>
        Newarr = 0x8d,

        /// <summary>
        /// Pushes the number of elements of a zero-based, one-dimensional array onto the evaluation stack.
        /// </summary>
        Ldlen = 0x8e,

        /// <summary>
        /// Loads the address of the array element at a specified array index onto the top of the evaluation stack as type & (managed pointer).
        /// </summary>
        Ldelema = 0x8f,

        /// <summary>
        /// Loads the element with type int8 at a specified array index onto the top of the evaluation stack as an int32.
        /// </summary>
        Ldelem_I1 = 0x90,

        /// <summary>
        /// Loads the element with type unsigned int8 at a specified array index onto the top of the evaluation stack as an int32.
        /// </summary>
        Ldelem_U1 = 0x91,

        /// <summary>
        /// Loads the element with type int16 at a specified array index onto the top of the evaluation stack as an int32.
        /// </summary>
        Ldelem_I2 = 0x92,

        /// <summary>
        /// Loads the element with type unsigned int16 at a specified array index onto the top of the evaluation stack as an int32.
        /// </summary>
        Ldelem_U2 = 0x93,

        /// <summary>
        /// Loads the element with type int32 at a specified array index onto the top of the evaluation stack as an int32.
        /// </summary>
        Ldelem_I4 = 0x94,

        /// <summary>
        /// Loads the element with type unsigned int32 at a specified array index onto the top of the evaluation stack as an int32.
        /// </summary>
        Ldelem_U4 = 0x95,

        /// <summary>
        /// Loads the element with type int64 at a specified array index onto the top of the evaluation stack as an int64.
        /// </summary>
        Ldelem_I8 = 0x96,

        /// <summary>
        /// Loads the element with type native int at a specified array index onto the top of the evaluation stack as a native int.
        /// </summary>
        Ldelem_I = 0x97,

        /// <summary>
        /// Loads the element with type float32 at a specified array index onto the top of the evaluation stack as type F (float).
        /// </summary>
        Ldelem_R4 = 0x98,

        /// <summary>
        /// Loads the element with type float64 at a specified array index onto the top of the evaluation stack as type F (float).
        /// </summary>
        Ldelem_R8 = 0x99,

        /// <summary>
        /// Loads the element containing an object reference at a specified array index onto the top of the evaluation stack as type O (object reference).
        /// </summary>
        Ldelem_Ref = 0x9a,

        /// <summary>
        /// Replaces the array element at a given index with the native int value on the evaluation stack.
        /// </summary>
        Stelem_I = 0x9b,

        /// <summary>
        /// Replaces the array element at a given index with the int8 value on the evaluation stack.
        /// </summary>
        Stelem_I1 = 0x9c,

        /// <summary>
        /// Replaces the array element at a given index with the int16 value on the evaluation stack.
        /// </summary>
        Stelem_I2 = 0x9d,

        /// <summary>
        /// Replaces the array element at a given index with the int32 value on the evaluation stack.
        /// </summary>
        Stelem_I4 = 0x9e,

        /// <summary>
        /// Replaces the array element at a given index with the int64 value on the evaluation stack.
        /// </summary>
        Stelem_I8 = 0x9f,

        /// <summary>
        /// Replaces the array element at a given index with the float32 value on the evaluation stack.
        /// </summary>
        Stelem_R4 = 0xa0,

        /// <summary>
        /// Replaces the array element at a given index with the float64 value on the evaluation stack.
        /// </summary>
        Stelem_R8 = 0xa1,

        /// <summary>
        /// Replaces the array element at a given index with the object ref value (type O) on the evaluation stack.
        /// </summary>
        Stelem_Ref = 0xa2,

        /// <summary>
        /// Loads the element at a specified array index onto the top of the evaluation stack as the type specified in the instruction.
        /// </summary>
        Ldelem = 0xa3,

        /// <summary>
        /// Replaces the array element at a given index with the value on the evaluation stack, whose type is specified in the instruction.
        /// </summary>
        Stelem = 0xa4,

        /// <summary>
        /// Converts the boxed representation of a type specified in the instruction to its unboxed form.
        /// </summary>
        Unbox_Any = 0xa5,

        /// <summary>
        /// Converts the signed value on top of the evaluation stack to signed int8 and extends it to int32, throwing System.OverflowException on overflow.
        /// </summary>
        Conv_Ovf_I1 = 0xb3,

        /// <summary>
        /// Converts the signed value on top of the evaluation stack to unsigned int8 and extends it to int32, throwing System.OverflowException on overflow.
        /// </summary>
        Conv_Ovf_U1 = 0xb4,

        /// <summary>
        /// Converts the signed value on top of the evaluation stack to signed int16 and extending it to int32, throwing System.OverflowException on overflow.
        /// </summary>
        Conv_Ovf_I2 = 0xb5,

        /// <summary>
        /// Converts the signed value on top of the evaluation stack to unsigned int16 and extends it to int32, throwing System.OverflowException on overflow.
        /// </summary>
        Conv_Ovf_U2 = 0xb6,

        /// <summary>
        /// Converts the signed value on top of the evaluation stack to signed int32, throwing System.OverflowException on overflow.
        /// </summary>
        Conv_Ovf_I4 = 0xb7,

        /// <summary>
        /// Converts the signed value on top of the evaluation stack to unsigned int32, throwing System.OverflowException on overflow.
        /// </summary>
        Conv_Ovf_U4 = 0xb8,

        /// <summary>
        /// Converts the signed value on top of the evaluation stack to signed int64, throwing System.OverflowException on overflow.
        /// </summary>
        Conv_Ovf_I8 = 0xb9,

        /// <summary>
        /// Converts the signed value on top of the evaluation stack to unsigned int64, throwing System.OverflowException on overflow.
        /// </summary>
        Conv_Ovf_U8 = 0xba,

        /// <summary>
        /// Retrieves the address (type &) embedded in a typed reference.
        /// </summary>
        Refanyval = 0xc2,

        /// <summary>
        /// Throws System.ArithmeticException if value is not a finite number.
        /// </summary>
        Ckfinite = 0xc3,

        /// <summary>
        /// Pushes a typed reference to an instance of a specific type onto the evaluation stack.
        /// </summary>
        Mkrefany = 0xc6,

        /// <summary>
        /// Converts a metadata token to its runtime representation, pushing it onto the evaluation stack.
        /// </summary>
        Ldtoken = 0xd0,

        /// <summary>
        /// Converts the value on top of the evaluation stack to unsigned int16, and extends it to int32.
        /// </summary>
        Conv_U2 = 0xd1,

        /// <summary>
        /// Converts the value on top of the evaluation stack to unsigned int8, and extends it to int32.
        /// </summary>
        Conv_U1 = 0xd2,

        /// <summary>
        /// Converts the value on top of the evaluation stack to native int.
        /// </summary>
        Conv_I = 0xd3,

        /// <summary>
        /// Converts the signed value on top of the evaluation stack to signed native int, throwing System.OverflowException on overflow.
        /// </summary>
        Conv_Ovf_I = 0xd4,

        /// <summary>
        /// Converts the signed value on top of the evaluation stack to unsigned native int, throwing System.OverflowException on overflow.
        /// </summary>
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

        /// <summary>
        /// Multiplies two integer values, performs an overflow check, and pushes the result onto the evaluation stack.
        /// </summary>
        Mul_Ovf = 0xd8,

        /// <summary>
        /// Multiplies two unsigned integer values, performs an overflow check, and pushes the result onto the evaluation stack.
        /// </summary>
        Mul_Ovf_Un = 0xd9,

        /// <summary>
        /// Subtracts one integer value from another, performs an overflow check, and pushes the result onto the evaluation stack.
        /// </summary>
        Sub_Ovf = 0xda,

        /// <summary>
        /// Subtracts one unsigned integer value from another, performs an overflow check, and pushes the result onto the evaluation stack.
        /// </summary>
        Sub_Ovf_Un = 0xdb,

        /// <summary>
        /// Transfers control from the fault or finally clause of an exception block back to the Common Language Infrastructure (CLI) exception handler.
        /// </summary>
        Endfinally = 0xdc,

        /// <summary>
        /// Exits a protected region of code, unconditionally transferring control to a specific target instruction.
        /// </summary>
        Leave = 0xdd,

        /// <summary>
        /// Exits a protected region of code, unconditionally transferring control to a target instruction (short form).
        /// </summary>
        Leave_S = 0xde,

        /// <summary>
        /// Stores a value of type native int at a supplied address.
        /// </summary>
        Stind_I = 0xdf,

        /// <summary>
        /// Converts the value on top of the evaluation stack to unsigned native int, and extends it to native int.
        /// </summary>
        Conv_U = 0xe0,

        /// <summary>
        /// This is a reserved instruction.
        /// </summary>
        Prefix7 = 0xf8,

        /// <summary>
        /// This is a reserved instruction.
        /// </summary>
        Prefix6 = 0xf9,

        /// <summary>
        /// This is a reserved instruction.
        /// </summary>
        Prefix5 = 0xfa,

        /// <summary>
        /// This is a reserved instruction.
        ///</summary>
        Prefix4 = 0xfb,

        /// <summary>
        /// This is a reserved instruction.
        /// </summary>
        Prefix3 = 0xfc,

        /// <summary>
        /// This is a reserved instruction.
        /// </summary>
        Prefix2 = 0xfd,

        /// <summary>
        /// Used to specify the OpCode is 2 byte opcode, specified by the next byte in the instruction stream which the lower hex value of the 0xfe{XX} instructions.
        /// </summary>
        Prefix1 = 0xfe,

        /// <summary>
        /// This is a reserved instruction.
        /// </summary>
        Prefixref = 0xff,

        /// <summary>
        /// Unsupported in Reflection/Dynamic code. Used in statically compiled code get a native <see cref="System.TypedReference"/> to an <see cref="System.ArgIterator"/>.
        /// Returns an unmanaged pointer to the argument list of the current method.
        /// </summary>
        Arglist = 0xfe00,

        /// <summary>
        /// Compares two values. If they are equal, the integer value 1 (int32) is pushed onto the evaluation stack; otherwise 0 (int32) is pushed onto the evaluation stack.
        /// </summary>
        Ceq = 0xfe01,

        /// <summary>
        /// Compares two values. If the first value is greater than the second, the integer value 1 (int32) is pushed onto the evaluation stack; otherwise 0 (int32) is pushed onto the evaluation stack.
        /// </summary>
        Cgt = 0xfe02,

        /// <summary>
        /// Compares two unsigned or unordered values. If the first value is greater than the second, the integer value 1 (int32) is pushed onto the evaluation stack; otherwise 0 (int32) is pushed onto the evaluation stack.
        /// </summary>
        Cgt_Un = 0xfe03,

        /// <summary>
        /// Compares two values. If the first value is less than the second, the integer value 1 (int32) is pushed onto the evaluation stack; otherwise 0 (int32) is pushed onto the evaluation stack.
        /// </summary>
        Clt = 0xfe04,

        /// <summary>
        /// Compares the unsigned or unordered values value1 and value2. If value1 is less than value2, then the integer value 1 (int32) is pushed onto the evaluation stack; otherwise 0 (int32) is pushed onto the evaluation stack.
        /// </summary>
        Clt_Un = 0xfe05,

        /// <summary>
        /// Pushes an unmanaged pointer (type native int) to the native code implementing a specific method onto the evaluation stack.
        /// </summary>
        Ldftn = 0xfe06,

        /// <summary>
        /// Pushes an unmanaged pointer (type native int) to the native code implementing a particular virtual method associated with a specified object onto the evaluation stack.
        /// </summary>
        Ldvirtftn = 0xfe07,

        /// <summary>
        /// Loads an argument (referenced by a specified index value) onto the stack.
        /// </summary>
        Ldarg = 0xfe09,

        /// <summary>
        /// Load an argument address onto the evaluation stack.
        /// </summary>
        Ldarga = 0xfe0a,

        /// <summary>
        /// Stores the value on top of the evaluation stack in the argument slot at a specified index.
        /// </summary>
        Starg = 0xfe0b,

        /// <summary>
        /// Loads the local variable at a specific index onto the evaluation stack.
        /// </summary>
        Ldloc = 0xfe0c,

        /// <summary>
        /// Loads the address of the local variable at a specific index onto the evaluation stack.
        /// </summary>
        Ldloca = 0xfe0d,

        /// <summary>
        /// Pops the current value from the top of the evaluation stack and stores it in a the local variable list at a specified index.
        /// </summary>
        Stloc = 0xfe0e,

        /// <summary>
        /// Allocates a certain number of bytes from the local dynamic memory pool and pushes the address (a transient pointer, type *) of the first allocated byte onto the evaluation stack.
        /// </summary>
        Localloc = 0xfe0f,

        /// <summary>
        /// Transfers control from the filter clause of an exception back to the Common Language Infrastructure (CLI) exception handler.
        /// </summary>
        Endfilter = 0xfe11,

        /// <summary>
        /// Indicates that an address currently atop the evaluation stack might not be aligned to the natural size of the immediately following ldind, stind, ldfld, stfld, ldobj, stobj, initblk, or cpblk instruction.
        /// </summary>
        Unaligned = 0xfe12,

        /// <summary>
        /// Specifies that an address currently atop the evaluation stack might be volatile, and the results of reading that location cannot be cached or that multiple stores to that location cannot be suppressed.
        /// </summary>
        Volatile = 0xfe13,

        /// <summary>
        /// Performs a postfixed method call instruction such that the current method's stack frame is removed before the actual call instruction is executed.
        /// </summary>
        Tailcall = 0xfe14,

        /// <summary>
        /// Initializes each field of the value type at a specified address to a null reference or a 0 of the appropriate primitive type.
        /// </summary>
        Initobj = 0xfe15,

        /// <summary>
        /// Constrains the type on which a virtual method call is made.
        /// </summary>
        Constrained = 0xfe16,

        /// <summary>
        /// Copies a specified number bytes from a source address to a destination address.
        /// </summary>
        Cpblk = 0xfe17,

        /// <summary>
        /// Initializes a specified block of memory at a specific address to a given size and initial value.
        /// </summary>
        Initblk = 0xfe18,

        /// <summary>
        /// Rethrows the current exception.
        /// </summary>
        Rethrow = 0xfe1a,

        /// <summary>
        /// Pushes the size, in bytes, of a supplied value type onto the evaluation stack.
        /// </summary>
        Sizeof = 0xfe1c,

        /// <summary>
        /// Retrieves the type token embedded in a typed reference.
        /// </summary>
        Refanytype = 0xfe1d,

        /// <summary>
        /// Specifies that the subsequent array address operation performs no type check at run time, and that it returns a managed pointer whose mutability is restricted.
        /// </summary>
        Readonly = 0xfe1e,

        /// <summary>
        /// Custom instruction to execute dynamic MSIL
        /// </summary>
        Exec_MSIL_I = 0x24,

        /// <summary>
        /// Custom instruction to execute dynamic MSIL
        /// </summary>
        Exec_MSIL_S = 0x77

    }
}
