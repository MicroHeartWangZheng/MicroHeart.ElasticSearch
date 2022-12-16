using ElasticSearch.Example.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

//ע��
builder.Services.AddRepository();

var app = builder.Build();



app.UseAuthorization();

app.MapControllers();

app.Run();
