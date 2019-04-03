SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
CREATE PROCEDURE [dbo].[Snapshots_Get]
	@GameId INT,
    @SnapshotId INT
AS
BEGIN
	SET NOCOUNT ON;

    IF @SnapshotId IS NOT NULL
        SELECT SnapshotId,
            s.GameId,
            s.CreatedByUserId,
            u.Name as CreatedByUserName,
            s.CreatedOn,
            s.Description,
            s.SnapshotJson
        FROM Snapshots s
            INNER JOIN Users u
                ON s.CreatedByUserId = u.UserId
        WHERE SnapshotId = @SnapshotId
    ELSE IF @GameId IS NOT NULL
        SELECT s.SnapshotId,
            s.GameId,
            s.CreatedByUserId,
            u.Name as CreatedByUserName,
            s.CreatedOn,
            s.Description,
            s.SnapshotJson
        FROM Snapshots s
            INNER JOIN Users u
                ON s.CreatedByUserId = u.UserId
        WHERE GameId = @GameId
    ELSE
		THROW 50500, 'Invalid parameters for snapshot query.', 1
END
GO
