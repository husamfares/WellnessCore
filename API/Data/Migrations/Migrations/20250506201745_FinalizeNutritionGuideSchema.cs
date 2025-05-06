using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class FinalizeNutritionGuideSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NutritionGuides",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AgeRangeStart = table.Column<int>(type: "integer", nullable: false),
                    AgeRangeEnd = table.Column<int>(type: "integer", nullable: false),
                    Gender = table.Column<string>(type: "text", nullable: false),
                    Goal = table.Column<string>(type: "text", nullable: false),
                    Calories = table.Column<int>(type: "integer", nullable: false),
                    ProteinGrams = table.Column<int>(type: "integer", nullable: false),
                    CarbsGrams = table.Column<int>(type: "integer", nullable: false),
                    FatGrams = table.Column<int>(type: "integer", nullable: false),
                    MealPlan = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NutritionGuides", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NutritionGuides");
        }
    }
}
