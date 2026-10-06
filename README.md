# TodoApp — POC .NET 10 + CQRS + Wolverine

API de Todo List demonstrando **Wolverine** (mensageria/CQRS) + **Wolverine.Http**
(endpoints HTTP) + **Wolverine.Http.FluentValidation** (validação), numa arquitetura
em camadas `Domain → Infrastructure → Api`. Os endpoints seguem um estilo parecido com
o do **FastEndpoints**: uma classe por endpoint (vertical slice), com o contrato da
requisição, o validator e o handler no mesmo arquivo.

## Stack

| Item                | Versão        |
|---------------------|---------------|
| .NET                | 10.0          |
| WolverineFx(.Http)  | 6.46.0        |
| FluentValidation    | 12.0.0        |
| OpenAPI UI          | Scalar        |

## Estrutura

```
TodoApp.sln
src/
  TodoApp.Domain/            # Entidades + regras + contratos (sem dependências)
    Todos/
      TodoItem.cs            #   aggregate root com comportamento (Create/Complete/...)
      TodoStatus.cs
      ITodoRepository.cs     #   porta de persistência
  TodoApp.Infrastructure/    # Adaptadores (persistência)
    Persistence/
      InMemoryTodoRepository.cs
    DependencyInjection.cs   #   AddInfrastructure()
  TodoApp.Api/               # Composição + endpoints HTTP (CQRS via Wolverine)
    Program.cs               #   wiring do Wolverine / Http / FluentValidation / OpenAPI
    Auth/
      AuthenticationSetup.cs #   scaffolding OAuth (JWT Bearer) — DESLIGADO por padrão
    Features/Todos/          #   uma PASTA por endpoint (estilo FastEndpoints)
      TodoResponse.cs        #   read model + mapeamento (compartilhado)
      CreateTodo/            #   endpoint COM body => 3 arquivos
        CreateTodoEndpoint.cs
        CreateTodoRequest.cs
        CreateTodoValidator.cs
      UpdateTodo/            #   endpoint COM body => 3 arquivos
        UpdateTodoEndpoint.cs
        UpdateTodoRequest.cs
        UpdateTodoValidator.cs
      GetTodo/               #   endpoints SEM body => só o endpoint
        GetTodoEndpoint.cs
      ListTodos/
        ListTodosEndpoint.cs
      CompleteTodo/
        CompleteTodoEndpoint.cs
      DeleteTodo/
        DeleteTodoEndpoint.cs
```

> Cada feature tem seu próprio sub-namespace (`...Features.Todos.CreateTodo`), então
> endpoint, request e validator da mesma pasta se enxergam sem `using`, e o
> `TodoResponse` (no namespace pai `...Features.Todos`) fica visível por aninhamento.

## Como rodar

```bash
dotnet run --project src/TodoApp.Api
```

Abre o navegador no **Scalar** (UI de testes) em `http://localhost:5153/scalar`.
O documento OpenAPI fica em `http://localhost:5153/openapi/v1.json`.
Há também um `src/TodoApp.Api/TodoApp.Api.http` com todas as chamadas prontas.

## Endpoints

| Método | Rota                          | Descrição                        | Respostas       |
|--------|-------------------------------|----------------------------------|-----------------|
| POST   | `/api/todos`                  | Cria um todo                     | 201 / 400       |
| GET    | `/api/todos?status=`          | Lista (filtro opcional)          | 200             |
| GET    | `/api/todos/{id}`             | Busca por id                     | 200 / 404       |
| PUT    | `/api/todos/{id}`             | Atualiza título/descrição        | 200 / 400 / 404 |
| POST   | `/api/todos/{id}/complete`    | Marca como concluído (idempot.)  | 200 / 404       |
| DELETE | `/api/todos/{id}`             | Remove                           | 204 / 404       |

## Padrões demonstrados

- **Endpoint estilo FastEndpoints**: cada endpoint tem sua própria pasta em
  `Features/Todos/<Feature>/`. Endpoints com body ganham 3 arquivos
  (`<Feature>Endpoint.cs`, `<Feature>Request.cs`, `<Feature>Validator.cs`);
  endpoints sem body ficam só com o arquivo de endpoint. Roteamento por atributo
  (`[WolverinePost]`, `[WolverineGet]`, ...).
- **FluentValidation como middleware HTTP**: `opts.UseFluentValidationProblemDetailMiddleware()`
  roda os validators antes do handler e devolve **400 ProblemDetails** automaticamente.
  Os validators são registrados por assembly scanning (`AddValidatorsFromAssemblyContaining`).
- **Compound handler (load-or-404)**: `GetTodoEndpoint` e `UpdateTodoEndpoint` usam um
  método `LoadAsync` que roda antes do `Handle`; se a entidade não existe ele
  curto-circuita com `Results.NotFound()`, senão o `TodoItem` carregado "cascateia"
  para o `Handle`.
  > ⚠️ Em POST/PUT **sem body**, o Wolverine trata o primeiro parâmetro concreto como
  > corpo JSON. Por isso `CompleteTodoEndpoint` (POST sem body) faz o load dentro do
  > próprio `Handle` em vez de usar `LoadAsync`.
- **CQRS**: commands mutam via `ITodoRepository`; queries só leem. Os handlers HTTP
  são os próprios command/query handlers do Wolverine.

## Autenticação OAuth (scaffolding pronto, desligado)

O arquivo `Auth/AuthenticationSetup.cs` já traz a configuração de **JWT Bearer
(OAuth2/OIDC)**, mas desativada. Para ligar contra um IdP real (Keycloak, Auth0,
Entra ID, ...), configure em `appsettings.json` / user-secrets:

```json
"Authentication": {
  "Enabled": true,
  "Authority": "https://seu-idp/realms/todo",
  "Audience": "todo-api"
}
```

Com `Enabled: true`, o `UseAuthentication/UseAuthorization` é ativado e
`opts.RequireAuthorizeOnAll()` passa a exigir usuário autenticado em todas as rotas.

## Geração de código do Wolverine

Esta POC usa **compilação em runtime** (pacote `WolverineFx.RuntimeCompilation`),
o caminho mais simples para desenvolvimento. Para produção, prefira **codegen estático**:

```bash
dotnet run --project src/TodoApp.Api -- codegen write
```

e defina `opts.CodeGeneration.TypeLoadMode = TypeLoadMode.Static`. Outros comandos
de diagnóstico ficam disponíveis via `-- ?` (ex.: `-- describe`), graças ao
`RunJasperFxCommands`.
