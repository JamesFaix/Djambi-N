CREATE TABLE [dbo].[Privileges]
(
    [PrivilegeId] TINYINT NOT NULL,
    [Name] NVARCHAR(20) NOT NULL,
    CONSTRAINT [PK_Privileges] PRIMARY KEY CLUSTERED ([PrivilegeId]),
    CONSTRAINT [UQ_Privileges, Name] UNIQUE NONCLUSTERED ([Name])
)