using BooksWishlist.Presentation.Modules;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.




string issuer;
string audience;
string signingKey;

issuer = builder.Configuration["Issuer"];
audience = builder.Configuration["Audience"];
signingKey = builder.Configuration["SigningKey"];


builder.Services.AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .ConfigureApiVersioning()
    .AddHttpContextAccessor()
    .AddAuthorization()
    .ConfigureAuthentication(issuer, audience, signingKey)
    .AddEndpointsApiExplorer();

//TODO: Add Open API Module

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    _ = app.UseSwagger()
        .UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
