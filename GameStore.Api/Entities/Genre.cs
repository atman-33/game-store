namespace GameStore.Api.Entities;

public class Genre
{
  public int Id { get; set; }

  // NOTE: required を付けるとNULL非許可
  public required string Name { get; set; }
}
