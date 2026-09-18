namespace TodoApi.Domain.Entities;

public class Tarefa
{
    public Guid Id { get; private set; }
    public string Titulo { get; private set; } = string.Empty;
    public string? Descricao { get; private set; }
    public bool Concluida { get; private set; }
    public DateTime CriadaEm { get; private set; }
    public DateTime? ConcluidaEm { get; private set; }

    public Guid UsuarioId { get; private set; }

    private Tarefa() { }

    public Tarefa(string titulo, string? descricao, Guid usuarioId)
    {
        if (string.IsNullOrWhiteSpace(titulo))
            throw new ArgumentException("Título é obrigatório.", nameof(titulo));

        if (usuarioId == Guid.Empty)
            throw new ArgumentException("Usuário é obrigatório.", nameof(usuarioId));

        Id = Guid.NewGuid();
        Titulo = titulo.Trim();
        Descricao = descricao?.Trim();
        UsuarioId = usuarioId;
        Concluida = false;
        CriadaEm = DateTime.UtcNow;
    }

    public void Concluir()
    {
        if (Concluida) return;
        Concluida = true;
        ConcluidaEm = DateTime.UtcNow;
    }

    public void Reabrir()
    {
        Concluida = false;
        ConcluidaEm = null;
    }

    public void Atualizar(string titulo, string? descricao)
    {
        if (string.IsNullOrWhiteSpace(titulo))
            throw new ArgumentException("Título é obrigatório.", nameof(titulo));

        Titulo = titulo.Trim();
        Descricao = descricao?.Trim();
    }
}