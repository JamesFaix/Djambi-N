CREATE TABLE [dbo].[PlayerKinds]
(
    [PlayerKindId] TINYINT NOT NULL,
    [Name] NVARCHAR(20) NOT NULL,
    CONSTRAINT [PK_PlayerKinds] PRIMARY KEY CLUSTERED ([PlayerKindId]),
    CONSTRAINT [UQ_PlayerKinds, Name] UNIQUE NONCLUSTERED ([Name])
)