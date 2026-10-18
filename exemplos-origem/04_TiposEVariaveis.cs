using System.Text;

namespace ExemplosOrigem;

public enum NivelAcesso
{
    Visitante = 0,
    Usuario = 1,
    Moderador = 5,
    Admin = 10
}

// Struct: TIPO POR VALOR. Atribuir copia todos os campos.
public struct Ponto2D
{
    public double X;
    public double Y;

    public Ponto2D(double x, double y)
    {
        X = x;
        Y = y;
    }

    public override string ToString() => $"({X}, {Y})";
}

// Class: TIPO POR REFERÊNCIA. Atribuir copia apenas a referência.
public class CaixaDeTexto
{
    public string Conteudo = "";
}

/// <summary>
/// TEMA: Tipos e variáveis.
///
/// Recursos demonstrados:
///   - tipos primitivos, var (inferência) e constantes;
///   - tipos por valor x tipos por referência;
///   - tipos anuláveis (int? e string?);
///   - conversões implícitas, explícitas, TryParse, checked/unchecked e boxing;
///   - enums, tuplas, arrays, coleções e strings.
/// </summary>
public static class TiposEVariaveis
{
    private const double Gravidade = 9.81;              // constante: valor fixo em tempo de compilação

    public static void Executar()
    {
        TiposPrimitivos();
        ValorVersusReferencia();
        TiposAnulaveis();
        Conversoes();
        EnumsETuplas();
        ArraysEColecoes();
        Strings();
    }

    private static void TiposPrimitivos()
    {
        Console.WriteLine("--- Tipos primitivos ---");

        byte pequeno = 255;
        int inteiro = int.MaxValue;
        long grande = long.MaxValue;
        float simples = 3.14159265f;
        double dupla = 3.14159265358979;
        decimal dinheiro = 1234.56m;
        char letra = 'Ç';
        bool ativo = true;

        Console.WriteLine($"byte    : {pequeno} ({sizeof(byte)} byte)");
        Console.WriteLine($"int     : {inteiro} ({sizeof(int)} bytes)");
        Console.WriteLine($"long    : {grande} ({sizeof(long)} bytes)");
        Console.WriteLine($"float   : {simples} ({sizeof(float)} bytes)");
        Console.WriteLine($"double  : {dupla} ({sizeof(double)} bytes)");
        Console.WriteLine($"decimal : {dinheiro} ({sizeof(decimal)} bytes)");
        Console.WriteLine($"char    : {letra} ({sizeof(char)} bytes)");
        Console.WriteLine($"bool    : {ativo} ({sizeof(bool)} byte)");

        // 'var' infere o tipo em tempo de compilação; a variável continua fortemente tipada.
        var inferido = 42;
        Console.WriteLine($"var inferido     -> {inferido.GetType().Name}");
        Console.WriteLine($"Constante Gravidade = {Gravidade} m/s²");

        // double é binário: 0,1 + 0,2 não é exatamente 0,3. decimal é indicado para dinheiro.
        double a = 0.1, b = 0.2;
        decimal c = 0.1m, d = 0.2m;
        Console.WriteLine($"0.1 + 0.2 == 0.3 em double?  {a + b == 0.3}");
        Console.WriteLine($"0.1m + 0.2m == 0.3m em decimal? {c + d == 0.3m}");

        // dynamic: tipo resolvido só em tempo de execução (evite fora de interoperabilidade).
        dynamic dinamico = 10;
        dinamico = "agora sou uma string";
        Console.WriteLine($"dynamic          -> {dinamico}");
    }

    private static void ValorVersusReferencia()
    {
        Console.WriteLine("\n--- Tipo por valor x tipo por referência ---");

        Ponto2D p1 = new(1, 2);
        Ponto2D p2 = p1;                // cópia independente
        p2.X = 99;
        Console.WriteLine($"struct: p1 = {p1}, p2 = {p2}   (p1 não mudou)");

        CaixaDeTexto c1 = new() { Conteudo = "original" };
        CaixaDeTexto c2 = c1;           // mesma instância
        c2.Conteudo = "alterado";
        Console.WriteLine($"class : c1 = {c1.Conteudo}, c2 = {c2.Conteudo}   (mesmo objeto)");
        Console.WriteLine($"ReferenceEquals(c1, c2) = {ReferenceEquals(c1, c2)}");
    }

    private static void TiposAnulaveis()
    {
        Console.WriteLine("\n--- Tipos anuláveis ---");

        int? nota = null;               // Nullable<int>: tipo por valor que aceita "sem valor"
        Console.WriteLine($"nota.HasValue = {nota.HasValue}; com ?? -> {nota ?? -1}");

        nota = 8;
        Console.WriteLine($"nota.Value = {nota.Value}");

        string? comentario = null;      // string? : referência que pode ser nula (checado pelo compilador)
        Console.WriteLine($"Tamanho do comentário: {comentario?.Length ?? 0}");

        Console.WriteLine($"default(int) = {default(int)}, default(bool) = {default(bool)}, default(string) é null? {default(string) is null}");
    }

    private static void Conversoes()
    {
        Console.WriteLine("\n--- Conversões ---");

        int inteiro = 42;
        double implicita = inteiro;                 // implícita: sem perda de informação
        double fracionado = 7.9;
        int explicita = (int)fracionado;            // explícita: trunca (7)
        Console.WriteLine($"implícita = {implicita}; (int)7.9 = {explicita}; Math.Round(7.9) = {Math.Round(fracionado)}");

        foreach (string entrada in new[] { "123", "12x", "" })
        {
            if (int.TryParse(entrada, out int numero))
                Console.WriteLine($"TryParse(\"{entrada}\") -> {numero}");
            else
                Console.WriteLine($"TryParse(\"{entrada}\") -> entrada inválida");
        }

        int maximo = int.MaxValue;
        int estourou = unchecked(maximo + 1);       // "dá a volta" silenciosamente
        Console.WriteLine($"unchecked(int.MaxValue + 1) = {estourou}");
        try
        {
            Console.WriteLine(checked(maximo + 1)); // checked lança OverflowException
        }
        catch (OverflowException)
        {
            Console.WriteLine("checked(int.MaxValue + 1) -> OverflowException");
        }

        // Boxing: tipo por valor "embrulhado" em object; unboxing exige o tipo exato.
        object caixa = 42;
        int desembrulhado = (int)caixa;
        Console.WriteLine($"boxing/unboxing: {caixa.GetType().Name} -> {desembrulhado}");
    }

    private static void EnumsETuplas()
    {
        Console.WriteLine("\n--- Enums e tuplas ---");

        NivelAcesso nivel = NivelAcesso.Moderador;
        Console.WriteLine($"{nivel} = {(int)nivel}");

        if (Enum.TryParse<NivelAcesso>("Admin", out var parseado))
            Console.WriteLine($"Texto \"Admin\" convertido para {parseado} ({(int)parseado})");

        foreach (NivelAcesso n in Enum.GetValues<NivelAcesso>())
            Console.WriteLine($"  {n,-10} -> {(int)n}");

        // Tupla com elementos nomeados e desconstrução.
        (string Nome, int Idade) pessoa = ("Bia", 21);
        var (nome, idade) = pessoa;
        Console.WriteLine($"tupla: {pessoa.Nome}/{pessoa.Idade}  desconstruída: {nome}/{idade}");
    }

    private static void ArraysEColecoes()
    {
        Console.WriteLine("\n--- Arrays e coleções ---");

        int[] numeros = { 10, 20, 30, 40, 50 };
        int[,] matriz = { { 1, 2 }, { 3, 4 } };                  // retangular
        int[][] irregular = { new[] { 1 }, new[] { 2, 3 }, new[] { 4, 5, 6 } };  // array de arrays

        Console.WriteLine($"último = {numeros[^1]}; fatia [1..4] = {string.Join(",", numeros[1..4])}");
        Console.WriteLine($"matriz[1,0] = {matriz[1, 0]}; irregular[2].Length = {irregular[2].Length}");

        List<string> frutas = new() { "uva", "maçã" };
        frutas.Add("pera");

        Dictionary<string, int> idades = new() { ["Ana"] = 30, ["Bruno"] = 25 };
        HashSet<int> conjunto = new() { 1, 2, 2, 3, 3, 3 };      // não guarda repetidos

        Console.WriteLine($"List: {string.Join(", ", frutas)}");
        Console.WriteLine($"Dictionary: Ana tem {idades["Ana"]} anos");
        Console.WriteLine($"HashSet com 6 inserções guarda {conjunto.Count} elementos");
    }

    private static void Strings()
    {
        Console.WriteLine("\n--- Strings ---");

        // Strings são imutáveis: cada operação cria uma nova string.
        string original = "compiladores";
        string maiuscula = original.ToUpper();
        Console.WriteLine($"{original} -> {maiuscula}");

        // Para muitas concatenações, StringBuilder evita criar cópias a cada passo.
        var sb = new StringBuilder();
        for (int i = 1; i <= 5; i++)
            sb.Append(i).Append(i < 5 ? "-" : "");
        Console.WriteLine($"StringBuilder: {sb}");

        string caminho = @"C:\projetos\transpilador";           // string literal verbatim
        Console.WriteLine($"{caminho} | {caminho.Length} caracteres | {caminho.Substring(3, 8)}");
    }
}
