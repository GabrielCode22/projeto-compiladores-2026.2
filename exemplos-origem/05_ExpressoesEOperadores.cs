namespace ExemplosOrigem;

[Flags]
public enum Permissao
{
    Nenhuma = 0,
    Leitura = 1,
    Escrita = 2,
    Execucao = 4,
    Total = Leitura | Escrita | Execucao
}

/// <summary>
/// TEMA: Expressões e operadores.
///
/// Recursos demonstrados:
///   - aritméticos, precedência, incremento pré/pós-fixo;
///   - lógicos com curto-circuito (&amp;&amp;, ||) x sem curto-circuito (&amp;, |);
///   - operadores bit a bit e enums [Flags];
///   - operadores de nulo (??, ??=, ?.), ternário, is/as;
///   - expressão switch com pattern matching (tipo, relacional, propriedade, tupla, lista).
/// </summary>
public static class ExpressoesEOperadores
{
    public static void Executar()
    {
        Aritmetica();
        Logicos();
        BitABit();
        OperadoresDeNulo();
        PatternMatching();
    }

    private static void Aritmetica()
    {
        Console.WriteLine("--- Operadores aritméticos ---");

        Console.WriteLine($"7 / 2   = {7 / 2}   (divisão inteira)");
        Console.WriteLine($"7 / 2.0 = {7 / 2.0}   (divisão real)");
        Console.WriteLine($"-7 % 3  = {-7 % 3}   (o resto tem o sinal do dividendo)");
        Console.WriteLine($"2 + 3 * 4 = {2 + 3 * 4}; (2 + 3) * 4 = {(2 + 3) * 4}");

        int i = 5;
        int resultado = i++ + ++i;      // 5 (i vira 6) + 7 (i vira 7)
        Console.WriteLine($"i = 5; i++ + ++i = {resultado}; i final = {i}");

        int total = 10;
        total += 5;                     // atribuição composta
        total *= 2;
        Console.WriteLine($"total após += 5 e *= 2: {total}");

        string situacao = total >= 18 ? "maior" : "menor";   // operador ternário
        Console.WriteLine($"ternário: {total} é {situacao} que 18");
    }

    // Método auxiliar que "anuncia" quando é avaliado, para vermos o curto-circuito.
    private static bool Avaliar(string nome, bool valor)
    {
        Console.WriteLine($"     avaliou {nome}");
        return valor;
    }

    private static void Logicos()
    {
        Console.WriteLine("\n--- Operadores lógicos e curto-circuito ---");

        Console.WriteLine("  A && B (A é falso, B nem é avaliado):");
        bool r1 = Avaliar("A", false) && Avaliar("B", true);

        Console.WriteLine("  C || D (C é verdadeiro, D nem é avaliado):");
        bool r2 = Avaliar("C", true) || Avaliar("D", false);

        Console.WriteLine("  E & F (sem curto-circuito: avalia os dois):");
        bool r3 = Avaliar("E", false) & Avaliar("F", true);

        Console.WriteLine($"  resultados: {r1}, {r2}, {r3}");
    }

    private static void BitABit()
    {
        Console.WriteLine("\n--- Operadores bit a bit e flags ---");

        int x = 0b1010;                 // 10 em binário
        Console.WriteLine($"x = {Convert.ToString(x, 2)}; x << 2 = {x << 2}; x >> 1 = {x >> 1}");
        Console.WriteLine($"x & 0b0110 = {x & 0b0110}; x | 0b0101 = {x | 0b0101}; x ^ 0b1111 = {x ^ 0b1111}");

        Permissao p = Permissao.Leitura | Permissao.Escrita;
        Console.WriteLine($"permissões: {p}; pode escrever? {p.HasFlag(Permissao.Escrita)}");

        p &= ~Permissao.Escrita;        // remove a flag Escrita
        p |= Permissao.Execucao;        // adiciona a flag Execucao
        Console.WriteLine($"depois de remover Escrita e adicionar Execucao: {p}");
    }

    private static void OperadoresDeNulo()
    {
        Console.WriteLine("\n--- Operadores de nulo, is e as ---");

        string? apelido = null;
        string exibicao = apelido ?? "(sem apelido)";       // valor padrão se for nulo
        Console.WriteLine($"apelido ?? padrão = {exibicao}");

        apelido ??= "Zé";                                    // atribui só se for nulo
        int? tamanho = apelido?.Length;                      // acesso condicional
        Console.WriteLine($"apelido = {apelido}, tamanho = {tamanho}");

        int[]? valores = null;
        int? primeiro = valores?[0];                         // indexação condicional: não lança exceção
        Console.WriteLine($"valores?[0] = {(primeiro.HasValue ? primeiro.Value.ToString() : "nulo")}");

        object texto = "Olá";
        string? comoString = texto as string;                // 'as' devolve null se não for compatível
        int? comoInt = texto as int?;
        Console.WriteLine($"as string = {comoString}; as int? = {(comoInt is null ? "nulo" : comoInt.ToString())}");

        if (texto is string s && s.Length > 2)               // 'is' com padrão de declaração
            Console.WriteLine($"is string com mais de 2 caracteres: {s}");
    }

    private static string Classificar(object? valor) => valor switch
    {
        null => "nulo",
        int n when n < 0 => "inteiro negativo",
        int n => $"inteiro {n}",
        double d when d > 100 => $"double grande ({d})",
        double d => $"double ({d})",
        string { Length: 0 } => "texto vazio",
        string s => $"texto com {s.Length} caractere(s)",
        int[] vetor => $"vetor com {vetor.Length} elemento(s)",
        _ => $"outro tipo: {valor.GetType().Name}"
    };

    private static string Conceito(double nota) => nota switch
    {
        >= 9 => "A",
        >= 7 => "B",
        >= 5 => "C",
        _ => "D"
    };

    private static string Recomendacao(bool chovendo, int temperatura) => (chovendo, temperatura) switch
    {
        (true, _) => "leve guarda-chuva",
        (false, > 30) => "use protetor solar",
        (false, < 15) => "leve um casaco",
        _ => "tempo agradável"
    };

    private static void PatternMatching()
    {
        Console.WriteLine("\n--- Expressão switch e pattern matching ---");

        object?[] amostras = { null, -5, 42, 150.5, 3.14, "", "C#", new[] { 1, 2, 3 }, DateTime.MinValue };
        foreach (object? amostra in amostras)
            Console.WriteLine($"  {Classificar(amostra)}");

        Console.WriteLine($"  Conceito de 9.5 / 7.2 / 4.0 = {Conceito(9.5)} / {Conceito(7.2)} / {Conceito(4.0)}");
        Console.WriteLine($"  Recomendação (sol, 35 °C): {Recomendacao(false, 35)}");
        Console.WriteLine($"  Recomendação (chuva, 20 °C): {Recomendacao(true, 20)}");

        // Padrões de lista (C# 11).
        int[][] sequencias = { new int[0], new[] { 7 }, new[] { 1, 2, 3, 4, 5 } };
        foreach (int[] sequencia in sequencias)
        {
            string descricao = sequencia switch
            {
                [] => "lista vazia",
                [var unico] => $"um elemento: {unico}",
                [var primeiro, .., var ultimo] => $"começa em {primeiro} e termina em {ultimo}"
            };
            Console.WriteLine($"  {descricao}");
        }
    }
}
