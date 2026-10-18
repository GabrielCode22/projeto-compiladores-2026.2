namespace ExemplosOrigem;

/// <summary>
/// TEMA: Paradigmas — Imperativo / Procedural.
///
/// No paradigma imperativo o programa é uma sequência de comandos que alteram
/// o ESTADO (variáveis) passo a passo. O fluxo é controlado explicitamente por
/// laços e condicionais, e o código é dividido em procedimentos (métodos).
///
/// Exemplo aplicável: algoritmos clássicos que um sistema bancário/utilitário
/// usaria — troco de saque, números primos, ordenação e contagem de palavras.
/// </summary>
public static class ParadigmaImperativo
{
    public static void Executar()
    {
        SacarComNotas(287);
        Console.WriteLine();

        ListarPrimos(50);
        Console.WriteLine();

        OrdenarPorBolha();
        Console.WriteLine();

        ContarPalavras("a vida é bela e a vida é curta");
    }

    // Algoritmo guloso: a variável 'restante' é atualizada (mutada) a cada
    // iteração do laço até que todo o valor tenha sido distribuído em notas.
    private static void SacarComNotas(int valor)
    {
        int[] notas = { 100, 50, 20, 10, 5, 2 };
        int restante = valor;

        Console.WriteLine($"Saque de R$ {valor}:");
        for (int i = 0; i < notas.Length; i++)
        {
            int quantidade = restante / notas[i];
            restante -= quantidade * notas[i];      // atribuição composta altera o estado

            if (quantidade > 0)
                Console.WriteLine($"  {quantidade} nota(s) de R$ {notas[i]}");
        }

        if (restante > 0)
            Console.WriteLine($"  Sobraram R$ {restante}, que não podem ser entregues");
    }

    // Crivo de Eratóstenes: um vetor de flags é marcado diretamente em memória.
    private static void ListarPrimos(int limite)
    {
        bool[] ehComposto = new bool[limite + 1];

        for (int n = 2; n * n <= limite; n++)
        {
            if (ehComposto[n]) continue;

            for (int multiplo = n * n; multiplo <= limite; multiplo += n)
                ehComposto[multiplo] = true;
        }

        Console.Write($"Primos até {limite}:");
        for (int n = 2; n <= limite; n++)
        {
            if (!ehComposto[n])
                Console.Write($" {n}");
        }
        Console.WriteLine();
    }

    // Ordenação "in-place": o próprio vetor é modificado por trocas sucessivas.
    private static void OrdenarPorBolha()
    {
        int[] vetor = { 42, 7, 19, 3, 25, 11 };
        Console.WriteLine("Antes : " + string.Join(", ", vetor));

        bool trocou;
        do
        {
            trocou = false;
            for (int i = 0; i < vetor.Length - 1; i++)
            {
                if (vetor[i] > vetor[i + 1])
                {
                    (vetor[i], vetor[i + 1]) = (vetor[i + 1], vetor[i]);
                    trocou = true;
                }
            }
        } while (trocou);

        Console.WriteLine("Depois: " + string.Join(", ", vetor));
    }

    // Acumulador em dicionário: o estado (contagem) cresce enquanto o laço roda.
    private static void ContarPalavras(string texto)
    {
        var contagem = new Dictionary<string, int>();

        foreach (string palavra in texto.Split(' '))
        {
            if (contagem.ContainsKey(palavra))
                contagem[palavra]++;
            else
                contagem[palavra] = 1;
        }

        Console.WriteLine($"Frequência das palavras em \"{texto}\":");
        foreach (var par in contagem)
            Console.WriteLine($"  {par.Key,-6} -> {par.Value}");
    }
}
