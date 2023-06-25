
using Microsoft.EntityFrameworkCore;
using RepositoryUOWDomain.Entities.System;
using RepositoryUOW.Data.Converter;
using RepositoryUOWDomain.Interface;

namespace RepositoryUOW.Data.Map;

public class UserMap : IBaseMapModel
{
    public UserMap(ModelBuilder MBuilder) 
    {
        var entityBuilder = MBuilder.Entity<User>();

        entityBuilder.ToTable("User");

        entityBuilder.HasKey(e => e.User_Security_ID);

        entityBuilder.Property(e => e.Change_Password).HasConversion<byte>();

        entityBuilder.Property(e => e.User_ID).IsUnicode(false).HasConversion(StringToUpperConverter.Instance!);

        //string? gPattern = typeof(User).GetProperty("User_ID")!
        //    .GetCustomAttribute<RegularExpressionAttribute>()?.Pattern;

        entityBuilder.ToTable( t=> t.HasCheckConstraint("CK_User_User_ID", "(NOT [User ID] like '%[^A-Z0-9]%')"));
    }
}
