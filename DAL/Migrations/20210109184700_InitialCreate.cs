using Microsoft.EntityFrameworkCore.Migrations;

namespace ClassLibrary1.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Game_Name = table.Column<string>(nullable: true),
                    Width = table.Column<int>(nullable: false),
                    Heigth = table.Column<int>(nullable: false),
                    IsPlayerMove = table.Column<bool>(nullable: false),
                    CarrierCount = table.Column<int>(nullable: false),
                    BattleshipCount = table.Column<int>(nullable: false),
                    SubmarineCount = table.Column<int>(nullable: false),
                    CruiserCount = table.Column<int>(nullable: false),
                    PatrolCount = table.Column<int>(nullable: false),
                    CanGoToAnother = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ships",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(nullable: true),
                    Length = table.Column<int>(nullable: false),
                    GameId = table.Column<int>(nullable: false),
                    ShipCounter = table.Column<int>(nullable: false),
                    IsPlayer = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ships_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShipGameAssignments",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Game_Id = table.Column<int>(nullable: false),
                    ShipName = table.Column<string>(nullable: true),
                    Length = table.Column<int>(nullable: false),
                    Type = table.Column<string>(nullable: true),
                    IsPlayer = table.Column<bool>(nullable: false),
                    IsRotated = table.Column<bool>(nullable: false),
                    GameId = table.Column<int>(nullable: true),
                    ShipId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipGameAssignments", x => x.id);
                    table.ForeignKey(
                        name: "FK_ShipGameAssignments_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShipGameAssignments_Ships_ShipId",
                        column: x => x.ShipId,
                        principalTable: "Ships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Cells",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CellId = table.Column<string>(nullable: true),
                    X = table.Column<int>(nullable: true),
                    Y = table.Column<int>(nullable: true),
                    ShipName = table.Column<string>(nullable: true),
                    GameId = table.Column<int>(nullable: false),
                    IsHit = table.Column<bool>(nullable: false),
                    IsPlayer = table.Column<bool>(nullable: false),
                    ShipGameAssignmentid = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cells", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cells_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Cells_ShipGameAssignments_ShipGameAssignmentid",
                        column: x => x.ShipGameAssignmentid,
                        principalTable: "ShipGameAssignments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cells_GameId",
                table: "Cells",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_Cells_ShipGameAssignmentid",
                table: "Cells",
                column: "ShipGameAssignmentid");

            migrationBuilder.CreateIndex(
                name: "IX_ShipGameAssignments_GameId",
                table: "ShipGameAssignments",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipGameAssignments_ShipId",
                table: "ShipGameAssignments",
                column: "ShipId");

            migrationBuilder.CreateIndex(
                name: "IX_Ships_GameId",
                table: "Ships",
                column: "GameId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cells");

            migrationBuilder.DropTable(
                name: "ShipGameAssignments");

            migrationBuilder.DropTable(
                name: "Ships");

            migrationBuilder.DropTable(
                name: "Games");
        }
    }
}
