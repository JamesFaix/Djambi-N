CREATE PROCEDURE [dbo].[Snapshots_Create]
    @GameId INT,
    @CreatedByUserId INT,
    @Description NVARCHAR(MAX),
    @SnapshotJson NVARCHAR(MAX)
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO Snapshots (
        GameId,
        CreatedByUserId,
        CreatedOn,
        Description,
        SnapshotJson)
	VALUES (
		@GameId,
        @CreatedByUserId,
        GETUTCDATE(),
        @Description,
        @SnapshotJson)

	SELECT SCOPE_IDENTITY()
END