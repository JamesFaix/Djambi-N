CREATE TABLE [dbo].[Players]
(
    [PlayerId]           INT NOT NULL IDENTITY(1, 1),
    [GameId]             INT NOT NULL,
    [UserId]             INT NULL,
    [PlayerKindId]       TINYINT NOT NULL,
    [Name]               NVARCHAR (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
    [PlayerStatusId]     TINYINT,
    [ColorId]            TINYINT NULL,
    [StartingRegion]     TINYINT NULL,
    [StartingTurnNumber] TINYINT NULL,
    CONSTRAINT [PK_Players] PRIMARY KEY CLUSTERED  ([GameId], [PlayerId]),
    CONSTRAINT [UQ_Players_Name] UNIQUE NONCLUSTERED  ([GameId], [Name]),
    CONSTRAINT [FK_Players_GameyId] FOREIGN KEY ([GameId]) REFERENCES [dbo].[Games] ([GameId]),
    CONSTRAINT [FK_Players_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId]),
    CONSTRAINT [FK_Players_PlayerStatusId] FOREIGN KEY ([PlayerStatusId]) REFERENCES [dbo].[PlayerStatuses] ([PlayerStatusId]),
    CONSTRAINT [FK_Players_PlayerKindId] FOREIGN KEY ([PlayerKindId]) REFERENCES [dbo].[PlayerKinds] ([PlayerKindId])
)