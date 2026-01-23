using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Reminder_WPF.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Reminders",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ReminderText = table.Column<string>(type: "TEXT", nullable: false),
                    ReminderTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Recurrence = table.Column<int>(type: "INTEGER", nullable: false),
                    RecurrenceData = table.Column<string>(type: "TEXT", nullable: false),
                    LastUpdated = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reminders", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reminders");
        }
    }
}
