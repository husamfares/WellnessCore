using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class AddTrainerSubscriptionManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrainerSubscription_AspNetUsers_TrainerId",
                table: "TrainerSubscription");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TrainerSubscription",
                table: "TrainerSubscription");

            migrationBuilder.RenameTable(
                name: "TrainerSubscription",
                newName: "TrainerSubscriptions");

            migrationBuilder.RenameIndex(
                name: "IX_TrainerSubscription_TrainerId",
                table: "TrainerSubscriptions",
                newName: "IX_TrainerSubscriptions_TrainerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TrainerSubscriptions",
                table: "TrainerSubscriptions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TrainerSubscriptions_AspNetUsers_TrainerId",
                table: "TrainerSubscriptions",
                column: "TrainerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrainerSubscriptions_AspNetUsers_TrainerId",
                table: "TrainerSubscriptions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TrainerSubscriptions",
                table: "TrainerSubscriptions");

            migrationBuilder.RenameTable(
                name: "TrainerSubscriptions",
                newName: "TrainerSubscription");

            migrationBuilder.RenameIndex(
                name: "IX_TrainerSubscriptions_TrainerId",
                table: "TrainerSubscription",
                newName: "IX_TrainerSubscription_TrainerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TrainerSubscription",
                table: "TrainerSubscription",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TrainerSubscription_AspNetUsers_TrainerId",
                table: "TrainerSubscription",
                column: "TrainerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
