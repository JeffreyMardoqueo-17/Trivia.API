CREATE OR ALTER PROC SP_StartGameSession
    @UserId INT,
    @CategoryId INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @GameSessionId INT = 0;

    BEGIN TRY

        -- Validar usuario activo
        IF NOT EXISTS (SELECT 1 FROM Users WHERE Id = @UserId AND IsActive = 1)
            GOTO Finish;

        -- Validar categoría activa
        IF NOT EXISTS (SELECT 1 FROM Categories WHERE Id = @CategoryId AND IsActive = 1)
            GOTO Finish;

        -- Validar mínimo de preguntas
        IF (
            SELECT COUNT(*) 
            FROM Questions 
            WHERE CategoryId = @CategoryId AND IsActive = 1
        ) < 3
            GOTO Finish;

        -- Crear sesión
        INSERT INTO GameSessions (UserId, CategoryId)
        VALUES (@UserId, @CategoryId);

        SET @GameSessionId = SCOPE_IDENTITY();

        -- Asignar preguntas
        INSERT INTO GameSessionQuestions (GameSessionId, QuestionId)
        SELECT TOP 3 @GameSessionId, Id
        FROM Questions
        WHERE CategoryId = @CategoryId AND IsActive = 1
        ORDER BY NEWID();

    END TRY
    BEGIN CATCH
        SET @GameSessionId = 0;
    END CATCH

Finish:
    SELECT @GameSessionId AS GameSessionId;
END
GO

CREATE OR ALTER PROC SP_GetGameSessionById
    @GameSessionId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM GameSessions
    WHERE Id = @GameSessionId;
END
GO
CREATE PROC SP_GetCorrectAnswerByQuestion
    @QuestionId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1 *
    FROM Answers
    WHERE QuestionId = @QuestionId
      AND IsCorrect = 1;
END




CREATE OR ALTER PROC SP_GetNextQuestion
    @GameSessionId INT
AS
BEGIN
    SET NOCOUNT ON;

    /*
    🔹 Función:
        - Retorna la siguiente pregunta no respondida
        - Trae sus 3 respuestas
        - Pensado para flujo en tiempo real
    */

    DECLARE @QuestionId INT = NULL;

    -- Tomar próxima pregunta no respondida
    SELECT TOP 1 @QuestionId = q.Id
    FROM GameSessionQuestions gsq
    JOIN Questions q ON q.Id = gsq.QuestionId
    WHERE gsq.GameSessionId = @GameSessionId
      AND NOT EXISTS (
          SELECT 1 FROM UserAnswers ua 
          WHERE ua.GameSessionId = @GameSessionId 
            AND ua.QuestionId = q.Id
      )
    ORDER BY gsq.Id;

    -- Si no hay pregunta, retorna NULL
    IF @QuestionId IS NULL
    BEGIN
        SELECT NULL AS QuestionId;
        RETURN;
    END

    -- Retornar pregunta + respuestas
    SELECT 
        q.Id AS QuestionId,
        q.Text AS QuestionText,
        q.Points,
        gsq.TimeLimitSeconds,
        a.Id AS AnswerId,
        a.Text AS AnswerText
    FROM Questions q
    JOIN GameSessionQuestions gsq ON gsq.QuestionId = q.Id AND gsq.GameSessionId = @GameSessionId
    JOIN Answers a ON a.QuestionId = q.Id
    WHERE q.Id = @QuestionId
    ORDER BY a.Id;
END
GO


CREATE OR ALTER PROC SP_GetNextQuestion
    @GameSessionId INT
AS
BEGIN
    SET NOCOUNT ON;

    /*
    🔹 Función:
        - Retorna la siguiente pregunta no respondida
        - Trae sus 3 respuestas
        - Pensado para flujo en tiempo real
    */

    DECLARE @QuestionId INT = NULL;

    -- Tomar próxima pregunta no respondida
    SELECT TOP 1 @QuestionId = q.Id
    FROM GameSessionQuestions gsq
    JOIN Questions q ON q.Id = gsq.QuestionId
    WHERE gsq.GameSessionId = @GameSessionId
      AND NOT EXISTS (
          SELECT 1 FROM UserAnswers ua 
          WHERE ua.GameSessionId = @GameSessionId 
            AND ua.QuestionId = q.Id
      )
    ORDER BY gsq.Id;

    -- Si no hay pregunta, retorna NULL
    IF @QuestionId IS NULL
    BEGIN
        SELECT NULL AS QuestionId;
        RETURN;
    END

    -- Retornar pregunta + respuestas
    SELECT 
        q.Id AS QuestionId,
        q.Text AS QuestionText,
        q.Points,
        gsq.TimeLimitSeconds,
        a.Id AS AnswerId,
        a.Text AS AnswerText
    FROM Questions q
    JOIN GameSessionQuestions gsq ON gsq.QuestionId = q.Id AND gsq.GameSessionId = @GameSessionId
    JOIN Answers a ON a.QuestionId = q.Id
    WHERE q.Id = @QuestionId
    ORDER BY a.Id;
END
GO


CREATE OR ALTER PROC SP_EndGameSession
    @GameSessionId INT
AS
BEGIN
    SET NOCOUNT ON;

    /*
    🔹 Función:
        - Marca la sesión como finalizada
        - No bloquea la ejecución si ya está cerrada
    */
    UPDATE GameSessions
    SET EndedAt = GETDATE()
    WHERE Id = @GameSessionId
      AND EndedAt IS NULL;
END
GO


CREATE OR ALTER PROC SP_GetRanking
AS
BEGIN
    SET NOCOUNT ON;

    /*
    🔹 Función:
        - Retorna usuarios ordenados por puntos acumulados
        - Se puede usar para leaderboard en SignalR
    */
    SELECT 
        u.Id AS UserId,
        u.Gmail,
        SUM(gs.TotalScore) AS TotalPoints
    FROM Users u
    JOIN GameSessions gs ON gs.UserId = u.Id
    GROUP BY u.Id, u.Gmail
    ORDER BY TotalPoints DESC;
END
GO

CREATE OR ALTER PROC SP_GetRanking
AS
BEGIN
    SET NOCOUNT ON;

    /*
    🔹 Función:
        - Retorna usuarios ordenados por puntos acumulados
        - Se puede usar para leaderboard en SignalR
    */
    SELECT 
        u.Id AS UserId,
        u.Gmail,
        SUM(gs.TotalScore) AS TotalPoints
    FROM Users u
    JOIN GameSessions gs ON gs.UserId = u.Id
    GROUP BY u.Id, u.Gmail
    ORDER BY TotalPoints DESC;
END
GO



CREATE OR ALTER PROC SP_SaveUserAnswer
    @GameSessionId INT,
    @QuestionId INT,
    @AnswerId INT,
    @TimeSpentSeconds INT,
    @IsCorrect BIT,
    @PointsEarned INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Guardar respuesta del usuario
    INSERT INTO UserAnswers (
        GameSessionId,
        QuestionId,
        AnswerId,
        TimeSpentSeconds,
        IsCorrect,
        PointsEarned,
        AnsweredAt
    )
    VALUES (
        @GameSessionId,
        @QuestionId,
        @AnswerId,
        @TimeSpentSeconds,
        @IsCorrect,
        @PointsEarned,
        GETDATE()
    );

    -- Actualizar score acumulado
    UPDATE GameSessions
    SET TotalScore = TotalScore + @PointsEarned
    WHERE Id = @GameSessionId;

    SELECT 1 AS Success;
END
GO

CREATE OR ALTER PROC SP_GetGameSessionTotalScore
    @GameSessionId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT ISNULL(SUM(PointsEarned),0) AS TotalScore
    FROM UserAnswers
    WHERE GameSessionId = @GameSessionId;
END
