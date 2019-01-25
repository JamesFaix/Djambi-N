CREATE PROCEDURE [dbo].[Events_Create]
    @GameId INT,
    @CreatedByPlayerId INT,
    @EventKindId TINYINT,
    @EffectsJson NVARCHAR(MAX)
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO Events (
        GameId,
        CreatedByPlayerId,
        CreatedOn,
        EventKindId,
        EffectsJson)
	VALUES (
		@GameId,
        @CreatedByPlayerId,
        GETUTCDATE(),
        @EventKindId,
        @EffectsJson)

	SELECT SCOPE_IDENTITY()
END