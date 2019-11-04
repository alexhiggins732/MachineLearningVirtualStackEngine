CREATE TABLE [dbo].[OpCode] (
    [Id]                   INT           IDENTITY (1, 1) NOT NULL,
    [ClrName]              VARCHAR (100) NOT NULL,
    [Name]                 VARCHAR (100) NOT NULL,
    [Value]                INT           NOT NULL,
    [Size]                 INT           NOT NULL,
    [FlowControlId]        INT           NOT NULL,
    [OpCodeTypeId]         INT           NOT NULL,
    [OperandTypeId]        INT           NOT NULL,
    [StackBehaviourPopId]  INT           NOT NULL,
    [StackBehaviourPushId] INT           NOT NULL,
    [Description]          VARCHAR (500) NOT NULL,
    CONSTRAINT [PK_OpCode] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_OpCode_FlowControlId] FOREIGN KEY ([FlowControlId]) REFERENCES [dbo].[FlowControl] ([Id]),
    CONSTRAINT [FK_OpCode_OpCodeTypeId] FOREIGN KEY ([OpCodeTypeId]) REFERENCES [dbo].[OpCodeType] ([Id]),
    CONSTRAINT [FK_OpCode_OperandTypeId] FOREIGN KEY ([OperandTypeId]) REFERENCES [dbo].[OperandType] ([Id]),
    CONSTRAINT [FK_OpCode_StackBehaviourPopId] FOREIGN KEY ([StackBehaviourPopId]) REFERENCES [dbo].[StackBehaviour] ([Id]),
    CONSTRAINT [FK_OpCode_StackBehaviourPushId] FOREIGN KEY ([StackBehaviourPushId]) REFERENCES [dbo].[StackBehaviour] ([Id]),
    CONSTRAINT [UN_OpCode_CLRName] UNIQUE NONCLUSTERED ([ClrName] ASC),
    CONSTRAINT [UN_OpCode_Name] UNIQUE NONCLUSTERED ([Name] ASC),
    CONSTRAINT [UN_OpCode_Value] UNIQUE NONCLUSTERED ([Value] ASC)
);

