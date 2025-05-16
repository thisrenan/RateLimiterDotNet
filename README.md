# RateLimiterDotNet

O middleware Microsoft.AspNetCore.RateLimitting fornece limitação de chamadas nas chamada a endpoints.

## Limitação de Taxa com Microsoft.AspNetCore.RateLimiting

O middleware Microsoft.AspNetCore.RateLimiting oferece um mecanismo eficiente para controlar o número de chamadas aos endpoints de uma aplicação ASP.NET Core. Ele permite aplicar políticas de limitação de taxa (rate limiting), protegendo a aplicação contra abusos, melhorando o desempenho e garantindo o uso equilibrado dos recursos.

🚦 Por que aplicar limitação de taxa?
A limitação de taxa é uma prática essencial para manter a saúde, estabilidade e segurança de aplicações web, especialmente APIs públicas. Veja os principais motivos para utilizá-la:

🔐 Prevenção de Abusos
Restringir o número de requisições por usuário ou cliente ajuda a evitar uso indevido, como ataques automatizados ou consumo excessivo de recursos por um único agente.

⚖️ Garantia de Uso Justo
Ao definir limites por usuário, a aplicação promove um acesso mais equitativo, evitando que poucos usuários monopolizem os recursos do sistema.

🛡️ Proteção de Recursos
Ao controlar o volume de requisições processadas, a limitação de taxa protege o back-end contra sobrecarga, contribuindo para a estabilidade geral da aplicação.

📉 Mitigação de Ataques DoS
Embora não substitua soluções dedicadas, a limitação de taxa ajuda a mitigar ataques de negação de serviço (DoS) ao dificultar que um único cliente sobrecarregue o sistema.

🚀 Otimização de Desempenho
Ao evitar picos de carga não controlados, o sistema mantém sua capacidade de resposta e oferece uma melhor experiência para todos os usuários.

💸 Gerenciamento de Custos
Em aplicações cujo custo está atrelado ao volume de requisições (como serviços em nuvem), a limitação de taxa ajuda a controlar e prever despesas.

## Aqui está um exemplo completo de como configurar o RateLimiterMiddleware no ASP.NET Core, utilizando o pacote Microsoft.AspNetCore.RateLimiting.

📦 Instale o pacote necessário (caso ainda não tenha)

```
dotnet add package Microsoft.AspNetCore.RateLimiting
```

🛠️ Configuração em Program.cs (ASP.NET Core 7 ou superior)

```dotnet
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddRateLimiter(_ => _
    .AddFixedWindowLimiter(policyName: "fixed", options =>
    {
        options.PermitLimit = 4;
        options.Window = TimeSpan.FromSeconds(12);
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 2;
    }));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => {
        options.SwaggerEndpoint("/openapi/v1.json", "Version One");
    });
}

app.UseHttpsRedirection();

List<PersonRecord> people = [
    new ("Renan", "Romig", "renanromig@hotmail.com"),
    new ("Arianne", "Romig", "annesantos.03@hotmail.com"),
    new ("Nicole", "Romig", "nicoleromig@hotmail.com")
    ];

app.UseRateLimiter();

static string GetTicks() => (DateTime.Now.Ticks & 0x11111).ToString("00000");

app.MapGet("/", () => Results.Ok($"Hello {GetTicks()}")).RequireRateLimiting("fixed");
app.MapGet("/person", () => people).RequireRateLimiting("fixed");
app.MapGet("/person/{id}", (int id) => people[id]);
app.MapPost("/person", (PersonRecord p) => people.Add(p));
app.MapDelete("/person", (int id) => people.RemoveAt(id));

app.Run();

public record PersonRecord(string FirstName, string LastName, string Email);
```

📌 Explicações rápidas

- PermitLimit: Quantas requisições são permitidas no intervalo
- Window: Duração da janela de tempo para esse limite
- QueueLimit Quantas requisições podem ficar aguardando na fila (0 = nenhuma)
- QueueProcessingOrder- Ordem de processamento das filas (ex: FIFO)

🚨 Observações

- O FixedWindowRateLimiter é simples e fácil de usar, mas você pode substituir por SlidingWindowRateLimiter, TokenBucketRateLimiter ou ConcurrencyLimiter conforme a necessidade.
- Políticas podem ser aplicadas a grupos de endpoints ou a rotas individuais com .RequireRateLimiting("NomeDaPolicy").

# Dicas

📌 Pode-se utilizar em paralelo com JWT ou controle por usuários

- Autenticação baseada em JWT integrada à limitação (por usuário)
- Uso de política baseada em User.Identity.Name em vez de IP
