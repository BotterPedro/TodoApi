using TodoApi.Domain.Entities;

namespace TodoApi.UnitTests.Domain.Entities;

public class TarefaTests
{
    [Fact]
    public void Construtor_ComTituloValido_DeveCriarTarefa()
    {
        
        var titulo = "Estudar C#";
        var descricao = "Terminar a API";

        
        var tarefa = new Tarefa(titulo, descricao);

        
        Assert.NotEqual(Guid.Empty, tarefa.Id);
        Assert.Equal(titulo, tarefa.Titulo);
        Assert.Equal(descricao, tarefa.Descricao);
        Assert.False(tarefa.Concluida);
        Assert.Null(tarefa.ConcluidaEm);
        Assert.True(tarefa.CriadaEm <= DateTime.UtcNow);
    }

    [Fact]
    public void Construtor_ComTituloVazio_DeveLancarExcecao()
    {
        
        Assert.Throws<ArgumentException>(() => new Tarefa(""));
    }

    [Fact]
    public void Construtor_ComTituloSoEspacos_DeveLancarExcecao()
    {
        Assert.Throws<ArgumentException>(() => new Tarefa("   "));
    }

    [Fact]
    public void Construtor_ComTituloComEspacos_DeveFazerTrim()
    {
        var tarefa = new Tarefa("  Estudar C#  ");

        Assert.Equal("Estudar C#", tarefa.Titulo);
    }

    [Fact]
    public void Construtor_SemDescricao_DeveCriarComDescricaoNula()
    {
        var tarefa = new Tarefa("Estudar C#");

        Assert.Null(tarefa.Descricao);
    }

    [Fact]
    public void Concluir_DeveMarcarComoConcluida()
    {
        var tarefa = new Tarefa("Estudar C#");

        tarefa.Concluir();

        Assert.True(tarefa.Concluida);
        Assert.NotNull(tarefa.ConcluidaEm);
    }

    [Fact]
    public void Concluir_QuandoJaConcluida_NaoDeveAlterarDataDeConclusao()
    {
        var tarefa = new Tarefa("Estudar C#");
        tarefa.Concluir();
        var primeiraData = tarefa.ConcluidaEm;

        Thread.Sleep(10);
        tarefa.Concluir();

        Assert.Equal(primeiraData, tarefa.ConcluidaEm);
    }

    [Fact]
    public void Reabrir_DeveDesmarcarConclusao()
    {
        var tarefa = new Tarefa("Estudar C#");
        tarefa.Concluir();

        tarefa.Reabrir();

        Assert.False(tarefa.Concluida);
        Assert.Null(tarefa.ConcluidaEm);
    }

    [Fact]
    public void Atualizar_DeveModificarTituloEDescricao()
    {
        var tarefa = new Tarefa("Título antigo", "Descrição antiga");

        tarefa.Atualizar("Título novo", "Descrição nova");

        Assert.Equal("Título novo", tarefa.Titulo);
        Assert.Equal("Descrição nova", tarefa.Descricao);
    }

    [Fact]
    public void Atualizar_ComTituloVazio_DeveLancarExcecao()
    {
        var tarefa = new Tarefa("Título válido");

        Assert.Throws<ArgumentException>(() => tarefa.Atualizar("", "Descrição"));
    }
}