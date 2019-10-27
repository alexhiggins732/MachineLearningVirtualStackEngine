
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.7.3081.0
//  Copyright (c) Microsoft Corporation.  All rights reserved.


// ----- DOS Header:
// Magic:                      0x5a4d
// Bytes on last page:         0x0090
// Pages in file:              0x0003
// Relocations:                0x0000
// Size of header (paragraphs):0x0004
// Min extra paragraphs:       0x0000
// Max extra paragraphs:       0xffff
// Initial (relative) SS:      0x0000
// Initial SP:                 0x00b8
// Checksum:                   0x0000
// Initial IP:                 0x0000
// Initial (relative) CS:      0x0000
// File addr. of reloc table:  0x0040
// Overlay number:             0x0000
// OEM identifier:             0x0000
// OEM info:                   0x0000
// File addr. of COFF header:  0x0080
// ----- COFF/PE Headers:
// Signature:                  0x00004550
// ----- COFF Header:
// Machine:                    0x014c
// Number of sections:         0x0003
// Time-date stamp:            0x87198c92
// Ptr to symbol table:        0x00000000
// Number of symbols:          0x00000000
// Size of optional header:    0x00e0
// Characteristics:            0x0022
// ----- PE Optional Header (32 bit):
// Magic:                          0x010b
// Major linker version:           0x30
// Minor linker version:           0x00
// Size of code:                   0x00000800
// Size of init.data:              0x00000800
// Size of uninit.data:            0x00000000
// Addr. of entry point:           0x00002706
// Base of code:                   0x00002000
// Base of data:                   0x00004000
// Image base:                     0x00400000
// Section alignment:              0x00002000
// File alignment:                 0x00000200
// Major OS version:               0x0004
// Minor OS version:               0x0000
// Major image version:            0x0000
// Minor image version:            0x0000
// Major subsystem version:        0x0006
// Minor subsystem version:        0x0000
// Size of image:                  0x00008000
// Size of headers:                0x00000200
// Checksum:                       0x00000000
// Subsystem:                      0x0003
// DLL characteristics:            0x8560
// Size of stack reserve:          0x00100000
// Size of stack commit:           0x00001000
// Size of heap reserve:           0x00100000
// Size of heap commit:            0x00001000
// Loader flags:                   0x00000000
// Directories:                    0x00000010
// 0x00000000 [0x00000000] address [size] of Export Directory:          
// 0x000026b3 [0x0000004f] address [size] of Import Directory:          
// 0x00004000 [0x000005dc] address [size] of Resource Directory:        
// 0x00000000 [0x00000000] address [size] of Exception Directory:       
// 0x00000000 [0x00000000] address [size] of Security Directory:        
// 0x00006000 [0x0000000c] address [size] of Base Relocation Table:     
// 0x00002600 [0x00000038] address [size] of Debug Directory:           
// 0x00000000 [0x00000000] address [size] of Architecture Specific:     
// 0x00000000 [0x00000000] address [size] of Global Pointer:            
// 0x00000000 [0x00000000] address [size] of TLS Directory:             
// 0x00000000 [0x00000000] address [size] of Load Config Directory:     
// 0x00000000 [0x00000000] address [size] of Bound Import Directory:    
// 0x00002000 [0x00000008] address [size] of Import Address Table:      
// 0x00000000 [0x00000000] address [size] of Delay Load IAT:            
// 0x00002008 [0x00000048] address [size] of CLR Header:                


// Image sections:
//              .text
//              0x0000070c Virtual Size
//              0x00002000 Virtual Address
//              0x00000800 Size of Raw Data
//              0x00000200 Pointer to Raw Data
//              0x00000000 Pointer to Relocations
//              0x00000000 Pointer to Linenumbers
//                  0x0000 Number of Relocations
//                  0x0000 Number of Linenumbers
//              0x60000020 Characteristics
//                         CNT_CODE
//                         MEM_EXECUTE
//                         MEM_READ

//              .rsrc
//              0x000005dc Virtual Size
//              0x00004000 Virtual Address
//              0x00000600 Size of Raw Data
//              0x00000a00 Pointer to Raw Data
//              0x00000000 Pointer to Relocations
//              0x00000000 Pointer to Linenumbers
//                  0x0000 Number of Relocations
//                  0x0000 Number of Linenumbers
//              0x40000040 Characteristics
//                         CNT_INITIALIZED_DATA
//                         MEM_READ

//              .reloc
//              0x0000000c Virtual Size
//              0x00006000 Virtual Address
//              0x00000200 Size of Raw Data
//              0x00001000 Pointer to Raw Data
//              0x00000000 Pointer to Relocations
//              0x00000000 Pointer to Linenumbers
//                  0x0000 Number of Relocations
//                  0x0000 Number of Linenumbers
//              0x42000040 Characteristics
//                         CNT_INITIALIZED_DATA
//                         MEM_DISCARDABLE
//                         MEM_READ

// Base Relocation Table
//              0x00002000 Page RVA
//              0x0000000c Block Size
//              0x00000002 Number of Entries
//              Entry 1: Type 0x3 Offset 0x00000708
//              Entry 2: Type 0x0 Offset 0x00000000

// Import Address Table
//     DLL : mscoree.dll
//              0x00002000 Import Address Table
//              0x000026f5 Import Name Table
//              0          Time Date Stamp
//              0          Index of First Forwarder Reference
//
//              0x0000  _CorExeMain

// Delay Load Import Address Table
// No data.

// Entry point code:
//FF 25 00 20 40 00 


// ----- CLR Header:
// Header size:                        0x00000048
// Major runtime version:              0x0002
// Minor runtime version:              0x0005
// 0x0000205c [0x000005a4] address [size] of Metadata Directory:        
// Flags:                              0x00020003
// Entry point token:                  0x06000001
// 0x00000000 [0x00000000] address [size] of Resources Directory:       
// 0x00000000 [0x00000000] address [size] of Strong Name Signature:     
// 0x00000000 [0x00000000] address [size] of CodeManager Table:         
// 0x00000000 [0x00000000] address [size] of VTableFixups Directory:    
// 0x00000000 [0x00000000] address [size] of Export Address Table:      
// 0x00000000 [0x00000000] address [size] of Precompile Header:         

// Metadata Header
//    Storage Signature:
//              0x424a5342 Signature
//                  0x0001 Major Version
//                  0x0001 Minor Version
//              0x00000000 Extra Data Offset
//              0x0000000c Version String Length
//              'v4.0.30319' Version String
//    Storage Header:
//                    0x00 Flags
//                  0x0005 Number of Streams
//    Stream 1:
//              0x0000006c Offset
//              0x000001c0 Size
//              '#~' Name
//    Stream 2:
//              0x0000022c Offset
//              0x00000250 Size
//              '#Strings' Name
//    Stream 3:
//              0x0000047c Offset
//              0x00000004 Size
//              '#US' Name
//    Stream 4:
//              0x00000480 Offset
//              0x00000010 Size
//              '#GUID' Name
//    Stream 5:
//              0x00000490 Offset
//              0x00000114 Size
//              '#Blob' Name

//    Metadata Stream Header:
//              0x00000000 Reserved
//                    0x02 Major
//                    0x00 Minor
//                    0x00 Heaps
//                    0x01 Rid
//      0x0000000900001547 MaskValid
//      0x000016003301fa00 Sorted

// Code Manager Table:
//  default


// Export Address Table Jumps:
// No data.

// File size            : 4608
// PE header size       : 512 (496 used)    (11.11%)
// PE additional info   : 1727              (37.48%)
// Num.of PE sections   : 3
// CLR header size     : 72                 ( 1.56%)
// CLR meta-data size  : 1444               (31.34%)
// CLR additional info : 0                  ( 0.00%)
// CLR method headers  : 2                  ( 0.04%)
// Managed code         : 10                ( 0.22%)
// Data                 : 2048              (44.44%)
// Unaccounted          : -1207             (-26.19%)

// Num.of PE sections   : 3
//   .text    - 2048
//   .rsrc    - 1536
//   .reloc   - 512

// CLR meta-data size  : 1444
//   Module        -    1 (10 bytes)
//   TypeDef       -    2 (28 bytes)      0 interfaces, 0 explicit layout
//   TypeRef       -   16 (96 bytes)
//   MethodDef     -    2 (28 bytes)      0 abstract, 0 native, 2 bodies
//   MemberRef     -   15 (90 bytes)
//   ParamDef      -    1 (6 bytes)
//   CustomAttribute-   14 (84 bytes)
//   Assembly      -    1 (22 bytes)
//   AssemblyRef   -    1 (20 bytes)
//   Strings       -   589 bytes
//   Blobs         -   276 bytes
//   UserStrings   -     4 bytes
//   Guids         -    16 bytes
//   Uncategorized -   175 bytes

// CLR method headers : 2
//   Num.of method bodies  - 2
//   Num.of fat headers    - 0
//   Num.of tiny headers   - 2

// Managed code : 10
//   Ave method size - 5
// Classes defined in this module:
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// Class Program                        (auto) (ansi) 
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

// ================================= M E T A I N F O ================================================

// ===========================================================
// Name,Size,cRecs,cbRec
// Strings,589
// Blobs,276
// Guids,16
// Module,10,1,10
// TypeRef,96,16,6
// TypeDef,28,2,14
// FieldPtr,0,0,2
// Field,0,0,6
// MethodPtr,0,0,2
// Method,28,2,14
// ParamPtr,0,0,2
// Param,6,1,6
// InterfaceImpl,0,0,4
// MemberRef,90,15,6
// Constant,0,0,6
// CustomAttribute,84,14,6
// FieldMarshal,0,0,4
// DeclSecurity,0,0,6
// ClassLayout,0,0,8
// FieldLayout,0,0,6
// StandAloneSig,0,0,2
// EventMap,0,0,4
// EventPtr,0,0,2
// Event,0,0,6
// PropertyMap,0,0,4
// PropertyPtr,0,0,2
// Property,0,0,6
// MethodSemantics,0,0,6
// MethodImpl,0,0,6
// ModuleRef,0,0,2
// TypeSpec,0,0,2
// ImplMap,0,0,8
// FieldRVA,0,0,6
// ENCLog,0,0,8
// ENCMap,0,0,4
// Assembly,22,1,22
// AssemblyProcessor,0,0,4
// AssemblyOS,0,0,12
// AssemblyRef,20,1,20
// AssemblyRefProcessor,0,0,6
// AssemblyRefOS,0,0,14
// File,0,0,8
// ExportedType,0,0,14
// ManifestResource,0,0,12
// NestedClass,0,0,4
// GenericParam,0,0,8
// MethodSpec,0,0,4
// GenericParamConstraint,0,0,4
// Metadata section: 0x424a5342, version: 1.1, extra: 0, version len: 12, version: v4.0.30319
//            flags: 0x00, streams: 5
//         : 42 53 4a 42 01 00 01 00  00 00 00 00 0c 00 00 00 >BSJB            <
//         : 76 34 2e 30 2e 33 30 33  31 39 00 00 00 00 05 00 >v4.0.30319      <
//         : 6c 00 00 00 c0 01 00 00  23 7e 00 00 2c 02 00 00 >l       #~  ,   <
//         : 50 02 00 00 23 53 74 72  69 6e 67 73 00 00 00 00 >P   #Strings    <
//         : 7c 04 00 00 04 00 00 00  23 55 53 00 80 04 00 00 >|       #US     <
//         : 10 00 00 00 23 47 55 49  44 00 00 00 90 04 00 00 >    #GUID       <
//         : 14 01 00 00 23 42 6c 6f  62 00 00 00             >    #Blob       <
// Stream 0: name: #~, size 448
// Stream 1: name: #Strings, size 592
// Stream 2: name: #US, size 4
// Stream 3: name: #GUID, size 16
// Stream 4: name: #Blob, size 276
// Metadata header: 2.0, heaps: 0x00, rid: 0x01, valid: 0x0000000900001547, sorted: 0x000016003301fa00
//         : 00 00 00 00 02 00 00 01  47 15 00 00 09 00 00 00 >        G       <
//         : 00 fa 01 33 00 16 00 00                          >   3            <
// 
// Strings: 589(0x24d), Blobs: 276(0x114), Guids: 16(0x10), User strings: 4(0x4)
//  0(0): Module               cRecs:    1(0x1), cbRec: 10(0xa), cbTable:    10(0xa)
//   col  0:  Generation   oCol: 0, cbCol:2, USHORT 
//   col  1:  Name         oCol: 2, cbCol:2, string 
//   col  2:  Mvid         oCol: 4, cbCol:2, GUID   
//   col  3:  EncId        oCol: 6, cbCol:2, GUID   
//   col  4:  EncBaseId    oCol: 8, cbCol:2, GUID   
//  1(0x1): TypeRef              cRecs:   16(0x10), cbRec:  6(0x6), cbTable:    96(0x60)
//   col  0:  ResolutionScope oCol: 0, cbCol:2, ResolutionScope
//   col  1:  Name         oCol: 2, cbCol:2, string 
//   col  2:  Namespace    oCol: 4, cbCol:2, string 
//  2(0x2): TypeDef              cRecs:    2(0x2), cbRec: 14(0xe), cbTable:    28(0x1c)
//   col  0:  Flags        oCol: 0, cbCol:4, ULONG  
//   col  1:  Name         oCol: 4, cbCol:2, string 
//   col  2:  Namespace    oCol: 6, cbCol:2, string 
//   col  3:  Extends      oCol: 8, cbCol:2, TypeDefOrRef
//   col  4:  FieldList    oCol: a, cbCol:2, Field  
//   col  5:  MethodList   oCol: c, cbCol:2, Method 
//  3(0x3): FieldPtr             cRecs:    0(0), cbRec:  2(0x2), cbTable:     0(0)
//   col  0:  Field        oCol: 0, cbCol:2, Field  
//  4(0x4): Field                cRecs:    0(0), cbRec:  6(0x6), cbTable:     0(0)
//   col  0:  Flags        oCol: 0, cbCol:2, USHORT 
//   col  1:  Name         oCol: 2, cbCol:2, string 
//   col  2:  Signature    oCol: 4, cbCol:2, blob   
//  5(0x5): MethodPtr            cRecs:    0(0), cbRec:  2(0x2), cbTable:     0(0)
//   col  0:  Method       oCol: 0, cbCol:2, Method 
//  6(0x6): Method               cRecs:    2(0x2), cbRec: 14(0xe), cbTable:    28(0x1c)
//   col  0:  RVA          oCol: 0, cbCol:4, ULONG  
//   col  1:  ImplFlags    oCol: 4, cbCol:2, USHORT 
//   col  2:  Flags        oCol: 6, cbCol:2, USHORT 
//   col  3:  Name         oCol: 8, cbCol:2, string 
//   col  4:  Signature    oCol: a, cbCol:2, blob   
//   col  5:  ParamList    oCol: c, cbCol:2, Param  
//  7(0x7): ParamPtr             cRecs:    0(0), cbRec:  2(0x2), cbTable:     0(0)
//   col  0:  Param        oCol: 0, cbCol:2, Param  
//  8(0x8): Param                cRecs:    1(0x1), cbRec:  6(0x6), cbTable:     6(0x6)
//   col  0:  Flags        oCol: 0, cbCol:2, USHORT 
//   col  1:  Sequence     oCol: 2, cbCol:2, USHORT 
//   col  2:  Name         oCol: 4, cbCol:2, string 
//  9(0x9): InterfaceImpl        cRecs:    0(0), cbRec:  4(0x4), cbTable:     0(0)
//   col  0:* Class        oCol: 0, cbCol:2, TypeDef
//   col  1:  Interface    oCol: 2, cbCol:2, TypeDefOrRef
// 10(0xa): MemberRef            cRecs:   15(0xf), cbRec:  6(0x6), cbTable:    90(0x5a)
//   col  0:  Class        oCol: 0, cbCol:2, MemberRefParent
//   col  1:  Name         oCol: 2, cbCol:2, string 
//   col  2:  Signature    oCol: 4, cbCol:2, blob   
// 11(0xb): Constant             cRecs:    0(0), cbRec:  6(0x6), cbTable:     0(0)
//   col  0:  Type         oCol: 0, cbCol:1, BYTE   
//   col  1:* Parent       oCol: 2, cbCol:2, HasConstant
//   col  2:  Value        oCol: 4, cbCol:2, blob   
// 12(0xc): CustomAttribute      cRecs:   14(0xe), cbRec:  6(0x6), cbTable:    84(0x54)
//   col  0:* Parent       oCol: 0, cbCol:2, HasCustomAttribute
//   col  1:  Type         oCol: 2, cbCol:2, CustomAttributeType
//   col  2:  Value        oCol: 4, cbCol:2, blob   
// 13(0xd): FieldMarshal         cRecs:    0(0), cbRec:  4(0x4), cbTable:     0(0)
//   col  0:* Parent       oCol: 0, cbCol:2, HasFieldMarshal
//   col  1:  NativeType   oCol: 2, cbCol:2, blob   
// 14(0xe): DeclSecurity         cRecs:    0(0), cbRec:  6(0x6), cbTable:     0(0)
//   col  0:  Action       oCol: 0, cbCol:2, short  
//   col  1:* Parent       oCol: 2, cbCol:2, HasDeclSecurity
//   col  2:  PermissionSet oCol: 4, cbCol:2, blob   
// 15(0xf): ClassLayout          cRecs:    0(0), cbRec:  8(0x8), cbTable:     0(0)
//   col  0:  PackingSize  oCol: 0, cbCol:2, USHORT 
//   col  1:  ClassSize    oCol: 2, cbCol:4, ULONG  
//   col  2:* Parent       oCol: 6, cbCol:2, TypeDef
// 16(0x10): FieldLayout          cRecs:    0(0), cbRec:  6(0x6), cbTable:     0(0)
//   col  0:  OffSet       oCol: 0, cbCol:4, ULONG  
//   col  1:* Field        oCol: 4, cbCol:2, Field  
// 17(0x11): StandAloneSig        cRecs:    0(0), cbRec:  2(0x2), cbTable:     0(0)
//   col  0:  Signature    oCol: 0, cbCol:2, blob   
// 18(0x12): EventMap             cRecs:    0(0), cbRec:  4(0x4), cbTable:     0(0)
//   col  0:  Parent       oCol: 0, cbCol:2, TypeDef
//   col  1:  EventList    oCol: 2, cbCol:2, Event  
// 19(0x13): EventPtr             cRecs:    0(0), cbRec:  2(0x2), cbTable:     0(0)
//   col  0:  Event        oCol: 0, cbCol:2, Event  
// 20(0x14): Event                cRecs:    0(0), cbRec:  6(0x6), cbTable:     0(0)
//   col  0:  EventFlags   oCol: 0, cbCol:2, USHORT 
//   col  1:  Name         oCol: 2, cbCol:2, string 
//   col  2:  EventType    oCol: 4, cbCol:2, TypeDefOrRef
// 21(0x15): PropertyMap          cRecs:    0(0), cbRec:  4(0x4), cbTable:     0(0)
//   col  0:  Parent       oCol: 0, cbCol:2, TypeDef
//   col  1:  PropertyList oCol: 2, cbCol:2, Property
// 22(0x16): PropertyPtr          cRecs:    0(0), cbRec:  2(0x2), cbTable:     0(0)
//   col  0:  Property     oCol: 0, cbCol:2, Property
// 23(0x17): Property             cRecs:    0(0), cbRec:  6(0x6), cbTable:     0(0)
//   col  0:  PropFlags    oCol: 0, cbCol:2, USHORT 
//   col  1:  Name         oCol: 2, cbCol:2, string 
//   col  2:  Type         oCol: 4, cbCol:2, blob   
// 24(0x18): MethodSemantics      cRecs:    0(0), cbRec:  6(0x6), cbTable:     0(0)
//   col  0:  Semantic     oCol: 0, cbCol:2, USHORT 
//   col  1:  Method       oCol: 2, cbCol:2, Method 
//   col  2:* Association  oCol: 4, cbCol:2, HasSemantic
// 25(0x19): MethodImpl           cRecs:    0(0), cbRec:  6(0x6), cbTable:     0(0)
//   col  0:* Class        oCol: 0, cbCol:2, TypeDef
//   col  1:  MethodBody   oCol: 2, cbCol:2, MethodDefOrRef
//   col  2:  MethodDeclaration oCol: 4, cbCol:2, MethodDefOrRef
// 26(0x1a): ModuleRef            cRecs:    0(0), cbRec:  2(0x2), cbTable:     0(0)
//   col  0:  Name         oCol: 0, cbCol:2, string 
// 27(0x1b): TypeSpec             cRecs:    0(0), cbRec:  2(0x2), cbTable:     0(0)
//   col  0:  Signature    oCol: 0, cbCol:2, blob   
// 28(0x1c): ImplMap              cRecs:    0(0), cbRec:  8(0x8), cbTable:     0(0)
//   col  0:  MappingFlags oCol: 0, cbCol:2, USHORT 
//   col  1:* MemberForwarded oCol: 2, cbCol:2, MemberForwarded
//   col  2:  ImportName   oCol: 4, cbCol:2, string 
//   col  3:  ImportScope  oCol: 6, cbCol:2, ModuleRef
// 29(0x1d): FieldRVA             cRecs:    0(0), cbRec:  6(0x6), cbTable:     0(0)
//   col  0:  RVA          oCol: 0, cbCol:4, ULONG  
//   col  1:* Field        oCol: 4, cbCol:2, Field  
// 30(0x1e): ENCLog               cRecs:    0(0), cbRec:  8(0x8), cbTable:     0(0)
//   col  0:  Token        oCol: 0, cbCol:4, ULONG  
//   col  1:  FuncCode     oCol: 4, cbCol:4, ULONG  
// 31(0x1f): ENCMap               cRecs:    0(0), cbRec:  4(0x4), cbTable:     0(0)
//   col  0:  Token        oCol: 0, cbCol:4, ULONG  
// 32(0x20): Assembly             cRecs:    1(0x1), cbRec: 22(0x16), cbTable:    22(0x16)
//   col  0:  HashAlgId    oCol: 0, cbCol:4, ULONG  
//   col  1:  MajorVersion oCol: 4, cbCol:2, USHORT 
//   col  2:  MinorVersion oCol: 6, cbCol:2, USHORT 
//   col  3:  BuildNumber  oCol: 8, cbCol:2, USHORT 
//   col  4:  RevisionNumber oCol: a, cbCol:2, USHORT 
//   col  5:  Flags        oCol: c, cbCol:4, ULONG  
//   col  6:  PublicKey    oCol:10, cbCol:2, blob   
//   col  7:  Name         oCol:12, cbCol:2, string 
//   col  8:  Locale       oCol:14, cbCol:2, string 
// 33(0x21): AssemblyProcessor    cRecs:    0(0), cbRec:  4(0x4), cbTable:     0(0)
//   col  0:  Processor    oCol: 0, cbCol:4, ULONG  
// 34(0x22): AssemblyOS           cRecs:    0(0), cbRec: 12(0xc), cbTable:     0(0)
//   col  0:  OSPlatformId oCol: 0, cbCol:4, ULONG  
//   col  1:  OSMajorVersion oCol: 4, cbCol:4, ULONG  
//   col  2:  OSMinorVersion oCol: 8, cbCol:4, ULONG  
// 35(0x23): AssemblyRef          cRecs:    1(0x1), cbRec: 20(0x14), cbTable:    20(0x14)
//   col  0:  MajorVersion oCol: 0, cbCol:2, USHORT 
//   col  1:  MinorVersion oCol: 2, cbCol:2, USHORT 
//   col  2:  BuildNumber  oCol: 4, cbCol:2, USHORT 
//   col  3:  RevisionNumber oCol: 6, cbCol:2, USHORT 
//   col  4:  Flags        oCol: 8, cbCol:4, ULONG  
//   col  5:  PublicKeyOrToken oCol: c, cbCol:2, blob   
//   col  6:  Name         oCol: e, cbCol:2, string 
//   col  7:  Locale       oCol:10, cbCol:2, string 
//   col  8:  HashValue    oCol:12, cbCol:2, blob   
// 36(0x24): AssemblyRefProcessor cRecs:    0(0), cbRec:  6(0x6), cbTable:     0(0)
//   col  0:  Processor    oCol: 0, cbCol:4, ULONG  
//   col  1:  AssemblyRef  oCol: 4, cbCol:2, AssemblyRef
// 37(0x25): AssemblyRefOS        cRecs:    0(0), cbRec: 14(0xe), cbTable:     0(0)
//   col  0:  OSPlatformId oCol: 0, cbCol:4, ULONG  
//   col  1:  OSMajorVersion oCol: 4, cbCol:4, ULONG  
//   col  2:  OSMinorVersion oCol: 8, cbCol:4, ULONG  
//   col  3:  AssemblyRef  oCol: c, cbCol:2, AssemblyRef
// 38(0x26): File                 cRecs:    0(0), cbRec:  8(0x8), cbTable:     0(0)
//   col  0:  Flags        oCol: 0, cbCol:4, ULONG  
//   col  1:  Name         oCol: 4, cbCol:2, string 
//   col  2:  HashValue    oCol: 6, cbCol:2, blob   
// 39(0x27): ExportedType         cRecs:    0(0), cbRec: 14(0xe), cbTable:     0(0)
//   col  0:  Flags        oCol: 0, cbCol:4, ULONG  
//   col  1:  TypeDefId    oCol: 4, cbCol:4, ULONG  
//   col  2:  TypeName     oCol: 8, cbCol:2, string 
//   col  3:  TypeNamespace oCol: a, cbCol:2, string 
//   col  4:  Implementation oCol: c, cbCol:2, Implementation
// 40(0x28): ManifestResource     cRecs:    0(0), cbRec: 12(0xc), cbTable:     0(0)
//   col  0:  Offset       oCol: 0, cbCol:4, ULONG  
//   col  1:  Flags        oCol: 4, cbCol:4, ULONG  
//   col  2:  Name         oCol: 8, cbCol:2, string 
//   col  3:  Implementation oCol: a, cbCol:2, Implementation
// 41(0x29): NestedClass          cRecs:    0(0), cbRec:  4(0x4), cbTable:     0(0)
//   col  0:* NestedClass  oCol: 0, cbCol:2, TypeDef
//   col  1:  EnclosingClass oCol: 2, cbCol:2, TypeDef
// 42(0x2a): GenericParam         cRecs:    0(0), cbRec:  8(0x8), cbTable:     0(0)
//   col  0:  Number       oCol: 0, cbCol:2, USHORT 
//   col  1:  Flags        oCol: 2, cbCol:2, USHORT 
//   col  2:* Owner        oCol: 4, cbCol:2, TypeOrMethodDef
//   col  3:  Name         oCol: 6, cbCol:2, string 
// 43(0x2b): MethodSpec           cRecs:    0(0), cbRec:  4(0x4), cbTable:     0(0)
//   col  0:  Method       oCol: 0, cbCol:2, MethodDefOrRef
//   col  1:  Instantiation oCol: 2, cbCol:2, blob   
// 44(0x2c): GenericParamConstraint cRecs:    0(0), cbRec:  4(0x4), cbTable:     0(0)
//   col  0:* Owner        oCol: 0, cbCol:2, GenericParam
//   col  1:  Constraint   oCol: 2, cbCol:2, TypeDefOrRef
// 
// Blob Heap:  276(0x114) bytes
//     0,0 :                                                  >                <
//     1,4 : 20 01 01 08                                      >                <
//     6,3 : 20 00 01                                         >                <
//     a,5 : 20 01 01 11 11                                   >                <
//    10,4 : 20 01 01 0e                                      >                <
//    15,4 : 20 01 01 02                                      >                <
//    1a,8 : b7 7a 5c 56 19 34 e0 89                          > z\V 4          <
//    23,5 : 00 01 01 1d 0e                                   >                <
//    29,8 : 01 00 08 00 00 00 00 00                          >                <
//    32,1e: 01 00 01 00 54 02 16 57  72 61 70 4e 6f 6e 45 78 >    T  WrapNonEx<
//         : 63 65 70 74 69 6f 6e 54  68 72 6f 77 73 01       >ceptionThrows   <
//    51,8 : 01 00 07 01 00 00 00 00                          >                <
//    5a,13: 01 00 0e 49 4c 44 69 73  61 73 73 65 6d 62 6c 65 >   ILDisassemble<
//         : 72 00 00                                         >r               <
//    6e,5 : 01 00 00 00 00                                   >                <
//    74,17: 01 00 12 43 6f 70 79 72  69 67 68 74 20 c2 a9 20 >   Copyright    <
//         : 20 32 30 31 39 00 00                             > 2019           <
//    8c,29: 01 00 24 66 36 38 65 34  66 64 31 2d 66 32 37 62 >  $f68e4fd1-f27b<
//         : 2d 34 36 62 65 2d 61 36  34 33 2d 36 61 33 37 61 >-46be-a643-6a37a<
//         : 62 66 38 63 62 62 64 00  00                      >bf8cbbd         <
//    b6,c : 01 00 07 31 2e 30 2e 30  2e 30 00 00             >   1.0.0.0      <
//    c3,4d: 01 00 1c 2e 4e 45 54 46  72 61 6d 65 77 6f 72 6b >   .NETFramework<
//         : 2c 56 65 72 73 69 6f 6e  3d 76 34 2e 37 2e 32 01 >,Version=v4.7.2 <
//         : 00 54 0e 14 46 72 61 6d  65 77 6f 72 6b 44 69 73 > T  FrameworkDis<
//         : 70 6c 61 79 4e 61 6d 65  14 2e 4e 45 54 20 46 72 >playName .NET Fr<
//         : 61 6d 65 77 6f 72 6b 20  34 2e 37 2e 32          >amework 4.7.2   <
//   111,0 :                                                  >                <
//   112,0 :                                                  >                <
//   113,0 :                                                  >                <
// 
// String Heap:  589(0x24d) bytes
// 00000000: 00                                               >                <
// 00000001: 3c 4d 6f 64 75 6c 65 3e  00                      ><Module>        <
// 0000000a: 6d 73 63 6f 72 6c 69 62  00                      >mscorlib        <
// 00000013: 47 75 69 64 41 74 74 72  69 62 75 74 65 00       >GuidAttribute   <
// 00000021: 44 65 62 75 67 67 61 62  6c 65 41 74 74 72 69 62 >DebuggableAttrib<
//         : 75 74 65 00                                      >ute             <
// 00000035: 43 6f 6d 56 69 73 69 62  6c 65 41 74 74 72 69 62 >ComVisibleAttrib<
//         : 75 74 65 00                                      >ute             <
// 00000049: 41 73 73 65 6d 62 6c 79  54 69 74 6c 65 41 74 74 >AssemblyTitleAtt<
//         : 72 69 62 75 74 65 00                             >ribute          <
// 00000060: 41 73 73 65 6d 62 6c 79  54 72 61 64 65 6d 61 72 >AssemblyTrademar<
//         : 6b 41 74 74 72 69 62 75  74 65 00                >kAttribute      <
// 0000007b: 54 61 72 67 65 74 46 72  61 6d 65 77 6f 72 6b 41 >TargetFrameworkA<
//         : 74 74 72 69 62 75 74 65  00                      >ttribute        <
// 00000094: 41 73 73 65 6d 62 6c 79  46 69 6c 65 56 65 72 73 >AssemblyFileVers<
//         : 69 6f 6e 41 74 74 72 69  62 75 74 65 00          >ionAttribute    <
// 000000b1: 41 73 73 65 6d 62 6c 79  43 6f 6e 66 69 67 75 72 >AssemblyConfigur<
//         : 61 74 69 6f 6e 41 74 74  72 69 62 75 74 65 00    >ationAttribute  <
// 000000d0: 41 73 73 65 6d 62 6c 79  44 65 73 63 72 69 70 74 >AssemblyDescript<
//         : 69 6f 6e 41 74 74 72 69  62 75 74 65 00          >ionAttribute    <
// 000000ed: 43 6f 6d 70 69 6c 61 74  69 6f 6e 52 65 6c 61 78 >CompilationRelax<
//         : 61 74 69 6f 6e 73 41 74  74 72 69 62 75 74 65 00 >ationsAttribute <
// 0000010d: 41 73 73 65 6d 62 6c 79  50 72 6f 64 75 63 74 41 >AssemblyProductA<
//         : 74 74 72 69 62 75 74 65  00                      >ttribute        <
// 00000126: 41 73 73 65 6d 62 6c 79  43 6f 70 79 72 69 67 68 >AssemblyCopyrigh<
//         : 74 41 74 74 72 69 62 75  74 65 00                >tAttribute      <
// 00000141: 41 73 73 65 6d 62 6c 79  43 6f 6d 70 61 6e 79 41 >AssemblyCompanyA<
//         : 74 74 72 69 62 75 74 65  00                      >ttribute        <
// 0000015a: 52 75 6e 74 69 6d 65 43  6f 6d 70 61 74 69 62 69 >RuntimeCompatibi<
//         : 6c 69 74 79 41 74 74 72  69 62 75 74 65 00       >lityAttribute   <
// 00000178: 49 4c 44 69 73 61 73 73  65 6d 62 6c 65 72 2e 65 >ILDisassembler.e<
//         : 78 65 00                                         >xe              <
// 0000018b: 53 79 73 74 65 6d 2e 52  75 6e 74 69 6d 65 2e 56 >System.Runtime.V<
//         : 65 72 73 69 6f 6e 69 6e  67 00                   >ersioning       <
// 000001a5: 50 72 6f 67 72 61 6d 00                          >Program         <
// 000001ad: 53 79 73 74 65 6d 00                             >System          <
// 000001b4: 4d 61 69 6e 00                                   >Main            <
// 000001b9: 53 79 73 74 65 6d 2e 52  65 66 6c 65 63 74 69 6f >System.Reflectio<
//         : 6e 00                                            >n               <
// 000001cb: 49 4c 44 69 73 61 73 73  65 6d 62 6c 65 72 00    >ILDisassembler  <
// 000001da: 2e 63 74 6f 72 00                                >.ctor           <
// 000001e0: 53 79 73 74 65 6d 2e 44  69 61 67 6e 6f 73 74 69 >System.Diagnosti<
//         : 63 73 00                                         >cs              <
// 000001f3: 53 79 73 74 65 6d 2e 52  75 6e 74 69 6d 65 2e 49 >System.Runtime.I<
//         : 6e 74 65 72 6f 70 53 65  72 76 69 63 65 73 00    >nteropServices  <
// 00000212: 53 79 73 74 65 6d 2e 52  75 6e 74 69 6d 65 2e 43 >System.Runtime.C<
//         : 6f 6d 70 69 6c 65 72 53  65 72 76 69 63 65 73 00 >ompilerServices <
// 00000232: 44 65 62 75 67 67 69 6e  67 4d 6f 64 65 73 00    >DebuggingModes  <
// 00000241: 61 72 67 73 00                                   >args            <
// 00000246: 4f 62 6a 65 63 74 00                             >Object          <
// 
// ===========================================================
// 
// Unresolved Externals
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// ===========================================================
// No warnings or errors found.
// ===========================================================
// ================================= END OF METAINFO ================================================


// Metadata version: v4.0.30319
.assembly extern /*23000001*/ mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly /*20000001*/ ILDisassembler
{
  .custom /*0C000001:0A000001*/ instance void [mscorlib/*23000001*/]System.Runtime.CompilerServices.CompilationRelaxationsAttribute/*01000001*/::.ctor(int32) /* 0A000001 */ = ( 01 00 08 00 00 00 00 00 ) 
  .custom /*0C000002:0A000002*/ instance void [mscorlib/*23000001*/]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute/*01000002*/::.ctor() /* 0A000002 */ = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                                                                                          63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom /*0C000003:0A000003*/ instance void [mscorlib/*23000001*/]System.Diagnostics.DebuggableAttribute/*01000003*/::.ctor(valuetype [mscorlib/*23000001*/]System.Diagnostics.DebuggableAttribute/*01000003*//DebuggingModes/*01000004*/) /* 0A000003 */ = ( 01 00 07 01 00 00 00 00 ) 

  .custom /*0C000004:0A000004*/ instance void [mscorlib/*23000001*/]System.Reflection.AssemblyTitleAttribute/*01000005*/::.ctor(string) /* 0A000004 */ = ( 01 00 0E 49 4C 44 69 73 61 73 73 65 6D 62 6C 65   // ...ILDisassemble
                                                                                                                                                           72 00 00 )                                        // r..
  .custom /*0C000005:0A000005*/ instance void [mscorlib/*23000001*/]System.Reflection.AssemblyDescriptionAttribute/*01000006*/::.ctor(string) /* 0A000005 */ = ( 01 00 00 00 00 ) 
  .custom /*0C000006:0A000006*/ instance void [mscorlib/*23000001*/]System.Reflection.AssemblyConfigurationAttribute/*01000007*/::.ctor(string) /* 0A000006 */ = ( 01 00 00 00 00 ) 
  .custom /*0C000007:0A000007*/ instance void [mscorlib/*23000001*/]System.Reflection.AssemblyCompanyAttribute/*01000008*/::.ctor(string) /* 0A000007 */ = ( 01 00 00 00 00 ) 
  .custom /*0C000008:0A000008*/ instance void [mscorlib/*23000001*/]System.Reflection.AssemblyProductAttribute/*01000009*/::.ctor(string) /* 0A000008 */ = ( 01 00 0E 49 4C 44 69 73 61 73 73 65 6D 62 6C 65   // ...ILDisassemble
                                                                                                                                                             72 00 00 )                                        // r..
  .custom /*0C000009:0A000009*/ instance void [mscorlib/*23000001*/]System.Reflection.AssemblyCopyrightAttribute/*0100000A*/::.ctor(string) /* 0A000009 */ = ( 01 00 12 43 6F 70 79 72 69 67 68 74 20 C2 A9 20   // ...Copyright .. 
                                                                                                                                                               20 32 30 31 39 00 00 )                            //  2019..
  .custom /*0C00000A:0A00000A*/ instance void [mscorlib/*23000001*/]System.Reflection.AssemblyTrademarkAttribute/*0100000B*/::.ctor(string) /* 0A00000A */ = ( 01 00 00 00 00 ) 
  .custom /*0C00000B:0A00000B*/ instance void [mscorlib/*23000001*/]System.Runtime.InteropServices.ComVisibleAttribute/*0100000C*/::.ctor(bool) /* 0A00000B */ = ( 01 00 00 00 00 ) 
  .custom /*0C00000C:0A00000C*/ instance void [mscorlib/*23000001*/]System.Runtime.InteropServices.GuidAttribute/*0100000D*/::.ctor(string) /* 0A00000C */ = ( 01 00 24 66 36 38 65 34 66 64 31 2D 66 32 37 62   // ..$f68e4fd1-f27b
                                                                                                                                                               2D 34 36 62 65 2D 61 36 34 33 2D 36 61 33 37 61   // -46be-a643-6a37a
                                                                                                                                                               62 66 38 63 62 62 64 00 00 )                      // bf8cbbd..
  .custom /*0C00000D:0A00000D*/ instance void [mscorlib/*23000001*/]System.Reflection.AssemblyFileVersionAttribute/*0100000E*/::.ctor(string) /* 0A00000D */ = ( 01 00 07 31 2E 30 2E 30 2E 30 00 00 )             // ...1.0.0.0..
  .custom /*0C00000E:0A00000E*/ instance void [mscorlib/*23000001*/]System.Runtime.Versioning.TargetFrameworkAttribute/*0100000F*/::.ctor(string) /* 0A00000E */ = ( 01 00 1C 2E 4E 45 54 46 72 61 6D 65 77 6F 72 6B   // ....NETFramework
                                                                                                                                                                     2C 56 65 72 73 69 6F 6E 3D 76 34 2E 37 2E 32 01   // ,Version=v4.7.2.
                                                                                                                                                                     00 54 0E 14 46 72 61 6D 65 77 6F 72 6B 44 69 73   // .T..FrameworkDis
                                                                                                                                                                     70 6C 61 79 4E 61 6D 65 14 2E 4E 45 54 20 46 72   // playName..NET Fr
                                                                                                                                                                     61 6D 65 77 6F 72 6B 20 34 2E 37 2E 32 )          // amework 4.7.2
  .hash algorithm 0x00008004
  .ver 1:0:0:0
}
.module ILDisassembler.exe
// MVID: {C7D342C4-128A-480D-9403-6FF69BAC4C44}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00020003    //  ILONLY 32BITPREFERRED
// Image base: 0x07080000


// =============== CLASS MEMBERS DECLARATION ===================

.class /*02000002*/ private auto ansi beforefieldinit ILDisassembler.Program
       extends [mscorlib/*23000001*/]System.Object/*01000010*/
{
  .method /*06000001*/ private hidebysig static 
          void  Main(string[] args) cil managed
  // SIG: 00 01 01 1D 0E
  {
    .entrypoint
    // Method begins at RVA 0x2050
    // Code size       2 (0x2)
    .maxstack  8
    .language '{3F5162F8-07C6-11D3-9053-00C04FA302A1}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
// Source File 'C:\Users\alexander.higgins\source\repos\ILDisassembler\ILDisassembler\Program.cs' 
    .line 12,12 : 9,10 'C:\\Users\\alexander.higgins\\source\\repos\\ILDisassembler\\ILDisassembler\\Program.cs'
//000012:         {
    IL_0000:  /* 00   |                  */ nop
    .line 13,13 : 9,10 ''
//000013:         }
    IL_0001:  /* 2A   |                  */ ret
  } // end of method Program::Main

  .method /*06000002*/ public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  // SIG: 20 00 01
  {
    // Method begins at RVA 0x2053
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  /* 02   |                  */ ldarg.0
    IL_0001:  /* 28   | (0A)00000F       */ call       instance void [mscorlib/*23000001*/]System.Object/*01000010*/::.ctor() /* 0A00000F */
    IL_0006:  /* 00   |                  */ nop
    IL_0007:  /* 2A   |                  */ ret
  } // end of method Program::.ctor

} // end of class ILDisassembler.Program


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file C:\Users\alexander.higgins\source\repos\ILDisassembler\Dumps\basicconsoleapp.res
