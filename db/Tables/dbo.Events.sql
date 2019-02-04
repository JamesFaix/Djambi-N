CREATE TABLE [dbo].[Events]
(
    [EventId]            INT NOT NULL IDENTITY(1, 1),
    [GameId]             INT NOT NULL,
    [CreatedByUserId]    INT NOT NULL,
    [ActingPlayerId]     INT NULL,
    [CreatedOn]          DATETIME2 (7) NOT NULL,
    [EventKindId]        TINYINT NOT NULL,
    [EffectsJson]        NVARCHAR(MAX) NOT NULL
    CONSTRAINT [PK_Events]              PRIMARY KEY CLUSTERED  ([EventId]),
    CONSTRAINT [FK_Events_GameId] FOREIGN KEY ([GameId]) REFERENCES [dbo].[Games] ([GameId]),
    CONSTRAINT [FK_Events_CreatedByUserId] FOREIGN KEY ([CreatedByUserId]) REFERENCES [dbo].[Users] ([UserId]),
    CONSTRAINT [FK_Events_ActingPlayerId] FOREIGN KEY ([GameId], [ActingPlayerId]) REFERENCES [dbo].[Players] ([GameId], [PlayerId]),
    CONSTRAINT [FK_Events_EventKindId] FOREIGN KEY ([EventKindId]) REFERENCES [dbo].[EventKinds] ([EventKindId])
)