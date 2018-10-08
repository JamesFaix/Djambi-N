CREATE TABLE [dbo].[Messages](
	[MessageId] [int] IDENTITY(1,1) NOT NULL,
	[GameId]    [int] NOT NULL,
	[Body]      [nvarchar](500) NOT NULL,
	[CreatedOn] [datetime2](7) NOT NULL,
	[UserId]    [int] NULL,
    CONSTRAINT [PK_Messages] PRIMARY KEY CLUSTERED ([GameId] ASC, [MessageId] ASC),
	CONSTRAINT [FK_Messages_GameId] FOREIGN KEY([GameId]) REFERENCES [dbo].[Games] ([GameId]),
	CONSTRAINT [FK_Messages_UserId] FOREIGN KEY([UserId]) REFERENCES [dbo].[Users] ([UserId])
)