CREATE TABLE [dbo].[GameStatuses]
(
[GameStatusId] [tinyint] NOT NULL,
[Name] [nvarchar] (20) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[GameStatuses] ADD CONSTRAINT [PK_GameStatuses] PRIMARY KEY CLUSTERED  ([GameStatusId]) ON [PRIMARY]
GO
ALTER TABLE [dbo].[GameStatuses] ADD CONSTRAINT [UQ_GameStatuses_Name] UNIQUE NONCLUSTERED  ([Name]) ON [PRIMARY]
GO
