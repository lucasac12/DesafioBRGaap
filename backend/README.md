# Backend - ASP.NET Core API

API REST desenvolvida em ASP.NET Core 8.0 para gerenciamento de tarefas.

## 🚀 Como Executar

### Pré-requisitos
- .NET 8.0 SDK
- Visual Studio 2022 ou VS Code

### Executar localmente
```bash
# Restaurar dependências
dotnet restore

# Executar a aplicação
dotnet run

# OU usar o Visual Studio
# Abrir DesafioBRGaap.sln e pressionar F5
```

A API estará disponível em: `https://localhost:7283`

## 📖 Endpoints

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/todos` | Lista todas as tarefas |
| GET | `/todos?title=texto` | Busca tarefas por título |
| GET | `/todos/{id}` | Detalhes de uma tarefa específica |
| POST | `/api/sync/force` | Força sincronização com API externa |
| GET | `/api/sync/status` | Status da sincronização |
| GET | `/api/sync/statistics` | Estatísticas do banco local |
| DELETE | `/api/sync/clear` | Limpa dados locais |

## 🧪 Executar Testes

```bash
# Executar todos os testes
dotnet test

# Executar com cobertura
dotnet test --collect:"XPlat Code Coverage"
```

## 🗄️ Banco de Dados

- **Desenvolvimento**: SQLite (`Data/tarefas.db`)
- **ORM**: Entity Framework Core
- **Migrations**: Criadas automaticamente na primeira execução

## 📁 Estrutura

```
DesafioBRGaap/
├── Controllers/           # Controllers da API
│   ├── TodosController.cs # Endpoints principais
│   └── SyncController.cs  # Sincronização
├── Models/               # Modelos de dados
│   ├── Tarefa.cs        # Modelo principal
│   └── TarefaContext.cs # DbContext
├── Services/            # Lógica de negócio
│   ├── ITarefaService.cs
│   ├── ITarefaLocalService.cs
│   ├── TarefaService.cs
│   └── TarefaLocalService.cs
└── Data/               # Banco SQLite (criado automaticamente)
```

## 🔧 Configurações

### appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=Data/tarefas.db"
  }
}
```

## 🌐 CORS

Configurado para permitir requisições do frontend SAPUI5:
- Origin: `http://localhost:8080`
- Methods: GET, POST, PUT, DELETE
- Headers: Todos permitidos

## 📊 Funcionalidades

### Implementadas ✅
- Proxy para JSONPlaceholder API
- Persistência local com SQLite
- Busca por título
- Sincronização automática
- Testes unitários
- Documentação Swagger
- CORS configurado
- Logging estruturado

### Bônus ✅
- Entity Framework Core
- Testes de integração
- Endpoints de sincronização
- Estatísticas do banco
- Arquitetura em camadas