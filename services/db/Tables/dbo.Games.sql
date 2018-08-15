CREATE TABLE [dbo].[Games]
(
[GameId] [int] NOT NULL IDENTITY(1, 1),
[GameStatusId] [int] NOT NULL,
[Description] [nvarchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[BoardRegionCount] [int] NOT NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Games] ADD CONSTRAINT [PK__Games__2AB897FD84BFC23C] PRIMARY KEY CLUSTERED  ([GameId]) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Games] ADD CONSTRAINT [FK_Games_GameStatusId] FOREIGN KEY ([GameStatusId]) REFERENCES [dbo].[GameStatuses] ([GameStatusId])
GO
