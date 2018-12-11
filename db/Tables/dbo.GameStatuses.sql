CREATE TABLE [dbo].[GameStatuses]
(
    [GameStatusId] [tinyint] NOT NULL,
    [Name]         [nvarchar] (20) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
    CONSTRAINT [PK_GameStatuses] PRIMARY KEY CLUSTERED  ([GameStatusId]),
    CONSTRAINT [UQ_GameStatuses_Name] UNIQUE NONCLUSTERED  ([Name])
)