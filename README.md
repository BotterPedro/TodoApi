# TodoApi

[![CI](https://github.com/BotterPedro/TodoApi/actions/workflows/ci.yml/badge.svg)](https://github.com/BotterPedro/TodoApi/actions/workflows/ci.yml)

API REST para gerenciamento de tarefas, construída com **.NET 10**, **Entity Framework Core**, **PostgreSQL** e **Docker**. Projeto de portfólio com arquitetura em camadas, testes automatizados e boas práticas de mercado.

---

## Sobre o projeto

A **TodoApi** é uma API REST completa que permite criar, listar, atualizar, concluir e deletar tarefas. O projeto foi desenvolvido com foco em **boas práticas de arquitetura**, **testes automatizados** e **facilidade de execução** — qualquer pessoa pode rodar o projeto localmente com apenas alguns comandos.

### Funcionalidades

- CRUD completo de tarefas
- Marcar tarefa como concluída / reabrir
- Validação de regras de negócio na entidade de domínio
- Tratamento global de exceções com respostas padronizadas (`ProblemDetails`)
- Documentação interativa via Swagger
- Testes unitários e de integração
- Containerização com Docker

---

## Tecnologias

| Categoria | Tecnologia |
|-----------|------------|
| **Linguagem** | C# 13 |
| **Plataforma** | .NET 10 |
| **Framework Web** | ASP.NET Core Web API |
| **ORM** | Entity Framework Core 10 |
| **Banco de dados** | PostgreSQL 16 |
| **Containerização** | Docker + Docker Compose |
| **Documentação** | Swagger (Swashbuckle) |
| **Testes** | xUnit, EF Core InMemory, WebApplicationFactory |

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
- **Localmente** — precisa do .NET SDK e do PostgreSQL.

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
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (apenas para o PostgreSQL)
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

**4. Aplique as migrations:**

```bash
dotnet ef database update -p src/TodoApi.Infrastructure -s src/TodoApi.Api
```

**5. Rode a API:**

```bash
dotnet run --project src/TodoApi.Api
```

**6. Acesse o Swagger:**

A porta exata aparece no terminal quando a API inicia (algo como `https://localhost:7123`). Abra:

```
https://localhost:<porta>/swagger
```
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
| `GET` | `/api/tarefas` | Lista todas as tarefas |
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
| `404 Not Found` | Recurso não encontrado |
| `500 Internal Server Error` | Erro inesperado no servidor |

---

## Testes

O projeto possui **cobertura de testes em três camadas**, garantindo que todas as partes críticas do código funcionem conforme esperado.

| Camada | O que testa | Quantidade |
|--------|-------------|------------|
| **Unitários (Domain)** | Regras da entidade `Tarefa` | 10 |
| **Unitários (Application)** | Lógica do `TarefaService` com banco em memória | 12 |
| **Integração** | API completa via HTTP com `WebApplicationFactory` | 12 |
| **Total** | | **34** |

### Como rodar os testes

```bash
dotnet test
```

**Resultado esperado:**

```
Resumo do teste: total: 34; falhou: 0; bem-sucedido: 34; ignorado: 0
```

### Tipos de teste

- **Testes unitários** — validam uma classe isolada, sem dependências externas (banco, HTTP). Rápidos e focados.
- **Testes de integração** — sobem a API inteira em memória e enviam requisições HTTP reais. Validam o fluxo completo de ponta a ponta.

---

## Docker

O projeto inclui um `docker-compose.yml` que sobe o PostgreSQL. Para subir:

```bash
docker compose up -d
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

## Decisões de arquitetura

Algumas decisões importantes tomadas durante o desenvolvimento:

- **Clean Architecture em camadas** — facilita testes, manutenção e evolução do projeto.
- **Entity Framework Core com migrations** — o schema do banco é versionado junto com o código.
- **DTOs separados** — a entidade de domínio `Tarefa` nunca é exposta diretamente pela API.
- **Regras de negócio no domínio** — a entidade `Tarefa` protege seu próprio estado (setters privados, métodos `Concluir`, `Reabrir`, `Atualizar`).
- **Tratamento global de exceções** — `ArgumentException` retorna `400`, exceções inesperadas retornam `500`, ambos com `ProblemDetails`.
- **Testes em três camadas** — unitários para o domínio, unitários para serviços com InMemory, integração para a API HTTP.

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
- [ ] Autenticação JWT com Identity
- [ ] Serilog para logs estruturados
- [ ] CI/CD com GitHub Actions
- [ ] Deploy em cloud (Azure / Railway)

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