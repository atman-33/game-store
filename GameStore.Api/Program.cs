using GameStore.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// NOTE: Gameに関するエンドポイントをマップ
app.MapGamesEndpoints();

app.Run();
