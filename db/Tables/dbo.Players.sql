CREATE TABLE [dbo].[Players]
(
    [PlayerId] INT NOT NULL IDENTITY(1, 1),
    [LobbyId]       INT NOT NULL,
    [UserId]        INT NULL,
    [PlayerTypeId]  TINYINT NOT NULL,
    [Name]          NVARCHAR (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
    CONSTRAINT [PK_Players] PRIMARY KEY CLUSTERED  ([PlayerId]),
    CONSTRAINT [UQ_Players_Name] UNIQUE NONCLUSTERED  ([LobbyId], [Name]),
    CONSTRAINT [FK_Players_LobbyId] FOREIGN KEY ([LobbyId]) REFERENCES [dbo].[Lobbies] ([LobbyId]),
    CONSTRAINT [FK_Players_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId]),
    CONSTRAINT [FK_Players_PlayerTypeId] FOREIGN KEY ([PlayerTypeId]) REFERENCES [dbo].[PlayerTypes] ([PlayerTypeId])
)