CREATE TABLE [dbo].[PlayerTypes]
(
    [PlayerTypeId] TINYINT NOT NULL,
    [Name] NVARCHAR(20) NOT NULL,
    CONSTRAINT [PK_PlayerTypes] PRIMARY KEY CLUSTERED ([PlayerTypeId]),
    CONSTRAINT [UQ_PlayerTypes, Name] UNIQUE NONCLUSTERED ([Name]) 
)