# GameStore

## 参考URL

[ASP.NET Core Full Course For Beginners](https://www.youtube.com/watch?v=AhAxLiGC7Pc)

NOTE: 2:56:27 まで視聴

## 開発環境構築方法

### NuGetパッケージを復元

```powershell
cd GameStore.Api
dotnet restore
```

## コマンドメモ

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
