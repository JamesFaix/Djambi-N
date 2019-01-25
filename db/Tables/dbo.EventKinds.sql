CREATE TABLE [dbo].[EventKinds]
(
    [EventKindId] TINYINT NOT NULL,
    [Name] NVARCHAR(50) NOT NULL,
    CONSTRAINT [PK_EventKinds] PRIMARY KEY CLUSTERED ([EventKindId]),
    CONSTRAINT [UQ_EventKinds, Name] UNIQUE NONCLUSTERED ([Name])
)