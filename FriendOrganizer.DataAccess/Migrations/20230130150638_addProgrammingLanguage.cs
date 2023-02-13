using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FriendOrganizer.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addProgrammingLanguage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FavoriteLanguageId",
                table: "Friends",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Languages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Friends",
                keyColumn: "Id",
                keyValue: 1,
                column: "FavoriteLanguageId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Friends",
                keyColumn: "Id",
                keyValue: 2,
                column: "FavoriteLanguageId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Friends",
                keyColumn: "Id",
                keyValue: 3,
                column: "FavoriteLanguageId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Friends",
                keyColumn: "Id",
                keyValue: 4,
                column: "FavoriteLanguageId",
                value: null);

            migrationBuilder.InsertData(
                table: "Languages",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "C#" },
                    { 2, "TypeScript" },
                    { 3, "F#" },
                    { 4, "Swift" },
                    { 5, "Java" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Friends_FavoriteLanguageId",
                table: "Friends",
                column: "FavoriteLanguageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Friends_Languages_FavoriteLanguageId",
                table: "Friends",
                column: "FavoriteLanguageId",
                principalTable: "Languages",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Friends_Languages_FavoriteLanguageId",
                table: "Friends");

            migrationBuilder.DropTable(
                name: "Languages");

            migrationBuilder.DropIndex(
                name: "IX_Friends_FavoriteLanguageId",
                table: "Friends");

            migrationBuilder.DropColumn(
                name: "FavoriteLanguageId",
                table: "Friends");
        }
    }
}
