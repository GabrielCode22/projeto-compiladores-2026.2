using System.Diagnostics;

namespace ExemplosOrigem;

/// <summary>
/// TEMA: Concorrência — programação assíncrona (async/await e Tasks).
///
/// Recursos demonstrados:
///   - async/await com Task e Task&lt;T&gt;;
///   - execução sequencial x paralela (Task.WhenAll) e "corrida" (Task.WhenAny);
///   - cancelamento cooperativo e timeout (CancellationToken);
///   - propagação de exceções em tarefas;
///   - Task.Run para trabalho de CPU;
///   - fluxos assíncronos (IAsyncEnumerable + await foreach).
///
/// Exemplo aplicável: comparador de preços que consulta várias lojas online.
/// As "lojas" são simuladas com Task.Delay, para o exemplo rodar offline.
/// </summary>
public static class ConcorrenciaAsync
{
    // Program.cs chama métodos síncronos; aqui "ligamos" o mundo async a ele.
    // Em um Main assíncrono bastaria: await ExecutarAsync();
    public static void Executar() => ExecutarAsync().GetAwaiter().GetResult();

    private static async Task ExecutarAsync()
    {
        await SequencialVersusParalelo();
        await PrimeiraRespostaVence();
        await CancelamentoETimeout();
        await TratamentoDeErros();
        await TrabalhoDeCpu();
        await FluxoAssincrono();
    }

    // Simula uma chamada de rede: não bloqueia a thread enquanto "espera".
    private static async Task<decimal> ConsultarPrecoAsync(string loja, int latenciaMs, decimal preco)
    {
        await Task.Delay(latenciaMs);
        Console.WriteLine($"    [{loja}] respondeu após {latenciaMs} ms");
        return preco;
    }

    private static async Task SequencialVersusParalelo()
    {
        Console.WriteLine("--- Sequencial x paralelo ---");

        var cronometro = Stopwatch.StartNew();
        decimal a = await ConsultarPrecoAsync("LojaA", 300, 99.90m);
        decimal b = await ConsultarPrecoAsync("LojaB", 200, 89.90m);
        decimal c = await ConsultarPrecoAsync("LojaC", 100, 95.00m);
        decimal menorSequencial = Math.Min(a, Math.Min(b, c));
        Console.WriteLine($"  Sequencial: menor preço R$ {menorSequencial:F2} em ~{cronometro.ElapsedMilliseconds} ms");

        cronometro.Restart();
        decimal[] precos = await Task.WhenAll(
            ConsultarPrecoAsync("LojaA", 300, 99.90m),
            ConsultarPrecoAsync("LojaB", 200, 89.90m),
            ConsultarPrecoAsync("LojaC", 100, 95.00m));
        Console.WriteLine($"  Paralelo  : menor preço R$ {precos.Min():F2} em ~{cronometro.ElapsedMilliseconds} ms");
    }

    private static async Task PrimeiraRespostaVence()
    {
        Console.WriteLine("\n--- Task.WhenAny: a primeira resposta vence ---");

        var consultas = new List<Task<decimal>>
        {
            ConsultarPrecoAsync("LojaLenta", 250, 80m),
            ConsultarPrecoAsync("LojaRapida", 50, 92m),
        };

        Task<decimal> primeira = await Task.WhenAny(consultas);
        decimal preco = await primeira;
        Console.WriteLine($"  Primeira resposta recebida: R$ {preco:F2}");

        await Task.WhenAll(consultas);              // aguarda a outra terminar antes de seguir
    }

    private static async Task ProcessarLoteAsync(CancellationToken token)
    {
        for (int passo = 1; passo <= 10; passo++)
        {
            token.ThrowIfCancellationRequested();   // ponto de verificação cooperativo
            await Task.Delay(100, token);
            Console.WriteLine($"    passo {passo}/10 concluído");
        }
    }

    private static async Task CancelamentoETimeout()
    {
        Console.WriteLine("\n--- Cancelamento com timeout de 350 ms ---");

        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(350));
        try
        {
            await ProcessarLoteAsync(cts.Token);
            Console.WriteLine("  Lote finalizado");
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("  Lote cancelado por timeout");
        }
    }

    private static async Task FalharAsync(string mensagem)
    {
        await Task.Delay(50);
        throw new InvalidOperationException(mensagem);
    }

    private static async Task TratamentoDeErros()
    {
        Console.WriteLine("\n--- Exceções em tarefas ---");

        try
        {
            await FalharAsync("servidor indisponível");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"  await relança a exceção original: {ex.Message}");
        }

        // Com WhenAll, 'await' relança só a primeira; a Task guarda todas.
        Task todas = Task.WhenAll(FalharAsync("erro 1"), FalharAsync("erro 2"));
        try
        {
            await todas;
        }
        catch (Exception)
        {
            AggregateException? agregada = todas.Exception;
            int quantidade = agregada?.InnerExceptions.Count ?? 0;
            Console.WriteLine($"  WhenAll falhou com {quantidade} exceções:");
            foreach (Exception interna in agregada!.InnerExceptions.OrderBy(e => e.Message))
                Console.WriteLine($"    - {interna.Message}");
        }
    }

    private static async Task TrabalhoDeCpu()
    {
        Console.WriteLine("\n--- Task.Run: trabalho de CPU fora da thread principal ---");

        long soma = await Task.Run(() => Enumerable.Range(1, 1_000_000).Sum(n => (long)n));
        Console.WriteLine($"  Soma de 1 a 1.000.000 = {soma}");
    }

    // Fluxo assíncrono: o consumidor recebe cada leitura assim que ela fica pronta.
    private static async IAsyncEnumerable<int> LerSensorAsync(int leituras)
    {
        var sorteio = new Random(42);               // semente fixa: resultado reproduzível
        for (int i = 1; i <= leituras; i++)
        {
            await Task.Delay(50);
            yield return sorteio.Next(20, 31);
        }
    }

    private static async Task FluxoAssincrono()
    {
        Console.WriteLine("\n--- IAsyncEnumerable e await foreach ---");

        await foreach (int temperatura in LerSensorAsync(4))
            Console.WriteLine($"    leitura do sensor: {temperatura} °C");
    }
}
