CREATE TABLE [dbo].[MetaSourceCode] (
    [Id]                    INT           IDENTITY (1, 1) NOT NULL,
    [Name]                  VARCHAR (100) NOT NULL,
    [AssemblyQualifiedName] VARCHAR (300) NOT NULL,
    [CodeBase]              VARCHAR (500) NOT NULL,
    [SourceCode]            VARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_MetaSourceCode] PRIMARY KEY CLUSTERED ([Id] ASC)
);

