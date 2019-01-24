CREATE TABLE [dbo].[PlayerStatuses]
(
    [PlayerStatusId] [tinyint] NOT NULL,
    [Name]         [nvarchar] (20) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
    CONSTRAINT [PK_PlayerStatuses] PRIMARY KEY CLUSTERED  ([PlayerStatusId]),
    CONSTRAINT [UQ_PlayerStatuses_Name] UNIQUE NONCLUSTERED  ([Name])
)