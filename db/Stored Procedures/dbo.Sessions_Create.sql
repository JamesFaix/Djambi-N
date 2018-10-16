CREATE PROCEDURE [dbo].[Sessions_Create]
	@UserId INT,
	@Token NVARCHAR(50),
	@ExpiresOn DATETIME2
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO Sessions (
		IsShared, 
		Token, 
		UserId,
		CreatedOn, 
		ExpiresOn)
	VALUES (
		0, 
		@Token,
		@UserId,
		GETUTCDATE(),
		@ExpiresOn)

	SELECT SCOPE_IDENTITY()
END