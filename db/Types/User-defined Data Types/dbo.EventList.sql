CREATE TYPE [dbo].[EventList] AS TABLE
(
[CreatedByUserId] INT NOT NULL,
[ActingPlayerId] INT NULL,
[CreatedOn] DATETIME2 NOT NULL,
EventKindId TINYINT NOT NULL,
EffectsJson NVARCHAR(MAX) NOT NULL
)
GO
