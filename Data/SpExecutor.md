# Dapper en TriviaGame.Api

## 1. Qué es Dapper

Dapper es un **micro-ORM (Object-Relational Mapper) para .NET**.  
Se trata de una librería ligera que extiende `IDbConnection` con métodos de alto rendimiento, facilitando la ejecución de consultas SQL y Stored Procedures sin la complejidad de un ORM completo como Entity Framework.

---

## 2. Cómo funciona

- Proporciona métodos de extensión como:

```csharp
connection.ExecuteAsync("SP_Name", parameters, commandType: CommandType.StoredProcedure);
connection.QueryFirstOrDefaultAsync<User>("SP_LoginUser", new { Gmail = gmail }, commandType: CommandType.StoredProcedure);
```

- Permite mapear resultados de consultas directamente a objetos C#.

- Permite manejar parámetros de entrada y salida mediante DynamicParameters.

- Optimiza la comunicación con la base de datos usando ADO.NET internamente, evitando código repetitivo para abrir, ejecutar y cerrar conexiones.

## 4. Justificación de su uso

- **Rendimiento** : ejecución casi tan rápida como ADO.NET puro.

- **Simplicidad**: reduce líneas de código y elimina boilerplate repetitivo.

- **Seguridad**: uso de parámetros SQL evita inyección de código.

- **Mantenimiento**: permite reutilización de lógica para múltiples SPs mediante SpExecutor.

- **Compatibilidad**: funciona con .NET 8 y se integra con la arquitectura basada en servicios y DI.

## 5. Ejemplo

```csharp
var parameters = new DynamicParameters();
parameters.Add("@Gmail", gmail);
parameters.Add("@PasswordHash", passwordHash);
parameters.Add("@PasswordSalt", passwordSalt);
parameters.Add("@Exists", dbType: DbType.Boolean, direction: ParameterDirection.Output);
parameters.Add("@ResponseMessage", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);

var result = await _spExecutor.ExecuteWithOutputAsync("SP_CreateUser", parameters);

bool userExists = result.Flag;
string message = result.Message;

```

En este ejemplo, **SpExecutor** centraliza la ejecución de SPs que devuelven parámetros de salida ( **@Success**, **@Exists**, **@ResponseMessage**), simplificando la lógica dentro del UserService.

## 6. Ejemplo de ejecución de SP sin Dapper

```csharp
using var connection = new SqlConnection(connectionString);
await connection.OpenAsync();

using var command = new SqlCommand("SP_CreateUser", connection);
command.CommandType = CommandType.StoredProcedure;

command.Parameters.AddWithValue("@Gmail", gmail);
command.Parameters.AddWithValue("@PasswordHash", passwordHash);
command.Parameters.AddWithValue("@PasswordSalt", passwordSalt);

var existsParam = new SqlParameter("@Exists", SqlDbType.Bit) { Direction = ParameterDirection.Output };
var messageParam = new SqlParameter("@ResponseMessage", SqlDbType.NVarChar, 100) { Direction = ParameterDirection.Output };

command.Parameters.Add(existsParam);
command.Parameters.Add(messageParam);

await command.ExecuteNonQueryAsync();

bool userExists = (bool)existsParam.Value;
string message = (string)messageParam.Value;

```

**Problemas sin Dapper:**

- Código repetitivo y verboso.

- Manejo manual de parámetros y outputs.

- Difícil de mantener si los SPs o outputs cambian.

1. Mejora y optimización

## Mejora y optimización (Lo que uso aqui)

```csharp
public DynamicParameters CreateOutputParameters(Dictionary<string, object> inputs)
{
    var parameters = new DynamicParameters();
    foreach (var kvp in inputs)
        parameters.Add(kvp.Key, kvp.Value);

    parameters.Add("@Success", dbType: DbType.Boolean, direction: ParameterDirection.Output);
    parameters.Add("@Exists", dbType: DbType.Boolean, direction: ParameterDirection.Output);
    parameters.Add("@ResponseMessage", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);

    return parameters;
}
```

Esto permite usar una única forma de generar parámetros de salida, reduciendo código repetitivo y cumpliendo principio DRY (**Don't Repeat Yourself**, o "**No te repitas**")).

## EJEMPLO DEL FLUJO COMPLETO

```csharp

// 1. Preparar inputs
var inputs = new Dictionary<string, object>
{
    { "@Gmail", "correo@gmail.com" },
    { "@PasswordHash", "hash123" },
    { "@PasswordSalt", "salt123" }
};

// 2. Crear parámetros de SP
var parameters = _spExecutor.CreateOutputParameters(inputs);

// 3. Ejecutar SP
var result = await _spExecutor.ExecuteWithOutputAsync("SP_CreateUser", parameters);

// 4. Leer resultados
bool userExists = result.Flag;
string message = result.Message;
Console.WriteLine($"Resultado: {userExists}, Mensaje: {message}");

```
