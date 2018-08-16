CREATE TABLE [dbo].[Games](
	[GameId] [int] IDENTITY(1,1) NOT NULL,
	[GameStatusId] [int] NOT NULL,
	[Description] [nvarchar](100) NULL,
	[BoardRegionCount] [int] NOT NULL,
	[CreatedOn] [datetime2](7) NOT NULL,
	[StartingConditionsJson] [nvarchar](max) NULL,
	[CurrentStateJson] [nvarchar](max) NULL,
 CONSTRAINT [PK__Games__2AB897FD84BFC23C] PRIMARY KEY CLUSTERED 
(
	[GameId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[Games]  WITH CHECK ADD  CONSTRAINT [FK_Games_GameStatusId] FOREIGN KEY([GameStatusId])
REFERENCES [dbo].[GameStatuses] ([GameStatusId])
GO

ALTER TABLE [dbo].[Games] CHECK CONSTRAINT [FK_Games_GameStatusId]
GO
