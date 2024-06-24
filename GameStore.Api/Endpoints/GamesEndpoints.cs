using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Entities;
using GameStore.Api.Mapping;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Endpoints;

public static class GamesEndpoints
{
  const string GetGameEndpointName = "GetGame";

  public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app)
  {
    // NOTE: MapGroup に "games" を設定することにより、MapGet などの設定で "games" を省略できる 
    // NOTE: WithParameterValidation は、DtoアノテーションとMinimalApis.Extensionsで値をチェックをする。
    var group = app.MapGroup("games").WithParameterValidation();

    // GET /games
    group.MapGet("/", async (GameStoreContext dbContext) =>
      await dbContext.Games
        .Include(game => game.Genre)  // NOTE: Genre の情報を含める（Genre.Nameを返すため）
        .Select(game => game.ToGameSummaryDto())
        .AsNoTracking() // NOTE: EF Core によるデータの変更追跡を無視する（パフォーマンス向上のため）
        .ToListAsync()  // NOTE: 非同期処理
        );

    // GET /games/{id}
    group.MapGet("/{id}", async (int id, GameStoreContext dbContext) =>
    {
      Game? game = await dbContext.Games.FindAsync(id);

      return game is null ? Results.NotFound() : Results.Ok(game.ToGameDetailsDto());
    })
    .WithName(GetGameEndpointName);

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

    // PUT /games/{id}
    group.MapPut("/{id}", async (int id, UpdateGameDto updatedGame, GameStoreContext dbContext) =>
    {
      var existingGame = await dbContext.Games.FindAsync(id);

      if (existingGame is null)
      {
        return Results.NotFound();
      }

      dbContext.Entry(existingGame).CurrentValues.SetValues(updatedGame.ToEntity(id));
      await dbContext.SaveChangesAsync();

      return Results.NoContent();
    });

    // DELETE /games/{id}
    group.MapDelete("/{id}", async (int id, GameStoreContext dbContext) =>
    {
      await dbContext.Games.Where(game => game.Id == id).ExecuteDeleteAsync();

      return Results.NoContent();
    });

    return group;
  }
}
