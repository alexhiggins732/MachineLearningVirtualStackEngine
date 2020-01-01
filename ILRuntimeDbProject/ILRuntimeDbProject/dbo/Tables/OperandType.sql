CREATE TABLE [dbo].[OperandType] (
    [Id]              INT           IDENTITY (1, 1) NOT NULL,
    [Name]            VARCHAR (100) NOT NULL,
    [Value]           INT           NOT NULL,
    [Description]     VARCHAR (500) NOT NULL,
    [ByteSize]        INT           CONSTRAINT [DF_OperandType_ByteSize] DEFAULT ((0)) NOT NULL,
    [BitSize]         INT           CONSTRAINT [DF_OperandType_BitSize] DEFAULT ((0)) NOT NULL,
    [IsFloatingPoint] BIT           CONSTRAINT [DF_OperandType_IsFloatingPoint] DEFAULT ((0)) NOT NULL,
    [SystemType]      VARCHAR (20)  CONSTRAINT [DF_OperandType_SystemType] DEFAULT ('') NOT NULL,
    CONSTRAINT [PK_OperandType] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UN_OperandType_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);

