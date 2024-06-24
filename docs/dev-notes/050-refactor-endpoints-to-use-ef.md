# エンティティフレームワークを利用するためにエンドポイントを改修

## マッピングを追加

- ソリューションエクスプローラー > `Mapping`フォルダを作成
- ソリューションエクスプローラー > `Mapping`フォルダ > `GameMapping`クラスを作成

`GameStore.Api\Mapping\GameMapping.cs`

```cs
using GameStore.Api.Dtos;
using GameStore.Api.Entities;

namespace GameStore.Api.Mapping;

public static class GameMapping
{
  public static Game ToEntity(this CreateGameDto game)
  {
    return new Game()
    {
      Name = game.Name,
      GenreId = game.GenreId,
      Price = game.Price,
      ReleaseDate = game.ReleaseDate
    };
  }

  public static GameDto ToDto(this Game game)
  {
    return new GameDto(game.Id, game.Name, game.Genre!.Name, game.Price, game.ReleaseDate);
  }
}
```

## エンドポイントを修正

各エンドポイントを修正する。

**ポイント**  

- HTTPメソッド登録に、コンテキストをDIする。
- レスポンスはDTO（レスポンス仕様を遵守するため）で返す。
- Maggpingを利用して、DTOとEntity変換を効率化する。
- 非同期処理に変更する。

e.g.  

`GameStore.Api\Endpoints\GamesEndpoints.cs`

```cs
    // POST /games
    group.MapPost("/", async (CreateGameDto newGame, GameStoreContext dbContext) =>
    {
      // NOTE: 下記のように入力値チェックはここでは行わず、DtoアノテーションとMinimalApis.Extensionsでチェックをする。
      // if (string.IsNullOrEmpty(newGame.Name))
      // {
      //   return Results.BadRequest("Name is required");
      // }

      Game game = newGame.ToEntity();

      dbContext.Games.Add(game);
      await dbContext.SaveChangesAsync();  // NOTE: コミット

      // NOTE: 201レスポンスを生成
      // GetGameEndpointName の名前付きルートを使用して、新しいゲームを返している。
      return Results.CreatedAtRoute(GetGameEndpointName, new { id = game.Id }, game.ToGameDetailsDto());
    });
```
