using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using TodoApi.Application.DTOs;

namespace TodoApi.IntegrationTests;

public class TarefasControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public TarefasControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task<string> RegistrarUsuarioAsync()
    {
        var dto = new RegistrarUsuarioDto
        {
            Nome = "Usuário Teste",
            Email = $"teste_{Guid.NewGuid()}@exemplo.com",
            Senha = "senha123"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/registrar", dto);
        response.EnsureSuccessStatusCode();

        var token = await response.Content.ReadFromJsonAsync<TokenDto>();
        return token!.Token;
    }

    private HttpClient ClienteAutenticado(string token)
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return _client;
    }

    [Fact]
    public async Task Get_SemToken_DeveRetornar401()
    {
        var response = await _client.GetAsync("/api/tarefas");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Post_SemToken_DeveRetornar401()
    {
        var response = await _client.PostAsJsonAsync("/api/tarefas",
            new CriarTarefaDto { Titulo = "Teste" });
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Post_ComDadosValidos_DeveRetornar201ComTarefa()
    {
        var token = await RegistrarUsuarioAsync();
        var client = ClienteAutenticado(token);

        var dto = new CriarTarefaDto
        {
            Titulo = "Estudar C#",
            Descricao = "Terminar a API"
        };

        var response = await client.PostAsJsonAsync("/api/tarefas", dto);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var tarefaCriada = await response.Content.ReadFromJsonAsync<TarefaDto>();
        Assert.NotNull(tarefaCriada);
        Assert.Equal("Estudar C#", tarefaCriada!.Titulo);
        Assert.False(tarefaCriada.Concluida);
    }

    [Fact]
    public async Task Post_ComTituloVazio_DeveRetornar400()
    {
        var token = await RegistrarUsuarioAsync();
        var client = ClienteAutenticado(token);

        var dto = new CriarTarefaDto { Titulo = "" };

        var response = await client.PostAsJsonAsync("/api/tarefas", dto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Get_AposCriarTarefas_DeveRetornarListaComTarefas()
    {
        var token = await RegistrarUsuarioAsync();
        var client = ClienteAutenticado(token);

        await client.PostAsJsonAsync("/api/tarefas", new CriarTarefaDto { Titulo = "Tarefa A" });
        await client.PostAsJsonAsync("/api/tarefas", new CriarTarefaDto { Titulo = "Tarefa B" });

        var response = await client.GetAsync("/api/tarefas");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var tarefas = await response.Content.ReadFromJsonAsync<List<TarefaDto>>();
        Assert.NotNull(tarefas);
        Assert.True(tarefas!.Count >= 2);
    }

    [Fact]
    public async Task Get_NaoDeveRetornarTarefasDeOutroUsuario()
    {
        var tokenA = await RegistrarUsuarioAsync();
        var clientA = ClienteAutenticado(tokenA);
        await clientA.PostAsJsonAsync("/api/tarefas", new CriarTarefaDto { Titulo = "Do usuário A" });

        var tokenB = await RegistrarUsuarioAsync();
        var clientB = ClienteAutenticado(tokenB);
        var response = await clientB.GetAsync("/api/tarefas");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var tarefas = await response.Content.ReadFromJsonAsync<List<TarefaDto>>();
        Assert.NotNull(tarefas);
        Assert.Empty(tarefas!);
    }

    [Fact]
    public async Task GetPorId_TarefaDeOutroUsuario_DeveRetornar404()
    {
        var tokenA = await RegistrarUsuarioAsync();
        var clientA = ClienteAutenticado(tokenA);
        var criarResponse = await clientA.PostAsJsonAsync("/api/tarefas",
            new CriarTarefaDto { Titulo = "Do usuário A" });
        var criada = await criarResponse.Content.ReadFromJsonAsync<TarefaDto>();

        var tokenB = await RegistrarUsuarioAsync();
        var clientB = ClienteAutenticado(tokenB);
        var response = await clientB.GetAsync($"/api/tarefas/{criada!.Id}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetPorId_QuandoExiste_DeveRetornar200ComTarefa()
    {
        var token = await RegistrarUsuarioAsync();
        var client = ClienteAutenticado(token);

        var criarResponse = await client.PostAsJsonAsync("/api/tarefas",
            new CriarTarefaDto { Titulo = "Estudar C#" });
        var criada = await criarResponse.Content.ReadFromJsonAsync<TarefaDto>();

        var response = await client.GetAsync($"/api/tarefas/{criada!.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var tarefa = await response.Content.ReadFromJsonAsync<TarefaDto>();
        Assert.NotNull(tarefa);
        Assert.Equal(criada.Id, tarefa!.Id);
        Assert.Equal("Estudar C#", tarefa.Titulo);
    }

    [Fact]
    public async Task GetPorId_QuandoNaoExiste_DeveRetornar404()
    {
        var token = await RegistrarUsuarioAsync();
        var client = ClienteAutenticado(token);

        var response = await client.GetAsync($"/api/tarefas/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Put_QuandoExiste_DeveRetornar200ComDadosAtualizados()
    {
        var token = await RegistrarUsuarioAsync();
        var client = ClienteAutenticado(token);

        var criarResponse = await client.PostAsJsonAsync("/api/tarefas",
            new CriarTarefaDto { Titulo = "Título antigo" });
        var criada = await criarResponse.Content.ReadFromJsonAsync<TarefaDto>();

        var atualizarDto = new AtualizarTarefaDto
        {
            Titulo = "Título novo",
            Descricao = "Descrição nova"
        };

        var response = await client.PutAsJsonAsync($"/api/tarefas/{criada!.Id}", atualizarDto);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var tarefa = await response.Content.ReadFromJsonAsync<TarefaDto>();
        Assert.NotNull(tarefa);
        Assert.Equal("Título novo", tarefa!.Titulo);
        Assert.Equal("Descrição nova", tarefa.Descricao);
    }

    [Fact]
    public async Task Put_QuandoNaoExiste_DeveRetornar404()
    {
        var token = await RegistrarUsuarioAsync();
        var client = ClienteAutenticado(token);

        var response = await client.PutAsJsonAsync($"/api/tarefas/{Guid.NewGuid()}",
            new AtualizarTarefaDto { Titulo = "Qualquer" });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_QuandoExiste_DeveRetornar204ESumir()
    {
        var token = await RegistrarUsuarioAsync();
        var client = ClienteAutenticado(token);

        var criarResponse = await client.PostAsJsonAsync("/api/tarefas",
            new CriarTarefaDto { Titulo = "Para deletar" });
        var criada = await criarResponse.Content.ReadFromJsonAsync<TarefaDto>();

        var response = await client.DeleteAsync($"/api/tarefas/{criada!.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var buscarResponse = await client.GetAsync($"/api/tarefas/{criada.Id}");
        Assert.Equal(HttpStatusCode.NotFound, buscarResponse.StatusCode);
    }

    [Fact]
    public async Task Delete_QuandoNaoExiste_DeveRetornar404()
    {
        var token = await RegistrarUsuarioAsync();
        var client = ClienteAutenticado(token);

        var response = await client.DeleteAsync($"/api/tarefas/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PatchConcluir_QuandoExiste_DeveMarcarComoConcluida()
    {
        var token = await RegistrarUsuarioAsync();
        var client = ClienteAutenticado(token);

        var criarResponse = await client.PostAsJsonAsync("/api/tarefas",
            new CriarTarefaDto { Titulo = "Para concluir" });
        var criada = await criarResponse.Content.ReadFromJsonAsync<TarefaDto>();

        var response = await client.PatchAsync($"/api/tarefas/{criada!.Id}/concluir", null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var tarefa = await response.Content.ReadFromJsonAsync<TarefaDto>();
        Assert.NotNull(tarefa);
        Assert.True(tarefa!.Concluida);
        Assert.NotNull(tarefa.ConcluidaEm);
    }

    [Fact]
    public async Task PatchReabrir_QuandoConcluida_DeveReabrir()
    {
        var token = await RegistrarUsuarioAsync();
        var client = ClienteAutenticado(token);

        var criarResponse = await client.PostAsJsonAsync("/api/tarefas",
            new CriarTarefaDto { Titulo = "Para reabrir" });
        var criada = await criarResponse.Content.ReadFromJsonAsync<TarefaDto>();
        await client.PatchAsync($"/api/tarefas/{criada!.Id}/concluir", null);

        var response = await client.PatchAsync($"/api/tarefas/{criada.Id}/reabrir", null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var tarefa = await response.Content.ReadFromJsonAsync<TarefaDto>();
        Assert.NotNull(tarefa);
        Assert.False(tarefa!.Concluida);
        Assert.Null(tarefa.ConcluidaEm);
    }
}