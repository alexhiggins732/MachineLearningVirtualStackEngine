CREATE TABLE [dbo].[FlowControl] (
    [Id]          INT           IDENTITY (1, 1) NOT NULL,
    [Name]        VARCHAR (100) NOT NULL,
    [Value]       INT           NOT NULL,
    [Description] VARCHAR (500) NOT NULL,
    CONSTRAINT [PK_FlowControl] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UN_FlowControl_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);

