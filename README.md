# TodoApi

[![CI](https://github.com/BotterPedro/TodoApi/actions/workflows/ci.yml/badge.svg)](https://github.com/BotterPedro/TodoApi/actions/workflows/ci.yml)

> **Front-end:** [https://botterpedro.github.io/TodoApi.Frontend/](https://botterpedro.github.io/TodoApi.Frontend/)

> **API pública:** [https://todoapi-lhrs.onrender.com/swagger](https://todoapi-lhrs.onrender.com/swagger)
>
> A primeira requisição pode demorar ~30 segundos (o plano gratuito do Render "dorme" após 15 minutos de inatividade).

API REST para gerenciamento de tarefas, construída com **.NET 10**, **Entity Framework Core**, **PostgreSQL** e **Docker**. Projeto de portfólio com arquitetura em camadas, autenticação JWT, testes automatizados, CI/CD e deploy em nuvem.

---

## Sobre o projeto

A **TodoApi** é uma API REST completa que permite criar, listar, atualizar, concluir e deletar tarefas. O projeto foi desenvolvido com foco em **boas práticas de arquitetura**, **testes automatizados**, **autenticação segura** e **facilidade de execução** — qualquer pessoa pode rodar o projeto localmente com poucos comandos, ou acessar a versão pública diretamente.

### Funcionalidades

- CRUD completo de tarefas
- Marcar tarefa como concluída / reabrir
- Cadastro e login de usuários com **JWT**
- Senhas armazenadas com **hash BCrypt**
- Autorização por usuário — cada pessoa só vê e gerencia **as próprias tarefas**
- Validação de regras de negócio na entidade de domínio
- Tratamento global de exceções com respostas padronizadas (`ProblemDetails`)
- Documentação interativa via **Swagger**
- Logs estruturados com **Serilog** (console e arquivo)
- Testes unitários e de integração
- Containerização com **Docker**
- CI/CD com **GitHub Actions**
- Deploy público em **Render** + **Neon**

---

## Tecnologias

| Categoria | Tecnologia |
|-----------|------------|
| **Linguagem** | C# 13 |
| **Plataforma** | .NET 10 |
| **Framework Web** | ASP.NET Core Web API |
| **ORM** | Entity Framework Core 10 |
| **Banco de dados** | PostgreSQL 16 (Neon em produção) |
| **Autenticação** | JWT (JSON Web Token) + BCrypt |
| **Containerização** | Docker + Docker Compose |
| **Documentação** | Swagger (Swashbuckle) |
| **Logs** | Serilog (console + arquivo) |
| **Testes** | xUnit, EF Core InMemory, WebApplicationFactory |
| **CI/CD** | GitHub Actions |
| **Deploy** | Render (API) + Neon (PostgreSQL) |

---

## Arquitetura

O projeto segue princípios de **Clean Architecture** com separação em camadas bem definidas:

```
TodoApi/
├── src/
│   ├── TodoApi.Api/              → Controllers, Program.cs, middlewares
│   ├── TodoApi.Application/      → DTOs, interfaces de serviços
│   ├── TodoApi.Domain/           → Entidades, regras de negócio
│   └── TodoApi.Infrastructure/   → EF Core, DbContext, serviços, migrations
└── tests/
    ├── TodoApi.UnitTests/        → Testes unitários (Domain + Services)
    └── TodoApi.IntegrationTests/ → Testes de integração (HTTP)
```

### Fluxo de uma requisição

```
Cliente HTTP → Controller → Service → DbContext → PostgreSQL
                  ↑            ↑
            valida HTTP   regras de negócio
```

### Princípios aplicados

- **Dependency Inversion** — a regra de negócio não conhece infraestrutura.
- **Dependency Injection** — uso extensivo do container de DI do .NET.
- **DTOs** — separação clara entre entrada/saída da API e entidades de domínio.
- **Separation of Concerns** — cada camada tem uma responsabilidade única.
- **Programação assíncrona** — todas as operações de I/O usam `async/await`.

---

## Como rodar o projeto

Existem **duas formas** de rodar o projeto:

- **Com Docker** (recomendado) — mais simples, só precisa do Docker instalado.
- **Localmente** — precisa do .NET SDK e de um PostgreSQL (local ou Neon).

### Opção 1: Com Docker (recomendado)

**Pré-requisitos:**

- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [Git](https://git-scm.com/downloads)

**Passo a passo:**

**1. Clone o repositório:**

```bash
git clone https://github.com/BotterPedro/TodoApi.git
cd TodoApi
```

**2. Suba os containers:**

```bash
docker compose up --build
```

Esse comando vai:

- Construir a imagem da API a partir do `Dockerfile`.
- Subir o PostgreSQL.
- Subir a API.
- Aplicar as migrations automaticamente.

**3. Acesse o Swagger:**

Abra o navegador em:

```
http://localhost:8080/swagger
```

**4. Para parar os containers:**

```bash
docker compose down
```

**Para apagar também os dados do banco:**

```bash
docker compose down -v
```

### Opção 2: Localmente

**Pré-requisitos:**

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Um banco PostgreSQL (Docker local ou Neon gratuito)
- [Git](https://git-scm.com/downloads)

**Passo a passo:**

**1. Clone o repositório:**

```bash
git clone https://github.com/BotterPedro/TodoApi.git
cd TodoApi
```

**2. Suba apenas o PostgreSQL via Docker:**

```bash
docker compose up postgres -d
```

**3. Instale as ferramentas do Entity Framework Core (uma vez só):**

```bash
dotnet tool install --global dotnet-ef
```

**4. Configure as variáveis de ambiente:**

Edite `src/TodoApi.Api/appsettings.Development.json` e ajuste:

- `ConnectionStrings:Default` — a connection string do seu PostgreSQL.
- `Jwt:Secret` — uma chave longa (mínimo 32 caracteres) para assinar tokens.

**5. Aplique as migrations:**

```bash
dotnet ef database update -p src/TodoApi.Infrastructure -s src/TodoApi.Api
```

**6. Rode a API:**

```bash
dotnet run --project src/TodoApi.Api
```

**7. Acesse o Swagger:**

A porta exata aparece no terminal quando a API inicia (algo como `http://localhost:5062`). Abra:

```
http://localhost:<porta>/swagger
```

---

## Autenticação

A API usa **JWT (JSON Web Token)** para autenticação. Toda requisição aos endpoints de tarefas precisa enviar um token válido no cabeçalho `Authorization`.

### Fluxo básico

**1. Registre um usuário:**

```http
POST /api/auth/registrar
Content-Type: application/json

{
  "nome": "Seu Nome",
  "email": "seu@email.com",
  "senha": "senha123"
}
```

**Resposta:**

```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "nome": "Seu Nome",
  "email": "seu@email.com",
  "expiraEm": "2026-09-18T23:00:00Z"
}
```

**2. Faça login (se já tiver conta):**

```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "seu@email.com",
  "senha": "senha123"
}
```

**3. Use o token nas requisições seguintes:**

```http
GET /api/tarefas
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### Regras

- A senha é armazenada com **hash BCrypt** (nunca em texto puro).
- O token expira em **8 horas** (configurável).
- Cada usuário só vê e gerencia **as próprias tarefas**.
- Requisições sem token ou com token inválido retornam **401 Unauthorized**.

### Endpoints de autenticação

| Método | Rota | Descrição |
|--------|------|-----------|
| `POST` | `/api/auth/registrar` | Cria um usuário novo e retorna o token |
| `POST` | `/api/auth/login` | Autentica e retorna o token |

---

## Endpoints da API

| Método | Rota | Descrição |
|--------|------|-----------|
| `GET` | `/api/tarefas` | Lista todas as tarefas do usuário autenticado |
| `GET` | `/api/tarefas/{id}` | Busca uma tarefa por ID |
| `POST` | `/api/tarefas` | Cria uma nova tarefa |
| `PUT` | `/api/tarefas/{id}` | Atualiza título e descrição |
| `PATCH` | `/api/tarefas/{id}/concluir` | Marca como concluída |
| `PATCH` | `/api/tarefas/{id}/reabrir` | Reabre uma tarefa concluída |
| `DELETE` | `/api/tarefas/{id}` | Remove uma tarefa |

### Exemplos de requisição

**Criar uma tarefa (POST `/api/tarefas`)**

```json
{
  "titulo": "Estudar C#",
  "descricao": "Terminar a API de tarefas"
}
```

**Resposta `201 Created`:**

```json
{
  "id": "f67c397b-8b0f-4623-9e36-2f1e705d4156",
  "titulo": "Estudar C#",
  "descricao": "Terminar a API de tarefas",
  "concluida": false,
  "criadaEm": "2026-09-15T18:56:14.7076252",
  "concluidaEm": null
}
```

**Listar tarefas (GET `/api/tarefas`)**

**Resposta `200 OK`:**

```json
[
  {
    "id": "f67c397b-8b0f-4623-9e36-2f1e705d4156",
    "titulo": "Estudar C#",
    "descricao": "Terminar a API de tarefas",
    "concluida": false,
    "criadaEm": "2026-09-15T18:56:14.7076252",
    "concluidaEm": null
  }
]
```

### Códigos de status

| Código | Significado |
|--------|-------------|
| `200 OK` | Requisição bem-sucedida |
| `201 Created` | Recurso criado com sucesso |
| `204 No Content` | Sucesso sem corpo de resposta (DELETE) |
| `400 Bad Request` | Dados inválidos (ex: título vazio) |
| `401 Unauthorized` | Token ausente ou inválido |
| `404 Not Found` | Recurso não encontrado |
| `500 Internal Server Error` | Erro inesperado no servidor |

---

## Testes

O projeto possui **cobertura de testes em três camadas**, garantindo que todas as partes críticas do código funcionem conforme esperado.

| Camada | O que testa | Quantidade |
|--------|-------------|------------|
| **Unitários (Domain)** | Regras da entidade `Tarefa` | 11 |
| **Unitários (Application)** | Lógica do `TarefaService` com banco em memória | 15 |
| **Integração** | API completa via HTTP com `WebApplicationFactory` | 16 |
| **Total** | | **42** |

### Como rodar os testes

```bash
dotnet test
```

**Resultado esperado:**

```
Resumo do teste: total: 42; falhou: 0; bem-sucedido: 42; ignorado: 0
```

### Tipos de teste

- **Testes unitários** — validam uma classe isolada, sem dependências externas (banco, HTTP). Rápidos e focados.
- **Testes de integração** — sobem a API inteira em memória e enviam requisições HTTP reais, incluindo fluxo de autenticação. Validam o comportamento completo de ponta a ponta.

---

## Docker

O projeto inclui um `docker-compose.yml` que sobe **a API e o PostgreSQL** juntos. Para subir tudo:

```bash
docker compose up --build
```

Para subir apenas o PostgreSQL (modo desenvolvimento):

```bash
docker compose up postgres -d
```

Para parar:

```bash
docker compose down
```

Para parar e **apagar os dados** do banco (reset completo):

```bash
docker compose down -v
```

> Atenção: o `-v` apaga o volume de dados. Use apenas quando quiser resetar o banco do zero.

---

## Logs com Serilog

O projeto usa **Serilog** para logs estruturados, com dois destinos configurados:

- **Console** — para visualização em tempo real.
- **Arquivo** — gravado em `logs/todoapi-YYYYMMDD.log`, rotacionado diariamente, mantendo os últimos 7 dias.

Exemplo de log:

```
[14:22:25 INF] Iniciando a API TodoApi...
[14:22:25 INF] Now listening on: http://localhost:5062
[14:22:25 INF] Application started. Press Ctrl+C to shut down.
```

Os logs em arquivo são formatados com timestamp completo, nível e mensagem, permitindo análise posterior. A pasta `logs/` está no `.gitignore` e nunca vai para o repositório.

---

## CI/CD com GitHub Actions

O repositório possui um workflow do **GitHub Actions** que roda a cada `push` e `pull request` na branch `main`:

1. Faz checkout do código.
2. Instala o .NET 10 SDK.
3. Restaura as dependências NuGet.
4. Compila em modo Release.
5. Roda **os 42 testes**.

Se qualquer etapa falhar, o GitHub envia uma notificação e o PR fica marcado como vermelho. O status é exibido no badge no topo deste README.

Arquivo do workflow: `.github/workflows/ci.yml`.

---

## Deploy

A aplicação está deployada em produção com dois serviços gratuitos:

| Componente | Serviço | URL |
|------------|---------|-----|
| **API** | Render (Docker) | https://todoapi-lhrs.onrender.com |
| **Banco de dados** | Neon (PostgreSQL Serverless) | (interno) |

**Características do deploy:**

- HTTPS automático (certificado válido).
- Migrations aplicadas automaticamente ao iniciar.
- Variáveis de ambiente configuradas no painel do Render (`ConnectionStrings__Default`, `Jwt__Secret`, etc.).
- Aplicação de logs do Serilog em produção.
- No plano gratuito, o serviço "dorme" após 15 minutos de inatividade (a primeira requisição leva ~30s).

---

## Decisões de arquitetura

Algumas decisões importantes tomadas durante o desenvolvimento:

- **Clean Architecture em camadas** — facilita testes, manutenção e evolução do projeto.
- **Entity Framework Core com migrations** — o schema do banco é versionado junto com o código.
- **DTOs separados** — a entidade de domínio `Tarefa` nunca é exposta diretamente pela API.
- **Regras de negócio no domínio** — a entidade `Tarefa` protege seu próprio estado (setters privados, métodos `Concluir`, `Reabrir`, `Atualizar`).
- **Tratamento global de exceções** — `ArgumentException` retorna `400`, `UnauthorizedAccessException` retorna `401`, exceções inesperadas retornam `500`, todos com `ProblemDetails`.
- **JWT em vez de sessão em cookie** — melhor para APIs REST consumidas por SPAs e apps móveis.
- **BCrypt em vez de Identity** — implementação "do zero" para fins de aprendizado, usando a biblioteca consolidada BCrypt para hash de senhas.
- **Serilog** — logs estruturados, essenciais para análise em produção.
- **Migração automática no startup** — simplicidade em ambiente de estudo. Em produção séria, migrations seriam aplicadas por uma etapa dedicada de CI/CD.
- **Testes em três camadas** — unitários para o domínio, unitários para serviços com InMemory, integração para a API HTTP completa.

---

## Roadmap

- [x] Estrutura da solução em camadas
- [x] Entidade `Tarefa` com regras de negócio
- [x] EF Core + PostgreSQL + Migrations
- [x] CRUD completo com DTOs
- [x] Swagger com documentação
- [x] Testes unitários (Domain + Application)
- [x] Testes de integração (HTTP)
- [x] Containerizar a própria API
- [x] Autenticação JWT com BCrypt
- [x] Autorização por usuário (cada um vê as próprias tarefas)
- [x] Serilog para logs estruturados
- [x] CI/CD com GitHub Actions
- [x] Deploy público (Render + Neon)
- [ ] Front-end Blazor WebAssembly consumindo a API

---

## Contribuindo

Contribuições são bem-vindas. Se você encontrou um bug ou tem sugestão de melhoria:

1. Faça um fork do projeto.
2. Crie uma branch para sua feature (`git checkout -b feature/MinhaFeature`).
3. Faça commit das suas mudanças (`git commit -m 'feat: adiciona MinhaFeature'`).
4. Faça push para a branch (`git push origin feature/MinhaFeature`).
5. Abra um Pull Request.

---

## Licença

Este projeto está sob a licença **MIT**. Sinta-se livre para usar, estudar e modificar.

---

## Autor

**Pedro Botter**

- GitHub: [@BotterPedro](https://github.com/BotterPedro)
- LinkedIn: [pedro-botter](https://www.linkedin.com/in/pedro-botter-22b936437/)
- E-mail: pedrobotter.s@gmail.com

---

## Agradecimentos

Projeto desenvolvido como parte de uma trilha de aprendizado em **C# e .NET**, com foco em construir um portfólio profissional.

Se este projeto te ajudou de alguma forma, considere dar uma estrela no repositório.