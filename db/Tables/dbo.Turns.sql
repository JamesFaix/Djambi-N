CREATE TABLE [dbo].[Turns]
(
	[GameId] [int] NOT NULL,
	[PlayerId] [int] NOT NULL,
	[TurnId] [int] IDENTITY(1,1) NOT NULL,
	[CreatedOn] [datetime2](7) NOT NULL,
	[SelectionsJson] [nvarchar](max) NOT NULL,
    CONSTRAINT [PK_Turns] PRIMARY KEY CLUSTERED ([GameId] ASC, [TurnId] ASC),
    CONSTRAINT [FK_Turns_PlayerId] FOREIGN KEY([GameId], [PlayerId]) REFERENCES [dbo].[Players] ([GameId], [PlayerId])
)