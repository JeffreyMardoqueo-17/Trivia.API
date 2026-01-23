CREATE PROCEDURE SP_CreatePasswordReset
    @Gmail NVARCHAR(100),
    @Code NVARCHAR(10),
    @ExpiresAt DATETIME,
    @Success BIT OUTPUT,
    @ResponseMessage NVARCHAR(100) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @UserId INT;

    SELECT @UserId = Id
    FROM Users
    WHERE Gmail = @Gmail AND IsActive = 1;

    IF @UserId IS NULL
    BEGIN
        SET @Success = 0;
        SET @ResponseMessage = 'Usuario no encontrado';
        RETURN;
    END

    INSERT INTO PasswordResets (UserId, Code, ExpiresAt)
    VALUES (@UserId, @Code, @ExpiresAt);

    SET @Success = 1;
    SET @ResponseMessage = 'Código generado';
END
GO

CREATE PROCEDURE SP_ValidatePasswordReset
    @Gmail NVARCHAR(100),
    @Code NVARCHAR(10),
    @UserId INT OUTPUT,
    @Success BIT OUTPUT,
    @ResponseMessage NVARCHAR(100) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1 @UserId = pr.UserId
    FROM PasswordResets pr
    INNER JOIN Users u ON u.Id = pr.UserId
    WHERE u.Gmail = @Gmail
      AND pr.Code = @Code
      AND pr.IsUsed = 0
      AND pr.ExpiresAt > GETUTCDATE();

    IF @UserId IS NULL
    BEGIN
        SET @Success = 0;
        SET @ResponseMessage = 'Código inválido o expirado';
        RETURN;
    END

    SET @Success = 1;
    SET @ResponseMessage = 'Código válido';
END
GO

CREATE PROCEDURE SP_InvalidatePasswordReset
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE PasswordResets
    SET IsUsed = 1
    WHERE UserId = @UserId;
END

