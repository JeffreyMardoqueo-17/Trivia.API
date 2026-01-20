
CREATE DATABASE TriviaGameDB
GO

USE TriviaGameDB
GO

-------------------------USUARIOS
CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Gmail NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash VARCHAR(500) NOT NULL,
    PasswordSalt VARCHAR(500) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
)
GO
-----------------------------------CATEGORIAS
CREATE TABLE Categories (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1
)
GO
------------------------------------preguntas
CREATE TABLE Questions (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    CategoryId INT NOT NULL,
    Text NVARCHAR(500) NOT NULL,
    Points INT NOT NULL DEFAULT 10,
    IsActive BIT NOT NULL DEFAULT 1,

    CONSTRAINT FK_Questions_Categories
        FOREIGN KEY (CategoryId) REFERENCES Categories(Id)
)
GO
-------------------------------------respuestas 
CREATE TABLE Answers (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    QuestionId INT NOT NULL,
    Text NVARCHAR(300) NOT NULL,
    IsCorrect BIT NOT NULL,

    CONSTRAINT FK_Answers_Questions
        FOREIGN KEY (QuestionId) REFERENCES Questions(Id)
)
GO
-----------------------------------sesion de juego + tiempo 
CREATE TABLE GameSessions (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    CategoryId INT NOT NULL,
    TotalScore INT NOT NULL DEFAULT 0,

    MaxDurationSeconds INT NOT NULL DEFAULT 90, -- 3 preguntas x 30s
    TimeSpentSeconds INT NOT NULL DEFAULT 0,

    StartedAt DATETIME NOT NULL DEFAULT GETDATE(),
    EndedAt DATETIME NULL,

    CONSTRAINT FK_GameSessions_Users
        FOREIGN KEY (UserId) REFERENCES Users(Id),

    CONSTRAINT FK_GameSessions_Categories
        FOREIGN KEY (CategoryId) REFERENCES Categories(Id)
)
GO

--------------------------------preguntas de la sesion de juego
CREATE TABLE GameSessionQuestions (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    GameSessionId INT NOT NULL,
    QuestionId INT NOT NULL,
    TimeLimitSeconds INT NOT NULL DEFAULT 30,

    CONSTRAINT FK_GSQ_GameSessions
        FOREIGN KEY (GameSessionId) REFERENCES GameSessions(Id),

    CONSTRAINT FK_GSQ_Questions
        FOREIGN KEY (QuestionId) REFERENCES Questions(Id)
)
GO
-------------------------------respuestas del usuario
CREATE TABLE UserAnswers (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    GameSessionId INT NOT NULL,
    QuestionId INT NOT NULL,
    AnswerId INT NOT NULL,

    IsCorrect BIT NOT NULL,
    PointsEarned INT NOT NULL,
    TimeSpentSeconds INT NOT NULL,

    AnsweredAt DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT FK_UserAnswers_GameSessions
        FOREIGN KEY (GameSessionId) REFERENCES GameSessions(Id),

    CONSTRAINT FK_UserAnswers_Questions
        FOREIGN KEY (QuestionId) REFERENCES Questions(Id),

    CONSTRAINT FK_UserAnswers_Answers
        FOREIGN KEY (AnswerId) REFERENCES Answers(Id)
)
GO
