using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class AddMealAnalyzerSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MealAnalyses_AspNetUsers_UserId",
                table: "MealAnalyses");

            migrationBuilder.DropIndex(
                name: "IX_MealAnalyses_UserId",
                table: "MealAnalyses");

            migrationBuilder.AlterColumn<double>(
                name: "Protein",
                table: "MealAnalyses",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Protein",
                table: "MealAnalyses",
                type: "integer",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.CreateIndex(
                name: "IX_MealAnalyses_UserId",
                table: "MealAnalyses",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_MealAnalyses_AspNetUsers_UserId",
                table: "MealAnalyses",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
