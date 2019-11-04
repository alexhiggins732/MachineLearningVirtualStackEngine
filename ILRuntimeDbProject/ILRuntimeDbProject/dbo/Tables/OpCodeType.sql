CREATE TABLE [dbo].[OpCodeType] (
    [Id]          INT           IDENTITY (1, 1) NOT NULL,
    [Name]        VARCHAR (100) NOT NULL,
    [Value]       INT           NOT NULL,
    [Description] VARCHAR (500) NOT NULL,
    CONSTRAINT [PK_OpCodeType] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UN_OpCodeType_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);

