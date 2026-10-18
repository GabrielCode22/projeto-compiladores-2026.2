// =============================================================================
// Program.cs — ponto de entrada dos exemplos em C#
//
// Cada exemplo fica em um arquivo próprio e expõe um método estático
// "Executar()". Este arquivo apenas registra os exemplos e permite escolher
// qual deles rodar:
//
//   dotnet run              -> abre um menu interativo
//   dotnet run -- 03        -> executa apenas o exemplo 03
//   dotnet run -- todos     -> executa todos os exemplos em sequência
// =============================================================================

using System.Text;
using ExemplosOrigem;

Console.OutputEncoding = Encoding.UTF8;

var exemplos = new (string Id, string Titulo, Action Executar)[]
{
    ("01", "Paradigma imperativo (procedural)",            ParadigmaImperativo.Executar),
    ("02", "Paradigma orientado a objetos",                ParadigmaOO.Executar),
    ("03", "Paradigma funcional (LINQ, lambdas, records)", ParadigmaFuncional.Executar),
    ("04", "Tipos e variáveis",                            TiposEVariaveis.Executar),
    ("05", "Expressões e operadores",                      ExpressoesEOperadores.Executar),
    ("06", "Controle de fluxo",                            ControleDeFluxo.Executar),
    ("07", "Subprogramas",                                 Subprogramas.Executar),
    ("08", "Subprogramas avançados (delegates, yield...)", SubprogramasAvancados.Executar),
    ("09", "Concorrência: threads e sincronização",        ConcorrenciaThreads.Executar),
    ("10", "Concorrência: async/await e Tasks",            ConcorrenciaAsync.Executar),
    ("11", "Concorrência: canais, Parallel e PLINQ",       ConcorrenciaPipeline.Executar),
    ("12", "Exceções",                                     Excecoes.Executar),
    ("13", "Eventos",                                      Eventos.Executar),
    ("14", "Paradigma orientado a eventos",                ParadigmaOrientadoAEventos.Executar),
};

static void Rodar((string Id, string Titulo, Action Executar) exemplo)
{
    Console.WriteLine();
    Console.WriteLine(new string('=', 72));
    Console.WriteLine($" [{exemplo.Id}] {exemplo.Titulo}");
    Console.WriteLine(new string('=', 72));
    exemplo.Executar();
}

// Modo não interativo: o primeiro argumento escolhe o exemplo.
if (args.Length > 0)
{
    string escolha = args[0].Trim().ToLowerInvariant();

    if (escolha is "todos" or "all")
    {
        foreach (var exemplo in exemplos)
            Rodar(exemplo);
        return 0;
    }

    var encontrado = Array.Find(exemplos, e => e.Id == escolha);
    if (encontrado.Executar is null)
    {
        Console.Error.WriteLine($"Exemplo '{escolha}' não existe. Use 01 a {exemplos.Length:D2} ou 'todos'.");
        return 1;
    }

    Rodar(encontrado);
    return 0;
}

// Modo interativo: menu em laço.
while (true)
{
    Console.WriteLine();
    Console.WriteLine("=== Exemplos em C# — transpilador (linguagem de origem) ===");
    foreach (var exemplo in exemplos)
        Console.WriteLine($"  {exemplo.Id} - {exemplo.Titulo}");
    Console.WriteLine("  todos - executa todos os exemplos");
    Console.WriteLine("  0     - sair");
    Console.Write("Escolha: ");

    string? entrada = Console.ReadLine()?.Trim().ToLowerInvariant();

    if (entrada is null or "0" or "sair")
        return 0;

    if (entrada is "todos" or "all")
    {
        foreach (var exemplo in exemplos)
            Rodar(exemplo);
        continue;
    }

    var selecionado = Array.Find(exemplos, e => e.Id == entrada);
    if (selecionado.Executar is null)
        Console.WriteLine("Opção inválida.");
    else
        Rodar(selecionado);
}
