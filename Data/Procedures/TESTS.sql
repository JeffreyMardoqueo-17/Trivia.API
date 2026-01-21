

contrase�a 
ecd71870d1963316a97e3ac3408c9835ad8cf0f3c1bc703527c30265534f75ae

--------------------PRUEBA DE REGISTRO 

DECLARE 
    @Exists BIT,
    @ResponseMessage VARCHAR(100);

EXEC SP_CreateUser
    @Gmail = 'jeffreymardoqueo09@gmail.com', ------------------------------------------SHA256 solo para probar el sp
    @PasswordHash = 'ecd71870d1963316a97e3ac3408c9835ad8cf0f3c1bc703527c30265534f75ae', ----test123
    @PasswordSalt = '1bc1a361f17092bc7af4b2f82bf9194ea9ee2ca49eb2e53e39f555bc1eeaed74', --salttest
    @Exists = @Exists OUTPUT,
    @ResponseMessage = @ResponseMessage OUTPUT;

SELECT 
    @Exists AS [Exist],
    @ResponseMessage AS ResponseMessage;


    SELECT * FROM Users

-----------------------PRUEBA DE LOGIN 

EXEC SP_LoginUser
    @Gmail = 'jeffreymardoqueo09@gmail.com';
------------------------PARA PROBAR SI FUCNIOAN EL DE ACTUAALIZAR 
DECLARE @Success BIT, @Response NVARCHAR(100);

EXEC SP_UpdateUserPassword
    @Gmail = 'jeffreymardoqueo09@gmail.com',
    @NewPasswordHash = 'nuevo_hash_ejemplo',
    @NewPasswordSalt = 'nuevo_salt_ejemplo',
    @Success = @Success OUTPUT,
    @ResponseMessage = @Response OUTPUT;

SELECT @Success AS Success, @Response AS ResponseMessage;





--------------------Prueba de elimianr soft delete 
DECLARE @Success BIT,
        @Response NVARCHAR(100);

-- Supongamos que el Id del usuario es 1
EXEC dbo.SP_DeactivateUser
    @UserId = 1,
    @Success = @Success OUTPUT,
    @ResponseMessage = @Response OUTPUT;

SELECT @Success AS Success, @Response AS ResponseMessage;





EXEC SP_GetUserGameHistory @UserId = 1

UPDATE GameSessions 
SET EndedAt = GETDATE() WHERE Id = 19


EXEC SP_GetNextQuestion @GameSessionId = 14
