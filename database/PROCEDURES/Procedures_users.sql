----------USER
--  Id INT PRIMARY KEY IDENTITY(1,1),
--  Gmail VARCHAR(100) NOT NULL,
--  PasswordHash VARCHAR(500) NOT NULL,
--  PasswordSalt VARCHAR(500) NOT NULL,

CREATE PROC SP_CreateUser
(
    @Gmail VARCHAR(100),
    @PasswordHash VARCHAR(500),
    @PasswordSalt VARCHAR(500),
    @Exists BIT OUTPUT,
    @ResponseMessage VARCHAR(100) OUTPUT 
)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        IF EXISTS (SELECT 1 FROM Users WHERE Gmail = @Gmail) -----solo quiero sber si existe no el dato
        BEGIN
            SET @Exists = 1;
            SET @ResponseMessage = 'El usuario ya existe';
            RETURN;
        END

        INSERT INTO Users (Gmail, PasswordHash, PasswordSalt)
        VALUES (@Gmail, @PasswordHash, @PasswordSalt);

        SET @Exists = 0;
        SET @ResponseMessage = 'Usuario creado exitosamente';
    END TRY
    BEGIN CATCH
        ----violacion al unique, violacion al indice unique (son dopulicado)
        IF ERROR_NUMBER() = 2627 OR ERROR_NUMBER() = 2601
        BEGIN
            SET @Exists = 1;
            SET @ResponseMessage = 'El usuario ya existe';
            RETURN;
        END;
    END CATCH
END
GO


--------------SP PARA CAMBIAR CONTRASEÑA
CREATE PROC SP_UpdateUserPassword
(
    @Gmail NVARCHAR(100),
    @NewPasswordHash VARCHAR(500),
    @NewPasswordSalt VARCHAR(500),
    @Success BIT OUTPUT,
    @ResponseMessage VARCHAR(100) OUTPUT
)
AS
BEGIN
    SET NOCOUNT ON;

    -- Verificar que el usuario exista
    IF NOT EXISTS (SELECT 1 FROM Users WHERE Gmail = @Gmail)
    BEGIN
        SET @Success = 0;
        SET @ResponseMessage = 'El usuario no existe';
        RETURN;
    END

    ---- Actualizar contraseña
    UPDATE Users
    SET 
        PasswordHash = @NewPasswordHash,
        PasswordSalt = @NewPasswordSalt
    WHERE Gmail = @Gmail;

    SET @Success = 1;
    SET @ResponseMessage = 'Contraseña actualizada correctamente';
END
GO

CREATE OR ALTER PROC SP_LoginUser
(
    @Gmail NVARCHAR(100)
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Id,
        Gmail,
        PasswordHash,
        PasswordSalt,
        IsActive,
        CreatedAt
    FROM Users
    WHERE Gmail = @Gmail;
END
GO




-----------SP PARA ELIMINAR PERO TIPO SOFT no el HARD 
CREATE PROC SP_DeactivateUser
(
    @UserId INT,
    @Success BIT OUTPUT,
    @ResponseMessage NVARCHAR(100) OUTPUT
)
AS
BEGIN
    SET NOCOUNT ON;

    ------- Verifica que el usuario exista y esté activo
    IF NOT EXISTS (
        SELECT 1 
        FROM Users
        WHERE Id = @UserId
          AND IsActive = 1
    )
    BEGIN
        SET @Success = 0;
        SET @ResponseMessage = 'Usuario no existe o ya está inactivo';
        RETURN;
    END

    ----------- Desactiva al usuario
    UPDATE Users
    SET IsActive = 0
    WHERE Id = @UserId;

    SET @Success = 1;
    SET @ResponseMessage = 'Usuario desactivado correctamente';
END
GO



DROP  PROCEDURE SP_DeactivateUser