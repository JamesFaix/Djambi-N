CREATE PROCEDURE [dbo].[Events_Create]
    @GameId INT,
    @CreatedByUserId INT,
    @EventKindId TINYINT,
    @EffectsJson NVARCHAR(MAX)
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO Events (
        GameId,
        CreatedByUserId,
        CreatedOn,
        EventKindId,
        EffectsJson)
	VALUES (
		@GameId,
        @CreatedByUserId,
        GETUTCDATE(),
        @EventKindId,
        @EffectsJson)

	SELECT SCOPE_IDENTITY()
END