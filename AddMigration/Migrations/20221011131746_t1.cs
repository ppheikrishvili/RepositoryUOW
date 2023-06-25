using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AddMigration.Migrations
{
    public partial class t1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "__stub");

            migrationBuilder.CreateTable(
                name: "__stub_query_data",
                schema: "__stub",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    long1 = table.Column<long>(type: "bigint", nullable: true),
                    long2 = table.Column<long>(type: "bigint", nullable: true),
                    long3 = table.Column<long>(type: "bigint", nullable: true),
                    double1 = table.Column<double>(type: "float", nullable: true),
                    double2 = table.Column<double>(type: "float", nullable: true),
                    double3 = table.Column<double>(type: "float", nullable: true),
                    string1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    string2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    string3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    date1 = table.Column<DateTime>(type: "datetime2", nullable: true),
                    date2 = table.Column<DateTime>(type: "datetime2", nullable: true),
                    date3 = table.Column<DateTime>(type: "datetime2", nullable: true),
                    guid1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    guid2 = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    guid3 = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK___stub_query_data", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    UserSecurityID = table.Column<Guid>(name: "User Security ID", type: "uniqueidentifier", nullable: false),
                    UserID = table.Column<string>(name: "User ID", type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    UserFullName = table.Column<string>(name: "User Full Name", type: "nvarchar(80)", maxLength: 80, nullable: true),
                    State = table.Column<int>(type: "int", nullable: false),
                    ExpiryDate = table.Column<DateTime>(name: "Expiry Date", type: "datetime2", nullable: false),
                    WindowsSecurityID = table.Column<string>(name: "Windows Security ID", type: "nvarchar(119)", maxLength: 119, nullable: true),
                    ChangePassword = table.Column<byte>(name: "Change Password", type: "tinyint", nullable: false),
                    LicenseType = table.Column<int>(name: "License Type", type: "int", nullable: false),
                    ContactEmail = table.Column<string>(name: "Contact Email", type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.UserSecurityID);
                    table.CheckConstraint("CK_User_User_ID", "(NOT [User ID] like '%[^A-Z0-9]%')");
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "__stub_query_data",
                schema: "__stub");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
