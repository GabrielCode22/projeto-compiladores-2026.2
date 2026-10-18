using System.Collections.Concurrent;
using System.Threading.Channels;

namespace ExemplosOrigem;

/// <summary>
/// TEMA: Concorrência — produtor/consumidor, paralelismo de dados e coleções concorrentes.
///
/// Recursos demonstrados:
///   - Channel&lt;T&gt;: fila assíncrona com capacidade limitada (backpressure);
///   - vários consumidores concorrentes lendo do mesmo canal;
///   - ConcurrentDictionary (coleção segura para várias threads);
///   - Parallel.For com acumulador local por thread;
///   - Parallel.ForEach e PLINQ (AsParallel).
///
/// Exemplo aplicável: fila de pedidos processada por 3 atendentes em paralelo.
/// </summary>
public static class ConcorrenciaPipeline
{
    public static void Executar() => ExecutarAsync().GetAwaiter().GetResult();

    private static async Task ExecutarAsync()
    {
        await FilaDePedidos();
        SomaParalela();
        ContagemDePrimos();
        FrequenciaDePalavras();
    }

    private static async Task FilaDePedidos()
    {
        Console.WriteLine("--- Channel: 1 produtor e 3 consumidores ---");

        // Capacidade 5: se o canal encher, o produtor espera (controle de vazão).
        var canal = Channel.CreateBounded<int>(new BoundedChannelOptions(5)
        {
            SingleWriter = true
        });

        var atendidosPorConsumidor = new ConcurrentDictionary<int, int>();

        Task produtor = Task.Run(async () =>
        {
            for (int pedido = 1; pedido <= 20; pedido++)
                await canal.Writer.WriteAsync(pedido);

            canal.Writer.Complete();                // sinaliza: não haverá mais pedidos
        });

        Task[] consumidores = Enumerable.Range(1, 3).Select(idConsumidor => Task.Run(async () =>
        {
            await foreach (int pedido in canal.Reader.ReadAllAsync())
            {
                await Task.Delay(10);               // simula o atendimento
                atendidosPorConsumidor.AddOrUpdate(idConsumidor, 1, (_, atual) => atual + 1);
            }
        })).ToArray();

        await Task.WhenAll(consumidores.Append(produtor));

        int total = atendidosPorConsumidor.Values.Sum();
        Console.WriteLine($"  Pedidos produzidos: 20 | pedidos atendidos: {total}");
        foreach (var par in atendidosPorConsumidor.OrderBy(p => p.Key))
            Console.WriteLine($"    consumidor {par.Key} atendeu {par.Value} pedido(s)   (a divisão varia a cada execução)");
    }

    private static void SomaParalela()
    {
        Console.WriteLine("\n--- Parallel.For com acumulador local ---");

        long total = 0;

        // Cada thread soma num acumulador próprio (sem disputa) e só no final
        // combina o resultado parcial com Interlocked.Add.
        Parallel.For(
            1, 1_000_001,
            () => 0L,
            (i, estado, parcial) => parcial + i,
            parcial => Interlocked.Add(ref total, parcial));

        Console.WriteLine($"  Soma de 1 a 1.000.000 = {total}");
    }

    private static bool EhPrimo(int n)
    {
        if (n < 2) return false;
        for (int divisor = 2; (long)divisor * divisor <= n; divisor++)
        {
            if (n % divisor == 0) return false;
        }
        return true;
    }

    private static void ContagemDePrimos()
    {
        Console.WriteLine("\n--- PLINQ: contar primos menores que 100.000 ---");

        int primos = Enumerable.Range(2, 99_998).AsParallel().Count(EhPrimo);
        Console.WriteLine($"  Quantidade de primos encontrados: {primos}");
    }

    private static void FrequenciaDePalavras()
    {
        Console.WriteLine("\n--- Parallel.ForEach + ConcurrentDictionary ---");

        string[] linhas =
        {
            "ser ou nao ser",
            "ser feliz e ser livre",
            "a vida e bela",
            "a vida e curta"
        };

        var frequencia = new ConcurrentDictionary<string, int>();

        Parallel.ForEach(linhas, linha =>
        {
            foreach (string palavra in linha.Split(' '))
                frequencia.AddOrUpdate(palavra, 1, (_, atual) => atual + 1);   // atualização atômica
        });

        // Ordenamos por frequência e depois alfabeticamente para a saída ser estável.
        var maisComuns = frequencia
            .OrderByDescending(par => par.Value)
            .ThenBy(par => par.Key)
            .Take(3);

        foreach (var par in maisComuns)
            Console.WriteLine($"  \"{par.Key}\" aparece {par.Value} vez(es)");
    }
}
