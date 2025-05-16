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
    new ("Renan", "Romig", "renanromig@outlook.com"),
    new ("Arianne", "Romig", "arianneromig@outlook.com"),
    new ("Nicole", "Romig", "nicoleromig@hotmail.com")
    ];

app.UseRateLimiter();

app.MapGet("/person", () => people).RequireRateLimiting("fixed");
app.MapGet("/person/{id}", (int id) => people[id]);
app.MapPost("/person", (PersonRecord p) => people.Add(p));
app.MapDelete("/person", (int id) => people.RemoveAt(id));

app.Run();

public record PersonRecord(string FirstName, string LastName, string Email);

