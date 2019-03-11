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
        SELECT SnapshotId, GameId, Description, SnapshotJson
        FROM Snapshots
        WHERE SnapshotId = @SnapshotId
    ELSE IF @GameId IS NOT NULL
        SELECT SnapshotId, GameId, Description, SnapshotJson
        FROM Snapshots
        WHERE GameId = @GameId
    ELSE
		THROW 50500, 'Invalid parameters for snapshot query.', 1
END
GO
