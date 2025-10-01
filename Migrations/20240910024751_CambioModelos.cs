using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillSwap.Migrations
{
    /// <inheritdoc />
    public partial class CambioModelos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users_Secondary_abilities");

            migrationBuilder.DropTable(
                name: "Secondary_abilities");

            migrationBuilder.DropTable(
                name: "Primary_abilities");

            migrationBuilder.CreateTable(
                name: "Abilities",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    category = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    abilities = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Abilities", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("Relational:Collation", "utf8_general_ci");

            migrationBuilder.CreateTable(
                name: "Users_abilities",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_user = table.Column<int>(type: "int(11)", nullable: false),
                    id_ability = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users_abilities", x => x.id);
                    table.ForeignKey(
                        name: "FK_Users_abilities_Abilities_id_ability",
                        column: x => x.id_ability,
                        principalTable: "Abilities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Users_abilities_Users_id_user",
                        column: x => x.id_user,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("Relational:Collation", "utf8_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_Users_abilities_id_ability",
                table: "Users_abilities",
                column: "id_ability");

            migrationBuilder.CreateIndex(
                name: "IX_Users_abilities_id_user",
                table: "Users_abilities",
                column: "id_user");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users_abilities");

            migrationBuilder.DropTable(
                name: "Abilities");

            migrationBuilder.CreateTable(
                name: "Primary_abilities",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("Relational:Collation", "utf8_general_ci");

            migrationBuilder.CreateTable(
                name: "Secondary_abilities",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_primary_abilitie = table.Column<int>(type: "int(11)", nullable: true),
                    name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "Secondary_abilities_ibfk_1",
                        column: x => x.id_primary_abilitie,
                        principalTable: "Primary_abilities",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("Relational:Collation", "utf8_general_ci");

            migrationBuilder.CreateTable(
                name: "Users_Secondary_abilities",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_secondary_abilitie = table.Column<int>(type: "int(11)", nullable: true),
                    id_user = table.Column<int>(type: "int(11)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "Users_Secondary_abilities_ibfk_1",
                        column: x => x.id_user,
                        principalTable: "Users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "Users_Secondary_abilities_ibfk_2",
                        column: x => x.id_secondary_abilitie,
                        principalTable: "Secondary_abilities",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("Relational:Collation", "utf8_general_ci");

            migrationBuilder.CreateIndex(
                name: "id_primary_abilitie",
                table: "Secondary_abilities",
                column: "id_primary_abilitie");

            migrationBuilder.CreateIndex(
                name: "id_secondary_abilitie",
                table: "Users_Secondary_abilities",
                column: "id_secondary_abilitie");

            migrationBuilder.CreateIndex(
                name: "id_user1",
                table: "Users_Secondary_abilities",
                column: "id_user");
        }
    }
}
