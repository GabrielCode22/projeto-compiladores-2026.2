using System.Text;

namespace ExemplosOrigem;

// ---------- Eventos de domínio (uma loja online) ----------

public abstract record EventoDeLoja;
public record PedidoCriado(int PedidoId, string Cliente, decimal Total) : EventoDeLoja;
public record PagamentoAprovado(int PedidoId) : EventoDeLoja;
public record PedidoEnviado(int PedidoId, string CodigoDeRastreio) : EventoDeLoja;

// Barramento de eventos (publish/subscribe): quem publica não conhece quem assina.
public sealed class BarramentoDeEventos
{
    private readonly Dictionary<Type, List<Delegate>> _assinantes = new();

    public void Assinar<T>(Action<T> tratador) where T : EventoDeLoja
    {
        if (!_assinantes.TryGetValue(typeof(T), out var lista))
        {
            lista = new List<Delegate>();
            _assinantes[typeof(T)] = lista;
        }
        lista.Add(tratador);
    }

    public void Publicar<T>(T evento) where T : EventoDeLoja
    {
        if (!_assinantes.TryGetValue(typeof(T), out var lista))
            return;

        // ToList(): copia a lista, pois um tratador pode assinar/publicar novos eventos.
        foreach (Delegate tratador in lista.ToList())
            ((Action<T>)tratador)(evento);
    }
}

// ---------- Eventos de interface (uma janela gráfica simulada) ----------

public abstract record EventoDeTela;
public record Clique(int X, int Y) : EventoDeTela;
public record TeclaPressionada(char Tecla) : EventoDeTela;
public record FecharJanela : EventoDeTela;

/// <summary>
/// TEMA: Paradigma orientado a eventos.
///
/// Neste paradigma o fluxo do programa não é ditado por uma sequência fixa de
/// chamadas, e sim pela OCORRÊNCIA de eventos, aos quais tratadores (handlers)
/// reagem. Dois estilos clássicos aparecem aqui:
///
///   1) Barramento de eventos (pub/sub) — serviços independentes que reagem
///      em cadeia: PedidoCriado -> PagamentoAprovado -> PedidoEnviado.
///   2) Laço de eventos (event loop) — uma fila de eventos de interface
///      processada um a um, como fazem as bibliotecas de GUI.
///
/// Complementa o exemplo 13, que mostra o mecanismo de "event" da linguagem.
/// </summary>
public static class ParadigmaOrientadoAEventos
{
    public static void Executar()
    {
        BarramentoDaLoja();
        LacoDeEventosDeInterface();
    }

    private static void BarramentoDaLoja()
    {
        Console.WriteLine("--- Barramento de eventos: reações em cadeia ---");

        var barramento = new BarramentoDeEventos();
        var auditoria = new List<string>();

        // Auditoria: ouve os três tipos de evento sem que os outros serviços saibam.
        // Assinada primeiro para registrar os eventos na ordem em que acontecem.
        barramento.Assinar<PedidoCriado>(e => auditoria.Add($"criado #{e.PedidoId}"));
        barramento.Assinar<PagamentoAprovado>(e => auditoria.Add($"pago #{e.PedidoId}"));
        barramento.Assinar<PedidoEnviado>(e => auditoria.Add($"enviado #{e.PedidoId}"));

        // Serviço de estoque: só reage a pedidos criados.
        barramento.Assinar<PedidoCriado>(e =>
            Console.WriteLine($"  [Estoque]       reservando itens do pedido #{e.PedidoId}"));

        // Serviço de pagamento: reage a PedidoCriado e produz PagamentoAprovado.
        barramento.Assinar<PedidoCriado>(e =>
        {
            Console.WriteLine($"  [Pagamento]     cobrando R$ {e.Total:F2} de {e.Cliente}");
            barramento.Publicar(new PagamentoAprovado(e.PedidoId));
        });

        // Serviço de logística: reage ao pagamento e produz PedidoEnviado.
        barramento.Assinar<PagamentoAprovado>(e =>
        {
            Console.WriteLine($"  [Logística]     despachando o pedido #{e.PedidoId}");
            barramento.Publicar(new PedidoEnviado(e.PedidoId, $"BR{e.PedidoId:D6}"));
        });

        // Serviço de notificação: avisa o cliente no final da cadeia.
        barramento.Assinar<PedidoEnviado>(e =>
            Console.WriteLine($"  [Notificação]   pedido #{e.PedidoId} enviado, rastreio {e.CodigoDeRastreio}"));

        // Um único evento inicial dispara todo o fluxo.
        barramento.Publicar(new PedidoCriado(1001, "Ana", 349.90m));

        Console.WriteLine($"  Auditoria: {string.Join(" -> ", auditoria)}");
    }

    private static void LacoDeEventosDeInterface()
    {
        Console.WriteLine("\n--- Laço de eventos (event loop) de uma janela ---");

        var fila = new Queue<EventoDeTela>(new EventoDeTela[]
        {
            new Clique(10, 20),
            new TeclaPressionada('o'),
            new TeclaPressionada('i'),
            new Clique(300, 40),
            new FecharJanela(),
            new Clique(1, 1),                  // chega depois do fechamento: nunca será tratado
        });

        var digitado = new StringBuilder();
        bool rodando = true;

        while (rodando && fila.Count > 0)
        {
            EventoDeTela evento = fila.Dequeue();

            switch (evento)
            {
                case Clique { X: < 100 } clique:
                    Console.WriteLine($"  clique em ({clique.X}, {clique.Y}): botão 'Salvar' acionado");
                    break;

                case Clique clique:
                    Console.WriteLine($"  clique em ({clique.X}, {clique.Y}): área vazia da janela");
                    break;

                case TeclaPressionada tecla:
                    digitado.Append(tecla.Tecla);
                    Console.WriteLine($"  tecla '{tecla.Tecla}' -> texto atual: \"{digitado}\"");
                    break;

                case FecharJanela:
                    Console.WriteLine("  janela fechada: encerrando o laço de eventos");
                    rodando = false;
                    break;
            }
        }

        Console.WriteLine($"  Eventos ignorados por terem chegado após o fechamento: {fila.Count}");
    }
}
