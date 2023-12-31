﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RepositoryUOW;

#nullable disable

namespace AddMigration.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("EntityFrameworkCore.MemoryJoin.QueryModelClass", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime?>("Date1")
                        .HasColumnType("datetime2")
                        .HasColumnName("date1");

                    b.Property<DateTime?>("Date2")
                        .HasColumnType("datetime2")
                        .HasColumnName("date2");

                    b.Property<DateTime?>("Date3")
                        .HasColumnType("datetime2")
                        .HasColumnName("date3");

                    b.Property<double?>("Double1")
                        .HasColumnType("float")
                        .HasColumnName("double1");

                    b.Property<double?>("Double2")
                        .HasColumnType("float")
                        .HasColumnName("double2");

                    b.Property<double?>("Double3")
                        .HasColumnType("float")
                        .HasColumnName("double3");

                    b.Property<Guid?>("Guid1")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("guid1");

                    b.Property<Guid?>("Guid2")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("guid2");

                    b.Property<Guid?>("Guid3")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("guid3");

                    b.Property<long?>("Long1")
                        .HasColumnType("bigint")
                        .HasColumnName("long1");

                    b.Property<long?>("Long2")
                        .HasColumnType("bigint")
                        .HasColumnName("long2");

                    b.Property<long?>("Long3")
                        .HasColumnType("bigint")
                        .HasColumnName("long3");

                    b.Property<string>("String1")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("string1");

                    b.Property<string>("String2")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("string2");

                    b.Property<string>("String3")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("string3");

                    b.HasKey("Id");

                    b.ToTable("__stub_query_data", "__stub");
                });

            modelBuilder.Entity("RepositoryUOWDomain.Entities.System.User", b =>
                {
                    b.Property<Guid>("User_Security_ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("User Security ID");

                    b.Property<byte>("Change_Password")
                        .HasColumnType("tinyint")
                        .HasColumnName("Change Password");

                    b.Property<string>("Contact_Email")
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)")
                        .HasColumnName("Contact Email");

                    b.Property<DateTime>("Expiry_Date")
                        .HasColumnType("datetime2")
                        .HasColumnName("Expiry Date");

                    b.Property<int>("License_Type")
                        .HasColumnType("int")
                        .HasColumnName("License Type");

                    b.Property<int>("State")
                        .HasColumnType("int");

                    b.Property<string>("User_FullName")
                        .HasMaxLength(80)
                        .HasColumnType("nvarchar(80)")
                        .HasColumnName("User Full Name");

                    b.Property<string>("User_ID")
                        .IsRequired()
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("varchar(20)")
                        .HasColumnName("User ID");

                    b.Property<string>("Windows_Security_ID")
                        .HasMaxLength(119)
                        .HasColumnType("nvarchar(119)")
                        .HasColumnName("Windows Security ID");

                    b.HasKey("User_Security_ID");

                    b.ToTable("User", (string)null);

                    b.HasCheckConstraint("CK_User_User_ID", "(NOT [User ID] like '%[^A-Z0-9]%')");
                });
#pragma warning restore 612, 618
        }
    }
}
