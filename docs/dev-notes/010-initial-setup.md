# VSCode セットアップ

## 参考URL

[ASP.NET Core Full Course For Beginners](https://www.youtube.com/watch?v=AhAxLiGC7Pc)

NOTE: 23:57まで視聴

## 前提

下記をインストール

- .NET SDK
- VSCode

## 拡張機能

下記をインストール

- C# Dev Kit
- REST Client
- SQLite

> 補足. コマンドからdotnetで作成可能なテンプレートを確認する方法

```powershell
dotnet new list
```

## プロジェクト作成

1. VSCode > Ctrl+Shift+P > .NET 新しいプロジェクトを選択
2. ASP.NET Core (空) を選択

## プロジェクトをビルド

### ソリューションエクスプローラーからビルド

VSCode > ソリューションエクスプローラー > プロジェクトを右クリック > ビルドを選択

### コマンドからビルド

slnファイルが存在するディレクトリに移動して、ビルドコマンドを実行する。  

```powershell
dotnet build
```

### ショートカットからビルド

Ctrl+Sihft+B > ビルド

## アプリケーションを実行

VSCode > F5(C#ファイルを開いた状態) > C# > Project名(Default Configuration) を選択
