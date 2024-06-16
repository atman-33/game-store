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
