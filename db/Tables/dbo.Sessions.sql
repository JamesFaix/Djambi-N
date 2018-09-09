CREATE TABLE [dbo].[Sessions]
(
[SessionId] [int] NOT NULL IDENTITY(1, 1),
[UserId] [int] NOT NULL,
[Token] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[CreatedOn] [datetime2] NOT NULL,
[ExpiresOn] [datetime2] NOT NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Sessions] ADD CONSTRAINT [PK_Sessions] PRIMARY KEY CLUSTERED  ([SessionId]) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Sessions] ADD CONSTRAINT [UQ_Sessions_Token] UNIQUE NONCLUSTERED  ([Token]) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Sessions] ADD CONSTRAINT [UQ_Sessions_UserId] UNIQUE NONCLUSTERED  ([UserId]) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Sessions] ADD CONSTRAINT [FK_Sessions_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId])
GO
