CREATE TABLE [dbo].[Games]
(
    [GameId]                 [int] NOT NULL IDENTITY(1, 1),
    [LobbyId]                [int] NOT NULL,
    [GameStatusId]           [tinyint] NOT NULL,
    [StartedOn]              [datetime2] NOT NULL,
    [StartingConditionsJson] [nvarchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    [GameStateJson]   [nvarchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    [TurnStateJson]   [nvarchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    CONSTRAINT [PK_Games]              PRIMARY KEY CLUSTERED  ([GameId]),
    CONSTRAINT [FK_Games_GameStatusId] FOREIGN KEY ([GameStatusId]) REFERENCES [dbo].[GameStatuses] ([GameStatusId]),
    CONSTRAINT [FK_Games_LobbyId] FOREIGN KEY ([LobbyId]) REFERENCES [dbo].[Lobbies] ([LobbyId]),    
    CONSTRAINT [UQ_Games_LobbyId] UNIQUE NONCLUSTERED ([LobbyId] ASC)
)