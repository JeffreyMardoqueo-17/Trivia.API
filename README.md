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

## Paquete que uso

dotnet add package Microsoft.Data.SqlClient
