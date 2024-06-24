namespace GameStore.Api.Dtos;

/// <summary>
/// ゲームの概要（Genreの名称を返す）
/// </summary>
/// <param name="Id"></param>
/// <param name="Name"></param>
/// <param name="Genre"></param>
/// <param name="Price"></param>
/// <param name="ReleaseDate"></param>
public record class GameSummaryDto(
  int Id,
  string Name,
  string Genre,
  decimal Price,
  DateOnly ReleaseDate);