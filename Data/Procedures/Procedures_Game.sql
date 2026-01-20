
-----------------------Al dar click sobre una categoría inicia el juego 

CREATE PROC SP_StartGameSession
    @UserId INT,
    @CategoryId INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @GameSessionId INT;

    --------------  Creo la sesion
    INSERT INTO GameSessions (UserId, CategoryId)
    VALUES (@UserId, @CategoryId);

    SET @GameSessionId = SCOPE_IDENTITY();

    -------------- Selecciono 3 preguntas aleatorias de la categoria
    INSERT INTO GameSessionQuestions (GameSessionId, QuestionId)
    SELECT TOP 3 @GameSessionId, Id
    FROM Questions
    WHERE CategoryId = @CategoryId
        AND IsActive = 1
    ORDER BY NEWID();

    -------------- Retorno la sesio
    SELECT @GameSessionId AS GameSessionId;
END
GO


GO

-------------------Cada partida solo muestre 3 preguntas y 3 respuestas
CREATE PROC SP_GetGameQuestions
    @GameSessionId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        q.Id AS QuestionId,
        q.Text AS QuestionText,
        q.Points,
        gsq.TimeLimitSeconds,
        a.Id AS AnswerId,
        a.Text AS AnswerText
    FROM GameSessionQuestions gsq
    JOIN Questions q ON q.Id = gsq.QuestionId
    JOIN Answers a ON a.QuestionId = q.Id
    WHERE gsq.GameSessionId = @GameSessionId
    ORDER BY q.Id;
END
GO


-------------- GUARDAR RESPUESTA DEL USUARIO (CON TIEMPO Y PUNTOS)

CREATE PROC SP_SaveUserAnswer
    @GameSessionId INT,
    @QuestionId INT,
    @AnswerId INT,
    @TimeSpentSeconds INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE 
        @IsCorrect BIT,
        @Points INT,
        @TimeLimit INT;

    -- Tiempo permitido
    SELECT @TimeLimit = TimeLimitSeconds
    FROM GameSessionQuestions
    WHERE GameSessionId = @GameSessionId
      AND QuestionId = @QuestionId;

    -- Validar respuesta
    SELECT 
        @IsCorrect = IsCorrect
    FROM Answers
    WHERE Id = @AnswerId;

    -- Obtener puntos
    SELECT @Points = Points
    FROM Questions
    WHERE Id = @QuestionId;

    -- Regla de negocio: si se pasa del tiempo → pierde
    IF (@TimeSpentSeconds > @TimeLimit)
    BEGIN
        SET @IsCorrect = 0;
        SET @Points = 0;
    END
    ELSE IF (@IsCorrect = 0)
    BEGIN
        SET @Points = 0;
    END

    -- Guardar respuesta
    INSERT INTO UserAnswers (
        GameSessionId,
        QuestionId,
        AnswerId,
        IsCorrect,
        PointsEarned,
        TimeSpentSeconds
    )
    VALUES (
        @GameSessionId,
        @QuestionId,
        @AnswerId,
        @IsCorrect,
        @Points,
        @TimeSpentSeconds
    );

    -- Actualizar sesión
    UPDATE GameSessions
    SET 
        TotalScore = TotalScore + @Points,
        TimeSpentSeconds = TimeSpentSeconds + @TimeSpentSeconds
    WHERE Id = @GameSessionId;
END
GO



--------------PARA TERMINAR EL JUEGO
CREATE PROC SP_EndGameSession
    @GameSessionId INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE GameSessions
    SET EndedAt = GETDATE()
    WHERE Id = @GameSessionId;
END
GO


-------------Ver mis los ultimos  juegos y puntos

CREATE PROC SP_GetUserGameHistory
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Id AS GameSessionId,
        CategoryId,
        TotalScore,
        TimeSpentSeconds,
        StartedAt,
        EndedAt
    FROM GameSessions
    WHERE UserId = @UserId
    ORDER BY StartedAt DESC;
END
GO
