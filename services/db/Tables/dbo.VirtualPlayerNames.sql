CREATE TABLE [dbo].[VirtualPlayerNames]
(
[Name] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[VirtualPlayerNames] ADD CONSTRAINT [PK_VirtualPlayerNames] PRIMARY KEY CLUSTERED  ([Name]) ON [PRIMARY]
GO
