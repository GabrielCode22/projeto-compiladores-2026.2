namespace ExemplosOrigem;

// Dados que acompanham o evento. Por convenção, herdam de EventArgs.
public class TemperaturaEventArgs : EventArgs
{
    public double Anterior { get; }
    public double Atual { get; }

    public TemperaturaEventArgs(double anterior, double atual)
    {
        Anterior = anterior;
        Atual = atual;
    }
}

// PUBLICADOR: expõe eventos, mas não sabe quem está ouvindo.
public class Termostato
{
    private readonly double _limite;
    private double _temperatura;

    // Padrão do .NET: EventHandler<TEventArgs> => assinatura (object? sender, TEventArgs e).
    public event EventHandler<TemperaturaEventArgs>? TemperaturaMudou;
    public event EventHandler<TemperaturaEventArgs>? LimiteExcedido;

    public Termostato(double temperaturaInicial, double limite)
    {
        _temperatura = temperaturaInicial;
        _limite = limite;
    }

    public double Temperatura
    {
        get => _temperatura;
        set
        {
            if (value == _temperatura) return;

            var args = new TemperaturaEventArgs(_temperatura, value);
            _temperatura = value;

            OnTemperaturaMudou(args);
            if (value > _limite)
                OnLimiteExcedido(args);
        }
    }

    // 'protected virtual On...': convenção que permite às classes filhas interceptar o evento.
    // O operador ?. evita NullReferenceException quando ninguém assinou o evento.
    protected virtual void OnTemperaturaMudou(TemperaturaEventArgs e) => TemperaturaMudou?.Invoke(this, e);

    protected virtual void OnLimiteExcedido(TemperaturaEventArgs e) => LimiteExcedido?.Invoke(this, e);
}

// ASSINANTE 1: exibe todas as mudanças.
public class PainelDeControle
{
    public void Assinar(Termostato termostato) => termostato.TemperaturaMudou += AoMudar;

    private void AoMudar(object? origem, TemperaturaEventArgs e)
    {
        string seta = e.Atual > e.Anterior ? "subiu" : "desceu";
        Console.WriteLine($"    [Painel] temperatura {seta}: {e.Anterior:F1} -> {e.Atual:F1} °C");
    }
}

// ASSINANTE 2: só se importa com o limite excedido.
public class AlarmeDeIncendio
{
    public int Disparos { get; private set; }

    public void AoExceder(object? origem, TemperaturaEventArgs e)
    {
        Disparos++;
        string nome = origem is Termostato ? "termostato" : "origem desconhecida";
        Console.WriteLine($"    [ALARME] {nome} acusou {e.Atual:F1} °C — disparo nº {Disparos}");
    }
}

/// <summary>
/// TEMA: Eventos.
///
/// Recursos demonstrados:
///   - delegates como base dos eventos e a palavra-chave "event";
///   - o padrão EventHandler&lt;TEventArgs&gt; com EventArgs personalizado;
///   - publicador x assinantes (Observer) totalmente desacoplados;
///   - assinatura (+=) e cancelamento de assinatura (-=);
///   - handlers como métodos, como lambdas e múltiplos assinantes (multicast).
///
/// Exemplo aplicável: monitoramento de temperatura de uma sala de servidores.
/// </summary>
public static class Eventos
{
    public static void Executar()
    {
        var termostato = new Termostato(temperaturaInicial: 22, limite: 30);
        var painel = new PainelDeControle();
        var alarme = new AlarmeDeIncendio();
        var historico = new List<double>();

        // Vários assinantes para o mesmo publicador (multicast).
        painel.Assinar(termostato);
        termostato.LimiteExcedido += alarme.AoExceder;
        termostato.TemperaturaMudou += (origem, e) => historico.Add(e.Atual);   // handler anônimo

        Console.WriteLine("--- Três mudanças de temperatura ---");
        termostato.Temperatura = 25;
        termostato.Temperatura = 31;        // passa do limite: o alarme dispara
        termostato.Temperatura = 28;

        Console.WriteLine("\n--- Cancelando a assinatura do alarme (-=) ---");
        termostato.LimiteExcedido -= alarme.AoExceder;
        termostato.Temperatura = 35;        // acima do limite, mas ninguém mais escuta o alarme

        Console.WriteLine("\n--- Resumo ---");
        Console.WriteLine($"  Disparos do alarme: {alarme.Disparos}");
        Console.WriteLine($"  Histórico registrado pela lambda: {string.Join(", ", historico)}");
        Console.WriteLine($"  Maior temperatura vista: {historico.Max():F1} °C");

        // Evento sem assinantes: o publicador continua funcionando normalmente.
        var semAssinantes = new Termostato(20, 25);
        semAssinantes.Temperatura = 40;
        Console.WriteLine($"  Termostato sem assinantes seguiu normalmente em {semAssinantes.Temperatura} °C");
    }
}
