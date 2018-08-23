CREATE TABLE [dbo].[Messages](
	[MessageId] [int] IDENTITY(1,1) NOT NULL,
	[GameId] [int] NOT NULL,
	[Body] [nvarchar](500) NOT NULL,
	[CreatedOn] [datetime2](7) NOT NULL,
	[UserId] [int] NULL,
 CONSTRAINT [PK_Messages] PRIMARY KEY CLUSTERED 
(
	[GameId] ASC,
	[MessageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Messages]  WITH CHECK ADD  CONSTRAINT [FK_Messages_GameId] FOREIGN KEY([GameId])
REFERENCES [dbo].[Games] ([GameId])
GO

ALTER TABLE [dbo].[Messages] CHECK CONSTRAINT [FK_Messages_GameId]
GO

ALTER TABLE [dbo].[Messages]  WITH CHECK ADD  CONSTRAINT [FK_Messages_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO

ALTER TABLE [dbo].[Messages] CHECK CONSTRAINT [FK_Messages_UserId]
GO

