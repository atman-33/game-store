# DTO 入力値チェック

## ステップ

### DTOにアノテーションを追加

`GameStore.Api\Dtos\CreateGameDto.cs`

- [Required] etc を追加

```cs
using System.ComponentModel.DataAnnotations;

namespace GameStore.Api.Dtos;

public record class CreateGameDto(
  [Required][StringLength(50)] string Name,
  [Required][StringLength(20)] string Genre,
  [Range(1, 100)] decimal Price,
  DateOnly ReleaseDate);
```

### NuGet MinimalApis.Extensions を追加

NuGet `MinimalApis.Extensions` パッケージを追加する。

- NuGet サイトに移動し、`MinimalApis.Extensions`で検索する。
- 表示された`MinimalApis.Extensions`の.NET CLI コマンドをコピーする。
- power shell で、コピーしたコマンドを実行する。

```powershell
cd GameStore.Api
dotnet add package MinimalApis.Extensions --version 0.11.0
```

### エンドポイントに WithParameterValidation を追加

- `WithParameterValidation()`を追加

#### エンドポイントグループにValidationを実装

`GameStore.Api\Endpoints\GamesEndpoints.cs`

```cs
    var group = app.MapGroup("games").WithParameterValidation();
```

#### 参考: 一つのエンドポイントにValidationを実装

※実際は、MapGroupにValidationを実装した方が良いため、こちらは参考

`GameStore.Api\Endpoints\GamesEndpoints.cs`

```cs
    // POST /games
    group.MapPost("/", (CreateGameDto newGame) =>
    {
      // ...
      return Results.CreatedAtRoute(GetGameEndpointName, new { id = game.Id }, game);
    }).WithParameterValidation();
```
