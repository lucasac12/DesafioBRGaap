# Desafio Técnico BR Gaap - Full Stack

Sistema completo de gerenciamento de tarefas desenvolvido com **ASP.NET Core** (backend) e **SAPUI5** (frontend).

## 🎯 Objetivo

Criar uma aplicação web full-stack que consuma e apresente uma lista de tarefas, permitindo busca, visualização de detalhes e interação com os dados.

## 🏗️ Arquitetura

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   SAPUI5        │    │  ASP.NET Core   │    │ JSONPlaceholder │
│   Frontend      │────│     API         │────│      API        │
│                 │    │                 │    │                 │
└─────────────────┘    └─────────────────┘    └─────────────────┘
```

## 📋 Funcionalidades Implementadas

### Backend (ASP.NET Core)
- ✅ API REST com endpoints `/todos` e `/todos/{id}`
- ✅ Busca por título via query string (`?title=expedita`)
- ✅ Proxy para JSONPlaceholder API
- ✅ Persistência local com Entity Framework Core (SQLite)
- ✅ Testes unitários com xUnit
- ✅ CORS configurado para frontend
- ✅ Swagger/OpenAPI documentação

### Frontend (SAPUI5)
- ✅ Listagem de tarefas com sap.m.Table
- ✅ Campo de pesquisa funcional (busca na API)
- ✅ Visualização de detalhes em popup
- ✅ CheckBox para status completed
- ✅ BusyIndicator durante carregamento
- ✅ Tratamento de erros com MessageToast
- ✅ Design responsivo
- ✅ Diferenciação visual para tarefas concluídas

## 🚀 Como Executar

### 1. Backend
```bash
cd backend
dotnet restore
dotnet run
```
Backend rodará em: `https://localhost:7283`

### 2. Frontend
```bash
cd frontend
npm install
npm run build
npm start
```
Frontend rodará em: `http://localhost:8080`

### 3. Testes
```bash
cd backend
dotnet test
```

## 🔧 Tecnologias Utilizadas

### Backend
- **ASP.NET Core 8.0** - Framework web
- **Entity Framework Core** - ORM e persistência
- **SQLite** - Banco de dados local
- **xUnit** - Framework de testes
- **Swagger** - Documentação da API

### Frontend
- **SAPUI5** - Framework UI empresarial
- **TypeScript** - Linguagem tipada
- **Node.js** - Runtime para build
- **Babel** - Transpilador

## 📖 Endpoints da API

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/todos` | Lista todas as tarefas |
| GET | `/todos?title=texto` | Busca tarefas por título |
| GET | `/todos/{id}` | Detalhes de uma tarefa |
| POST | `/api/sync/force` | Força sincronização |
| GET | `/api/sync/status` | Status da sincronização |

## 🗂️ Estrutura do Projeto

```
DesafioBRGaap/
├── backend/                 # API ASP.NET Core
│   ├── Controllers/         # Controllers da API
│   ├── Models/             # Modelos de dados
│   ├── Services/           # Lógica de negócio
│   └── Tests/              # Testes unitários
├── frontend/               # Aplicação SAPUI5
│   ├── webapp/            # Código fonte
│   ├── controller/        # Controllers SAPUI5
│   └── view/              # Views XML
└── docs/                  # Documentação
```

## 🧪 Cobertura de Testes

- ✅ Testes unitários para Services
- ✅ Testes de integração para Controllers
- ✅ Testes de persistência local
- ✅ Cobertura de cenários de erro

## 📱 Screenshots

### Tela Principal
![Lista de Tarefas](docs/screenshots/lista-tarefas.png)

### Detalhes da Tarefa
![Detalhes](docs/screenshots/detalhes-tarefa.png)

## 🔗 Demonstração

- **API Backend**: `https://localhost:7283/swagger`
- **Frontend**: `http://localhost:8080`

## 👨‍💻 Desenvolvido por

Lucas Cordeiro - Desenvolvedor Full Stack

---

## 📝 Notas Técnicas

### Decisões de Arquitetura
- **Persistência Local**: Implementada como bônus para melhor performance
- **TypeScript**: Escolhido para melhor manutenibilidade
- **Padrão Repository**: Usado para separação de responsabilidades
- **Dependency Injection**: Configurado em ambos os projetos

### Melhorias Futuras
-  Autenticação e autorização
-  Cache distribuído (Redis)
-  Deploy em containers Docker
-  CI/CD pipeline
-  Monitoramento e logging
