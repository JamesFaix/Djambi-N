CREATE TABLE [dbo].[Games]
(
    [GameId]             INT NOT NULL IDENTITY(1, 1),
    [CreatedByUserId]    INT NOT NULL,
    [CreatedOn]          DATETIME2 (7) NOT NULL,
    [GameStatusId]       TINYINT NOT NULL,
    --Game parameters
    [Description]        NVARCHAR (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    [RegionCount]        TINYINT NOT NULL,
    [AllowGuests]        BIT NOT NULL,
    [IsPublic]           BIT NOT NULL,
    [TurnCycleJson]      NVARCHAR (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    [PiecesJson]         NVARCHAR (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    [CurrentTurnJson]    NVARCHAR (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    CONSTRAINT [PK_Games]              PRIMARY KEY CLUSTERED  ([GameId]),
    CONSTRAINT [FK_Games_GameStatusId] FOREIGN KEY ([GameStatusId]) REFERENCES [dbo].[GameStatuses] ([GameStatusId]),
    CONSTRAINT [FK_Games_CreatedByUserId] FOREIGN KEY ([CreatedByUserId]) REFERENCES [dbo].[Users] ([UserId])
)