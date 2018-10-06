CREATE TABLE [dbo].[Players]
(
    [PlayerId] [int] NOT NULL IDENTITY(1, 1),
    [GameId]   [int] NOT NULL,
    [UserId]   [int] NULL,
    [Name]     [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
    CONSTRAINT [PK_Players] PRIMARY KEY CLUSTERED  ([GameId], [PlayerId]),
    CONSTRAINT [UQ_Players_Name] UNIQUE NONCLUSTERED  ([GameId], [Name]),
    CONSTRAINT [FK_Players_GameId] FOREIGN KEY ([GameId]) REFERENCES [dbo].[Games] ([GameId]),
    CONSTRAINT [FK_Players_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId])
)