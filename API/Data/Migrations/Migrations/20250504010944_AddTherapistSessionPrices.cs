using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class AddTherapistSessionPrices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TherapistSessionPrice_AspNetUsers_TherapistId",
                table: "TherapistSessionPrice");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TherapistSessionPrice",
                table: "TherapistSessionPrice");

            migrationBuilder.RenameTable(
                name: "TherapistSessionPrice",
                newName: "TherapistSessionPrices");

            migrationBuilder.RenameIndex(
                name: "IX_TherapistSessionPrice_TherapistId",
                table: "TherapistSessionPrices",
                newName: "IX_TherapistSessionPrices_TherapistId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TherapistSessionPrices",
                table: "TherapistSessionPrices",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TherapistSessionPrices_AspNetUsers_TherapistId",
                table: "TherapistSessionPrices",
                column: "TherapistId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TherapistSessionPrices_AspNetUsers_TherapistId",
                table: "TherapistSessionPrices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TherapistSessionPrices",
                table: "TherapistSessionPrices");

            migrationBuilder.RenameTable(
                name: "TherapistSessionPrices",
                newName: "TherapistSessionPrice");

            migrationBuilder.RenameIndex(
                name: "IX_TherapistSessionPrices_TherapistId",
                table: "TherapistSessionPrice",
                newName: "IX_TherapistSessionPrice_TherapistId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TherapistSessionPrice",
                table: "TherapistSessionPrice",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TherapistSessionPrice_AspNetUsers_TherapistId",
                table: "TherapistSessionPrice",
                column: "TherapistId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
