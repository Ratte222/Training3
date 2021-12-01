using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL_NS.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    TypeNotification = table.Column<int>(nullable: false),
                    Recipient = table.Column<string>(nullable: true),
                    Sender = table.Column<string>(nullable: true),
                    Header = table.Column<string>(nullable: true),
                    MessageBody = table.Column<string>(nullable: true),
                    IsSend = table.Column<bool>(nullable: false),
                    DateTimeCreate = table.Column<DateTime>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DateTimeOfTheLastAttemptToSend = table.Column<DateTime>(nullable: true),
                    NumberOfAttemptToSent = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Credentials",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Login = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    SmtpHost = table.Column<string>(nullable: true),
                    SmtpPort = table.Column<int>(nullable: false),
                    NotificationId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Credentials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Credentials_Notifications_NotificationId",
                        column: x => x.NotificationId,
                        principalTable: "Notifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NotificationException",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Type = table.Column<string>(nullable: true),
                    HResult = table.Column<long>(nullable: true),
                    Message = table.Column<string>(nullable: true),
                    NotificationId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationException", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationException_Notifications_NotificationId",
                        column: x => x.NotificationId,
                        principalTable: "Notifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Credentials_NotificationId",
                table: "Credentials",
                column: "NotificationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NotificationException_NotificationId",
                table: "NotificationException",
                column: "NotificationId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Credentials");

            migrationBuilder.DropTable(
                name: "NotificationException");

            migrationBuilder.DropTable(
                name: "Notifications");
        }
    }
}
