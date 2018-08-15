CREATE TABLE [dbo].[Players]
(
[PlayerId] [int] NOT NULL IDENTITY(1, 1),
[GameId] [int] NOT NULL,
[UserId] [int] NULL,
[Name] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Players] ADD CONSTRAINT [PK_Players] PRIMARY KEY CLUSTERED  ([GameId], [PlayerId]) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Players] ADD CONSTRAINT [UQ__Players__737584F6BFBF2D0F] UNIQUE NONCLUSTERED  ([Name]) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Players] ADD CONSTRAINT [FK_Players_GameId] FOREIGN KEY ([GameId]) REFERENCES [dbo].[Games] ([GameId])
GO
ALTER TABLE [dbo].[Players] ADD CONSTRAINT [FK_Players_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId])
GO
