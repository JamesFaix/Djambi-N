CREATE PROCEDURE [Lobby].[AddVirtualPlayerToGame] 
	@GameId INT,
	@Name NVARCHAR(50),
	@PlayerId INT OUTPUT
AS
BEGIN
	SET NOCOUNT ON;

	IF EXISTS(SELECT 1 FROM Players WHERE GameId = @GameId AND Name = @Name)
		THROW 50000, 'Duplicate player', 1

    IF (SELECT COUNT(1) FROM Players WHERE GameId = @GameId) 
     = (SELECT BoardRegionCount FROM Games WHERE GameId = @GameId)
		THROW 50000, 'Max player count reached', 1
                      
    IF (SELECT GameStatusId FROM Games WHERE GameId = @GameId) <> 1
	    THROW 50000, 'Game no longer open', 1

    INSERT INTO Players (
		GameId, 
		UserId, 
		[Name])
    VALUES (
		@GameId, 
		NULL, 
		@Name)

	SET @PlayerId = SCOPE_IDENTITY()
	
END