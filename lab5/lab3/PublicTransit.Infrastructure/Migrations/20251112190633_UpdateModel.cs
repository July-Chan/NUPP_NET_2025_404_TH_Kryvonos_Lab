using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PublicTransit.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "InsectPredators",
                keyColumn: "Id",
                keyValue: new Guid("aaaabbbb-cccc-dddd-eeee-111122223333"),
                column: "FirstEncounterDate",
                value: new DateTime(2025, 10, 13, 19, 6, 32, 557, DateTimeKind.Utc).AddTicks(4584));

            migrationBuilder.UpdateData(
                table: "InsectPredators",
                keyColumn: "Id",
                keyValue: new Guid("bbbbcccc-dddd-eeee-ffff-222233334444"),
                column: "FirstEncounterDate",
                value: new DateTime(2025, 9, 28, 19, 6, 32, 557, DateTimeKind.Utc).AddTicks(5044));

            migrationBuilder.UpdateData(
                table: "InsectPredators",
                keyColumn: "Id",
                keyValue: new Guid("ccccdddd-eeee-ffff-aaaa-333344445555"),
                column: "FirstEncounterDate",
                value: new DateTime(2025, 9, 13, 19, 6, 32, 557, DateTimeKind.Utc).AddTicks(5069));

            migrationBuilder.UpdateData(
                table: "InsectPredators",
                keyColumn: "Id",
                keyValue: new Guid("ddddeeee-ffff-aaaa-bbbb-444455556666"),
                column: "FirstEncounterDate",
                value: new DateTime(2025, 10, 23, 19, 6, 32, 557, DateTimeKind.Utc).AddTicks(5079));

            migrationBuilder.UpdateData(
                table: "Insects",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2025, 11, 12, 19, 6, 32, 556, DateTimeKind.Utc).AddTicks(7474));

            migrationBuilder.UpdateData(
                table: "Insects",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                column: "CreatedAt",
                value: new DateTime(2025, 11, 12, 19, 6, 32, 556, DateTimeKind.Utc).AddTicks(9010));

            migrationBuilder.UpdateData(
                table: "Insects",
                keyColumn: "Id",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                column: "CreatedAt",
                value: new DateTime(2025, 11, 12, 19, 6, 32, 556, DateTimeKind.Utc).AddTicks(9470));

            migrationBuilder.UpdateData(
                table: "Insects",
                keyColumn: "Id",
                keyValue: new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                column: "CreatedAt",
                value: new DateTime(2025, 11, 12, 19, 6, 32, 557, DateTimeKind.Utc).AddTicks(37));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "InsectPredators",
                keyColumn: "Id",
                keyValue: new Guid("aaaabbbb-cccc-dddd-eeee-111122223333"),
                column: "FirstEncounterDate",
                value: new DateTime(2025, 9, 29, 18, 29, 4, 931, DateTimeKind.Utc).AddTicks(2755));

            migrationBuilder.UpdateData(
                table: "InsectPredators",
                keyColumn: "Id",
                keyValue: new Guid("bbbbcccc-dddd-eeee-ffff-222233334444"),
                column: "FirstEncounterDate",
                value: new DateTime(2025, 9, 14, 18, 29, 4, 931, DateTimeKind.Utc).AddTicks(3205));

            migrationBuilder.UpdateData(
                table: "InsectPredators",
                keyColumn: "Id",
                keyValue: new Guid("ccccdddd-eeee-ffff-aaaa-333344445555"),
                column: "FirstEncounterDate",
                value: new DateTime(2025, 8, 30, 18, 29, 4, 931, DateTimeKind.Utc).AddTicks(3228));

            migrationBuilder.UpdateData(
                table: "InsectPredators",
                keyColumn: "Id",
                keyValue: new Guid("ddddeeee-ffff-aaaa-bbbb-444455556666"),
                column: "FirstEncounterDate",
                value: new DateTime(2025, 10, 9, 18, 29, 4, 931, DateTimeKind.Utc).AddTicks(3242));

            migrationBuilder.UpdateData(
                table: "Insects",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2025, 10, 29, 18, 29, 4, 930, DateTimeKind.Utc).AddTicks(4519));

            migrationBuilder.UpdateData(
                table: "Insects",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                column: "CreatedAt",
                value: new DateTime(2025, 10, 29, 18, 29, 4, 930, DateTimeKind.Utc).AddTicks(6292));

            migrationBuilder.UpdateData(
                table: "Insects",
                keyColumn: "Id",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                column: "CreatedAt",
                value: new DateTime(2025, 10, 29, 18, 29, 4, 930, DateTimeKind.Utc).AddTicks(7180));

            migrationBuilder.UpdateData(
                table: "Insects",
                keyColumn: "Id",
                keyValue: new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                column: "CreatedAt",
                value: new DateTime(2025, 10, 29, 18, 29, 4, 930, DateTimeKind.Utc).AddTicks(7758));
        }
    }
}
