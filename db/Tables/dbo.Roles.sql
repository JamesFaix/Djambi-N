CREATE TABLE [dbo].[Roles]
(
    [RoleId] [tinyint] NOT NULL,
    [Name]   [nchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
    CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED ([RoleId]),
    CONSTRAINT [UQ_Roles_Name] UNIQUE   ([Name])
)