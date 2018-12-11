CREATE TABLE [dbo].[NeutralPlayerNames]
(
    [Name] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
    CONSTRAINT [PK_NeutralPlayerNames] PRIMARY KEY CLUSTERED ([Name])
)