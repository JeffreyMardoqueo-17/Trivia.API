# 📌 Configuración de Connection String con Variables de Entorno

Este documento describe **cómo se configuró y por qué** la conexión a base de datos del proyecto **TriviaGame.Api** usando **variables de entorno**, siguiendo buenas practicas profesionales.

---

## 🧠 Decisión técnica clave

Se utiliza **variables de entorno nativas del sistema operativo**, que son soportadas **por defecto** por el sistema de configuración de ASP.NET Core.

> ⚠️ Importante: `.NET NO lee archivos .env automáticamente`. Aquí **NO** se usa `.env`, sino variables de entorno reales.

---

## 🧱 Estructura del proyecto

```
TriviaGame.Api
│
├── Controllers
│   ├── AuthController.cs
│   └── TriviaController.cs
│
├── Data
│   └── DapperContext.cs
│
├── Services
│   ├── Interfaces
│   │   ├── IUserService.cs
│   │   └── ITriviaService.cs
│   │
│   ├── UserService.cs
│   └── TriviaService.cs
│
├── Models
│   ├── User.cs
│   ├── Category.cs
│   ├── Question.cs
│   └── Answer.cs
│
├── DTOs
│   ├── RegisterRequestDto.cs
│   ├── LoginRequestDto.cs
│   ├── CategoryDto.cs
│   ├── QuestionDto.cs
│   └── AnswerDto.cs
│
├── appsettings.json
├── Program.cs
└── TriviaGame.Api.csproj
```

---

## 📦 Paquetes utilizados

```bash
dotnet add package Microsoft.Data.SqlClient
dotnet add package AutoMapper --version 12.0.1
dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection 
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer --version 8.0.1



```

- **Microsoft.Data.SqlClient**: Proveedor oficial de Microsoft para conectarse a SQL Server desde .NET; permite abrir conexiones, ejecutar comandos y manejar transacciones (es la base que usa Dapper para hablar con la base de datos).

- **Dapper**: Micro-ORM de alto rendimiento que ejecuta consultas SQL de forma directa y mapea los resultados a objetos C# sin la sobrecarga de un ORM completo.

- **AutoMapper**: Librería para mapear automáticamente objetos entre capas (por ejemplo, de entidades de dominio a DTOs y viceversa), reduciendo código repetitivo y errores manuales.

- **AutoMapper.Extensions.Microsoft.DependencyInjection**: Integración de AutoMapper con el contenedor de dependencias de ASP.NET Core, permitiendo registrar perfiles y usar IMapper mediante inyección de dependencias.

- **dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer** Esto es para usar JWT

---

## Definición de la variable de entorno

En **Windows (PowerShell)**:

```powershell
$env:ConnectionStrings__DefaultConnection="Server=Server;Database=NmbreBD;Trusted_Connection=True;TrustServerCertificate=True;"
```

### Detalle importante

- `ConnectionStrings__DefaultConnection`
- El doble guion bajo (`__`) representa `:` en .NET
- Esto equivale internamente a:

```json
"ConnectionStrings": {
  "DefaultConnection": "..."
}
```

Estas son las variables apra que funcione JWT

```powershell
$env:Jwt__Key="ESTE_ES_UN_SECRET_LARGO_Y_DIFICIL_DE_ADIVINAR"
$env:Jwt__Issuer="TriviaGame.Api"
$env:Jwt__Audience="TriviaGame.Frontend"
$env:Jwt__ExpireMinutes="60"

```

---

# ⚙️ Program.cs

No se requiere configuración adicional.

ASP.NET Core **ya carga automáticamente**:

- appsettings.json
- appsettings.{Environment}.json
- Variables de entorno

Mientras exista:

```csharp
var builder = WebApplication.CreateBuilder(args);
```

---

## 🗄️ DapperContext

La clase encargada de crear la conexión a base de datos.

```csharp
public class DapperContext
    {
        private readonly string _connectionString; //la cadena de conexion

        public DapperContext(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") // esta es la que agregue en los secretos de usuario
                ?? throw new InvalidOperationException("No se encontró la connection string 'DefaultConnection'.");
        }

        public IDbConnection CreateConnection()
            => new SqlConnection(_connectionString);
    }
```

📌 Si `connectionString` es `null`, el problema **NO es Dapper**, sino la variable de entorno.

---

## 🔄 Flujo de funcionamiento

1. El sistema operativo expone la variable de entorno
2. ASP.NET Core la carga automáticamente
3. `IConfiguration` la resuelve
4. `DapperContext` obtiene la conexión
5. Los servicios usan Dapper sin conocer la cadena

➡️ Separación clara de responsabilidades

---

## 🧪 Ejecución

```bash
dotnet run
```

Si falla con:

```
No se encontró la connection string 'DefaultConnection'
```

Verificar:

- Que la terminal tenga la variable cargada
- Que el nombre sea exactamente `ConnectionStrings__DefaultConnection`
- Que no se esté ejecutando otra consola

```bash
Get-ChildItem Env:

```

> Para enlistar las variables de entorno

---

## 📎 Nota final
