using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FishPokedex.Migrations
{
    /// <inheritdoc />
    public partial class AddLocationAndImageSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create Location table
            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                });

            // Add new columns to Catches table
            migrationBuilder.AddColumn<int>(
                name: "LocationId",
                table: "Catches",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Catches",
                type: "text",
                nullable: true);

            // Migrate existing locations from string to Location records
            migrationBuilder.Sql(@"
                INSERT INTO ""Locations"" (""Name"") 
                SELECT DISTINCT ""Location"" FROM ""Catches"" 
                WHERE ""Location"" IS NOT NULL;
            ");

            // Update Catches to reference the new Location records
            migrationBuilder.Sql(@"
                UPDATE ""Catches"" c 
                SET ""LocationId"" = l.""Id""
                FROM ""Locations"" l 
                WHERE c.""Location"" = l.""Name"";
            ");

            // Make LocationId NOT NULL after migration
            migrationBuilder.AlterColumn<int>(
                name: "LocationId",
                table: "Catches",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            // Drop the old Location column
            migrationBuilder.DropColumn(
                name: "Location",
                table: "Catches");

            // Add foreign key
            migrationBuilder.CreateIndex(
                name: "IX_Catches_LocationId",
                table: "Catches",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Catches_Locations_LocationId",
                table: "Catches",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove foreign key
            migrationBuilder.DropForeignKey(
                name: "FK_Catches_Locations_LocationId",
                table: "Catches");

            // Drop LocationId index
            migrationBuilder.DropIndex(
                name: "IX_Catches_LocationId",
                table: "Catches");

            // Drop Locations table
            migrationBuilder.DropTable(
                name: "Locations");

            // Recreate Location column
            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Catches",
                type: "text",
                nullable: false,
                defaultValue: "");

            // Restore location values from LocationId (optional - can be skipped)
            migrationBuilder.Sql(@"
                UPDATE ""Catches"" c 
                SET ""Location"" = l.""Name""
                FROM ""Locations"" l 
                WHERE c.""LocationId"" = l.""Id"";
            ");

            // Drop ImageUrl
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Catches");

            // Drop LocationId
            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "Catches");
        }
    }
}
