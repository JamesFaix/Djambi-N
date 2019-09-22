SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[VLatestEvents]
AS

WITH LatestEvents AS
(
	SELECT e.EventID,
		e.GameID,
		e.CreatedByUserId,
		e.CreatedOn,
		ROW_NUMBER() OVER(ORDER BY e.CreatedOn DESC) AS [Row]
	FROM [Events] e
)

SELECT e.EventID,
	e.GameID,
	e.CreatedByUserId,
	e.CreatedOn
FROM LatestEvents e
WHERE e.[Row] = 1

GO