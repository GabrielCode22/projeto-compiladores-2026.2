# exemplos-origem — exemplos em C#

Esta pasta contém **14 arquivos-fonte de exemplo em C#** (linguagem de origem do transpilador),
um para cada tema tratado nos slides do seminário. Todos fazem parte de um único projeto
.NET, e um menu em `Program.cs` permite executar cada exemplo separadamente.

## Arquivos

| Arquivo                            | Tema                | O que demonstra                                                                 |
|------------------------------------|---------------------|---------------------------------------------------------------------------------|
| `01_ParadigmaImperativo.cs`        | Paradigmas          | Estado mutável, laços e procedimentos (troco de saque, crivo de primos, ordenação) |
| `02_ParadigmaOO.cs`                | Paradigmas          | Classes, herança, abstract/override, interfaces, polimorfismo e composição (biblioteca) |
| `03_ParadigmaFuncional.cs`         | Paradigmas          | Lambdas, closures, currying, LINQ, `record` imutável, avaliação preguiçosa       |
| `04_TiposEVariaveis.cs`            | Tipos e variáveis   | Primitivos, valor x referência, anuláveis, conversões, enums, tuplas, coleções, strings |
| `05_ExpressoesEOperadores.cs`      | Expressões          | Operadores, curto-circuito, bit a bit, operadores de nulo, `switch` com pattern matching |
| `06_ControleDeFluxo.cs`            | Controle de fluxo   | `if`, `switch`, `for`, `foreach`, `while`, `do-while`, `break`/`continue`/`goto`, máquina de estados |
| `07_Subprogramas.cs`               | Subprogramas        | `ref`/`out`/`in`, `params`, opcionais e nomeados, sobrecarga, recursão, função local |
| `08_SubprogramasAvancados.cs`      | Subprogramas        | Delegates, closures, genéricos, iteradores (`yield`), métodos de extensão, memoização |
| `09_ConcorrenciaThreads.cs`        | Concorrência        | `Thread`, `lock`, `Interlocked`, `SemaphoreSlim`, `ThreadPool`, prevenção de deadlock |
| `10_ConcorrenciaAsync.cs`          | Concorrência        | `async`/`await`, `Task.WhenAll`/`WhenAny`, cancelamento, `IAsyncEnumerable`      |
| `11_ConcorrenciaPipeline.cs`       | Concorrência        | `Channel<T>` (produtor/consumidor), `Parallel`, PLINQ, `ConcurrentDictionary`    |
| `12_Excecoes.cs`                   | Exceções            | `try`/`catch`/`finally`, exceções próprias, filtros `when`, `throw;`, `using`    |
| `13_Eventos.cs`                    | Eventos             | `event`, `EventHandler<T>`, `EventArgs`, assinatura (`+=`) e cancelamento (`-=`) |
| `14_ParadigmaOrientadoAEventos.cs` | Eventos/Paradigmas  | Barramento de eventos (pub/sub) e laço de eventos de interface                   |

Arquivos de apoio: `Program.cs` (menu/ponto de entrada) e `ExemplosOrigem.csproj` (configuração do projeto).

## Compilador utilizado

Os exemplos são compilados pelo **compilador Roslyn (`csc`)**, que acompanha o **.NET SDK**.
Versão alvo: **.NET 10** (os exemplos também funcionam no **.NET 8 ou superior**; basta trocar a linha
`<TargetFramework>` em `ExemplosOrigem.csproj`, por exemplo para `net8.0`).

### Como instalar o .NET SDK

Baixe o instalador oficial em <https://dotnet.microsoft.com/download> ou use um gerenciador de pacotes:

- **Windows** (PowerShell):
  ```powershell
  winget install Microsoft.DotNet.SDK.10
  ```
- **macOS** (Homebrew):
  ```bash
  brew install --cask dotnet-sdk
  ```
- **Linux (Ubuntu/Debian)**:
  ```bash
  sudo apt-get update
  sudo apt-get install -y dotnet-sdk-10.0
  ```
  Se o pacote não existir na sua distribuição, use o script oficial de instalação:
  ```bash
  curl -sSL https://dot.net/v1/dotnet-install.sh -o dotnet-install.sh
  bash dotnet-install.sh --channel 10.0
  export PATH="$HOME/.dotnet:$PATH"
  ```

Confirme a instalação:

```bash
dotnet --version
```

## Como compilar e executar

A partir da raiz do repositório:

```bash
cd exemplos-origem

# 1) Compilar (verifica todos os arquivos .cs)
dotnet build

# 2) Executar com o menu interativo
dotnet run

# 3) Executar um exemplo específico (número de 01 a 14)
dotnet run -- 03

# 4) Executar todos os exemplos em sequência
dotnet run -- todos
```

Para gerar apenas o executável em modo de produção:

```bash
dotnet publish -c Release -o publicado
```

## Observações

- A **saída dos exemplos de concorrência** (09, 10 e 11) pode variar entre execuções, pois depende do
  escalonamento das threads. Isso é intencional: o exemplo 09 mostra uma condição de corrida em que o
  contador sem proteção costuma ficar abaixo do valor esperado.
- Os exemplos 10 e 11 usam `Task.Delay` para simular operações de rede, portanto **não precisam de internet**.
- Cada arquivo começa com um comentário explicando o tema, os recursos da linguagem usados e o cenário de aplicação.
