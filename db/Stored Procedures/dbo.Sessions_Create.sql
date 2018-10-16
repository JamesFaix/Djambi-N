CREATE PROCEDURE [dbo].[Sessions_Create]
	@UserId INT,
	@Token NVARCHAR(50),
	@ExpiresOn DATETIME2
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO [Sessions] (
		Token, 
		UserId,
		CreatedOn, 
		ExpiresOn)
	VALUES (
		@Token,
		@UserId,
		GETUTCDATE(),
		@ExpiresOn)

	SELECT SCOPE_IDENTITY()
END