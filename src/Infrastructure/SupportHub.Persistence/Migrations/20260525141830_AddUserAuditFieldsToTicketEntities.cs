using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUserAuditFieldsToTicketEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthorName",
                table: "TicketComments");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "Tickets",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AuthorUserId",
                table: "TicketComments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ActorUserId",
                table: "TicketActivities",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "AuthorUserId",
                table: "TicketComments");

            migrationBuilder.DropColumn(
                name: "ActorUserId",
                table: "TicketActivities");

            migrationBuilder.AddColumn<string>(
                name: "AuthorName",
                table: "TicketComments",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
