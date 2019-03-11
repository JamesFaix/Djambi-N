CREATE PROCEDURE [dbo].[Snapshots_Delete]
	@SnapshotId INT
AS
BEGIN
	SET NOCOUNT ON;

	IF NOT EXISTS(SELECT 1 FROM Snapshots WHERE SnapshotId = @SnapshotId)
		THROW 50404, 'Snapshot not found.', 1

	BEGIN TRAN

		DELETE FROM Snapshots
		WHERE SnapshotId = @SnapshotId

	COMMIT
END
GO
