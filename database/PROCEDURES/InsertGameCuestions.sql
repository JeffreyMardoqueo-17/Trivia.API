INSERT INTO Questions (CategoryId, Text)
VALUES (2, '¿Cuál es el animal terrestre más grande del mundo?');

DECLARE @Q1 INT = SCOPE_IDENTITY();

INSERT INTO Answers (QuestionId, Text, IsCorrect) VALUES
(@Q1, 'Elefante africano', 1),
(@Q1, 'Rinoceronte', 0),
(@Q1, 'Hipopótamo', 0);

INSERT INTO Questions (CategoryId, Text)
VALUES (2, '¿Qué animal es conocido como el rey de la selva?');

DECLARE @Q2 INT = SCOPE_IDENTITY();

INSERT INTO Answers (QuestionId, Text, IsCorrect) VALUES
(@Q2, 'León', 1),
(@Q2, 'Tigre', 0),
(@Q2, 'Jaguar', 0);

INSERT INTO Questions (CategoryId, Text)
VALUES (2, '¿Cuál de estos animales es un mamífero marino?');

DECLARE @Q3 INT = SCOPE_IDENTITY();

INSERT INTO Answers (QuestionId, Text, IsCorrect) VALUES
(@Q3, 'Delfín', 1),
(@Q3, 'Tiburón', 0),
(@Q3, 'Pulpo', 0);

INSERT INTO Questions (CategoryId, Text)
VALUES (2, '¿Qué animal puede cambiar el color de su piel para camuflarse?');

DECLARE @Q4 INT = SCOPE_IDENTITY();

INSERT INTO Answers (QuestionId, Text, IsCorrect) VALUES
(@Q4, 'Camaleón', 1),
(@Q4, 'Iguana', 0),
(@Q4, 'Serpiente', 0);

INSERT INTO Questions (CategoryId, Text)
VALUES (2, '¿Cuál es el animal más rápido en tierra?');

DECLARE @Q5 INT = SCOPE_IDENTITY();

INSERT INTO Answers (QuestionId, Text, IsCorrect) VALUES
(@Q5, 'Guepardo', 1),
(@Q5, 'León', 0),
(@Q5, 'Caballo', 0);

INSERT INTO Questions (CategoryId, Text)
VALUES (2, '¿Qué animal es conocido por tener una memoria excepcional?');

DECLARE @Q6 INT = SCOPE_IDENTITY();

INSERT INTO Answers (QuestionId, Text, IsCorrect) VALUES
(@Q6, 'Elefante', 1),
(@Q6, 'Perro', 0),
(@Q6, 'Mono', 0);

INSERT INTO Questions (CategoryId, Text)
VALUES (2, '¿Cuál de estos animales pone huevos?');

DECLARE @Q7 INT = SCOPE_IDENTITY();

INSERT INTO Answers (QuestionId, Text, IsCorrect) VALUES
(@Q7, 'Ornitorrinco', 1),
(@Q7, 'Delfín', 0),
(@Q7, 'Ballena', 0);

INSERT INTO Questions (CategoryId, Text)
VALUES (2, '¿Qué animal duerme la mayor parte del día?');

DECLARE @Q8 INT = SCOPE_IDENTITY();

INSERT INTO Answers (QuestionId, Text, IsCorrect) VALUES
(@Q8, 'Koala', 1),
(@Q8, 'Oso', 0),
(@Q8, 'Perezoso', 0);

INSERT INTO Questions (CategoryId, Text)
VALUES (2, '¿Cuál de estos animales es un reptil?');

DECLARE @Q9 INT = SCOPE_IDENTITY();

INSERT INTO Answers (QuestionId, Text, IsCorrect) VALUES
(@Q9, 'Cocodrilo', 1),
(@Q9, 'Rana', 0),
(@Q9, 'Nutria', 0);

INSERT INTO Questions (CategoryId, Text)
VALUES (2, '¿Qué animal tiene el cuello más largo?');

DECLARE @Q10 INT = SCOPE_IDENTITY();

INSERT INTO Answers (QuestionId, Text, IsCorrect) VALUES
(@Q10, 'Jirafa', 1),
(@Q10, 'Camello', 0),
(@Q10, 'Avestruz', 0);


SELECT q.Id, q.Text, a.Text, a.IsCorrect
FROM Questions q
JOIN Answers a ON a.QuestionId = q.Id
WHERE q.CategoryId = 2
ORDER BY q.Id;

GO
-------------------------------PREGUNTAS DE CARRORS
INSERT INTO Questions (CategoryId, Text)
VALUES (1, '¿Cuál es la marca de carros conocida por el modelo Mustang?');

DECLARE @Q1 INT = SCOPE_IDENTITY();

INSERT INTO Answers (QuestionId, Text, IsCorrect) VALUES
(@Q1, 'Ford', 1),
(@Q1, 'Chevrolet', 0),
(@Q1, 'Dodge', 0);

INSERT INTO Questions (CategoryId, Text)
VALUES (1, '¿Qué tipo de motor usan la mayoría de los carros deportivos?');

DECLARE @Q2 INT = SCOPE_IDENTITY();

INSERT INTO Answers (QuestionId, Text, IsCorrect) VALUES
(@Q2, 'Motor V8', 1),
(@Q2, 'Motor diésel', 0),
(@Q2, 'Motor eléctrico', 0);

INSERT INTO Questions (CategoryId, Text)
VALUES (1, '¿Cuál de estos carros es eléctrico?');

DECLARE @Q3 INT = SCOPE_IDENTITY();

INSERT INTO Answers (QuestionId, Text, IsCorrect) VALUES
(@Q3, 'Tesla Model S', 1),
(@Q3, 'Toyota Corolla', 0),
(@Q3, 'Honda Civic', 0);

INSERT INTO Questions (CategoryId, Text)
VALUES (1, '¿Qué significa ABS en un carro?');

DECLARE @Q4 INT = SCOPE_IDENTITY();

INSERT INTO Answers (QuestionId, Text, IsCorrect) VALUES
(@Q4, 'Sistema de frenos antibloqueo', 1),
(@Q4, 'Control de aire', 0),
(@Q4, 'Sistema de transmisión automática', 0);

INSERT INTO Questions (CategoryId, Text)
VALUES (1, '¿Cuál es el carro más vendido del mundo?');

DECLARE @Q5 INT = SCOPE_IDENTITY();

INSERT INTO Answers (QuestionId, Text, IsCorrect) VALUES
(@Q5, 'Toyota Corolla', 1),
(@Q5, 'Ford F-150', 0),
(@Q5, 'Honda Civic', 0);

INSERT INTO Questions (CategoryId, Text)
VALUES (1, '¿Qué parte del carro mide la velocidad?');

DECLARE @Q6 INT = SCOPE_IDENTITY();

INSERT INTO Answers (QuestionId, Text, IsCorrect) VALUES
(@Q6, 'Velocímetro', 1),
(@Q6, 'Tachómetro', 0),
(@Q6, 'Odómetro', 0);

INSERT INTO Questions (CategoryId, Text)
VALUES (1, '¿Qué combustible usan la mayoría de los carros?');

DECLARE @Q7 INT = SCOPE_IDENTITY();

INSERT INTO Answers (QuestionId, Text, IsCorrect) VALUES
(@Q7, 'Gasolina', 1),
(@Q7, 'Agua', 0),
(@Q7, 'Aceite', 0);

INSERT INTO Questions (CategoryId, Text)
VALUES (1, '¿Cuál es la marca de carros de lujo italiana famosa por sus deportivos?');

DECLARE @Q8 INT = SCOPE_IDENTITY();

INSERT INTO Answers (QuestionId, Text, IsCorrect) VALUES
(@Q8, 'Ferrari', 1),
(@Q8, 'Kia', 0),
(@Q8, 'Hyundai', 0);

INSERT INTO Questions (CategoryId, Text)
VALUES (1, '¿Qué parte del carro convierte la energía del motor en movimiento?');

DECLARE @Q9 INT = SCOPE_IDENTITY();

INSERT INTO Answers (QuestionId, Text, IsCorrect) VALUES
(@Q9, 'Transmisión', 1),
(@Q9, 'Radiador', 0),
(@Q9, 'Freno de mano', 0);

INSERT INTO Questions (CategoryId, Text)
VALUES (1, '¿Cuál de estos carros es famoso por su sistema híbrido?');

DECLARE @Q10 INT = SCOPE_IDENTITY();

INSERT INTO Answers (QuestionId, Text, IsCorrect) VALUES
(@Q10, 'Toyota Prius', 1),
(@Q10, 'Mazda RX-7', 0),
(@Q10, 'Chevrolet Camaro', 0);
