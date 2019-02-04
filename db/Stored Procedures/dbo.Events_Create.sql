CREATE PROCEDURE [dbo].[Events_Create]
    @GameId INT,
    @CreatedByUserId INT,
    @ActingPlayerId INT,
    @EventKindId TINYINT,
    @EffectsJson NVARCHAR(MAX)
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO Events (
        GameId,
        CreatedByUserId,
        ActingPlayerId,
        CreatedOn,
        EventKindId,
        EffectsJson)
	VALUES (
		@GameId,
        @CreatedByUserId,
        @ActingPlayerId,
        GETUTCDATE(),
        @EventKindId,
        @EffectsJson)

	SELECT SCOPE_IDENTITY()
END