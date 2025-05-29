using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class AddWeeklyDaysToNutritionGuide : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MealPlan",
                table: "NutritionGuides",
                newName: "Wednesday");

            migrationBuilder.AddColumn<string>(
                name: "Friday",
                table: "NutritionGuides",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Monday",
                table: "NutritionGuides",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Saturday",
                table: "NutritionGuides",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Sunday",
                table: "NutritionGuides",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Thursday",
                table: "NutritionGuides",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Tuesday",
                table: "NutritionGuides",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Friday",
                table: "NutritionGuides");

            migrationBuilder.DropColumn(
                name: "Monday",
                table: "NutritionGuides");

            migrationBuilder.DropColumn(
                name: "Saturday",
                table: "NutritionGuides");

            migrationBuilder.DropColumn(
                name: "Sunday",
                table: "NutritionGuides");

            migrationBuilder.DropColumn(
                name: "Thursday",
                table: "NutritionGuides");

            migrationBuilder.DropColumn(
                name: "Tuesday",
                table: "NutritionGuides");

            migrationBuilder.RenameColumn(
                name: "Wednesday",
                table: "NutritionGuides",
                newName: "MealPlan");
        }
    }
}
