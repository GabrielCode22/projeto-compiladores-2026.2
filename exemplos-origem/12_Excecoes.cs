namespace ExemplosOrigem;

// ---------- Exceções personalizadas ----------

// Herdar de Exception (ou de uma exceção mais específica) permite carregar dados do erro.
public class SaldoInsuficienteException : Exception
{
    public decimal Saldo { get; }
    public decimal ValorSolicitado { get; }

    public SaldoInsuficienteException(decimal saldo, decimal valorSolicitado)
        : base($"Saldo de R$ {saldo:F2} insuficiente para sacar R$ {valorSolicitado:F2}.")
    {
        Saldo = saldo;
        ValorSolicitado = valorSolicitado;
    }
}

public class ContaBloqueadaException : Exception
{
    public ContaBloqueadaException(string mensagem, Exception? interna = null)
        : base(mensagem, interna)
    {
    }
}

public class ContaCorrente
{
    public string Titular { get; }
    public decimal Saldo { get; private set; }
    public bool Bloqueada { get; set; }

    public ContaCorrente(string titular, decimal saldoInicial)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(titular);     // valida argumentos com exceção padrão
        Titular = titular;
        Saldo = saldoInicial;
    }

    public void Sacar(decimal valor)
    {
        if (Bloqueada)
            throw new ContaBloqueadaException($"A conta de {Titular} está bloqueada.");
        if (valor <= 0)
            throw new ArgumentOutOfRangeException(nameof(valor), "O valor do saque deve ser positivo.");
        if (valor > Saldo)
            throw new SaldoInsuficienteException(Saldo, valor);

        Saldo -= valor;
    }
}

// Recurso que precisa ser liberado: implementa IDisposable para funcionar com 'using'.
public sealed class ConexaoFalsa : IDisposable
{
    private readonly string _nome;

    public ConexaoFalsa(string nome)
    {
        _nome = nome;
        Console.WriteLine($"    [{_nome}] conexão aberta");
    }

    public void Consultar(bool falhar)
    {
        if (falhar)
            throw new TimeoutException($"A conexão '{_nome}' demorou demais para responder.");
        Console.WriteLine($"    [{_nome}] consulta executada");
    }

    public void Dispose() => Console.WriteLine($"    [{_nome}] conexão fechada");
}

/// <summary>
/// TEMA: Exceções.
///
/// Recursos demonstrados:
///   - try / catch / finally e vários catch em ordem do mais específico ao mais geral;
///   - exceções personalizadas com dados extras;
///   - filtros de exceção (catch ... when);
///   - encadeamento (InnerException) e relançamento preservando a pilha (throw;);
///   - using / IDisposable para liberar recursos mesmo em caso de erro;
///   - exceções padrão da plataforma e AggregateException em código paralelo.
///
/// Exemplo aplicável: operações de saque em uma conta corrente.
/// </summary>
public static class Excecoes
{
    public static void Executar()
    {
        SaquesComTratamento();
        FiltrosDeExcecao();
        EncadeamentoERelancamento();
        LiberacaoDeRecursos();
        ExcecoesPadrao();
        ExcecoesEmCodigoParalelo();
    }

    private static void SaquesComTratamento()
    {
        Console.WriteLine("--- try / catch / finally ---");

        var conta = new ContaCorrente("Ana", 500m);
        var bloqueada = new ContaCorrente("Bruno", 900m) { Bloqueada = true };

        var tentativas = new (string Descricao, ContaCorrente Conta, decimal Valor)[]
        {
            ("sacar R$ 200 (ok)", conta, 200m),
            ("sacar R$ 800 (saldo insuficiente)", conta, 800m),
            ("sacar -10 (valor inválido)", conta, -10m),
            ("sacar de conta bloqueada", bloqueada, 50m),
        };

        foreach (var (descricao, contaAlvo, valor) in tentativas)
        {
            try
            {
                contaAlvo.Sacar(valor);
                Console.WriteLine($"  {descricao}: sucesso, saldo = R$ {contaAlvo.Saldo:F2}");
            }
            catch (SaldoInsuficienteException ex)                // mais específica primeiro
            {
                Console.WriteLine($"  {descricao}: faltam R$ {ex.ValorSolicitado - ex.Saldo:F2}");
            }
            catch (ContaBloqueadaException ex)
            {
                Console.WriteLine($"  {descricao}: {ex.Message}");
            }
            catch (ArgumentException ex)                         // mais geral por último
            {
                Console.WriteLine($"  {descricao}: argumento inválido ({ex.GetType().Name})");
            }
            finally
            {
                Console.WriteLine("    (finally: registro de auditoria gravado)");
            }
        }
    }

    private static void FiltrosDeExcecao()
    {
        Console.WriteLine("\n--- Filtros de exceção (catch ... when) ---");

        var conta = new ContaCorrente("Carla", 100m);

        try
        {
            conta.Sacar(0m);
        }
        catch (ArgumentOutOfRangeException ex) when (ex.ParamName == "valor")
        {
            Console.WriteLine($"  Filtro aceitou: o parâmetro '{ex.ParamName}' estava fora do intervalo");
        }
        catch (ArgumentOutOfRangeException)
        {
            Console.WriteLine("  Outro parâmetro fora do intervalo");
        }
    }

    // Camada de baixo: lê um número de configuração e deixa o erro "técnico" subir.
    private static int LerLimiteDaConfiguracao(string texto) => int.Parse(texto);

    // Camada de cima: converte o erro técnico em um erro do domínio, guardando a causa.
    private static int CarregarConfiguracao(string texto)
    {
        try
        {
            return LerLimiteDaConfiguracao(texto);
        }
        catch (FormatException ex)
        {
            throw new InvalidOperationException("Configuração do limite é inválida.", ex);
        }
    }

    private static void EncadeamentoERelancamento()
    {
        Console.WriteLine("\n--- Encadeamento (InnerException) e relançamento ---");

        try
        {
            CarregarConfiguracao("abc");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"  Erro de domínio : {ex.Message}");
            Console.WriteLine($"  Causa original  : {ex.InnerException?.GetType().Name}");
        }

        try
        {
            try
            {
                CarregarConfiguracao("999999999999");       // número grande demais para int
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  Log interno: {ex.GetType().Name}");
                throw;                                      // relança preservando a pilha original
            }
        }
        catch (OverflowException)
        {
            Console.WriteLine("  Exceção relançada e tratada pelo chamador (OverflowException)");
        }
    }

    private static void LiberacaoDeRecursos()
    {
        Console.WriteLine("\n--- using: o recurso é fechado mesmo quando há erro ---");

        try
        {
            using var conexao = new ConexaoFalsa("banco-principal");
            conexao.Consultar(falhar: false);
            conexao.Consultar(falhar: true);                // lança TimeoutException
            Console.WriteLine("    esta linha nunca executa");
        }
        catch (TimeoutException ex)
        {
            Console.WriteLine($"  Erro tratado: {ex.Message}");
        }
    }

    private static void ExcecoesPadrao()
    {
        Console.WriteLine("\n--- Exceções padrão da plataforma ---");

        int zero = 0;
        int[] vetor = new int[3];
        string? nulo = null;
        object texto = "texto";

        var casos = new (string Nome, Action Acao)[]
        {
            ("divisão por zero",       () => Console.WriteLine(10 / zero)),
            ("índice fora do vetor",   () => Console.WriteLine(vetor[5])),
            ("referência nula",        () => Console.WriteLine(nulo!.Length)),
            ("cast inválido",          () => Console.WriteLine((int)texto)),
            ("conversão de formato",   () => Console.WriteLine(int.Parse("12x"))),
        };

        foreach (var (nome, acao) in casos)
        {
            try
            {
                acao();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  {nome,-22} -> {ex.GetType().Name}");
            }
        }
    }

    private static void ExcecoesEmCodigoParalelo()
    {
        Console.WriteLine("\n--- AggregateException em Parallel.ForEach ---");

        try
        {
            Parallel.ForEach(new[] { 1, 2, 3, 4 }, numero =>
            {
                if (numero % 2 == 0)
                    throw new InvalidOperationException($"falha no item {numero}");
            });
        }
        catch (AggregateException agregada)
        {
            // Várias iterações podem falhar; todas chegam juntas numa única exceção.
            foreach (Exception interna in agregada.Flatten().InnerExceptions.OrderBy(e => e.Message))
                Console.WriteLine($"  - {interna.Message}");
        }
    }
}
