CREATE TABLE [dbo].[Lobbies] (
    [LobbyId]            INT IDENTITY (1, 1) NOT NULL,
    [Description]        NVARCHAR (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    [RegionCount]        TINYINT NOT NULL,
    [CreatedByUserId]    INT NOT NULL,
    [CreatedOn]          DATETIME2 (7) NOT NULL,
    [AllowGuests]        BIT NOT NULL,
    [IsPublic]           BIT NOT NULL,
    CONSTRAINT [PK_Lobbies] PRIMARY KEY CLUSTERED ([LobbyId] ASC),
    CONSTRAINT [FK_Lobbies_CreatedByUserId] FOREIGN KEY ([CreatedByUserId]) REFERENCES [dbo].[Users] ([UserId])
);