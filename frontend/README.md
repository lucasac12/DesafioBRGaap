# Frontend - SAPUI5 Application

Aplicação frontend desenvolvida em SAPUI5 com TypeScript para interface de gerenciamento de tarefas.

## 🚀 Como Executar

### Pré-requisitos
- Node.js 14+ 
- npm ou yarn

### Executar localmente
```bash
# Instalar dependências
npm install

# Compilar TypeScript para JavaScript
npm run build

# Iniciar servidor de desenvolvimento
npm start

# OU usar UI5 tooling
ui5 serve
```

A aplicação estará disponível em: `http://localhost:8080`

## 🎯 Funcionalidades

### Interface ✅
- Listagem de tarefas com sap.m.Table
- Campo de pesquisa funcional
- Filtros por status (All/Active/Completed)
- Visualização de detalhes em popup
- Design responsivo (desktop/tablet/mobile)

### UX/UI ✅
- BusyIndicator durante carregamento
- Tratamento de erros com MessageToast
- CheckBox para status completed
- Diferenciação visual para tarefas concluídas
- Texto tachado para itens finalizados
- Background cinza (#f5f6f7) para concluídas
- Cor de texto (#878787) para concluídas

### Integração ✅
- Conexão com API ASP.NET Core
- Busca por título via API
- Exibição de ID, UserID e Title
- Refresh manual de dados
- Adição local de tarefas (simulada)

## 📁 Estrutura

```
frontend/
├── webapp/
│   ├── controller/           # Controllers TypeScript
│   │   ├── App.controller.ts # Controller principal
│   │   └── BaseController.ts # Controller base
│   ├── view/                # Views XML
│   │   └── App.view.xml     # View principal
│   ├── css/                 # Estilos
│   │   └── styles.css       # CSS customizado
│   ├── i18n/               # Internacionalização
│   │   ├── i18n.properties # Textos em inglês
│   │   └── i18n_de.properties # Textos em alemão
│   ├── Component.ts        # Componente principal
│   ├── manifest.json       # Configuração da app
│   └── index.html         # Página inicial
├── package.json           # Dependências Node.js
├── tsconfig.json         # Configuração TypeScript
├── ui5.yaml             # Configuração UI5
└── .babelrc.json       # Configuração Babel
```

## 🔧 Configuração da API

A URL da API está configurada no controller:
```typescript
private apiBaseUrl: string = "https://localhost:7283";
```

Para alterar, edite `webapp/controller/App.controller.ts` linha ~17.

## 🎨 Temas e Estilos

### Tema SAPUI5
- **Tema**: `sap_horizon` (padrão moderno)
- **Responsivo**: Adaptado para todos os dispositivos
- **Cores**: Seguindo design system SAP


## 📱 Componentes SAPUI5 Utilizados

- `sap.m.Table` - Listagem de tarefas
- `sap.m.SearchField` - Campo de busca
- `sap.m.CheckBox` - Status completed
- `sap.m.Button` - Ações e detalhes
- `sap.m.BusyIndicator` - Loading
- `sap.m.MessageToast` - Notificações
- `sap.m.MessageBox` - Detalhes em popup
- `sap.f.DynamicPage` - Layout responsivo
- `sap.f.ShellBar` - Header da aplicação

## 🔄 Fluxo de Dados

1. **Inicialização**: Carrega tarefas da API
2. **Busca**: Envia query para API backend
3. **Filtros**: Aplicados localmente na UI
4. **Detalhes**: Nova requisição para `/todos/{id}`
5. **Refresh**: Recarrega dados da API

## 🧪 Build e Deploy

### Desenvolvimento
```bash
npm run build    # Compila TS para JS
npm run watch    # Modo watch para desenvolvimento
```

### Produção
```bash
npm run build    # Build otimizado
ui5 build        # Build UI5 para produção
```

## 🌐 Compatibilidade

- **SAPUI5**: 1.108.0+
- **Browsers**: Chrome, Firefox, Safari, Edge
- **Dispositivos**: Desktop, Tablet, Mobile
- **TypeScript**: 5.0+
- **Node.js**: 14+