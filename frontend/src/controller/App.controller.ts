import Device from "sap/ui/Device";
import Filter from "sap/ui/model/Filter";
import FilterOperator from "sap/ui/model/FilterOperator";
import JSONModel from "sap/ui/model/json/JSONModel";
import Event from "sap/ui/base/Event";
import BaseController from "./BaseController"
import ResourceModel from "sap/ui/model/resource/ResourceModel";
import ResourceBundle from "sap/base/i18n/ResourceBundle";
import ListBinding from "sap/ui/model/ListBinding";
import FilterType from "sap/ui/model/FilterType";
import Input from "sap/m/Input";
import List from "sap/m/List";
import MessageToast from "sap/m/MessageToast";
import MessageBox from "sap/m/MessageBox";

// Tipo atualizado para nossa API
type Todo = {
	id: number,
	userId: number,
	title: string,
	completed: boolean
}

/**
 * @namespace sap.m.sample.TsTodos.webapp.controller
 */
export default class App extends BaseController {
	searchFilters: Filter[];
	tabFilters: Filter[];
	searchQuery: string;
	filterKey: ("active" | "completed" | "all");
	private apiBaseUrl: string = "https://localhost:7283"; // URL da nossa API

	public onInit(): void {
		this.searchFilters = [];
		this.tabFilters = [];

		this.setModel(new JSONModel({
			isMobile: Device.browser.mobile,
			filterText: undefined,
			loading: false
		}), "view");

		// Carregar dados da API ao invés do arquivo JSON
		this.loadTodosFromAPI();
	}

	/**
	 * Carrega tarefas da nossa API
	 */
	private async loadTodosFromAPI(titleFilter?: string): Promise<void> {
		const model = this.getModel();
		const viewModel = this.getModel("view");
		
		viewModel.setProperty("/loading", true);

		try {
			// Construir URL com filtro se fornecido
			let url = `${this.apiBaseUrl}/todos`;
			if (titleFilter && titleFilter.trim()) {
				url += `?title=${encodeURIComponent(titleFilter.trim())}`;
			}

			const response = await fetch(url);
			
			if (!response.ok) {
				throw new Error(`HTTP error! status: ${response.status}`);
			}

			const todos: Todo[] = await response.json();
			
			// Atualizar modelo com dados da API
			model.setProperty("/todos", todos);
			model.setProperty("/newTodo", "");
			model.setProperty("/itemsRemovable", !titleFilter);
			
			// Atualizar contador de itens
			this.updateItemsLeftCount();
			
			viewModel.setProperty("/loading", false);
			MessageToast.show(`${todos.length} tarefas carregadas com sucesso!`);

		} catch (error) {
			console.error("Erro ao carregar tarefas:", error);
			viewModel.setProperty("/loading", false);
			MessageBox.error("Erro ao carregar tarefas da API. Verifique se o backend está rodando.");
		}
	}

	/**
	 * Busca tarefa específica por ID
	 */
	public async loadTodoDetails(todoId: number): Promise<void> {
		try {
			const response = await fetch(`${this.apiBaseUrl}/todos/${todoId}`);
			
			if (!response.ok) {
				throw new Error(`HTTP error! status: ${response.status}`);
			}

			const todo: Todo = await response.json();
			
			// Exibir detalhes em um MessageBox
			const details = `ID: ${todo.id}\nUsuário: ${todo.userId}\nTítulo: ${todo.title}\nStatus: ${todo.completed ? 'Concluída' : 'Pendente'}`;
			MessageBox.information(details, {
				title: "Detalhes da Tarefa"
			});

		} catch (error) {
			console.error("Erro ao carregar detalhes:", error);
			MessageBox.error("Erro ao carregar detalhes da tarefa.");
		}
	}

	/**
	 * NOTA: Como nossa API é read-only, vamos apenas simular localmente
	 */
	public addTodo(): void {
		const model = this.getModel();
		const newTodoTitle = model.getProperty("/newTodo");
		
		if (!newTodoTitle || !newTodoTitle.trim()) {
			MessageToast.show("Digite um título para a tarefa");
			return;
		}

		const todos: Todo[] = model.getProperty("/todos") || [];
		const newId = Math.max(...todos.map(t => t.id), 0) + 1;
		
		const newTodo: Todo = {
			id: newId,
			userId: 1, 
			title: newTodoTitle.trim(),
			completed: false
		};

		todos.push(newTodo);
		model.setProperty("/todos", todos);
		model.setProperty("/newTodo", "");
		
		this.updateItemsLeftCount();
		MessageToast.show("Tarefa adicionada localmente!");
	}

	/**
	 * NOTA: Como nossa API é read-only, vamos apenas simular localmente
	 */
	public clearCompleted(): void {
		const model = this.getModel();
		const todos: Todo[] = model.getProperty("/todos").map((todo: Todo) => Object.assign({}, todo));

		const initialCount = todos.length;
		let i = todos.length;
		while (i--) {
			const todo = todos[i];
			if (todo.completed) {
				todos.splice(i, 1);
			}
		}

		model.setProperty("/todos", todos);
		this.updateItemsLeftCount();
		
		const removedCount = initialCount - todos.length;
		if (removedCount > 0) {
			MessageToast.show(`${removedCount} tarefa(s) concluída(s) removida(s) localmente!`);
		}
	}

	/**
	 * Updates the number of items not yet completed
	 */
	public updateItemsLeftCount(): void {
		const model = this.getModel();
		const todos: Todo[] = model.getProperty("/todos") || [];

		const itemsLeft = todos.filter((todo: Todo) => !todo.completed).length;
		model.setProperty("/itemsLeftCount", itemsLeft);
	}

	/**
	 * Trigger search for specific items via API
	 */
	public onSearch(event: Event): void {
		const input = event.getSource() as Input;
		const searchValue = input.getValue();
		
		// Fazer busca na API
		this.loadTodosFromAPI(searchValue);
		
		// Limpar filtros locais pois estamos fazendo busca na API
		this.searchFilters = [];
		this.tabFilters = [];
		this.searchQuery = searchValue;
		
		// Atualizar texto do filtro
		this.updateFilterText();
	}

	/**
	 * Filtro local por status (active/completed/all)
	 */
	public onFilter(event: Event): void {
		// Reset tab filters
		this.tabFilters = [];

		// Corrigir acesso ao parâmetro
		const item = (event as any).getParameter("item");
		this.filterKey = item.getKey();

		switch (this.filterKey) {
			case "active":
				this.tabFilters.push(new Filter("completed", FilterOperator.EQ, false));
				break;
			case "completed":
				this.tabFilters.push(new Filter("completed", FilterOperator.EQ, true));
				break;
			case "all":
			default:
				// Don't use any filter
		}

		this._applyListFilters();
	}

	/**
	 * Aplicar filtros locais na lista
	 */
	public _applyListFilters(): void {
		const list = this.byId("todoList") as List;
		const binding = list.getBinding("items") as ListBinding;

		// Aplicar apenas filtros de tab (não search, pois search vai na API)
		binding.filter(this.tabFilters, "todos" as FilterType);

		this.updateFilterText();
	}

	/**
	 * Atualizar texto do filtro
	 */
	private updateFilterText(): void {
		let i18nKey;
		if (this.filterKey && this.filterKey !== "all") {
			if (this.filterKey === "active") {
				i18nKey = "ACTIVE_ITEMS";
			} else {
				i18nKey = "COMPLETED_ITEMS";
			}
			if (this.searchQuery) {
				i18nKey += "_CONTAINING";
			}
		} else if (this.searchQuery) {
			i18nKey = "ITEMS_CONTAINING";
		}

		let filterText;
		if (i18nKey) {
			const resourceModel = this.getView()?.getModel("i18n") as ResourceModel;
			const resourceBundle = resourceModel.getResourceBundle() as ResourceBundle;
			filterText = resourceBundle.getText(i18nKey, [this.searchQuery]);
		}

		this.getModel("view").setProperty("/filterText", filterText);
	}

	/**
	 * Limpar busca e recarregar todos os dados
	 */
	public clearSearch(): void {
		// Limpar campo de busca
		const searchField = this.byId("searchTodoItemsInput") as Input;
		searchField.setValue("");
		
		// Recarregar todos os dados
		this.searchQuery = "";
		this.loadTodosFromAPI();
		
		// Limpar filtros
		this.searchFilters = [];
		this._applyListFilters();
	}

	/**
	 * Refresh - recarregar dados da API
	 */
	public refreshData(): void {
		this.loadTodosFromAPI(this.searchQuery);
	}

	/**
	 * Handler para clique em item da lista - mostrar detalhes
	 */
	public onTodoItemPress(event: Event): void {
		const source = event.getSource() as any;
		const bindingContext = source.getBindingContext();
		const todo: Todo = bindingContext.getObject();
		this.loadTodoDetails(todo.id);
	}

	/**
	 * Formatter para estilo do título baseado no status
	 */
	public formatTitleStyle(completed: boolean): string {
		return completed ? "sapMTextLineThrough" : "";
	}

	/**
	 * Formatter para classe de background do item baseado no status
	 */
	public formatItemBackgroundClass(completed: boolean): string {
		return completed ? "todoItemCompleted" : "todoItemActive";
	}
}