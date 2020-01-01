﻿namespace ILEngine.CodeGenerator
{
    // Autogenerated using ILOpcodeActionCodeBuilder
    public interface IILOpcodeActionGenerator
    {
        void GenerateAll();
        void GenerateNop();
        void GenerateBreak();
        void GenerateLdarg_0();
        void GenerateLdarg_1();
        void GenerateLdarg_2();
        void GenerateLdarg_3();
        void GenerateLdloc_0();
        void GenerateLdloc_1();
        void GenerateLdloc_2();
        void GenerateLdloc_3();
        void GenerateStloc_0();
        void GenerateStloc_1();
        void GenerateStloc_2();
        void GenerateStloc_3();
        void GenerateLdarg_S();
        void GenerateLdarga_S();
        void GenerateStarg_S();
        void GenerateLdloc_S();
        void GenerateLdloca_S();
        void GenerateStloc_S();
        void GenerateLdnull();
        void GenerateLdc_I4_M1();
        void GenerateLdc_I4_0();
        void GenerateLdc_I4_1();
        void GenerateLdc_I4_2();
        void GenerateLdc_I4_3();
        void GenerateLdc_I4_4();
        void GenerateLdc_I4_5();
        void GenerateLdc_I4_6();
        void GenerateLdc_I4_7();
        void GenerateLdc_I4_8();
        void GenerateLdc_I4_S();
        void GenerateLdc_I4();
        void GenerateLdc_I8();
        void GenerateLdc_R4();
        void GenerateLdc_R8();
        void GenerateExec_MSIL_I();
        void GenerateDup();
        void GeneratePop();
        void GenerateJmp();
        void GenerateCall();
        void GenerateCalli();
        void GenerateRet();
        void GenerateBr_S();
        void GenerateBrfalse_S();
        void GenerateBrtrue_S();
        void GenerateBeq_S();
        void GenerateBge_S();
        void GenerateBgt_S();
        void GenerateBle_S();
        void GenerateBlt_S();
        void GenerateBne_Un_S();
        void GenerateBge_Un_S();
        void GenerateBgt_Un_S();
        void GenerateBle_Un_S();
        void GenerateBlt_Un_S();
        void GenerateBr();
        void GenerateBrfalse();
        void GenerateBrtrue();
        void GenerateBeq();
        void GenerateBge();
        void GenerateBgt();
        void GenerateBle();
        void GenerateBlt();
        void GenerateBne_Un();
        void GenerateBge_Un();
        void GenerateBgt_Un();
        void GenerateBle_Un();
        void GenerateBlt_Un();
        void GenerateSwitch();
        void GenerateLdind_I1();
        void GenerateLdind_U1();
        void GenerateLdind_I2();
        void GenerateLdind_U2();
        void GenerateLdind_I4();
        void GenerateLdind_U4();
        void GenerateLdind_I8();
        void GenerateLdind_I();
        void GenerateLdind_R4();
        void GenerateLdind_R8();
        void GenerateLdind_Ref();
        void GenerateStind_Ref();
        void GenerateStind_I1();
        void GenerateStind_I2();
        void GenerateStind_I4();
        void GenerateStind_I8();
        void GenerateStind_R4();
        void GenerateStind_R8();
        void GenerateAdd();
        void GenerateSub();
        void GenerateMul();
        void GenerateDiv();
        void GenerateDiv_Un();
        void GenerateRem();
        void GenerateRem_Un();
        void GenerateAnd();
        void GenerateOr();
        void GenerateXor();
        void GenerateShl();
        void GenerateShr();
        void GenerateShr_Un();
        void GenerateNeg();
        void GenerateNot();
        void GenerateConv_I1();
        void GenerateConv_I2();
        void GenerateConv_I4();
        void GenerateConv_I8();
        void GenerateConv_R4();
        void GenerateConv_R8();
        void GenerateConv_U4();
        void GenerateConv_U8();
        void GenerateCallvirt();
        void GenerateCpobj();
        void GenerateLdobj();
        void GenerateLdstr();
        void GenerateNewobj();
        void GenerateCastclass();
        void GenerateIsinst();
        void GenerateConv_R_Un();
        void GenerateExec_MSIL_S();
        void GenerateUnbox();
        void GenerateThrow();
        void GenerateLdfld();
        void GenerateLdflda();
        void GenerateStfld();
        void GenerateLdsfld();
        void GenerateLdsflda();
        void GenerateStsfld();
        void GenerateStobj();
        void GenerateConv_Ovf_I1_Un();
        void GenerateConv_Ovf_I2_Un();
        void GenerateConv_Ovf_I4_Un();
        void GenerateConv_Ovf_I8_Un();
        void GenerateConv_Ovf_U1_Un();
        void GenerateConv_Ovf_U2_Un();
        void GenerateConv_Ovf_U4_Un();
        void GenerateConv_Ovf_U8_Un();
        void GenerateConv_Ovf_I_Un();
        void GenerateConv_Ovf_U_Un();
        void GenerateBox();
        void GenerateNewarr();
        void GenerateLdlen();
        void GenerateLdelema();
        void GenerateLdelem_I1();
        void GenerateLdelem_U1();
        void GenerateLdelem_I2();
        void GenerateLdelem_U2();
        void GenerateLdelem_I4();
        void GenerateLdelem_U4();
        void GenerateLdelem_I8();
        void GenerateLdelem_I();
        void GenerateLdelem_R4();
        void GenerateLdelem_R8();
        void GenerateLdelem_Ref();
        void GenerateStelem_I();
        void GenerateStelem_I1();
        void GenerateStelem_I2();
        void GenerateStelem_I4();
        void GenerateStelem_I8();
        void GenerateStelem_R4();
        void GenerateStelem_R8();
        void GenerateStelem_Ref();
        void GenerateLdelem();
        void GenerateStelem();
        void GenerateUnbox_Any();
        void GenerateConv_Ovf_I1();
        void GenerateConv_Ovf_U1();
        void GenerateConv_Ovf_I2();
        void GenerateConv_Ovf_U2();
        void GenerateConv_Ovf_I4();
        void GenerateConv_Ovf_U4();
        void GenerateConv_Ovf_I8();
        void GenerateConv_Ovf_U8();
        void GenerateRefanyval();
        void GenerateCkfinite();
        void GenerateMkrefany();
        void GenerateLdtoken();
        void GenerateConv_U2();
        void GenerateConv_U1();
        void GenerateConv_I();
        void GenerateConv_Ovf_I();
        void GenerateConv_Ovf_U();
        void GenerateAdd_Ovf();
        void GenerateAdd_Ovf_Un();
        void GenerateMul_Ovf();
        void GenerateMul_Ovf_Un();
        void GenerateSub_Ovf();
        void GenerateSub_Ovf_Un();
        void GenerateEndfinally();
        void GenerateLeave();
        void GenerateLeave_S();
        void GenerateStind_I();
        void GenerateConv_U();
        void GeneratePrefix7();
        void GeneratePrefix6();
        void GeneratePrefix5();
        void GeneratePrefix4();
        void GeneratePrefix3();
        void GeneratePrefix2();
        void GeneratePrefix1();
        void GeneratePrefixref();
        void GenerateArglist();
        void GenerateCeq();
        void GenerateCgt();
        void GenerateCgt_Un();
        void GenerateClt();
        void GenerateClt_Un();
        void GenerateLdftn();
        void GenerateLdvirtftn();
        void GenerateLdarg();
        void GenerateLdarga();
        void GenerateStarg();
        void GenerateLdloc();
        void GenerateLdloca();
        void GenerateStloc();
        void GenerateLocalloc();
        void GenerateEndfilter();
        void GenerateUnaligned_();
        void GenerateVolatile_();
        void GenerateTail_();
        void GenerateInitobj();
        void GenerateConstrained_();
        void GenerateCpblk();
        void GenerateInitblk();
        void GenerateRethrow();
        void GenerateSizeof();
        void GenerateRefanytype();
        void GenerateReadonly_();
    }


}