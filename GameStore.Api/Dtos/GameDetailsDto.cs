namespace GameStore.Api.Dtos;

/// <summary>
/// ゲーム詳細（フロントエンドが利用し易いGenreIdを返す）
/// </summary>
/// <param name="Id"></param>
/// <param name="Name"></param>
/// <param name="GenreId"></param>
/// <param name="Price"></param>
/// <param name="ReleaseDate"></param>
public record class GameDetailsDto(
  int Id,
  string Name,
  int GenreId,
  decimal Price,
  DateOnly ReleaseDate);