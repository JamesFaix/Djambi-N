CREATE TABLE [dbo].[Games]
(
    [GameId]                 [int] NOT NULL IDENTITY(1, 1),
    [GameStatusId]           [tinyint] NOT NULL,
    [Description]            [nvarchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    [BoardRegionCount]       [tinyint] NOT NULL,
    [CreatedOn]              [datetime2] NOT NULL,
    [CreatedByUserId]        [int] NOT NULL,
    [StartingConditionsJson] [nvarchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    [CurrentGameStateJson]   [nvarchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    [CurrentTurnStateJson]   [nvarchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    CONSTRAINT [PK_Games]              PRIMARY KEY CLUSTERED  ([GameId]),
    CONSTRAINT [FK_Games_GameStatusId] FOREIGN KEY ([GameStatusId]) REFERENCES [dbo].[GameStatuses] ([GameStatusId]),
    CONSTRAINT [FK_Games_CreatedByUserId] FOREIGN KEY ([CreatedByUserId]) REFERENCES [dbo].[Users] ([UserId])
)