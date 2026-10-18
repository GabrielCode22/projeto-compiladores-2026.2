namespace ExemplosOrigem;

// =============================================================================
// TEMA: Paradigmas — Orientado a Objetos.
//
// Recursos demonstrados:
//   - classes, objetos, construtores e propriedades (encapsulamento);
//   - herança, classes abstratas, métodos virtuais/override e classes sealed;
//   - interfaces e polimorfismo;
//   - composição (Biblioteca "tem" itens) e membros estáticos.
//
// Exemplo aplicável: sistema de empréstimos de uma biblioteca, em que cada
// tipo de item tem prazo e regra de multa diferentes.
// =============================================================================

// Contrato: qualquer coisa que possa ser emprestada implementa esta interface.
public interface IEmprestavel
{
    bool Disponivel { get; }
    void Emprestar(string usuario);
    void Devolver();
}

// Classe abstrata: guarda o comportamento comum e obriga as filhas a
// definirem o que varia (prazo e multa).
public abstract class ItemAcervo : IEmprestavel
{
    private static int _proximoId = 1;          // membro estático: compartilhado por todas as instâncias

    public int Id { get; }
    public string Titulo { get; }
    public bool Disponivel { get; private set; } = true;   // só a própria classe altera
    public string? EmprestadoPara { get; private set; }

    protected ItemAcervo(string titulo)
    {
        if (string.IsNullOrWhiteSpace(titulo))
            throw new ArgumentException("O título é obrigatório.", nameof(titulo));

        Id = _proximoId++;
        Titulo = titulo;
    }

    public abstract int PrazoEmDias { get; }
    protected abstract decimal MultaPorDia { get; }

    // Método virtual: as filhas podem sobrescrever a regra padrão.
    public virtual decimal CalcularMulta(int diasDeAtraso) =>
        diasDeAtraso <= 0 ? 0m : diasDeAtraso * MultaPorDia;

    public void Emprestar(string usuario)
    {
        if (!Disponivel)
            throw new InvalidOperationException($"'{Titulo}' já está emprestado para {EmprestadoPara}.");

        Disponivel = false;
        EmprestadoPara = usuario;
    }

    public void Devolver()
    {
        Disponivel = true;
        EmprestadoPara = null;
    }

    public override string ToString() => $"#{Id} [{GetType().Name}] {Titulo}";
}

public sealed class Livro : ItemAcervo
{
    public string Autor { get; }

    public Livro(string titulo, string autor) : base(titulo) => Autor = autor;

    public override int PrazoEmDias => 14;
    protected override decimal MultaPorDia => 1.50m;
}

public sealed class Revista : ItemAcervo
{
    public int Edicao { get; }

    public Revista(string titulo, int edicao) : base(titulo) => Edicao = edicao;

    public override int PrazoEmDias => 7;
    protected override decimal MultaPorDia => 0.75m;
}

public sealed class Dvd : ItemAcervo
{
    public Dvd(string titulo) : base(titulo) { }

    public override int PrazoEmDias => 3;
    protected override decimal MultaPorDia => 5.00m;

    // Sobrescrita: a multa de DVD é limitada a R$ 30,00.
    public override decimal CalcularMulta(int diasDeAtraso) =>
        Math.Min(base.CalcularMulta(diasDeAtraso), 30m);
}

// Composição: a biblioteca possui uma coleção de itens, sem conhecer os tipos concretos.
public class Biblioteca
{
    private readonly List<ItemAcervo> _acervo = new();

    public string Nome { get; }
    public IReadOnlyList<ItemAcervo> Todos => _acervo;

    public Biblioteca(string nome) => Nome = nome;

    public void Adicionar(ItemAcervo item) => _acervo.Add(item);

    public IEnumerable<ItemAcervo> Disponiveis() => _acervo.Where(i => i.Disponivel);

    public ItemAcervo? Buscar(string trecho) =>
        _acervo.FirstOrDefault(i => i.Titulo.Contains(trecho, StringComparison.OrdinalIgnoreCase));
}

/// <summary>
/// Demonstração do paradigma orientado a objetos com o acervo de uma biblioteca.
/// </summary>
public static class ParadigmaOO
{
    public static void Executar()
    {
        var biblioteca = new Biblioteca("Biblioteca Central");
        biblioteca.Adicionar(new Livro("Dom Casmurro", "Machado de Assis"));
        biblioteca.Adicionar(new Revista("Superinteressante", 450));
        biblioteca.Adicionar(new Dvd("Cidade de Deus"));

        Console.WriteLine($"Acervo de {biblioteca.Nome}:");
        foreach (ItemAcervo item in biblioteca.Todos)
            Console.WriteLine($"  {item} — prazo de {item.PrazoEmDias} dia(s)");

        // Polimorfismo: a mesma chamada, comportamentos diferentes por tipo.
        Console.WriteLine("\nMulta para 10 dias de atraso:");
        foreach (ItemAcervo item in biblioteca.Todos)
        {
            string detalhe = item switch
            {
                Livro l => $"autor: {l.Autor}",
                Revista r => $"edição: {r.Edicao}",
                _ => "mídia"
            };
            Console.WriteLine($"  {item.Titulo,-20} R$ {item.CalcularMulta(10):F2}  ({detalhe})");
        }

        // Encapsulamento: o estado só muda pelos métodos da classe.
        Console.WriteLine("\nEmpréstimos:");
        ItemAcervo? filme = biblioteca.Buscar("cidade");
        if (filme is not null)
        {
            filme.Emprestar("Ana");
            Console.WriteLine($"  {filme.Titulo} emprestado para {filme.EmprestadoPara}");

            try
            {
                filme.Emprestar("Bruno");               // já emprestado: lança exceção
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"  Erro esperado: {ex.Message}");
            }
        }

        Console.WriteLine($"  Itens ainda disponíveis: {biblioteca.Disponiveis().Count()}");

        // Invariante protegida no construtor.
        try
        {
            _ = new Livro("   ", "Autor Desconhecido");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"  Erro esperado: {ex.Message}");
        }
    }
}
