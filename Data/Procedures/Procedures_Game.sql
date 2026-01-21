USE TriviaGameDB
GO
-----------------------INICIAR SESION   DE JUEGO-----------------------
CREATE OR ALTER PROC SP_StartGameSession
    @UserId INT,
    @CategoryId INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM Users WHERE Id = @UserId AND IsActive = 1)
        THROW 50001, 'Usuario inválido', 1;

    IF NOT EXISTS (SELECT 1 FROM Categories WHERE Id = @CategoryId AND IsActive = 1)
        THROW 50002, 'Categoría inválida', 1;

    IF (
        SELECT COUNT(*) 
        FROM Questions 
        WHERE CategoryId = @CategoryId AND IsActive = 1
    ) < 3
        THROW 50003, 'No hay suficientes preguntas en la categoría', 1;

    IF EXISTS (
    SELECT 1 FROM GameSessions
    WHERE UserId = @UserId
    AND EndedAt IS NULL
    )
    THROW 50004, 'El usuario ya tiene una sesión activa', 1; ----si no el usaurio puede abrir muchas sesiones


    BEGIN TRAN;

    DECLARE @GameSessionId INT;

    INSERT INTO GameSessions (UserId, CategoryId)
    VALUES (@UserId, @CategoryId);

    SET @GameSessionId = SCOPE_IDENTITY();

    INSERT INTO GameSessionQuestions (GameSessionId, QuestionId)
    SELECT TOP 3 @GameSessionId, Id
    FROM Questions
    WHERE CategoryId = @CategoryId AND IsActive = 1
    ORDER BY NEWID();

    COMMIT;

    SELECT @GameSessionId AS GameSessionId;
END
GO

-----------------------OBTENER PREGUNTAS DE LA SESION (SIN RESPUESTAS)-----------------------
CREATE OR ALTER PROC SP_GetGameQuestions
    @GameSessionId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        q.Id AS QuestionId,
        q.Text AS QuestionText,
        q.Points,
        gsq.TimeLimitSeconds
    FROM GameSessionQuestions gsq
    JOIN Questions q ON q.Id = gsq.QuestionId
    WHERE gsq.GameSessionId = @GameSessionId
    ORDER BY gsq.Id;
END
GO

-----------------------OBTENER RESPUESTAS DE UNA PREGUNTA-----------------------
CREATE OR ALTER PROC SP_GetQuestionAnswers
    @QuestionId INT
AS
BEGIN
    SET NOCOUNT ON;

 SELECT 
    Id AS AnswerId,
    Text AS AnswerText
FROM Answers
WHERE QuestionId = @QuestionId;

END
GO

-----------------------GUARDAR RESPUESTA DEL USUARIO-----------------------
CREATE OR ALTER PROC SP_SaveUserAnswer
    @GameSessionId INT,
    @QuestionId INT,
    @AnswerId INT,
    @TimeSpentSeconds INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (
        SELECT 1 FROM GameSessions 
        WHERE Id = @GameSessionId AND EndedAt IS NULL
    )
        THROW 50010, 'Sesión no activa', 1;

    IF NOT EXISTS (
        SELECT 1 FROM GameSessionQuestions
        WHERE GameSessionId = @GameSessionId
          AND QuestionId = @QuestionId
    )
        THROW 50011, 'Pregunta no pertenece a la sesión', 1;

    IF EXISTS (
        SELECT 1 FROM UserAnswers
        WHERE GameSessionId = @GameSessionId
          AND QuestionId = @QuestionId
    )
        THROW 50012, 'Pregunta ya respondida', 1;

    IF NOT EXISTS (
        SELECT 1 FROM Answers
        WHERE Id = @AnswerId AND QuestionId = @QuestionId
    )
        THROW 50013, 'Respuesta inválida', 1;

    DECLARE 
        @IsCorrect BIT,
        @Points INT,
        @TimeLimit INT;

    SELECT @TimeLimit = TimeLimitSeconds
    FROM GameSessionQuestions
    WHERE GameSessionId = @GameSessionId
      AND QuestionId = @QuestionId;

    SELECT @IsCorrect = IsCorrect
    FROM Answers
    WHERE Id = @AnswerId;

    SELECT @Points = Points
    FROM Questions
    WHERE Id = @QuestionId;

    IF (@TimeSpentSeconds > @TimeLimit OR @IsCorrect = 0)
        SET @Points = 0;

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

    UPDATE GameSessions
    SET 
        TotalScore = TotalScore + @Points,
        TimeSpentSeconds = TimeSpentSeconds + @TimeSpentSeconds
    WHERE Id = @GameSessionId;
END
GO

-----------------------FINALIZAR SESIÓN DE JUEGO-----------------------
CREATE OR ALTER PROC SP_EndGameSession
    @GameSessionId INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE GameSessions
    SET EndedAt = GETDATE()
    WHERE Id = @GameSessionId
      AND EndedAt IS NULL;
END
GO

-----------------------HISTORIAL DE JUEGOS DEL USUARIO-----------------------
CREATE OR ALTER PROC SP_GetUserGameHistory
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
SELECT 
    gs.Id AS GameSessionId,
    c.Name AS CategoryName,  -- <- renombrado
    gs.TotalScore,
    gs.TimeSpentSeconds,
    gs.StartedAt,
    gs.EndedAt
FROM GameSessions gs
JOIN Categories c ON c.Id = gs.CategoryId
WHERE gs.UserId = @UserId
ORDER BY gs.StartedAt DESC;
END
GO


USE TriviaGameDB
GO

CREATE OR ALTER PROC SP_GetAnsweredCount
    @GameSessionId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COUNT(*) AS AnsweredCount
    FROM UserAnswers
    WHERE GameSessionId = @GameSessionId;
END
GO


CREATE OR ALTER PROC SP_GetNextQuestion
    @GameSessionId INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Validar sesión activa
    IF NOT EXISTS (
        SELECT 1 
        FROM GameSessions 
        WHERE Id = @GameSessionId 
          AND EndedAt IS NULL
    )
        THROW 50020, 'Sesión inválida o finalizada', 1;

    DECLARE @QuestionId INT;

    -- Obtener la siguiente pregunta NO respondida
    SELECT TOP 1 
        @QuestionId = q.Id
    FROM GameSessionQuestions gsq
    JOIN Questions q ON q.Id = gsq.QuestionId
    WHERE gsq.GameSessionId = @GameSessionId
      AND NOT EXISTS (
          SELECT 1 
          FROM UserAnswers ua
          WHERE ua.GameSessionId = @GameSessionId
            AND ua.QuestionId = q.Id
      )
    ORDER BY gsq.Id;

    -- Si no hay más preguntas
    IF @QuestionId IS NULL
    BEGIN
        SELECT NULL AS QuestionId;
        RETURN;
    END

    -- RESULTSET 1: Pregunta
    SELECT 
        q.Id AS QuestionId,
        q.Text AS QuestionText,
        q.Points,
        gsq.TimeLimitSeconds
    FROM Questions q
    JOIN GameSessionQuestions gsq 
        ON gsq.QuestionId = q.Id
    WHERE q.Id = @QuestionId;

    -- RESULTSET 2: Respuestas
    SELECT 
        a.Id AS AnswerId,
        a.Text AS AnswerText
    FROM Answers a
    WHERE a.QuestionId = @QuestionId;

END
GO
