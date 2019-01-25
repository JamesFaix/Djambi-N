CREATE TABLE [dbo].[Events]
(
    [EventId]            INT NOT NULL IDENTITY(1, 1),
    [GameId]             INT NOT NULL,
    [CreatedByPlayerId]    INT NOT NULL,
    [CreatedOn]          DATETIME2 (7) NOT NULL,
    [EventKindId]        TINYINT NOT NULL,
    CONSTRAINT [PK_Events]              PRIMARY KEY CLUSTERED  ([EventId]),
    CONSTRAINT [FK_Events_GameId] FOREIGN KEY ([GameId]) REFERENCES [dbo].[Games] ([GameId]),
    CONSTRAINT [FK_Events_CreatedByPlayerId] FOREIGN KEY ([CreatedByPlayerId]) REFERENCES [dbo].[Players] ([PlayerId]),
    CONSTRAINT [FK_Events_EventKindId] FOREIGN KEY ([EventKindId]) REFERENCES [dbo].[EventKinds] ([EventKindId])
)