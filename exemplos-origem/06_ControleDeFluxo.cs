namespace ExemplosOrigem;

public enum EstadoSemaforo
{
    Verde,
    Amarelo,
    Vermelho
}

/// <summary>
/// TEMA: Controle de fluxo.
///
/// Recursos demonstrados:
///   - if / else if / else;
///   - switch clássico (com agrupamento de cases) e expressão switch;
///   - for, foreach, while e do-while;
///   - break, continue e goto (para sair de laços aninhados);
///   - máquina de estados com enum + switch.
///
/// Exemplo aplicável: classificação de IMC, FizzBuzz, busca em tabuleiro e
/// simulação de um semáforo.
/// </summary>
public static class ControleDeFluxo
{
    public static void Executar()
    {
        Condicionais();
        SelecaoMultipla();
        Lacos();
        DesviosDeFluxo();
        MaquinaDeEstados();
    }

    private static void Condicionais()
    {
        Console.WriteLine("--- if / else if / else ---");

        double[] imcs = { 17.5, 22.0, 27.3, 34.9 };
        foreach (double imc in imcs)
        {
            string faixa;
            if (imc < 18.5)
                faixa = "abaixo do peso";
            else if (imc < 25)
                faixa = "peso normal";
            else if (imc < 30)
                faixa = "sobrepeso";
            else
                faixa = "obesidade";

            Console.WriteLine($"  IMC {imc:F1} -> {faixa}");
        }
    }

    private static void SelecaoMultipla()
    {
        Console.WriteLine("\n--- switch clássico ---");

        for (int dia = 1; dia <= 7; dia++)
        {
            string tipo;
            switch (dia)
            {
                case 1:
                case 7:                         // cases agrupados compartilham o mesmo bloco
                    tipo = "fim de semana";
                    break;
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                    tipo = "dia útil";
                    break;
                default:
                    tipo = "inválido";
                    break;
            }
            Console.WriteLine($"  dia {dia} -> {tipo}");
        }

        Console.WriteLine("\n--- expressão switch (FizzBuzz) ---");
        Console.Write("  ");
        for (int n = 1; n <= 15; n++)
        {
            string saida = (n % 3, n % 5) switch
            {
                (0, 0) => "FizzBuzz",
                (0, _) => "Fizz",
                (_, 0) => "Buzz",
                _ => n.ToString()
            };
            Console.Write(saida + " ");
        }
        Console.WriteLine();
    }

    private static void Lacos()
    {
        Console.WriteLine("\n--- Laços: for, foreach, while, do-while ---");

        // for com duas variáveis de controle.
        for (int i = 0, j = 10; i < j; i += 3, j -= 2)
            Console.WriteLine($"  for: i = {i}, j = {j}");

        // while: sequência de Collatz (quantidade de passos até chegar em 1).
        long valor = 27;
        int passos = 0;
        while (valor != 1)
        {
            valor = valor % 2 == 0 ? valor / 2 : 3 * valor + 1;
            passos++;
        }
        Console.WriteLine($"  while: Collatz(27) chega a 1 em {passos} passos");

        // do-while: executa pelo menos uma vez (simula tentativas de conexão).
        int tentativa = 0;
        bool conectado;
        do
        {
            tentativa++;
            conectado = tentativa >= 3;          // simula sucesso na 3ª tentativa
            Console.WriteLine($"  do-while: tentativa {tentativa} -> {(conectado ? "conectado" : "falhou")}");
        } while (!conectado);

        // foreach com continue e break.
        int[] leituras = { 12, -1, 30, 45, 0, 99 };
        int soma = 0;
        foreach (int leitura in leituras)
        {
            if (leitura < 0) continue;           // ignora leituras inválidas
            if (leitura == 0) break;             // zero marca o fim dos dados
            soma += leitura;
        }
        Console.WriteLine($"  foreach: soma das leituras válidas até o zero = {soma}");
    }

    private static void DesviosDeFluxo()
    {
        Console.WriteLine("\n--- goto para sair de laços aninhados ---");

        int[,] tabuleiro =
        {
            { 1, 0, 0 },
            { 0, 0, 2 },
            { 0, 3, 0 }
        };
        int alvo = 3;

        for (int linha = 0; linha < tabuleiro.GetLength(0); linha++)
        {
            for (int coluna = 0; coluna < tabuleiro.GetLength(1); coluna++)
            {
                if (tabuleiro[linha, coluna] == alvo)
                {
                    Console.WriteLine($"  Tesouro {alvo} encontrado em ({linha}, {coluna})");
                    goto fim;                    // sai dos dois laços de uma vez
                }
            }
        }
        Console.WriteLine($"  Tesouro {alvo} não encontrado");

    fim:
        Console.WriteLine("  Busca encerrada");
    }

    private static EstadoSemaforo Proximo(EstadoSemaforo atual) => atual switch
    {
        EstadoSemaforo.Verde => EstadoSemaforo.Amarelo,
        EstadoSemaforo.Amarelo => EstadoSemaforo.Vermelho,
        EstadoSemaforo.Vermelho => EstadoSemaforo.Verde,
        _ => throw new ArgumentOutOfRangeException(nameof(atual))
    };

    private static int DuracaoEmSegundos(EstadoSemaforo estado) => estado switch
    {
        EstadoSemaforo.Verde => 30,
        EstadoSemaforo.Amarelo => 5,
        EstadoSemaforo.Vermelho => 25,
        _ => 0
    };

    private static void MaquinaDeEstados()
    {
        Console.WriteLine("\n--- Máquina de estados: semáforo ---");

        EstadoSemaforo estado = EstadoSemaforo.Vermelho;
        int tempoTotal = 0;

        for (int ciclo = 1; ciclo <= 6; ciclo++)
        {
            estado = Proximo(estado);
            int duracao = DuracaoEmSegundos(estado);
            tempoTotal += duracao;
            Console.WriteLine($"  transição {ciclo}: {estado,-8} por {duracao,2}s (tempo acumulado: {tempoTotal}s)");
        }
    }
}
