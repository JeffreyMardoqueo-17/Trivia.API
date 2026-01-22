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

CREATE OR ALTER PROC SP_GetCorrectAnswerByQuestion
    @QuestionId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1 *
    FROM Answers
    WHERE QuestionId = @QuestionId
      AND IsCorrect = 1;
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

END
GO
CREATE OR ALTER PROC SP_SaveUserAnswer
    @GameSessionId INT,
    @QuestionId INT,
    @AnswerId INT,              -- puede venir 0 desde frontend
    @TimeSpentSeconds INT,
    @IsCorrect BIT,
    @PointsEarned INT
AS
BEGIN
    SET NOCOUNT ON;

    -- 🔒 Evitar doble respuesta por pregunta
    IF EXISTS (
        SELECT 1
        FROM UserAnswers
        WHERE GameSessionId = @GameSessionId
          AND QuestionId = @QuestionId
    )
        RETURN;

    -- 💾 Guardar respuesta (0 => NULL)
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
        NULLIF(@AnswerId, 0),     -- 🔥 CLAVE ABSOLUTA
        @TimeSpentSeconds,
        @IsCorrect,
        @PointsEarned,
        GETDATE()
    );

    -- ⏱️ + 🧮 Acumular tiempo y score EN TIEMPO REAL
    UPDATE GameSessions
    SET
        TotalScore = ISNULL(TotalScore, 0) + @PointsEarned,
        TimeSpentSeconds = ISNULL(TimeSpentSeconds, 0) + @TimeSpentSeconds
    WHERE Id = @GameSessionId;
END
GO



--------------------------PARA MOSTRAR EL RESULTADO DE LA PARTIDA
CREATE OR ALTER PROC SP_GetGameSessionResult
    @GameSessionId INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @TotalQuestions INT;

    -- Total de preguntas de la sesión
    SELECT @TotalQuestions = COUNT(*)
    FROM GameSessionQuestions
    WHERE GameSessionId = @GameSessionId;

    SELECT
        gs.Id AS GameSessionId,
        gs.TotalScore,
        @TotalQuestions AS TotalQuestions,

        -- Respondidas
        COUNT(ua.Id) AS AnsweredQuestions,

        -- Correctas
        SUM(CASE WHEN ua.IsCorrect = 1 THEN 1 ELSE 0 END) AS CorrectAnswers,

        -- Incorrectas (incluye no respondidas)
        @TotalQuestions - SUM(CASE WHEN ua.IsCorrect = 1 THEN 1 ELSE 0 END) AS IncorrectAnswers,

        -- No respondidas
        SUM(CASE WHEN ua.AnswerId IS NULL THEN 1 ELSE 0 END) AS NotAnswered

    FROM GameSessions gs
    LEFT JOIN UserAnswers ua 
        ON ua.GameSessionId = gs.Id
    WHERE gs.Id = @GameSessionId
    GROUP BY gs.Id, gs.TotalScore;
END
GO
CREATE PROC SP_GetGameSessionInfo
    @GameSessionId INT
AS
BEGIN
    SELECT 
        u.UserId,
        u.UserName
    FROM GameSessions gs
    INNER JOIN Users u ON u.UserId = gs.UserId
    WHERE gs.GameSessionId = @GameSessionId
END

CREATE PROC SP_GetGameSessionInfo
    @GameSessionId INT
AS
BEGIN
    SELECT 
        u.UserId,
        u.UserName
    FROM GameSessions gs
    INNER JOIN Users u ON u.UserId = gs.UserId
    WHERE gs.GameSessionId = @GameSessionId
END

CREATE OR ALTER PROC SP_GetCategoryRanking
    @CategoryId INT,
    @Top INT = 10 -- top N, si 0 o NULL devuelve todos
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        u.Id AS UserId,
        u.Gmail,
        SUM(gs.TotalScore) AS TotalPoints
    FROM Users u
    INNER JOIN GameSessions gs ON gs.UserId = u.Id
    WHERE u.IsActive = 1
      AND gs.CategoryId = @CategoryId
    GROUP BY u.Id, u.Gmail
    ORDER BY TotalPoints DESC
    OFFSET 0 ROWS
    FETCH NEXT CASE WHEN @Top IS NULL OR @Top <= 0 THEN 1000000 ELSE @Top END ROWS ONLY;
END
GO