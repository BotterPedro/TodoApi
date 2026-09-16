namespace TodoApi.Domain.Entities
{
    public class Tarefa
    {
        public Guid Id { get; private set; }
        public string Titulo { get; private set; } = string.Empty;
        public string? Descricao { get; private set; }
        public bool Concluida { get; private set; }
        public DateTime CriadaEm { get; private set; }
        public DateTime? ConcluidaEm { get; private set; }
        private Tarefa() { }
        public Tarefa(string titulo, string? descricao = null)
        {
            if (string.IsNullOrWhiteSpace(titulo))
                throw new ArgumentException("O título da tarefa não pode ser vazio.", nameof(titulo));
            
            Id = Guid.NewGuid();
            Titulo = titulo.Trim();
            Descricao = descricao?.Trim();
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
                throw new ArgumentException("O título da tarefa não pode ser vazio.", nameof(titulo));
            
            Titulo = titulo.Trim();
            Descricao = descricao?.Trim();
        }

    }
}