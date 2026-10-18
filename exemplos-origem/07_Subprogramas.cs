namespace ExemplosOrigem;

// 'readonly record struct': tipo por valor, imutável, ideal para passar com 'in'.
public readonly record struct Coordenada(double X, double Y);

/// <summary>
/// TEMA: Subprogramas (métodos/funções).
///
/// Recursos demonstrados:
///   - passagem de parâmetros: por valor, ref, out e in;
///   - retorno de múltiplos valores com tuplas;
///   - params, parâmetros opcionais e argumentos nomeados;
///   - sobrecarga (overloading);
///   - recursão (fatorial, Fibonacci com memoização, Torre de Hanói);
///   - métodos com corpo de expressão e funções locais.
///
/// Exemplo aplicável: pequena biblioteca de cálculos financeiros e geométricos.
/// </summary>
public static class Subprogramas
{
    public static void Executar()
    {
        PassagemDeParametros();
        RetornoMultiplo();
        ParametrosEspeciais();
        Sobrecarga();
        Recursao();
        FuncoesLocais();
    }

    // ---------- Passagem de parâmetros ----------

    private static void IncrementarPorValor(int numero) => numero++;     // altera só a cópia local
    private static void IncrementarPorReferencia(ref int numero) => numero++;   // altera a variável do chamador

    // 'out': o método é obrigado a atribuir o parâmetro antes de retornar.
    private static bool TentarDividir(double dividendo, double divisor, out double quociente)
    {
        if (divisor == 0)
        {
            quociente = 0;
            return false;
        }

        quociente = dividendo / divisor;
        return true;
    }

    // 'in': passa por referência somente leitura (evita cópia de structs grandes).
    private static double Distancia(in Coordenada a, in Coordenada b) =>
        Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));

    private static void PassagemDeParametros()
    {
        Console.WriteLine("--- Passagem de parâmetros ---");

        int x = 10;
        IncrementarPorValor(x);
        Console.WriteLine($"  após IncrementarPorValor      : x = {x}");
        IncrementarPorReferencia(ref x);
        Console.WriteLine($"  após IncrementarPorReferencia : x = {x}");

        if (TentarDividir(10, 4, out double q))
            Console.WriteLine($"  10 / 4 = {q}");
        if (!TentarDividir(1, 0, out _))                // '_' descarta o valor de saída
            Console.WriteLine("  1 / 0 -> divisão impossível");

        Console.WriteLine($"  Distância entre (0,0) e (3,4) = {Distancia(new Coordenada(0, 0), new Coordenada(3, 4))}");
    }

    // ---------- Retorno múltiplo ----------

    private static (int Minimo, int Maximo, double Media) Estatisticas(int[] valores) =>
        (valores.Min(), valores.Max(), valores.Average());

    private static void RetornoMultiplo()
    {
        Console.WriteLine("\n--- Retorno múltiplo com tupla ---");

        var (minimo, maximo, media) = Estatisticas(new[] { 4, 8, 15, 16, 23, 42 });
        Console.WriteLine($"  mínimo = {minimo}, máximo = {maximo}, média = {media}");
    }

    // ---------- params, opcionais e nomeados ----------

    private static int Somar(params int[] valores) => valores.Sum();

    // Parâmetros opcionais têm valor padrão.
    private static decimal CalcularMontante(decimal capital, decimal taxaMensal = 0.01m, int meses = 12)
    {
        decimal montante = capital;
        for (int i = 0; i < meses; i++)
            montante *= 1 + taxaMensal;
        return montante;
    }

    private static void ParametrosEspeciais()
    {
        Console.WriteLine("\n--- params, parâmetros opcionais e argumentos nomeados ---");

        Console.WriteLine($"  Somar(1, 2, 3, 4) = {Somar(1, 2, 3, 4)}");
        Console.WriteLine($"  Montante padrão (1% a.m., 12 meses)  : R$ {CalcularMontante(1000m):F2}");
        Console.WriteLine($"  Montante com meses: 6                 : R$ {CalcularMontante(1000m, meses: 6):F2}");
        Console.WriteLine($"  Montante com taxaMensal: 0.02m, 3 meses: R$ {CalcularMontante(1000m, taxaMensal: 0.02m, meses: 3):F2}");
    }

    // ---------- Sobrecarga ----------

    private static double Area(double raio) => Math.PI * raio * raio;              // círculo
    private static double Area(double largura, double altura) => largura * altura; // retângulo
    private static double Area(double ladoA, double ladoB, double ladoC)           // triângulo (Heron)
    {
        double s = (ladoA + ladoB + ladoC) / 2;
        return Math.Sqrt(s * (s - ladoA) * (s - ladoB) * (s - ladoC));
    }

    private static void Sobrecarga()
    {
        Console.WriteLine("\n--- Sobrecarga: o compilador escolhe pela assinatura ---");

        Console.WriteLine($"  Area(2)       (círculo)   = {Area(2):F2}");
        Console.WriteLine($"  Area(3, 4)    (retângulo) = {Area(3, 4):F2}");
        Console.WriteLine($"  Area(3, 4, 5) (triângulo) = {Area(3, 4, 5):F2}");
    }

    // ---------- Recursão ----------

    private static long Fatorial(int n) => n <= 1 ? 1 : n * Fatorial(n - 1);

    private static readonly Dictionary<int, long> _memo = new();

    // Memoização: guarda resultados já calculados para evitar recomputação exponencial.
    private static long Fibonacci(int n)
    {
        if (n <= 1) return n;
        if (_memo.TryGetValue(n, out long guardado)) return guardado;

        return _memo[n] = Fibonacci(n - 1) + Fibonacci(n - 2);
    }

    private static int Hanoi(int discos, char origem, char destino, char auxiliar)
    {
        if (discos == 0) return 0;

        int movimentos = Hanoi(discos - 1, origem, auxiliar, destino);
        Console.WriteLine($"    mover disco {discos}: {origem} -> {destino}");
        movimentos++;
        movimentos += Hanoi(discos - 1, auxiliar, destino, origem);
        return movimentos;
    }

    private static void Recursao()
    {
        Console.WriteLine("\n--- Recursão ---");

        Console.WriteLine($"  Fatorial(10)  = {Fatorial(10)}");
        Console.WriteLine($"  Fibonacci(50) = {Fibonacci(50)}");

        Console.WriteLine("  Torre de Hanói com 3 discos:");
        int total = Hanoi(3, 'A', 'C', 'B');
        Console.WriteLine($"  Total de movimentos: {total}");
    }

    // ---------- Funções locais ----------

    private static string FormatarCpf(string digitos)
    {
        if (!ApenasDigitos(digitos) || digitos.Length != 11)
            return "CPF inválido";

        return $"{digitos[..3]}.{digitos[3..6]}.{digitos[6..9]}-{digitos[9..]}";

        // Função local: só existe dentro de FormatarCpf.
        static bool ApenasDigitos(string texto) => texto.All(char.IsDigit);
    }

    private static void FuncoesLocais()
    {
        Console.WriteLine("\n--- Função local ---");

        Console.WriteLine($"  FormatarCpf(\"12345678901\") = {FormatarCpf("12345678901")}");
        Console.WriteLine($"  FormatarCpf(\"123abc\")      = {FormatarCpf("123abc")}");
    }
}
