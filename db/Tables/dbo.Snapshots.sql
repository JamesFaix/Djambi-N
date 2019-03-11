CREATE TABLE [dbo].[Snapshots]
(
    [SnapshotId]   INT NOT NULL IDENTITY(1, 1),
    [GameId]       INT NOT NULL,
    [Description]  NVARCHAR (MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    [SnapshotJson] NVARCHAR (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    CONSTRAINT [PK_Snapshots]       PRIMARY KEY CLUSTERED  ([SnapshotId]),
    CONSTRAINT [FK_Snapshots_Games] FOREIGN KEY ([GameId]) REFERENCES [dbo].[Games] ([GameId]),
)