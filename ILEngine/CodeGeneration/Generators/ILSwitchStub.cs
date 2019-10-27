using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILEngine
{
    class ILSwitchStub
    {
        public static void IlSwitch(short opcodeValue)
        {
            var iLOpcode = (ILOpCodeValues)opcodeValue;
            switch (iLOpcode)
            {
                case ILOpCodeValues.Nop: //nop: 0
                                         //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Break: //break: 1
                                           //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Ldarg_0: //ldarg.0: 2
                                             //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Ldarg_1: //ldarg.1: 3
                                             //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Ldarg_2: //ldarg.2: 4
                                             //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Ldarg_3: //ldarg.3: 5
                                             //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Ldloc_0: //ldloc.0: 6
                                             //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Ldloc_1: //ldloc.1: 7
                                             //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Ldloc_2: //ldloc.2: 8
                                             //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Ldloc_3: //ldloc.3: 9
                                             //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Stloc_0: //stloc.0: 10
                                             //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Stloc_1: //stloc.1: 11
                                             //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Stloc_2: //stloc.2: 12
                                             //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Stloc_3: //stloc.3: 13
                                             //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Ldarg_S: //ldarg.s: 14
                                             //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Ldarga_S: //ldarga.s: 15
                                              //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Starg_S: //starg.s: 16
                                             //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Ldloc_S: //ldloc.s: 17
                                             //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Ldloca_S: //ldloca.s: 18
                                              //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Stloc_S: //stloc.s: 19
                                             //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Ldnull: //ldnull: 20
                                            //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Ldc_I4_M1: //ldc.i4.m1: 21
                                               //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Ldc_I4_0: //ldc.i4.0: 22
                                              //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Ldc_I4_1: //ldc.i4.1: 23
                                              //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Ldc_I4_2: //ldc.i4.2: 24
                                              //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Ldc_I4_3: //ldc.i4.3: 25
                                              //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Ldc_I4_4: //ldc.i4.4: 26
                                              //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Ldc_I4_5: //ldc.i4.5: 27
                                              //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Ldc_I4_6: //ldc.i4.6: 28
                                              //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Ldc_I4_7: //ldc.i4.7: 29
                                              //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Ldc_I4_8: //ldc.i4.8: 30
                                              //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Ldc_I4_S: //ldc.i4.s: 31
                                              //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Ldc_I4: //ldc.i4: 32
                                            //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Ldc_I8: //ldc.i8: 33
                                            //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Ldc_R4: //ldc.r4: 34
                                            //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Ldc_R8: //ldc.r8: 35
                                            //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Dup: //dup: 37
                                         //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Pop: //pop: 38
                                         //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Jmp: //jmp: 39
                                         //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Call: //call: 40
                                          //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Calli: //calli: 41
                    throw new NotImplementedException("calli is not implemented");
                case ILOpCodeValues.Ret: //ret: 42
                                         //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Br_S: //br.s: 43
                                          //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Brfalse_S: //brfalse.s: 44
                                               //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Brtrue_S: //brtrue.s: 45
                                              //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Beq_S: //beq.s: 46
                                           //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Bge_S: //bge.s: 47
                                           //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Bgt_S: //bgt.s: 48
                                           //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Ble_S: //ble.s: 49
                                           //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Blt_S: //blt.s: 50
                                           //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Bne_Un_S: //bne.un.s: 51
                                              //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Bge_Un_S: //bge.un.s: 52
                                              //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Bgt_Un_S: //bgt.un.s: 53
                                              //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Ble_Un_S: //ble.un.s: 54
                                              //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Blt_Un_S: //blt.un.s: 55
                                              //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Br: //br: 56
                                        //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Brfalse: //brfalse: 57
                                             //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Brtrue: //brtrue: 58
                                            //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Beq: //beq: 59
                                         //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Bge: //bge: 60
                                         //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Bgt: //bgt: 61
                                         //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Ble: //ble: 62
                                         //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Blt: //blt: 63
                                         //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Bne_Un: //bne.un: 64
                                            //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Bge_Un: //bge.un: 65
                                            //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Bgt_Un: //bgt.un: 66
                                            //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Ble_Un: //ble.un: 67
                                            //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Blt_Un: //blt.un: 68
                                            //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Switch: //switch: 69
                                            //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Ldind_I1: //ldind.i1: 70
                                              //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Ldind_U1: //ldind.u1: 71
                                              //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Ldind_I2: //ldind.i2: 72
                                              //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Ldind_U2: //ldind.u2: 73
                                              //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Ldind_I4: //ldind.i4: 74
                                              //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Ldind_U4: //ldind.u4: 75
                                              //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Ldind_I8: //ldind.i8: 76
                                              //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Ldind_I: //ldind.i: 77
                                             //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Ldind_R4: //ldind.r4: 78
                                              //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Ldind_R8: //ldind.r8: 79
                                              //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Ldind_Ref: //ldind.ref: 80
                                               //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Stind_Ref: //stind.ref: 81
                                               //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Stind_I1: //stind.i1: 82
                                              //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Stind_I2: //stind.i2: 83
                                              //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Stind_I4: //stind.i4: 84
                                              //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Stind_I8: //stind.i8: 85
                                              //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Stind_R4: //stind.r4: 86
                                              //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Stind_R8: //stind.r8: 87
                                              //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Add: //add: 88
                                         //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Sub: //sub: 89
                                         //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Mul: //mul: 90
                                         //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Div: //div: 91
                                         //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Div_Un: //div.un: 92
                                            //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Rem: //rem: 93
                                         //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Rem_Un: //rem.un: 94
                                            //TODO: Implement opcode
                    break;
                case ILOpCodeValues.And: //and: 95
                                         //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Or: //or: 96
                                        //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Xor: //xor: 97
                                         //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Shl: //shl: 98
                                         //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Shr: //shr: 99
                                         //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Shr_Un: //shr.un: 100
                                            //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Neg: //neg: 101
                                         //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Not: //not: 102
                                         //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Conv_I1: //conv.i1: 103
                                             //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Conv_I2: //conv.i2: 104
                                             //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Conv_I4: //conv.i4: 105
                                             //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Conv_I8: //conv.i8: 106
                                             //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Conv_R4: //conv.r4: 107
                                             //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Conv_R8: //conv.r8: 108
                                             //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Conv_U4: //conv.u4: 109
                                             //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Conv_U8: //conv.u8: 110
                                             //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Callvirt: //callvirt: 111
                    throw new NotImplementedException("callvirt is not implemented");
                case ILOpCodeValues.Cpobj: //cpobj: 112
                    throw new NotImplementedException("cpobj is not implemented");
                case ILOpCodeValues.Ldobj: //ldobj: 113
                                           //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Ldstr: //ldstr: 114
                                           //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Newobj: //newobj: 115
                                            //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Castclass: //castclass: 116
                    throw new NotImplementedException("castclass is not implemented");
                case ILOpCodeValues.Isinst: //isinst: 117
                    throw new NotImplementedException("isinst is not implemented");
                case ILOpCodeValues.Conv_R_Un: //conv.r.un: 118
                                               //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Unbox: //unbox: 121
                                           //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Throw: //throw: 122
                                           //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Ldfld: //ldfld: 123
                                           //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Ldflda: //ldflda: 124
                                            //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Stfld: //stfld: 125
                                           //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Ldsfld: //ldsfld: 126
                                            //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Ldsflda: //ldsflda: 127
                                             //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Stsfld: //stsfld: 128
                                            //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Stobj: //stobj: 129
                                           //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Conv_Ovf_I1_Un: //conv.ovf.i1.un: 130
                                                    //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Conv_Ovf_I2_Un: //conv.ovf.i2.un: 131
                                                    //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Conv_Ovf_I4_Un: //conv.ovf.i4.un: 132
                                                    //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Conv_Ovf_I8_Un: //conv.ovf.i8.un: 133
                                                    //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Conv_Ovf_U1_Un: //conv.ovf.u1.un: 134
                                                    //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Conv_Ovf_U2_Un: //conv.ovf.u2.un: 135
                                                    //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Conv_Ovf_U4_Un: //conv.ovf.u4.un: 136
                                                    //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Conv_Ovf_U8_Un: //conv.ovf.u8.un: 137
                                                    //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Conv_Ovf_I_Un: //conv.ovf.i.un: 138
                                                   //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Conv_Ovf_U_Un: //conv.ovf.u.un: 139
                                                   //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Box: //box: 140
                                         //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Newarr: //newarr: 141
                                            //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Ldlen: //ldlen: 142
                                           //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Ldelema: //ldelema: 143
                                             //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Ldelem_I1: //ldelem.i1: 144
                                               //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Ldelem_U1: //ldelem.u1: 145
                                               //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Ldelem_I2: //ldelem.i2: 146
                                               //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Ldelem_U2: //ldelem.u2: 147
                                               //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Ldelem_I4: //ldelem.i4: 148
                                               //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Ldelem_U4: //ldelem.u4: 149
                                               //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Ldelem_I8: //ldelem.i8: 150
                                               //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Ldelem_I: //ldelem.i: 151
                                              //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Ldelem_R4: //ldelem.r4: 152
                                               //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Ldelem_R8: //ldelem.r8: 153
                                               //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Ldelem_Ref: //ldelem.ref: 154
                                                //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Stelem_I: //stelem.i: 155
                                              //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Stelem_I1: //stelem.i1: 156
                                               //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Stelem_I2: //stelem.i2: 157
                                               //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Stelem_I4: //stelem.i4: 158
                                               //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Stelem_I8: //stelem.i8: 159
                                               //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Stelem_R4: //stelem.r4: 160
                                               //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Stelem_R8: //stelem.r8: 161
                                               //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Stelem_Ref: //stelem.ref: 162
                                                //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Ldelem: //ldelem: 163
                                            //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Stelem: //stelem: 164
                                            //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Unbox_Any: //unbox.any: 165
                                               //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Conv_Ovf_I1: //conv.ovf.i1: 179
                                                 //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Conv_Ovf_U1: //conv.ovf.u1: 180
                                                 //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Conv_Ovf_I2: //conv.ovf.i2: 181
                                                 //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Conv_Ovf_U2: //conv.ovf.u2: 182
                                                 //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Conv_Ovf_I4: //conv.ovf.i4: 183
                                                 //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Conv_Ovf_U4: //conv.ovf.u4: 184
                                                 //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Conv_Ovf_I8: //conv.ovf.i8: 185
                                                 //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Conv_Ovf_U8: //conv.ovf.u8: 186
                                                 //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Refanyval: //refanyval: 194
                    throw new NotImplementedException("refanyval is not implemented");
                case ILOpCodeValues.Ckfinite: //ckfinite: 195
                                              //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Mkrefany: //mkrefany: 198
                    throw new NotImplementedException("mkrefany is not implemented");
                case ILOpCodeValues.Ldtoken: //ldtoken: 208
                                             //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Conv_U2: //conv.u2: 209
                                             //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Conv_U1: //conv.u1: 210
                                             //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Conv_I: //conv.i: 211
                                            //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Conv_Ovf_I: //conv.ovf.i: 212
                                                //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Conv_Ovf_U: //conv.ovf.u: 213
                                                //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Add_Ovf: //add.ovf: 214
                                             //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Add_Ovf_Un: //add.ovf.un: 215
                                                //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Mul_Ovf: //mul.ovf: 216
                                             //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Mul_Ovf_Un: //mul.ovf.un: 217
                                                //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Sub_Ovf: //sub.ovf: 218
                                             //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Sub_Ovf_Un: //sub.ovf.un: 219
                                                //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Endfinally: //endfinally: 220
                    throw new NotImplementedException("endfinally is not implemented");
                case ILOpCodeValues.Leave: //leave: 221
                    throw new NotImplementedException("leave is not implemented");
                case ILOpCodeValues.Leave_S: //leave.s: 222
                    throw new NotImplementedException("leave.s is not implemented");
                case ILOpCodeValues.Stind_I: //stind.i: 223
                                             //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Conv_U: //conv.u: 224
                                            //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Prefix7: //prefix7: 248
                    throw new NotImplementedException("prefix7 is not implemented");
                case ILOpCodeValues.Prefix6: //prefix6: 249
                    throw new NotImplementedException("prefix6 is not implemented");
                case ILOpCodeValues.Prefix5: //prefix5: 250
                    throw new NotImplementedException("prefix5 is not implemented");
                case ILOpCodeValues.Prefix4: //prefix4: 251
                    throw new NotImplementedException("prefix4 is not implemented");
                case ILOpCodeValues.Prefix3: //prefix3: 252
                    throw new NotImplementedException("prefix3 is not implemented");
                case ILOpCodeValues.Prefix2: //prefix2: 253
                    throw new NotImplementedException("prefix2 is not implemented");
                case ILOpCodeValues.Prefix1: //prefix1: 254
                    throw new NotImplementedException("prefix1 is not implemented");
                case ILOpCodeValues.Prefixref: //prefixref: 255
                    throw new NotImplementedException("prefixref is not implemented");
                case ILOpCodeValues.Arglist: //arglist: -512
                    throw new NotImplementedException("arglist is not implemented");
                case ILOpCodeValues.Ceq: //ceq: -511
                                         //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Cgt: //cgt: -510
                                         //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Cgt_Un: //cgt.un: -509
                                            //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Clt: //clt: -508
                                         //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Clt_Un: //clt.un: -507
                                            //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Ldftn: //ldftn: -506
                    throw new NotImplementedException("ldftn is not implemented");
                case ILOpCodeValues.Ldvirtftn: //ldvirtftn: -505
                    throw new NotImplementedException("ldvirtftn is not implemented");
                case ILOpCodeValues.Ldarg: //ldarg: -503
                                           //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Ldarga: //ldarga: -502
                                            //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Starg: //starg: -501
                                           //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Ldloc: //ldloc: -500
                                           //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Ldloca: //ldloca: -499
                                            //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Stloc: //stloc: -498
                                           //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Localloc: //localloc: -497
                    throw new NotImplementedException("localloc is not implemented");
                case ILOpCodeValues.Endfilter: //endfilter: -495
                    throw new NotImplementedException("endfilter is not implemented");
                case ILOpCodeValues.Unaligned: //unaligned.: -494
                                                //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Volatile: //volatile.: -493
                                               //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Tailcall: //tail.: -492
                                           //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Initobj: //initobj: -491
                    throw new NotImplementedException("initobj is not implemented");
                case ILOpCodeValues.Constrained: //constrained.: -490
                    throw new NotImplementedException("constrained. is not implemented");
                case ILOpCodeValues.Cpblk: //cpblk: -489
                    throw new NotImplementedException("cpblk is not implemented");
                case ILOpCodeValues.Initblk: //initblk: -488
                    throw new NotImplementedException("initblk is not implemented");
                case ILOpCodeValues.Rethrow: //rethrow: -486
                                             //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Sizeof: //sizeof: -484
                                            //TODO: Implement opcode
                    break;
                case ILOpCodeValues.Refanytype: //refanytype: -483
                    throw new NotImplementedException("refanytype is not implemented");
                case ILOpCodeValues.Readonly: //readonly.: -482
                    throw new NotImplementedException("readonly. is not implemented");


            }
        }
    }
}
