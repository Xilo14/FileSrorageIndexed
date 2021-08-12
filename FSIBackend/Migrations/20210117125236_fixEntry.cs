using Microsoft.EntityFrameworkCore.Migrations;

namespace FSIBackend.Migrations
{
    public partial class fixEntry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LemmaId",
                table: "LemmaEntries",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_LemmaEntries_LemmaId",
                table: "LemmaEntries",
                column: "LemmaId");

            migrationBuilder.AddForeignKey(
                name: "FK_LemmaEntries_Lemmas_LemmaId",
                table: "LemmaEntries",
                column: "LemmaId",
                principalTable: "Lemmas",
                principalColumn: "LemmaId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LemmaEntries_Lemmas_LemmaId",
                table: "LemmaEntries");

            migrationBuilder.DropIndex(
                name: "IX_LemmaEntries_LemmaId",
                table: "LemmaEntries");

            migrationBuilder.DropColumn(
                name: "LemmaId",
                table: "LemmaEntries");
        }
    }
}
