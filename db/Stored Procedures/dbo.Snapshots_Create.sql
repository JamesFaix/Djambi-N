CREATE PROCEDURE [dbo].[Snapshots_Create]
    @GameId INT,
    @Description NVARCHAR(MAX),
    @SnapshotJson NVARCHAR(MAX)
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO Snapshots (
        GameId,
        Description,
        SnapshotJson)
	VALUES (
		@GameId,
        @Description,
        @SnapshotJson)

	SELECT SCOPE_IDENTITY()
END