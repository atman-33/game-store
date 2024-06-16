# REST APIを追加

## ステップ

### デバッグ用のリクエスト送信準備

#### games.http を追加

`GameStore.Api\games.http`

```http
GET http://localhost:5280
```

#### サーバーを起動

```powershell
cd GameStore.Api
dotnet run
```

#### games.http に記載した URIに対して Send Request を実行

games.http の Send Request をクリックする。

#### デバッグ時にブラウザを表示しないように設定

`launchSettings.json`の http/https の launchBrowser を`false`に変更する。

`GameStore.Api\Properties\launchSettings.json`

#### デバッグを実行

キーボード F5 ボタンを押す。

### Dtoを作成

- ソリューションエクスプローラー > Dtos フォルダを作成
- ソリューションエクスプローラー > Dtos フォルダを右クリック > レコードファイルを作成

`GameStore.Api\Dtos\GameDto.cs`

> レコード型としているのは、実装の手間を省けるため。
> class（参照型）で値ベースを比較するには、自前でEqualsやGetHashCodeをoverrideする必要があるが、recordはそれらを自動で実装してくれる=実装の手間が省ける。

Dtoファイルを下記のように修正する。  

- namespace を適切な名称に修正
- 必要なプロパティを設定

`GameStore.Api\Dtos\GameDto.cs`

```cs
namespace GameStore.Api.Dtos;

public record class GameDto(
  int Id,
  string Name,
  string Genre,
  decimal Price,
  DateOnly ReleaseDate);
```

### エンドポイントを設定

`GameStore.Api\Program.cs`

e.g. Game一覧を取得するAPI  

```cs
// ...

app.MapGet("/", () => "Hello World!");

List<GameDto> games = [
  new GameDto(1, "The Legend of Zelda: Breath of the Wild", "Adventure", 59.99m, new DateOnly(2017, 3, 3)),
  new GameDto(2, "Super Mario Odyssey", "Platformer", 49.99m, new DateOnly(2017, 10, 27)),
  new GameDto(3, "Red Dead Redemption 2", "Action", 39.99m, new DateOnly(2018, 10, 26)),
  new GameDto(4, "Minecraft", "Sandbox", 29.99m, new DateOnly(2011, 11, 18)),
  new GameDto(5, "The Witcher 3: Wild Hunt", "RPG", 39.99m, new DateOnly(2015, 5, 19))
];

app.MapGet("/games", () => games);

// ...
```

### 同様に他のDtoとエンドポイントを作成

それぞれ、Dtoとエンドポイントを作成する。  

- Create
- Update
- Delete

### エンドポイント用のファイルを作成

`GameStore.Api\Endpoints\GamesEndpoints.cs`

```cs
using GameStore.Api.Dtos;

namespace GameStore.Api.Endpoints;

public static class GamesEndpoints
{
  const string GetGameEndpointName = "GetGame";

  private static readonly List<GameDto> games = [
    new GameDto(1, "The Legend of Zelda: Breath of the Wild", "Adventure", 59.99m, new DateOnly(2017, 3, 3)),
    new GameDto(2, "Super Mario Odyssey", "Platformer", 49.99m, new DateOnly(2017, 10, 27)),
    new GameDto(3, "Red Dead Redemption 2", "Action", 39.99m, new DateOnly(2018, 10, 26)),
    new GameDto(4, "Minecraft", "Sandbox", 29.99m, new DateOnly(2011, 11, 18)),
    new GameDto(5, "The Witcher 3: Wild Hunt", "RPG", 39.99m, new DateOnly(2015, 5, 19))
  ];

  public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app)
  {
    // NOTE: MapGroup に "games" を設定することにより、MapGet などの設定で "games" を省略できる 
    var group = app.MapGroup("games");

    // GET /games
    group.MapGet("/", () => games);

    // GET /games/{id}
    group.MapGet("/{id}", (int id) =>
    {
      GameDto? game = games.Find(game => game.Id == id);

      return game is null ? Results.NotFound() : Results.Ok(game);
    })
    .WithName(GetGameEndpointName);

    // POST /games
    group.MapPost("/", (CreateGameDto newGame) =>
    {
      GameDto game = new(
        games.Count + 1,
        newGame.Name,
        newGame.Genre,
        newGame.Price,
        newGame.ReleaseDate
      );
      games.Add(game);

      // NOTE: 201レスポンスを生成
      // GetGameEndpointName の名前付きルートを使用して、新しいゲームを返している。
      return Results.CreatedAtRoute(GetGameEndpointName, new { id = game.Id }, game);
    });

    // PUT /games/{id}
    group.MapPut("/{id}", (int id, UpdateGameDto updatedGame) =>
    {
      var index = games.FindIndex(game => game.Id == id);

      if (index == -1)
      {
        return Results.NotFound();
      }

      games[index] = new GameDto(
        id,
        updatedGame.Name,
        updatedGame.Genre,
        updatedGame.Price,
        updatedGame.ReleaseDate
      );

      return Results.NoContent();
    });

    // DELETE /games/{id}
    group.MapDelete("/{id}", (int id) =>
    {
      games.RemoveAll(game => game.Id == id);

      return Results.NoContent();
    });

    return group;
  }
}
```

`GameStore.Api\Program.cs`

```cs
using GameStore.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// NOTE: Gameに関するエンドポイントをマップ
app.MapGamesEndpoints();

app.Run();
```

> 他のAPIを追加していく場合も、同様に機能毎にエンドポイントファイルを作成し、Program.csでマッピングしていく。
