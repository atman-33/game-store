# GameStore

## 参考URL

[ASP.NET Core Full Course For Beginners](https://www.youtube.com/watch?v=AhAxLiGC7Pc)

## 開発環境構築方法

### NuGetパッケージを復元

```powershell
cd GameStore.Api
dotnet restore
```

## コマンドメモ

### アプリ起動

```powershell
cd GameStore.Api
dotnet run
```

### デバッグ開始

VSCode上でF5を押す。

### DBマイグレーションファイル作成

```powershell
cd GameStore.Api
dotnet ef migrations add InitialCreate --output-dir Data\Migrations
```

### DBマイグレーション

```powershell
cd GameStore.Api
dotnet ef database update
```

> DBマイグレーションは、アプリ起動時に実行するようにしたため開発環境構築時に実行する必要は無い。
