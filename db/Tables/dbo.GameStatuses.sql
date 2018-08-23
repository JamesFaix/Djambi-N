CREATE TABLE [dbo].[GameStatuses]
(
[GameStatusId] [int] NOT NULL,
[Name] [nvarchar] (20) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[GameStatuses] ADD CONSTRAINT [PK_GameStatuses] PRIMARY KEY CLUSTERED  ([GameStatusId]) ON [PRIMARY]
GO
ALTER TABLE [dbo].[GameStatuses] ADD CONSTRAINT [UQ__GameStat__737584F6D7969358] UNIQUE NONCLUSTERED  ([Name]) ON [PRIMARY]
GO
