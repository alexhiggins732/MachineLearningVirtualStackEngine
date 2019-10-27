using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace ILEngine
{
    /// <summary>
    /// Helper class to lookup <see cref="System.Reflection.Emit.OpCode"/> and dynamically injected <see cref="ILDynamicOpcode"/> by name and value.
    /// </summary>
    public class OpCodeLookup
    {
        public static readonly Dictionary<int, OpCode> OpCodes;
        public static readonly Dictionary<string, OpCode> OpCodesByName;
        static OpCodeLookup()
        {
            var opCodeFields = typeof(OpCodes).GetFields(BindingFlags.Public | BindingFlags.Static);


            OpCodes = opCodeFields.ToDictionary(x => (int)((OpCode)x.GetValue(null)).Value, x => (OpCode)x.GetValue(null));
            OpCodes.Add(ILDynamicOpcode.ExecMsilInstanceMethod.Value, ILDynamicOpcode.ExecMsilInstanceMethod);
            OpCodes.Add(ILDynamicOpcode.ExecMsilStaticMethod.Value, ILDynamicOpcode.ExecMsilStaticMethod);

            OpCodesByName = opCodeFields.ToDictionary(x => x.Name, x => (OpCode)x.GetValue(null));
            OpCodesByName.Add(ILDynamicOpcode.ExecMsilInstanceMethod.Name, ILDynamicOpcode.ExecMsilInstanceMethod);
            OpCodesByName.Add(ILDynamicOpcode.ExecMsilStaticMethod.Name, ILDynamicOpcode.ExecMsilStaticMethod);

        }
        public static int GetOpCodeArgByteSize(ILOpCodeValues value)
        {
            switch (value)
            {
                case ILOpCodeValues.Add: return 0; // Adds two values and  pushes result onto the stack
                case ILOpCodeValues.Add_Ovf: return 0; // Adds two integer values: return 0; performs overflow and pushes result onto the stack
                case ILOpCodeValues.Add_Ovf_Un: return 0; // Adds two unsigned integer values: return 0; performs overflow and pushes result onto the stack
                case ILOpCodeValues.And: return 0; //Computes the bitwise and and pushes the result onto the stack   

                //TODO: Implement later
                case ILOpCodeValues.Arglist: return 0; // Return unmanaged pointer to argument list for current method

                case ILOpCodeValues.Beq: return 4;// break to target if two values are equal
                case ILOpCodeValues.Beq_S: return 1; // break to target if two values are equal (short form)
                case ILOpCodeValues.Bge: return 4; // break to target if first value is greater than or equal to the second value
                case ILOpCodeValues.Bge_S: return 1;  // break to target if first value is greater than or equal to the second value (short form)
                case ILOpCodeValues.Bge_Un: return 4;  // break to target if first value (unsigned int or float) is greater or equal to the second value 
                case ILOpCodeValues.Bge_Un_S: return 1;  // break to target if first value (unsigned int or float) is greater or equal to the second value (short form)
                case ILOpCodeValues.Bgt: return 4; //break to the target if the first value is greater than the second
                case ILOpCodeValues.Bgt_S: return 1; //break to the target if the first value is greater than the second (short)
                case ILOpCodeValues.Bgt_Un: return 4; // break to target if first value (unsigned int or float) is greater or equal to the second value 
                case ILOpCodeValues.Bgt_Un_S: return 4;  // break to target if first value (unsigned int or float) is greater or equal to the second value (short)
                case ILOpCodeValues.Ble: return 4;  // break to target if the first value is less than or equal to the second.
                case ILOpCodeValues.Ble_S: return 1;  // break to target if the first value is less than or equal to the second (Short).
                case ILOpCodeValues.Ble_Un: return 4;  // break to target if the first value (unsigned int or float) is less than or equal to the second.
                case ILOpCodeValues.Ble_Un_S: return 1;  // break to target if the first value (unsigned int or float)is less than or equal to the second (Short).
                case ILOpCodeValues.Blt: return 4;  // break to target if the first value is less than the second.
                case ILOpCodeValues.Blt_S: return 1;  // break to target if the first value is less than the second (Short).
                case ILOpCodeValues.Blt_Un: return 4;  // break to target if the first value (unsigned int or float) is less than the second.
                case ILOpCodeValues.Blt_Un_S: return 1;  // break to target if the first value (unsigned int or float)is less than the second (Short).
                case ILOpCodeValues.Bne_Un: return 4; // Break to targer if the two values are not equal (unsigned or float).
                case ILOpCodeValues.Bne_Un_S: return 1; // Break to targer if the two values are not equal (unsigned or float).
                case ILOpCodeValues.Box: return 0; // converts a value type of an object reference
                case ILOpCodeValues.Br: return 4; // branches to target
                case ILOpCodeValues.Break: return 0; // tells debugger breakpoing has been hit
                case ILOpCodeValues.Brfalse: return 0; // break to target if value is false: return 0; null or zero
                case ILOpCodeValues.Brfalse_S: return 0; // break to target if value is false: return 0; null or zero (short)
                case ILOpCodeValues.Brtrue: return 0; // break to target if value is true: return 0; not null or non-zero
                case ILOpCodeValues.Brtrue_S: return 0; // break to target if value is true: return 0; not null or non-zero (short)
                case ILOpCodeValues.Br_S: return 1; // branches to target (short)
                case ILOpCodeValues.Call: return 4; // calls target specified by method descriptor (methoddesc or token)

                // TODO: Implement later
                case ILOpCodeValues.Calli: return 4; // calls method specified by pointer on the stack using specified calling convention

                // TODO: Implement later
                case ILOpCodeValues.Callvirt: return 4; //calls late-bound method on an object

                // // TODO: Implement later
                case ILOpCodeValues.Castclass: return 4; // attempts to cast object to the specified class

                case ILOpCodeValues.Ceq: return 0; // push one on the stack if two values are equal otherwise zero
                case ILOpCodeValues.Cgt: return 0; // push one on the stack if first value is greater than the second otherwise zero
                case ILOpCodeValues.Cgt_Un: return 0; // push one on the stack if first value (unsigned) is greater than the second otherwise zero
                case ILOpCodeValues.Ckfinite: return 0; // Throws arithmetic exception is value is not finite
                case ILOpCodeValues.Clt: return 0; // push on on the stack if first value is less than the second otherwise zero
                case ILOpCodeValues.Clt_Un: return 0; // push on on the stack if first value (unsigned) is less than the second otherwise zero

                // TODO: waiting blocked by Callvirt
                case ILOpCodeValues.Constrained: return 0; // constrains a type on which a virtual call is made

                case ILOpCodeValues.Conv_I: return 0; // convert value on the stack to native int
                case ILOpCodeValues.Conv_I1: return 0; // convert value on the stack to int8 then extends (pads)  it to int32
                case ILOpCodeValues.Conv_I2: return 0; // convert value on the stack to int16 return 0; then extends (pads)  it to int32
                case ILOpCodeValues.Conv_I4: return 0; // convert value on the stack to int32
                case ILOpCodeValues.Conv_I8: return 0; // convert value on the stack to int64

                case ILOpCodeValues.Conv_Ovf_I: return 0;// converts value on the stack to native int checking for overlow
                case ILOpCodeValues.Conv_Ovf_I1: return 0;// convert value on the stack to int8 and then int32 checking for overlow
                case ILOpCodeValues.Conv_Ovf_I1_Un: return 0;// convert value on the stack to uint8 and then int33 checking for overlow

                case ILOpCodeValues.Conv_Ovf_I2: return 0;// converts value on the stack to native int16 checking for overlow
                case ILOpCodeValues.Conv_Ovf_I2_Un: return 0;//convert value on the stack to uint16 and then int32 checking for overlow

                case ILOpCodeValues.Conv_Ovf_I4: return 0;//convert value on the stack to int32 checking for overlow
                case ILOpCodeValues.Conv_Ovf_I4_Un: return 0;// convert value on the stack to uint32checking for overlow

                case ILOpCodeValues.Conv_Ovf_I8: return 0;// convert value on the stack to  int32 checking for overlow
                case ILOpCodeValues.Conv_Ovf_I8_Un: return 0;// convert value on the stack  to uint32 checking for overlow

                case ILOpCodeValues.Conv_Ovf_I_Un: return 0; // unsigned to  int32checking for overlow
                case ILOpCodeValues.Conv_Ovf_U: return 0; // to unsigned32 checking for overlow
                case ILOpCodeValues.Conv_Ovf_U1: return 0; // signed to uint8//extended to int32 with overflow
                case ILOpCodeValues.Conv_Ovf_U1_Un: return 0; // unsigned to uint8 extended to int32 with overlow
                case ILOpCodeValues.Conv_Ovf_U2: return 0; // signed to uint16 extended to int32 with overflow
                case ILOpCodeValues.Conv_Ovf_U2_Un: return 0; // unsigned to uint16 extended to int32 with overlow
                case ILOpCodeValues.Conv_Ovf_U4: return 0; //  signed to uint32 with overflow
                case ILOpCodeValues.Conv_Ovf_U4_Un: return 0; // unsigned to uint32 with overlow
                case ILOpCodeValues.Conv_Ovf_U8: return 0; // signed to uint64 with overflow
                case ILOpCodeValues.Conv_Ovf_U8_Un: return 0; // unsigned to uint64 with overlow??

                case ILOpCodeValues.Conv_Ovf_U_Un: return 0; // unsigned to unsigned native int

                case ILOpCodeValues.Conv_R4: return 0; // converts value on the stack to float32
                case ILOpCodeValues.Conv_R8: return 0; // to float64
                case ILOpCodeValues.Conv_R_Un: return 0;// unsigned to float

                case ILOpCodeValues.Conv_U: return 0; //  to native unsigned int
                case ILOpCodeValues.Conv_U1: return 0; // to uint8 padded to int32
                case ILOpCodeValues.Conv_U2: return 0; // to uint16 padded to int32
                case ILOpCodeValues.Conv_U4: return 0; // to uint32 padded to int32
                case ILOpCodeValues.Conv_U8: return 0; // to uint32 padded to int64

                //TODO: Implement later
                case ILOpCodeValues.Cpblk: return 4;// copies number of bytes from source address to destination address

                //TODO: Implement later
                case ILOpCodeValues.Cpobj: return 4;// copies value type at address ( & * or native int) to destination address ( & * or native int)

                case ILOpCodeValues.Div: return 0; // divides and pushes result(float) or quotient(int) onto stack
                case ILOpCodeValues.Div_Un: return 0; // divides two values and pushes result(int) into the stack
                case ILOpCodeValues.Dup: return 0; // pushes copy of top of stack onto the stack

                // TODO: Implement later
                case ILOpCodeValues.Endfilter: return 0; // transfers control from filter to cli excpetion

                //TODO: Imlement later
                case ILOpCodeValues.Endfinally: return 0; // 
                case ILOpCodeValues.Exec_MSIL_I: return 4;
                case ILOpCodeValues.Exec_MSIL_S: return 4;
                //case ILOpcodeValues.Exec_MSIL_I: return 4;
                //TODO: Implement later
                case ILOpCodeValues.Initblk: return 0; // initializes block of memory at at address to given size and value

                // TODO: implement later
                case ILOpCodeValues.Initobj: return 0; // initializes each field of a value type to null or default

                // TODO: implement later
                case ILOpCodeValues.Isinst: return 4; // determin if object reference is particiluar class

                //
                case ILOpCodeValues.Jmp: return 4; // exits current method and jumps to specified method

                case ILOpCodeValues.Ldarg: return 4; // loads an argument specified by index onto the stack
                case ILOpCodeValues.Ldarga: return 4; // loads argument address onto the stack
                case ILOpCodeValues.Ldarga_S: return 1;  // loads an address  onto the stack (short)
                case ILOpCodeValues.Ldarg_0: return 4; // load argument 0 onto the stack (this for instance methods first argument for static)
                case ILOpCodeValues.Ldarg_1: return 4;// load argument 1 onto the stack (second argument for instance methods first argument for static)
                case ILOpCodeValues.Ldarg_2: return 4;// load argument 2 onto the stack
                case ILOpCodeValues.Ldarg_3: return 4;// load argument 3 onto the stack
                case ILOpCodeValues.Ldarg_S: return 4; // loads an argument specified by index onto the stack (short)

                case ILOpCodeValues.Ldc_I4: return 0; // pushes the specified int32 value onto the stack
                case ILOpCodeValues.Ldc_I4_0: return 0; // pushes 0 onto the stack
                case ILOpCodeValues.Ldc_I4_1: return 0; // pushes 1 onto the stack
                case ILOpCodeValues.Ldc_I4_2: return 0; // pushes 2 onto the stack
                case ILOpCodeValues.Ldc_I4_3: return 0; // pushes 3 onto the stack
                case ILOpCodeValues.Ldc_I4_4: return 0; // pushes 4 onto the stack
                case ILOpCodeValues.Ldc_I4_5: return 0; // pushes 5 onto the stack
                case ILOpCodeValues.Ldc_I4_6: return 0; // pushes 6 onto the stack
                case ILOpCodeValues.Ldc_I4_7: return 0; // pushes 7 onto the stack
                case ILOpCodeValues.Ldc_I4_8: return 0; // pushes 8 onto the stack

                case ILOpCodeValues.Ldc_I4_M1: return 0; // pushes -1 onto the stack
                case ILOpCodeValues.Ldc_I4_S: return 0; // pushes specified int8 onto the stack as int32
                case ILOpCodeValues.Ldc_I8: return 0; // pushes specified int64 onto the stack
                case ILOpCodeValues.Ldc_R4: return 0; // pushes specified f32 onto the stack
                case ILOpCodeValues.Ldc_R8: return 0; // pushes specified f64 onto the stack

                //TODO: test if we need type references here
                case ILOpCodeValues.Ldelem: return 4; // loads element at index onto the stack as type speficied in instruction
                case ILOpCodeValues.Ldelema: return 4; // loads element address at index onto the stack as & (managed pointer)  

                //TODO: May need to hold off
                case ILOpCodeValues.Ldelem_I: return 4; // loads element with type native int at specified index onto the stack as native int
                case ILOpCodeValues.Ldelem_I1: return 4; // loads int8 element at index onto the stack as int32
                case ILOpCodeValues.Ldelem_I2: return 4; // loads int16 element at index onto the stack as int32
                case ILOpCodeValues.Ldelem_I4: return 4; // loads int32 at index onto the stack
                case ILOpCodeValues.Ldelem_I8: return 4; // loads int64 at index onto the stack

                case ILOpCodeValues.Ldelem_R4: return 4; //loads f32 at index onto the stack
                case ILOpCodeValues.Ldelem_R8: return 4; // loads f64 at index onto the stack

                case ILOpCodeValues.Ldelem_Ref: return 4; // loads element containing object reference onto stack as object reference
                case ILOpCodeValues.Ldelem_U1: return 4; // load uint8 onto the stack extended to int32
                case ILOpCodeValues.Ldelem_U2: return 4; // load uint8 onto the stack extended to int32
                case ILOpCodeValues.Ldelem_U4: return 4; // load uint8 onto the stack extended to int32

                case ILOpCodeValues.Ldfld: return 4;// load field of object reference on the stack
                case ILOpCodeValues.Ldflda: return 4;// load address of field of object reference on the stack

                //TODO: Implement later
                case ILOpCodeValues.Ldftn: return 4;// loads native int (unmanaged pointer) to native code for a method onto the stack
                case ILOpCodeValues.Ldind_I: return 4; // indirectlyloads value type of native into onto the stack as native int 
                case ILOpCodeValues.Ldind_I1: return 4; // indirectlyloads value type of i8 onto the stack as native int
                case ILOpCodeValues.Ldind_I2: return 4; // indirectlyloads value type of i16 onto the stack as native int
                case ILOpCodeValues.Ldind_I4: return 4; /// indirectly loads value type of int32 onto the stack as native int
                case ILOpCodeValues.Ldind_I8: return 4; /// indirectly loads value type of int34 onto the stack as int64

                case ILOpCodeValues.Ldind_R4: return 4; // loads f32 onto the stack as native float
                case ILOpCodeValues.Ldind_R8: return 4; // loads f64 onto the stack as native float

                case ILOpCodeValues.Ldind_Ref: return 4; // indirectly loads object reference on stack as type O  (object reference)
                case ILOpCodeValues.Ldind_U1: return 4; // indirectly loads uint8 onto the stack as int32
                case ILOpCodeValues.Ldind_U2: return 4; // indirectly loads uint16 onto the stack as int32
                case ILOpCodeValues.Ldind_U4: return 4; // indirectly loads uint8 onto the stack as int32
                case ILOpCodeValues.Ldlen: return 4; // pushes the number of zero based 1-d array elements onto the stack

                case ILOpCodeValues.Ldloc: return 4;// loads local variable at specified index onto the stack
                case ILOpCodeValues.Ldloca: return 4; // loads address of local variable at specified index onto the stack
                case ILOpCodeValues.Ldloca_S: return 4; // loads local variable at specified index onto the stack (short)
                case ILOpCodeValues.Ldloc_0: return 4; // loads local variable at index 0 onto the stack (short)
                case ILOpCodeValues.Ldloc_1: return 4; // loads local variable at index 1 onto the stack (short)
                case ILOpCodeValues.Ldloc_2: return 4; // loads local variable at index 2 onto the stack (short)
                case ILOpCodeValues.Ldloc_3: return 4; // loads local variable at index 3 onto the stack (short)
                case ILOpCodeValues.Ldloc_S: return 4; // loads local variable at specified index onto the stack (short)

                case ILOpCodeValues.Ldnull: return 0; // load null value onto the stack
                case ILOpCodeValues.Ldobj: return 4; // copies value type pointed to by with object reference onto the stack

                case ILOpCodeValues.Ldsfld: return 4; // pushes value of static field onto the stack
                case ILOpCodeValues.Ldsflda: return 4; // pushes address of static field onto stack
                case ILOpCodeValues.Ldstr: return 4; // pushes object reference to string stored in meta
                case ILOpCodeValues.Ldtoken: return 4; // converts metadatatoken to runtime type handle and pushes it on the stack

                //TODO: Implement later
                case ILOpCodeValues.Ldvirtftn: return 4; // pushes unmanaged prt(native int) to native code of virtual method onto the stack

                //TODO: Implement later
                case ILOpCodeValues.Leave: return 4; // breaks from protected region of code to specified target
                //TODO: Implement later
                case ILOpCodeValues.Leave_S: return 1; // breaks from protected region of code to specified target (short)

                // TODO: Implement later
                case ILOpCodeValues.Localloc: return 4; // allocates number of bytes from memory pool and pushes transient ptr* to first byte address on the stack

                // TODO: Implement later
                case ILOpCodeValues.Mkrefany: return 4;// pushes typed refrence to specified instance onto the stack

                case ILOpCodeValues.Mul: return 0; // multiplies two values
                case ILOpCodeValues.Mul_Ovf: return 0;// multiplies two values with overflow
                case ILOpCodeValues.Mul_Ovf_Un: return 0; // mutltiplies two usigned values with overflow

                case ILOpCodeValues.Neg: return 0; // negates value and pushes result onto stack

                case ILOpCodeValues.Newarr: return 0; // pushes object reference to zero index 1d array onto the stack
                case ILOpCodeValues.Newobj: return 4; // creates new object or new instance of value type pushes object reference on stack

                case ILOpCodeValues.Nop: return 0; // Fills space if opcodes are patched 
                case ILOpCodeValues.Not: return 0;// bitwise complement of int on the stack
                case ILOpCodeValues.Or: return 0; // bitwise complement of two integers on the stack
                case ILOpCodeValues.Pop: return 0; // removes a value from the stack

                case ILOpCodeValues.Prefix1: return 0; // reserved
                case ILOpCodeValues.Prefix2: return 0; // reserved
                case ILOpCodeValues.Prefix3: return 0; // reserved
                case ILOpCodeValues.Prefix4: return 0; // reserved
                case ILOpCodeValues.Prefix5: return 0; // reserved
                case ILOpCodeValues.Prefix6: return 0; // reserved
                case ILOpCodeValues.Prefix7: return 0; // reserved
                case ILOpCodeValues.Prefixref: return 0; // reserved

                // TODO: Implement later
                case ILOpCodeValues.Readonly: return 4; // specifies susequent array address performs no type check returns pointer with restricted mutability
                //Todo: Implement later
                case ILOpCodeValues.Refanytype: return 4; // retrieves type token embedded in type reference
                // TODO: Implement later
                case ILOpCodeValues.Refanyval: return 4; //retrieves the address (&) embeeded int a type reference

                case ILOpCodeValues.Rem: return 0;// divides two values an pushes remainder on the stack
                case ILOpCodeValues.Rem_Un: return 0;// divides two unsigned values and pushes remainder on the stack
                case ILOpCodeValues.Ret: return 0;// returns from current method and pushes return value from evaluation stack(if present) to top of caller's stack
                case ILOpCodeValues.Rethrow: return 0; // rethrows current exception

                case ILOpCodeValues.Shl: return 0; // shifts int on stack left by number of bits
                case ILOpCodeValues.Shr: return 0; // shifts int on stack right by number of bits
                case ILOpCodeValues.Shr_Un: return 0;// shift unsigned into right by number of bits

                case ILOpCodeValues.Sizeof: return 4; // pushes size in bytes of current value type on stack 
                case ILOpCodeValues.Starg: return 4; // stores value on stack to argument slot at index
                case ILOpCodeValues.Starg_S: return 4;  // stores value on stack to argument slot at index (short)

                case ILOpCodeValues.Stelem: return 4; // replace element array at index with value on stack type specied in instruction
                case ILOpCodeValues.Stelem_I: return 4; // replace element array at index with value native int on stack
                case ILOpCodeValues.Stelem_I1: return 4; // replace element array at index with int8 on stack
                case ILOpCodeValues.Stelem_I2: return 4; // replace element array at index with int16 on stack
                case ILOpCodeValues.Stelem_I4: return 4; // replace element array at index with int32 on stack
                case ILOpCodeValues.Stelem_I8: return 4; // replace element array at index with int32 on stack
                case ILOpCodeValues.Stelem_R4: return 4; // replace element array at index with f32 on stack
                case ILOpCodeValues.Stelem_R8: return 4; // replace element array at index with f64 on stack

                case ILOpCodeValues.Stelem_Ref: return 4;// replace element array at index with object ref value on the stack
                case ILOpCodeValues.Stfld: return 4; // replace value stored in field

                case ILOpCodeValues.Stind_I: return 4; // store native int at supplied address
                case ILOpCodeValues.Stind_I1: return 4; // store int8 at supplied address
                case ILOpCodeValues.Stind_I2: return 4; // store int16 at supplied address
                case ILOpCodeValues.Stind_I4: return 4; // store int32 at supplied address
                case ILOpCodeValues.Stind_I8: return 4; // store int64 at supplied address

                case ILOpCodeValues.Stind_R4: return 4; // store f32 at supplied address
                case ILOpCodeValues.Stind_R8: return 4; //store f64 at supplied address
                case ILOpCodeValues.Stind_Ref: return 4; // store object reference value at supplied address
                case ILOpCodeValues.Stloc: return 4; // pops top of stack and stores it to local variable at specified index
                case ILOpCodeValues.Stloc_0: return 4; // pops top of stack and stores it to local variable at index 0
                case ILOpCodeValues.Stloc_1: return 0; // pops top of stack and stores it to local variable at index 1
                case ILOpCodeValues.Stloc_2: return 4; // pops top of stack and stores it to local variable at index 2
                case ILOpCodeValues.Stloc_3: return 4; // pops top of stack and stores it to local variable at index 3
                case ILOpCodeValues.Stloc_S: return 4; // pops top of stack and stores it to local variable at specified index (short)

                case ILOpCodeValues.Stobj: return 4; // Copies value at top of evaluation stack to specified address
                case ILOpCodeValues.Stsfld: return 4; // replaces value of stack field with value on stack

                case ILOpCodeValues.Sub: return 0; // subtract and push result onto the stack
                case ILOpCodeValues.Sub_Ovf: return 0; // subtract with overflow check and push result onto the stack
                case ILOpCodeValues.Sub_Ovf_Un: return 0; // subtract unsigned with overflow check and push result onto the stack
                case ILOpCodeValues.Switch: return 4; // Implements a jump table
                //TODO: Implement later
                case ILOpCodeValues.Tailcall: return 4;// preforms call after moving current methods stack frame
                case ILOpCodeValues.Throw: return 4; // Throws exception object on the stack

                case ILOpCodeValues.Unaligned: return 0;// specifies value on stack might not be aligned following following ldind stind ldfld stfld ldobj stobj initblk or cpblk instruction
                case ILOpCodeValues.Unbox: return 0; // converts boxed value type to unboxed form
                case ILOpCodeValues.Unbox_Any: return 0; // converts boxed representation of type to unboxed form
                case ILOpCodeValues.Volatile: return 4; // Specifies address cannot be cached and multiple stores can not be supressed
                case ILOpCodeValues.Xor: return 0; // Bitwise XOR
                default: throw new NotImplementedException();
            }

        }
        public static OpCode GetILOpcode(int opcodeValue) => OpCodes[unchecked((short)opcodeValue)];
        public static OpCode GetILOpcodeDebug(int opcodeValue)
        {
            if (OpCodes.TryGetValue(opcodeValue, out OpCode result))
                return result;
            else
                System.Diagnostics.Debug.Assert(false, "$Invalid opcode: {opcodeValue}");
            return OpCodes[0];

        }
    }
}
