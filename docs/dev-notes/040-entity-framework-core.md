# エンティティフレームワークコア

## ステップ

### Entitiesフォルダを作成

- ソリューションエクスプローラー > `Entities`フォルダを作成

### Entityクラスを作成

- ソリューションエクスプローラー > `Entities`フォルダ > `Genre`クラスを作成
- ソリューションエクスプローラー > `Entities`フォルダ > `Game`クラスを作成

e.g.  

`GameStore.Api\Entities\Game.cs`

```cs
namespace GameStore.Api.Entities;

public class Game
{
  public int Id { get; set; }
  public required string Name { get; set; }

  public int GenreId { get; set; }

  public Genre? Genre { get; set; }

  public decimal Price { get; set; }

  public DateOnly ReleaseDate { get; set; }
}
```

### NuGet Entity Framework Core をインストール

- NuGet サイトに移動し、`Microsoft.EntityFrameworkCore.Sqlite`で検索する。
- 表示された`Microsoft.EntityFrameworkCore.Sqlite`の.NET CLI コマンドをコピーする。
- power shell で、コピーしたコマンドを実行する。

```powershell
cd GameStore.Api
dotnet add package Microsoft.EntityFrameworkCore.Sqlite --version 8.0.6
```

### コンテキストファイルを追加

- ソリューションエクスプローラー > `Data`フォルダを作成
- ソリューションエクスプローラー > `Data`フォルダ > `GameStoreContext`クラスを作成
- クラスに`DbContext`を継承
- プライマリコンストラクタにて、DbContextOptionsの引数を追加
- DbSetプロパティを追加

`GameStore.Api\Data\GameStoreContext.cs`

```cs
using GameStore.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Data;

// NOTE: プライマリコンストラクタの記述により、クラスに引数を渡すことができる
public class GameStoreContext(DbContextOptions<GameStoreContext> options) : DbContext(options)
{
  public DbSet<Game> Games => Set<Game>();

  public DbSet<Genre> Genres => Set<Genre>();
}
```

### WebApplicationビルダーに、SQLite連携用のコンテキストを追加

`GameStore.Api\Program.cs`

```cs
using GameStore.Api.Data;
using GameStore.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// NOTE: GameStoreContext を SQLite で利用する設定
var connString = "Data Source=GameStore.db";
builder.Services.AddSqlite<GameStoreContext>(connString);

// ...
```

### DB接続文字列をConfigを利用するよう修正

- appssettings.json にDB接続文字列を追加

`GameStore.Api\appsettings.json`

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "GameStore": "Data Source=GameStore.db"
  }
}
```

- Configから取得したDB接続文字列を利用するように変更

`GameStore.Api\Program.cs`

```cs
using GameStore.Api.Data;
using GameStore.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// NOTE: GameStoreContext を SQLite で利用する設定
// var connString = "Data Source=GameStore.db"; // <= ハードコーディングは非推奨!!
var connString = builder.Configuration.GetConnectionString("GameStore");
builder.Services.AddSqlite<GameStoreContext>(connString);

// ...
```

### EntityFramwwork コマンドを使用可能にするパッケージをインストール

NuGet サイトから下記パッケージをインストールする。  

- dotnet-ef
- EntityFrameworkCore.Design

```powershell
cd GameStore.Api
dotnet tool install --global dotnet-ef --version 8.0.6
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.6
```

### DBマイグレーションを実行

#### 1. マイグレーションファイルを作成

```powershell
cd GameStore.Api
dotnet ef migrations add InitialCreate --output-dir Data\Migrations
```

> InitialCreateは、マイグレーション内容などのコメント。内容に応じて変更する。

#### 2. マイグレーションを実行

```powershell
dotnet ef database update
```

> コマンド実行後、appsettings.jsoon で指定したDBファイルが作成される。

DBファイルは、git管理から除外した方が良いのでgitigonoreで指定しておくこと。

```gitignore
# DB
# DB
/GameStore.Api/*.db
/GameStore.Api/*.db-shm
/GameStore.Api/*.db-wal
```

### アプリ起動時にDBマイグレーションを実行する方法

- ソリューションエクスプローラー > `Data`フォルダ > `DataExtensions`クラスを作成

`GameStore.Api\Data\DataExtensions.cs`

```cs
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Data;

public static class DataExtensions
{
  /// <summary>
  /// データベースをマイグレーション
  /// </summary>
  /// <param name="app"></param>
  public static void MigrateDb(this WebApplication app)
  {
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<GameStoreContext>();
    dbContext.Database.Migrate();
  }
}
```

- Program.cs にマイグレーション処理を追加

`GameStore.Api\Program.cs`

```cs
// ...

app.MapGamesEndpoints();

// NOTE: データベースをマイグレーション
app.MigrateDb();

app.Run();
```

### DBマイグレーション時の不要なログを表示させないようにする

例えば、下記のような`Microsoft.EntityFrameworkCore.Database.Command`のinfoレベルのログを表示させないようにする。

```powershell
info: Microsoft.EntityFrameworkCore.Database.Command[20101]
      Executed DbCommand (0ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
```

appsettings.json の Logging に、`Microsoft.EntityFrameworkCore.Database.Command`はWarningレベル以上を表示するように変更する。

`GameStore.Api\appsettings.json`

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore.Database.Command": "Warning"
    }
  },
  ...
```

### 初期データの生成方法

- コンテキストのOnModelCreatingをオーバーライドする。

`GameStore.Api\Data\GameStoreContext.cs`

```cs
public class GameStoreContext(DbContextOptions<GameStoreContext> options) : DbContext(options)
{
  // ...
  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<Genre>().HasData(
        new { Id = 1, Name = "Fighting" },
        new { Id = 2, Name = "Roleplaying" },
        new { Id = 3, Name = "Sports" },
        new { Id = 4, Name = "Racing" },
        new { Id = 5, Name = "Kids and Family" }
    );
  }
}
```

- マイグレーションファイルを更新

```powershell
cd GameStore.Api
dotnet ef migrations add SeedGenres --output-dir Data\Migrations
```

後は、マイグレーションを実行（アプリを起動）すれば、DBデータが初期生成される。
