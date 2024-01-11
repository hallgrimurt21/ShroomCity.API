using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShroomCity.API.Migrations
{
    /// <inheritdoc />
    public partial class ChangeAttributeTypeEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Attributes_AttributeTypeId",
                table: "Attributes");

            migrationBuilder.CreateIndex(
                name: "IX_Attributes_AttributeTypeId",
                table: "Attributes",
                column: "AttributeTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Attributes_AttributeTypeId",
                table: "Attributes");

            migrationBuilder.CreateIndex(
                name: "IX_Attributes_AttributeTypeId",
                table: "Attributes",
                column: "AttributeTypeId",
                unique: true);
        }
    }
}
