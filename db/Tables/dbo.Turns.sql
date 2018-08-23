CREATE TABLE [dbo].[Turns](
	[GameId] [int] NOT NULL,
	[PlayerId] [int] NOT NULL,
	[TurnId] [int] IDENTITY(1,1) NOT NULL,
	[CreatedOn] [datetime2](7) NOT NULL,
	[SelectionsJson] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_Turns] PRIMARY KEY CLUSTERED 
(
	[GameId] ASC,
	[TurnId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[Turns]  WITH CHECK ADD  CONSTRAINT [FK_Turns_PlayerId] FOREIGN KEY([GameId], [PlayerId])
REFERENCES [dbo].[Players] ([GameId], [PlayerId])
GO

ALTER TABLE [dbo].[Turns] CHECK CONSTRAINT [FK_Turns_PlayerId]
GO
