using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PublicTransit.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FoodSources",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    NutritionalValue = table.Column<int>(type: "integer", nullable: false),
                    IsAvailableYearRound = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodSources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Predators",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Species = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    HuntingSuccess = table.Column<int>(type: "integer", nullable: false),
                    Size = table.Column<double>(type: "numeric(7,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Predators", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Insects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Legs = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    HabitatId = table.Column<Guid>(type: "uuid", nullable: true),
                    FoodSourceId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Insects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Insects_FoodSources_FoodSourceId",
                        column: x => x.FoodSourceId,
                        principalTable: "FoodSources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Flies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WingSpan = table.Column<double>(type: "numeric(5,2)", nullable: false),
                    FlightSpeed = table.Column<int>(type: "integer", nullable: false),
                    CanHover = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Flies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Flies_Insects_Id",
                        column: x => x.Id,
                        principalTable: "Insects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Habitats",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Location = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Climate = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Temperature = table.Column<double>(type: "numeric(5,2)", nullable: false),
                    Humidity = table.Column<int>(type: "integer", nullable: false),
                    InsectId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Habitats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Habitats_Insects_InsectId",
                        column: x => x.InsectId,
                        principalTable: "Insects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "InsectPredators",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InsectId = table.Column<Guid>(type: "uuid", nullable: false),
                    PredatorId = table.Column<Guid>(type: "uuid", nullable: false),
                    RiskLevel = table.Column<int>(type: "integer", nullable: false),
                    FirstEncounterDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DefenseMechanism = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsectPredators", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InsectPredators_Insects_InsectId",
                        column: x => x.InsectId,
                        principalTable: "Insects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InsectPredators_Predators_PredatorId",
                        column: x => x.PredatorId,
                        principalTable: "Predators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Spiders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IsPoisonous = table.Column<bool>(type: "boolean", nullable: false),
                    WebStrength = table.Column<double>(type: "numeric(5,2)", nullable: false),
                    VenomPotency = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spiders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Spiders_Insects_Id",
                        column: x => x.Id,
                        principalTable: "Insects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "FoodSources",
                columns: new[] { "Id", "IsAvailableYearRound", "Name", "NutritionalValue", "Type" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), false, "Нектар квітів", 85, "Нектар" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), true, "Органічні відходи", 45, "Органіка" },
                    { new Guid("33333333-3333-3333-3333-333333333333"), true, "Кров тварин", 95, "Кров" }
                });

            migrationBuilder.InsertData(
                table: "Insects",
                columns: new[] { "Id", "CreatedAt", "FoodSourceId", "HabitatId", "Legs", "Name" },
                values: new object[,]
                {
                    { new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), new DateTime(2025, 10, 29, 18, 29, 4, 930, DateTimeKind.Utc).AddTicks(7180), null, null, 8, "Павук-хрестовик" },
                    { new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), new DateTime(2025, 10, 29, 18, 29, 4, 930, DateTimeKind.Utc).AddTicks(7758), null, null, 8, "Чорна вдова" }
                });

            migrationBuilder.InsertData(
                table: "Predators",
                columns: new[] { "Id", "HuntingSuccess", "Name", "Size", "Species" },
                values: new object[,]
                {
                    { new Guid("11111111-aaaa-aaaa-aaaa-111111111111"), 65, "Горобець", 14.5, "Птах" },
                    { new Guid("22222222-bbbb-bbbb-bbbb-222222222222"), 85, "Богомол", 8.0, "Комаха" },
                    { new Guid("33333333-cccc-cccc-cccc-333333333333"), 70, "Ящірка звичайна", 18.0, "Рептилія" }
                });

            migrationBuilder.InsertData(
                table: "Habitats",
                columns: new[] { "Id", "Climate", "Humidity", "InsectId", "Location", "Temperature" },
                values: new object[] { new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff"), "Помірний", 70, new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), "Львів, ботанічний сад", 16.0 });

            migrationBuilder.InsertData(
                table: "InsectPredators",
                columns: new[] { "Id", "DefenseMechanism", "FirstEncounterDate", "InsectId", "PredatorId", "RiskLevel" },
                values: new object[,]
                {
                    { new Guid("ccccdddd-eeee-ffff-aaaa-333344445555"), "Павутина", new DateTime(2025, 8, 30, 18, 29, 4, 931, DateTimeKind.Utc).AddTicks(3228), new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), new Guid("11111111-aaaa-aaaa-aaaa-111111111111"), 8 },
                    { new Guid("ddddeeee-ffff-aaaa-bbbb-444455556666"), "Отрута", new DateTime(2025, 10, 9, 18, 29, 4, 931, DateTimeKind.Utc).AddTicks(3242), new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), new Guid("33333333-cccc-cccc-cccc-333333333333"), 5 }
                });

            migrationBuilder.InsertData(
                table: "Insects",
                columns: new[] { "Id", "CreatedAt", "FoodSourceId", "HabitatId", "Legs", "Name" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new DateTime(2025, 10, 29, 18, 29, 4, 930, DateTimeKind.Utc).AddTicks(4519), new Guid("22222222-2222-2222-2222-222222222222"), null, 6, "Муха звичайна" },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new DateTime(2025, 10, 29, 18, 29, 4, 930, DateTimeKind.Utc).AddTicks(6292), new Guid("11111111-1111-1111-1111-111111111111"), null, 6, "Дрозофіла" }
                });

            migrationBuilder.InsertData(
                table: "Spiders",
                columns: new[] { "Id", "IsPoisonous", "VenomPotency", "WebStrength" },
                values: new object[,]
                {
                    { new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), false, 2, 75.5 },
                    { new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), true, 9, 90.0 }
                });

            migrationBuilder.InsertData(
                table: "Flies",
                columns: new[] { "Id", "CanHover", "FlightSpeed", "WingSpan" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), true, 8, 3.5 },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), false, 5, 1.2 }
                });

            migrationBuilder.InsertData(
                table: "Habitats",
                columns: new[] { "Id", "Climate", "Humidity", "InsectId", "Location", "Temperature" },
                values: new object[] { new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), "Помірно-континентальний", 65, new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "Київ, Голосіївський парк", 18.5 });

            migrationBuilder.InsertData(
                table: "InsectPredators",
                columns: new[] { "Id", "DefenseMechanism", "FirstEncounterDate", "InsectId", "PredatorId", "RiskLevel" },
                values: new object[,]
                {
                    { new Guid("aaaabbbb-cccc-dddd-eeee-111122223333"), "Швидкий політ", new DateTime(2025, 9, 29, 18, 29, 4, 931, DateTimeKind.Utc).AddTicks(2755), new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("11111111-aaaa-aaaa-aaaa-111111111111"), 7 },
                    { new Guid("bbbbcccc-dddd-eeee-ffff-222233334444"), "Маневрування", new DateTime(2025, 9, 14, 18, 29, 4, 931, DateTimeKind.Utc).AddTicks(3205), new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("33333333-cccc-cccc-cccc-333333333333"), 6 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_FoodSources_Type",
                table: "FoodSources",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Habitats_InsectId",
                table: "Habitats",
                column: "InsectId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InsectPredators_InsectId_PredatorId",
                table: "InsectPredators",
                columns: new[] { "InsectId", "PredatorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InsectPredators_PredatorId",
                table: "InsectPredators",
                column: "PredatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Insects_CreatedAt",
                table: "Insects",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Insects_FoodSourceId",
                table: "Insects",
                column: "FoodSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_Insects_Name",
                table: "Insects",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Predators_Species",
                table: "Predators",
                column: "Species");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Flies");

            migrationBuilder.DropTable(
                name: "Habitats");

            migrationBuilder.DropTable(
                name: "InsectPredators");

            migrationBuilder.DropTable(
                name: "Spiders");

            migrationBuilder.DropTable(
                name: "Predators");

            migrationBuilder.DropTable(
                name: "Insects");

            migrationBuilder.DropTable(
                name: "FoodSources");
        }
    }
}
