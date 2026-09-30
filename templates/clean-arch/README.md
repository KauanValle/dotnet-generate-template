# ApiTemplate — Template base de API .NET 10

Template de partida para APIs com **.NET 10**, organizado em camadas (Domain, Application, Infrastructure, Api) e pronto para evoluir para um projeto real. Inclui um CRUD de exemplo (`Products`) demonstrando o fluxo completo: **Controller → Service → Repository → EF Core**.

## O que já vem incluído

- ✅ .NET 10 / C# com solution no novo formato `.slnx`
- ✅ Arquitetura em camadas com inversão de dependência (a API não conhece o EF Core diretamente)
- ✅ CRUD de exemplo completo com validação e DTOs
- ✅ OpenAPI nativo + UI do [Scalar](https://github.com/scalar/scalar) (substituto do Swagger UI)
- ✅ Health check em `/health`
- ✅ Tratamento global de erros com **ProblemDetails** (RFC 9457): 400 para validação, 404 para não encontrado, 500 genérico sem vazar detalhes
- ✅ EF Core com provider **InMemory** por padrão (roda sem banco) e troca para **SQL Server** por configuração
- ✅ Auditoria automática (`CreatedAtUtc`/`UpdatedAtUtc`) no `SaveChangesAsync` do contexto
- ✅ Testes com xUnit: unitários de serviço + testes de integração com `WebApplicationFactory`
- ✅ Dockerfile multi-stage + docker-compose
- ✅ CI pronta com GitHub Actions
- ✅ `Directory.Build.props` (TFM e configurações centralizados), `global.json` (fixa o SDK 10), `.editorconfig` e `.gitignore`

## Estrutura

```
dotnet10-api-template/
├── ApiTemplate.slnx
├── Directory.Build.props        # TargetFramework/Nullable/ImplicitUsings centralizados
├── global.json                  # Fixa o SDK do .NET 10
├── Dockerfile
├── docker-compose.yml
├── .github/workflows/ci.yml     # build + test no push/PR
├── src/
│   ├── Api/                     # ASP.NET Core (controllers, middleware, Program.cs)
│   │   ├── Controllers/ProductsController.cs
│   │   ├── ExceptionHandling/GlobalExceptionHandler.cs
│   │   └── Program.cs
│   ├── Application/             # Casos de uso, DTOs, validação e abstrações
│   │   ├── Common/Exceptions/   # NotFoundException, ValidationException
│   │   ├── Common/Interfaces/   # IRepository<T> (implementado pela Infrastructure)
│   │   └── Products/            # IProductService, ProductService, DTOs
│   ├── Domain/                  # Entidades e regras (sem dependências)
│   │   ├── Common/BaseEntity.cs
│   │   └── Entities/Product.cs
│   └── Infrastructure/          # EF Core, repositórios, acesso externo
│       ├── Data/AppDbContext.cs
│       ├── Persistence/Repository.cs
│       ├── DependencyInjection.cs
│       └── DatabaseInitialization.cs
└── tests/
    └── ApiTemplate.Tests/       # unitários + integração (WebApplicationFactory)
```

**Fluxo de dependências:** `Api → Application + Infrastructure`, `Infrastructure → Application + Domain`, `Application → Domain`. O Domain não referencia nada.

## Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Docker (opcional, para rodar via compose)

## Rodando a API

```bash
dotnet run --project src/Api
```

| URL | Descrição |
| --- | --- |
| http://localhost:5080/scalar | UI do Scalar (documentação interativa) |
| http://localhost:5080/openapi/v1.json | Documento OpenAPI em JSON |
| http://localhost:5080/health | Health check |
| http://localhost:5080/api/products | CRUD de exemplo |

> A API sobe com 3 produtos de exemplo já cadastrados (seed via `HasData`).

## Endpoints de exemplo

```bash
# listar
curl http://localhost:5080/api/products

# criar
curl -X POST http://localhost:5080/api/products \
  -H "Content-Type: application/json" \
  -d '{"name": "Monitor 27", "description": "144 Hz", "price": 1299.90}'

# obter por id
curl http://localhost:5080/api/products/{id}

# atualizar
curl -X PUT http://localhost:5080/api/products/{id} \
  -H "Content-Type: application/json" \
  -d '{"name": "Monitor 27", "description": "165 Hz", "price": 1399.90, "isActive": true}'

# remover
curl -X DELETE http://localhost:5080/api/products/{id}
```

Erros de validação retornam 400 com ProblemDetails:

```json
{
  "title": "Validation failed",
  "status": 400,
  "errors": {
    "Price": ["Price must be greater than zero."]
  }
}
```

## Testes

```bash
dotnet test
```

- `tests/Products/ProductServiceTests.cs` — testes unitários do serviço usando EF InMemory
- `tests/Integration/ProductsApiTests.cs` — testes de ponta a ponta via `WebApplicationFactory<Program>`

## Docker

```bash
docker compose up --build
# API em http://localhost:8080
```

O compose usa o provider InMemory por padrão. Veja os comentários no `docker-compose.yml` para ligar o SQL Server.

## Trocando o banco de dados (InMemory → SQL Server)

1. Em `src/Api/appsettings.json`, troque o provider e ajuste a connection string:

```json
"Database": {
  "Provider": "SqlServer"
},
"ConnectionStrings": {
  "SqlServer": "Server=localhost,1433;Database=ApiTemplate;User Id=sa;Password=Your_password123;TrustServerCertificate=True"
}
```

2. Gere e aplique as migrations (os seeds do `HasData` entram junto):

```bash
dotnet tool install --global dotnet-ef   # se ainda não tiver
dotnet ef migrations add InitialCreate -p src/Infrastructure -s src/Api
dotnet ef database update -p src/Infrastructure -s src/Api
```

A troca acontece inteiramente em `src/Infrastructure/DependencyInjection.cs` — nenhum outro projeto muda.

## Reutilizando o template para um novo projeto

1. Copie a pasta e apague `bin/`, `obj/` e `.git` (se existirem).
2. Renomeie a solution (`ApiTemplate.slnx`), as pastas em `src/` e `tests/` e os arquivos `.csproj`.
3. Atualize os `ProjectReference` dentro dos `.csproj`.
4. Faça um *search & replace* global de `ApiTemplate` pelo nome do seu projeto (namespaces seguem o nome dos projetos).
5. `dotnet build` para confirmar.

> No Visual Studio, o passo 2–4 pode ser feito com *rename* dos projetos pela Solution Explorer (ele atualiza as referências); ainda assim o replace global dos namespaces é necessário.

## Próximos passos sugeridos

- **Autenticação/autorização**: JWT Bearer (`AddAuthentication().AddJwtBearer()`) antes de expor publicamente
- **Validação**: trocar a validação manual por [FluentValidation](https://docs.fluentvalidation.net/) nos requests
- **Logging estruturado**: Serilog com sink no Seq/ELK
- **Paginação**: `PagedResult<T>` para listagens
- **Versionamento de API**: pacote `Asp.Versioning`
- **Observabilidade**: OpenTelemetry (traces + metrics)

## Decisões de design

- **Controllers em vez de Minimal APIs**: mais explícito para base de código que cresce; o padrão é idêntico para novos recursos (Controller fino → Service com regra → Repository).
- **Repositório genérico** (`IRepository<T>`): mantém a Application testável sem EF; se preferir consultas ricas (projeções, includes), adicione interfaces específicas por agregado.
- **Exceções de domínio + handler global**: controllers ficam sem `try/catch`; o `GlobalExceptionHandler` converte `NotFoundException`/`ValidationException` em 404/400.
- **InMemory como padrão**: objetivo é rodar o template imediatamente sem infraestratura; é só trocar uma chave de configuração para SQL Server.
