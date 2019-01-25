CREATE PROCEDURE [dbo].[Events_Get]
    @GameId INT
AS
BEGIN
	SET NOCOUNT ON;

    SELECT
        EventId,
        GameId,
        CreatedByPlayerid,
        CreatedOn,
        EventKindId,
        EffectsJson
    FROM Events
    WHERE GameId = @GameId
END