using GameStore.Api.Data;
using GameStore.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// NOTE: GameStoreContext を SQLite で利用する設定
// var connString = "Data Source=GameStore.db"; // <= ハードコーディングは非推奨!!
var connString = builder.Configuration.GetConnectionString("GameStore");
builder.Services.AddSqlite<GameStoreContext>(connString);

var app = builder.Build();

// NOTE: Gameに関するエンドポイントをマップ
app.MapGamesEndpoints();

// NOTE: データベースをマイグレーション
app.MigrateDb();

app.Run();
