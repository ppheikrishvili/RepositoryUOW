
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using RepositoryUOWDomain.Interface;

namespace RepositoryUOW;

public sealed class AppDbContext : DbContext, IAppDbContext
{
    // ReSharper disable once UnusedMember.Local
    private DbSet<EntityFrameworkCore.MemoryJoin.QueryModelClass>? QueryData { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured) return;
        optionsBuilder.UseSqlServer(@"Data Source=localhost;Initial Catalog=RepositoryUOWTest;" +
                                    "User ID=sa;Password=123;Connection Timeout=30;TrustServerCertificate=True",
            builder => { builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null); });
    }

    internal static List<Type> ApplyEntityMapsFromAssembly() => Assembly.GetCallingAssembly().GetTypes()
        .Where(w => typeof(IBaseMapModel).IsAssignableFrom(w) &&
                    w.IsClass && w.BaseType!=null && !w.IsInterface && !w.IsAbstract).ToList();

    public AppDbContext()
    {
        ChangeTracker.LazyLoadingEnabled = false;
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    public AppDbContext(DbContextOptions options) : base(options)
    {
        ChangeTracker.LazyLoadingEnabled = false;
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ApplyEntityMapsFromAssembly().ForEach(t => Activator.CreateInstance(t, modelBuilder));
        //RemovePluralizingTableNameConvention( modelBuilder);
    }

    //public static void RemovePluralizingTableNameConvention(ModelBuilder modelBuilder)
    //{
    //    foreach (IMutableEntityType entity in modelBuilder.Model.GetEntityTypes())
    //    {
    //        if (entity.BaseType == null)
    //        {
    //            entity.SetTableName(entity.DisplayName());
    //        }
    //    }
    //}
}