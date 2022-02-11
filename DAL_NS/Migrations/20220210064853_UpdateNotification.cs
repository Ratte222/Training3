using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL_NS.Migrations
{
    public partial class UpdateNotification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "TelegramChatId",
                table: "Notifications",
                type: "bigint",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TelegramChatId",
                table: "Notifications");
        }
    }
}
