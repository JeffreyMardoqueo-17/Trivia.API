CREATE DATABASE TriviaGameDB
 GO
 USE TriviaGameDB
 GO

---------Tabla de usaurios
 CREATE TABLE Users
 (
     Id INT PRIMARY KEY IDENTITY(1,1),
     Gmail NVARCHAR(100) NOT NULL UNIQUE,
     PasswordHash VARCHAR(500) NOT NULL,
     PasswordSalt VARCHAR(500) NOT NULL,
     IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
 )
 GO
