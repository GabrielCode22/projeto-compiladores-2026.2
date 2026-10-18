using System.Globalization;
using System.Text;

namespace ExemplosOrigem;

// Tipo delegate: "ponteiro seguro" para qualquer método com esta assinatura.
public delegate decimal CalculoDeImposto(decimal valor);

// Métodos de extensão: parecem métodos da classe string, mas vivem numa classe estática.
public static class ExtensoesDeTexto
{
    public static string Capitalizar(this string texto)
    {
        if (string.IsNullOrWhiteSpace(texto)) return texto;

        var palavras = texto.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(p => char.ToUpper(p[0]) + p[1..].ToLower());
        return string.Join(" ", palavras);
    }

    public static int ContarPalavras(this string texto) =>
        texto.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;

    // Converte "Programação Funcional em C#!" em "programacao-funcional-em-c".
    public static string ParaSlug(this string texto)
    {
        var semAcentos = new StringBuilder();
        foreach (char c in texto.Normalize(NormalizationForm.FormD))
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) == UnicodeCategory.NonSpacingMark)
                continue;
            semAcentos.Append(char.IsLetterOrDigit(c) ? char.ToLowerInvariant(c) : '-');
        }

        return string.Join("-", semAcentos.ToString().Split('-', StringSplitOptions.RemoveEmptyEntries));
    }
}

/// <summary>
/// TEMA: Subprogramas avançados.
///
/// Recursos demonstrados:
///   - delegates, Func/Action/Predicate e lambdas;
///   - closures (lambdas que capturam variáveis);
///   - métodos genéricos com restrições (where);
///   - iteradores com yield return (sequências preguiçosas);
///   - métodos de extensão;
///   - função de ordem superior que devolve outra função (memoização).
/// </summary>
public static class SubprogramasAvancados
{
    public static void Executar()
    {
        Delegates();
        Closures();
        MetodosGenericos();
        Iteradores();
        MetodosDeExtensao();
        Memoizacao();
    }

    // ---------- Delegates ----------

    private static decimal Icms(decimal valor) => valor * 0.18m;
    private static decimal Iss(decimal valor) => valor * 0.05m;

    private static void Delegates()
    {
        Console.WriteLine("--- Delegates e lambdas ---");

        // Uma lista de "comportamentos" intercambiáveis.
        var impostos = new List<(string Nome, CalculoDeImposto Calculo)>
        {
            ("ICMS", Icms),                         // método nomeado
            ("ISS", Iss),
            ("Taxa fixa", valor => 2.50m),          // lambda
        };

        decimal produto = 200m;
        decimal total = 0m;
        foreach (var (nome, calculo) in impostos)
        {
            decimal valorDoImposto = calculo(produto);
            total += valorDoImposto;
            Console.WriteLine($"  {nome,-10} R$ {valorDoImposto:F2}");
        }
        Console.WriteLine($"  Total de impostos sobre R$ {produto:F2}: R$ {total:F2}");

        // Predicate<T> e Action<T> são delegates prontos da biblioteca padrão.
        Predicate<int> ehPar = n => n % 2 == 0;
        Action<string> anunciar = msg => Console.WriteLine($"  >> {msg}");
        anunciar($"8 é par? {ehPar(8)}; 7 é par? {ehPar(7)}");
    }

    // ---------- Closures ----------

    private static Func<int> CriarContador(int inicio)
    {
        int atual = inicio;                         // variável capturada pela lambda
        return () => atual++;
    }

    private static void Closures()
    {
        Console.WriteLine("\n--- Closures: lambdas que capturam variáveis ---");

        Func<int> contadorA = CriarContador(1);
        Func<int> contadorB = CriarContador(100);   // cada contador tem seu próprio estado

        Console.WriteLine($"  A: {contadorA()}, {contadorA()}, {contadorA()}");
        Console.WriteLine($"  B: {contadorB()}, {contadorB()}");
    }

    // ---------- Genéricos ----------

    // 'where T : IComparable<T>' garante que dois T podem ser comparados.
    private static T Maior<T>(IEnumerable<T> itens) where T : IComparable<T>
    {
        using var enumerador = itens.GetEnumerator();
        if (!enumerador.MoveNext())
            throw new InvalidOperationException("A sequência está vazia.");

        T maior = enumerador.Current;
        while (enumerador.MoveNext())
        {
            if (enumerador.Current.CompareTo(maior) > 0)
                maior = enumerador.Current;
        }
        return maior;
    }

    private static void Trocar<T>(ref T a, ref T b) => (a, b) = (b, a);

    private static void MetodosGenericos()
    {
        Console.WriteLine("\n--- Métodos genéricos ---");

        Console.WriteLine($"  Maior de inteiros : {Maior(new[] { 3, 17, 5 })}");
        Console.WriteLine($"  Maior de textos   : {Maior(new[] { "banana", "uva", "maçã" })}");
        Console.WriteLine($"  Maior de decimais : {Maior(new List<decimal> { 1.5m, 0.2m })}");

        string esquerda = "esquerda", direita = "direita";
        Trocar(ref esquerda, ref direita);
        Console.WriteLine($"  Após Trocar: {esquerda} / {direita}");
    }

    // ---------- Iteradores ----------

    // Sequência infinita: cada 'yield return' devolve um valor e pausa o método.
    private static IEnumerable<long> Fibonacci()
    {
        long a = 0, b = 1;
        while (true)
        {
            yield return a;
            (a, b) = (b, a + b);
        }
    }

    private static IEnumerable<int> ContagemRegressiva(int inicio)
    {
        for (int i = inicio; i >= 0; i--)
        {
            if (i == 13) yield break;               // encerra a sequência antecipadamente
            yield return i;
        }
    }

    private static void Iteradores()
    {
        Console.WriteLine("\n--- Iteradores com yield ---");

        var paresDeFibonacci = Fibonacci().Where(n => n % 2 == 0).Take(6);
        Console.WriteLine($"  Fibonacci pares: {string.Join(", ", paresDeFibonacci)}");
        Console.WriteLine($"  Contagem regressiva interrompida em 13: {string.Join(" ", ContagemRegressiva(16))}");
    }

    // ---------- Métodos de extensão ----------

    private static void MetodosDeExtensao()
    {
        Console.WriteLine("\n--- Métodos de extensão ---");

        string titulo = "  seminário de COMPILADORES em c#  ";
        Console.WriteLine($"  Capitalizar : \"{titulo.Capitalizar()}\"");
        Console.WriteLine($"  Palavras    : {titulo.ContarPalavras()}");
        Console.WriteLine($"  Slug        : {"Programação Funcional em C#!".ParaSlug()}");
    }

    // ---------- Função de ordem superior ----------

    // Recebe uma função e devolve uma versão dela com cache.
    private static Func<TEntrada, TSaida> Memoizar<TEntrada, TSaida>(Func<TEntrada, TSaida> funcao)
        where TEntrada : notnull
    {
        var cache = new Dictionary<TEntrada, TSaida>();
        return entrada =>
        {
            if (cache.TryGetValue(entrada, out var existente))
                return existente;

            var novo = funcao(entrada);
            cache[entrada] = novo;
            return novo;
        };
    }

    private static void Memoizacao()
    {
        Console.WriteLine("\n--- Memoização (função que devolve função) ---");

        int chamadasReais = 0;
        Func<int, long> quadradoCaro = n =>
        {
            chamadasReais++;                        // closure: conta quantas vezes o cálculo "caro" rodou
            return (long)n * n;
        };

        Func<int, long> quadradoComCache = Memoizar(quadradoCaro);

        long soma = 0;
        foreach (int n in new[] { 4, 9, 4, 4, 9, 12 })
            soma += quadradoComCache(n);

        Console.WriteLine($"  6 chamadas, soma = {soma}, execuções reais da função: {chamadasReais}");
    }
}
