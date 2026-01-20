# SpExecutor.md

## Guía de uso de métodos y casos de aplicación

Este documento describe **cuándo usar cada método** de la clase `SpExecutor`.
No explica lógica de negocio ni otros componentes del sistema.

## RESUMEN RÁPIDO

| Método | Uso principal |
|------|--------------|
| `ExecuteWithOutputAsync` | Acciones que modifican datos y devuelven éxito/fracaso |
| `CreateOutputParameters` | Preparar parámetros OUTPUT estándar para SP |
| `QuerySingleAsync` | Obtener un solo registro (o ninguno) |
| `QueryAsync` | Obtener listas o colecciones |

---

## 1. ExecuteWithOutputAsync

```csharp
Task<(bool Flag, string Message)> ExecuteWithOutputAsync(
    string spName,
    DynamicParameters parameters
)
## CUÁNDO USARLO
Usa este método cuando el Stored Procedure:

Modifica datos (INSERT, UPDATE, DELETE)

Ejecuta validaciones de negocio

Devuelve el resultado mediante parámetros OUTPUT

Necesitas saber si la operación fue exitosa o no

REQUISITOS DEL STORED PROCEDURE
El SP debe devolver al menos:

@Success o @Exists (BIT)

@ResponseMessage (VARCHAR)

CASOS DE USO TÍPICOS
Registrar usuario

Iniciar sesión

Crear una sesión de juego

Guardar una respuesta del usuario

Finalizar una partida

Validar si un recurso ya existe

EJEMPLO DE USO
var parameters = new DynamicParameters();
parameters.Add("@UserId", userId);
parameters.Add("@CategoryId", categoryId);
parameters.Add("@Success", dbType: DbType.Boolean, direction: ParameterDirection.Output);
parameters.Add("@ResponseMessage", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);

var result = await _spExecutor.ExecuteWithOutputAsync("SP_StartGame", parameters);
2. CreateOutputParameters
DynamicParameters CreateOutputParameters(
    Dictionary<string, object> inputs
)
CUÁNDO USARLO
Usa este método antes de llamar a ExecuteWithOutputAsync cuando:

El SP utiliza parámetros OUTPUT estándar

Quieres evitar repetir la definición de parámetros de salida

Necesitas una forma consistente de pasar inputs y outputs

QUÉ HACE
Agrega todos los parámetros de entrada

Agrega automáticamente:

@Success

@Exists

@ResponseMessage

CASOS DE USO TÍPICOS
Registro de usuario

Login

Cualquier SP que devuelva éxito/fracaso

EJEMPLO DE USO
var parameters = _spExecutor.CreateOutputParameters(new()
{
    { "@Gmail", gmail },
    { "@PasswordHash", passwordHash }
});

var result = await _spExecutor.ExecuteWithOutputAsync("SP_LoginUser", parameters);
3. QuerySingleAsync<T>
Task<T?> QuerySingleAsync<T>(
    string spName,
    object? parameters = null
)
CUÁNDO USARLO
Usa este método cuando el Stored Procedure:

Devuelve un solo registro

O devuelve ningún registro

Representa una entidad única

CASOS DE USO TÍPICOS
Obtener un usuario por Id

Obtener una sesión de juego activa

Obtener una pregunta específica

Obtener el resumen de una partida

EJEMPLO DE USO
var session = await _spExecutor.QuerySingleAsync<GameSessionDto>(
    "SP_GetActiveGameSession",
    new { UserId = userId }
);
4. QueryAsync<T>
Task<IEnumerable<T>> QueryAsync<T>(
    string spName,
    object? parameters = null
)
CUÁNDO USARLO
Usa este método cuando el Stored Procedure:

Devuelve múltiples registros

Devuelve listas o colecciones

No modifica datos

CASOS DE USO TÍPICOS
Listar categorías

Obtener preguntas por categoría

Obtener respuestas de una pregunta

Obtener historial de partidas

Obtener ranking de usuarios

EJEMPLO DE USO
var questions = await _spExecutor.QueryAsync<QuestionDto>(
    "SP_GetQuestionsByCategory",
    new { CategoryId = categoryId }
);
RESUMEN RÁPIDO
Método Uso principal
ExecuteWithOutputAsync Acciones con éxito/fracaso
CreateOutputParameters Preparar parámetros OUTPUT estándar
QuerySingleAsync Obtener un solo registro
QueryAsync Obtener listas o colecciones
