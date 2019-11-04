CREATE TABLE [dbo].[StackBehaviour] (
    [Id]          INT           IDENTITY (1, 1) NOT NULL,
    [Name]        VARCHAR (100) NOT NULL,
    [Value]       INT           NOT NULL,
    [Description] VARCHAR (500) NOT NULL,
    [PopCount] INT NOT NULL DEFAULT 0, 
    [PopType0] VARCHAR(3) NOT NULL DEFAULT '', 
    [PopType1] VARCHAR(3) NOT NULL DEFAULT '', 
    [PopType2] VARCHAR(3) NOT NULL DEFAULT '', 
    [PushCount] INT NOT NULL DEFAULT 0, 
    [PushType0] VARCHAR(3) NOT NULL DEFAULT '', 
    [PushType1] VARCHAR(3) NOT NULL DEFAULT '', 
    CONSTRAINT [PK_StackBehaviour] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UN_StackBehaviour_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);

