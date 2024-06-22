using GameStore.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Data;

// NOTE: プライマリコンストラクタの記述により、クラスに引数を渡すことができる
public class GameStoreContext(DbContextOptions<GameStoreContext> options) : DbContext(options)
{
  public DbSet<Game> Games => Set<Game>();

  public DbSet<Genre> Genres => Set<Genre>();
}
