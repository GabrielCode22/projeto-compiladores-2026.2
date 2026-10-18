namespace ExemplosOrigem;

/// <summary>
/// Conta bancária preparada para uso por várias threads ao mesmo tempo.
/// Cada conta tem sua própria trava (lock) para proteger o saldo.
/// </summary>
public sealed class ContaSegura
{
    private static int _sequencia;

    private readonly object _trava = new();
    private decimal _saldo;

    public int Id { get; } = Interlocked.Increment(ref _sequencia);

    public ContaSegura(decimal saldoInicial) => _saldo = saldoInicial;

    public decimal Saldo
    {
        get
        {
            lock (_trava)
            {
                return _saldo;
            }
        }
    }

    // Transfere 'valor' de uma conta para outra de forma atômica.
    // Sempre travamos primeiro a conta de MENOR Id: com essa ordem global, duas
    // transferências em sentidos opostos (A->B e B->A) nunca ficam em deadlock.
    public static void Transferir(ContaSegura origem, ContaSegura destino, decimal valor)
    {
        var (primeira, segunda) = origem.Id < destino.Id ? (origem, destino) : (destino, origem);

        lock (primeira._trava)
        {
            lock (segunda._trava)
            {
                if (origem._saldo < valor)
                    return;                         // saldo insuficiente: ignora a operação

                origem._saldo -= valor;
                destino._saldo += valor;
            }
        }
    }
}

/// <summary>
/// TEMA: Concorrência — threads e sincronização.
///
/// Recursos demonstrados:
///   - criação e junção de threads (Thread, Start, Join);
///   - condição de corrida (race condition) e como evitá-la com lock e Interlocked;
///   - SemaphoreSlim para limitar o acesso a um recurso (vagas de estacionamento);
///   - ThreadPool com CountdownEvent;
///   - prevenção de deadlock com ordem global de travas (ContaSegura).
///
/// ATENÇÃO: o resultado do contador SEM proteção varia a cada execução — é
/// justamente isso que o exemplo quer mostrar.
/// </summary>
public static class ConcorrenciaThreads
{
    private const int NumeroDeThreads = 4;
    private const int IncrementosPorThread = 100_000;

    private static int _semProtecao;
    private static int _comLock;
    private static int _atomico;
    private static readonly object _trava = new();

    public static void Executar()
    {
        CondicaoDeCorrida();
        Estacionamento();
        PoolDeThreads();
        TransferenciasSimultaneas();
    }

    // Cria N threads executando a mesma tarefa e espera todas terminarem.
    private static void RodarEmParalelo(int quantidade, Action trabalho)
    {
        var threads = new List<Thread>();
        for (int i = 0; i < quantidade; i++)
        {
            var thread = new Thread(() => trabalho());
            threads.Add(thread);
            thread.Start();
        }

        foreach (Thread thread in threads)
            thread.Join();                          // bloqueia até a thread terminar
    }

    private static void CondicaoDeCorrida()
    {
        Console.WriteLine("--- Condição de corrida ---");

        RodarEmParalelo(NumeroDeThreads, () =>
        {
            for (int i = 0; i < IncrementosPorThread; i++)
            {
                _semProtecao++;                     // NÃO é atômico: ler, somar e gravar são 3 passos

                lock (_trava)                       // exclusão mútua: uma thread por vez
                {
                    _comLock++;
                }

                Interlocked.Increment(ref _atomico);    // operação atômica em nível de hardware
            }
        });

        int esperado = NumeroDeThreads * IncrementosPorThread;
        Console.WriteLine($"  Valor esperado : {esperado}");
        Console.WriteLine($"  Sem proteção   : {_semProtecao}   (costuma ser menor: atualizações perdidas)");
        Console.WriteLine($"  Com lock       : {_comLock}");
        Console.WriteLine($"  Com Interlocked: {_atomico}");
    }

    private static int _carrosEstacionados;
    private static int _maximoSimultaneo;

    private static void Estacionamento()
    {
        Console.WriteLine("\n--- SemaphoreSlim: estacionamento com 3 vagas e 8 carros ---");

        var vagas = new SemaphoreSlim(3);           // no máximo 3 threads dentro da seção crítica
        var carros = new List<Thread>();

        for (int id = 1; id <= 8; id++)
        {
            var carro = new Thread(() =>
            {
                vagas.Wait();                       // espera uma vaga ficar livre
                try
                {
                    int dentro = Interlocked.Increment(ref _carrosEstacionados);
                    lock (_trava)
                    {
                        _maximoSimultaneo = Math.Max(_maximoSimultaneo, dentro);
                    }

                    Thread.Sleep(40);               // o carro fica estacionado um tempo
                    Interlocked.Decrement(ref _carrosEstacionados);
                }
                finally
                {
                    vagas.Release();                // libera a vaga, mesmo se houver exceção
                }
            });
            carro.Name = $"carro-{id}";
            carros.Add(carro);
            carro.Start();
        }

        carros.ForEach(c => c.Join());
        Console.WriteLine($"  Máximo de carros ao mesmo tempo: {_maximoSimultaneo} (nunca passa de 3)");
    }

    private static void PoolDeThreads()
    {
        Console.WriteLine("\n--- ThreadPool + CountdownEvent ---");

        const int tarefas = 5;
        long somaTotal = 0;

        using var concluidas = new CountdownEvent(tarefas);

        for (int bloco = 0; bloco < tarefas; bloco++)
        {
            int inicio = bloco * 1000 + 1;          // cada tarefa soma um bloco de 1000 números
            ThreadPool.QueueUserWorkItem(_ =>
            {
                long parcial = 0;
                for (int n = inicio; n < inicio + 1000; n++)
                    parcial += n;

                Interlocked.Add(ref somaTotal, parcial);
                concluidas.Signal();                // avisa que esta tarefa terminou
            });
        }

        concluidas.Wait();                          // espera as 5 tarefas
        Console.WriteLine($"  Soma de 1 a 5000 calculada por {tarefas} tarefas = {somaTotal}");
    }

    private static void TransferenciasSimultaneas()
    {
        Console.WriteLine("\n--- Transferências simultâneas sem deadlock ---");

        var contaA = new ContaSegura(1000m);
        var contaB = new ContaSegura(1000m);

        var threadAB = new Thread(() =>
        {
            for (int i = 0; i < 2000; i++)
                ContaSegura.Transferir(contaA, contaB, 1m);
        });
        var threadBA = new Thread(() =>
        {
            for (int i = 0; i < 2000; i++)
                ContaSegura.Transferir(contaB, contaA, 1m);
        });

        threadAB.Start();
        threadBA.Start();
        threadAB.Join();
        threadBA.Join();

        Console.WriteLine($"  Saldo A = {contaA.Saldo}, saldo B = {contaB.Saldo}");
        Console.WriteLine($"  Soma dos saldos = {contaA.Saldo + contaB.Saldo} (sempre 2000: nenhum dinheiro some)");
    }
}
