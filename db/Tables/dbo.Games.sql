CREATE TABLE [dbo].[Games]
(
[GameId] [int] NOT NULL IDENTITY(1, 1),
[GameStatusId] [tinyint] NOT NULL,
[Description] [nvarchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[BoardRegionCount] [tinyint] NOT NULL,
[CreatedOn] [datetime2] NOT NULL,
[StartingConditionsJson] [nvarchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[CurrentGameStateJson] [nvarchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[CurrentTurnStateJson] [nvarchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[Games] ADD CONSTRAINT [PK_Games] PRIMARY KEY CLUSTERED  ([GameId]) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Games] ADD CONSTRAINT [FK_Games_GameStatusId] FOREIGN KEY ([GameStatusId]) REFERENCES [dbo].[GameStatuses] ([GameStatusId])
GO

ALTER TABLE [dbo].[Games] CHECK CONSTRAINT [FK_Games_GameStatusId]
GO
