CREATE PROC SP_GetCategories
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, Name
    FROM Categories
    WHERE IsActive = 1;
END
GO

CREATE PROC SP_GetCategoryById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, Name
    FROM Categories
    WHERE Id = @Id AND IsActive = 1;
END