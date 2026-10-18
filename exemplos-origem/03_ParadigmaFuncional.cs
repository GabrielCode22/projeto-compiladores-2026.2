namespace ExemplosOrigem;

/// <summary>
/// Registro imutável: uma vez criado, não muda. Cópias modificadas usam "with".
/// </summary>
public record ProdutoLoja(string Nome, string Categoria, decimal Preco, int Estoque);

/// <summary>
/// TEMA: Paradigmas — Funcional.
///
/// C# é multiparadigma e oferece vários recursos funcionais:
///   - funções como valores (Func/Action/lambdas) e funções de ordem superior;
///   - closures, currying e composição de funções;
///   - dados imutáveis (record + with);
///   - pipelines declarativos com LINQ e avaliação preguiçosa (lazy);
///   - recursão e Aggregate (fold) no lugar de laços com estado.
///
/// Exemplo aplicável: relatórios e promoções de uma loja, sem alterar variáveis.
/// </summary>
public static class ParadigmaFuncional
{
    public static void Executar()
    {
        var produtos = new List<ProdutoLoja>
        {
            new("Notebook", "Informática", 4200m, 5),
            new("Mouse",    "Informática",   80m, 40),
            new("Teclado",  "Informática",  150m, 25),
            new("Cadeira",  "Móveis",       900m, 8),
            new("Mesa",     "Móveis",      1300m, 3),
            new("Caneta",   "Papelaria",      3m, 500),
        };

        FuncoesComoValores();
        PipelineLinq(produtos);
        Imutabilidade(produtos[0]);
        ClosuresECurrying();
        FoldERecursao();
        AvaliacaoPreguicosa();
    }

    // Função de ordem superior: recebe duas funções e devolve uma nova.
    private static Func<T, T> Compor<T>(Func<T, T> primeira, Func<T, T> segunda) =>
        x => segunda(primeira(x));

    private static void FuncoesComoValores()
    {
        Console.WriteLine("--- Funções como valores e composição ---");

        Func<decimal, decimal> desconto10 = preco => preco * 0.9m;
        Func<decimal, decimal> somarFrete = preco => preco + 15m;

        Func<decimal, decimal> precoFinal = Compor(desconto10, somarFrete);
        Console.WriteLine($"R$ 100,00 com 10% de desconto + frete = R$ {precoFinal(100m):F2}");
    }

    private static void PipelineLinq(List<ProdutoLoja> produtos)
    {
        Console.WriteLine("\n--- Pipeline LINQ (declarativo, sem laços nem estado) ---");

        var resumo = produtos
            .Where(p => p.Estoque > 0)
            .GroupBy(p => p.Categoria)
            .Select(g => new
            {
                Categoria = g.Key,
                Itens = g.Count(),
                ValorEmEstoque = g.Sum(p => p.Preco * p.Estoque)
            })
            .OrderByDescending(r => r.ValorEmEstoque);

        foreach (var r in resumo)
            Console.WriteLine($"  {r.Categoria,-12} itens: {r.Itens}  valor em estoque: R$ {r.ValorEmEstoque:F2}");

        var maisCaro = produtos.MaxBy(p => p.Preco);
        Console.WriteLine($"  Produto mais caro: {maisCaro?.Nome}");
    }

    private static void Imutabilidade(ProdutoLoja original)
    {
        Console.WriteLine("\n--- Imutabilidade: 'with' cria uma cópia alterada ---");

        ProdutoLoja promocao = original with { Preco = original.Preco * 0.8m };

        Console.WriteLine($"  Original : {original.Nome} por R$ {original.Preco:F2}");
        Console.WriteLine($"  Promoção : {promocao.Nome} por R$ {promocao.Preco:F2}");
        Console.WriteLine($"  São iguais? {original == promocao}");
    }

    private static void ClosuresECurrying()
    {
        Console.WriteLine("\n--- Closures e currying ---");

        // A lambda devolvida "lembra" o valor de 'fator' (closure).
        static Func<int, int> CriarMultiplicador(int fator) => x => x * fator;

        Func<int, int> triplo = CriarMultiplicador(3);
        Console.WriteLine($"  triplo(14) = {triplo(14)}");

        // Currying: somar(2)(3) em vez de somar(2, 3).
        Func<int, Func<int, int>> somar = a => b => a + b;
        Func<int, int> maisDez = somar(10);
        Console.WriteLine($"  maisDez(5) = {maisDez(5)}   somar(2)(3) = {somar(2)(3)}");
    }

    private static void FoldERecursao()
    {
        Console.WriteLine("\n--- Fold (Aggregate) e recursão ---");

        int fatorial5 = Enumerable.Range(1, 5).Aggregate(1, (acumulado, n) => acumulado * n);
        Console.WriteLine($"  5! via Aggregate = {fatorial5}");

        Console.WriteLine($"  Soma dos quadrados de 1..4 (recursão) = {SomaDosQuadrados(4)}");
    }

    // Recursão pura: sem variável mutável, sem laço.
    private static int SomaDosQuadrados(int n) => n == 0 ? 0 : n * n + SomaDosQuadrados(n - 1);

    private static void AvaliacaoPreguicosa()
    {
        Console.WriteLine("\n--- Avaliação preguiçosa (lazy) ---");

        // Sequência quase infinita: só calcula o que for realmente consumido.
        var quadradosDeMultiplosDeSete = Enumerable.Range(1, int.MaxValue)
            .Where(n => n % 7 == 0)
            .Select(n => n * n)
            .Take(5);

        Console.WriteLine("  " + string.Join(", ", quadradosDeMultiplosDeSete));
    }
}
