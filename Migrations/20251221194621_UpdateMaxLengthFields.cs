using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillSwap.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMaxLengthFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "created_at", "password" },
                values: new object[] { new DateOnly(2025, 12, 21), "AQAAAAIAAYagAAAAEB36laoPBB1Efr4V834kvtXgAayp3R/GTucJFFaCLHmKhBO4TaAgynpQWt05Bam2cA==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "id",
                keyValue: 2,
                columns: new[] { "created_at", "password" },
                values: new object[] { new DateOnly(2025, 12, 21), "AQAAAAIAAYagAAAAEGRruyCtZs2JVTNQkiw5yIupbA3lr2YT6Ck0PGD1YQGO2t8hbScsSZLxUX2NvLGUYw==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "id",
                keyValue: 3,
                columns: new[] { "created_at", "password" },
                values: new object[] { new DateOnly(2025, 12, 21), "AQAAAAIAAYagAAAAEBrSvBSI3iXskhCZ/+a3swILo7dxMKOkOuLek/w1rB2MPq6tInKEWTMZLIWaTx/fcw==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "id",
                keyValue: 4,
                column: "password",
                value: "AQAAAAIAAYagAAAAEMVfy3JXyKCCOIAAkEqrp9oCGqZMB9gWg+BzLxCEfj+K5ksiS/NP9LLqjj7WNJUDLA==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "id",
                keyValue: 5,
                column: "password",
                value: "AQAAAAIAAYagAAAAEMAQ22bTlI3OCBIEqJYa0avmye6/bhi3UlMF3l6T28AFJW+JMo3ObNM9dcKARkwQZQ==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "id",
                keyValue: 6,
                column: "password",
                value: "AQAAAAIAAYagAAAAEFFx5nVGEpbShW0l8Yhx2AViayr//ijs7/+4dHrB38O76DAzSBeZca6tKlUm0kfdrA==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "id",
                keyValue: 7,
                column: "password",
                value: "AQAAAAIAAYagAAAAEFn85Pp2ssDtFJaM2W/PTk0FXKDjqQJd71MbKtSbTIxczuYshnjcviyWLZT61F6XCg==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "id",
                keyValue: 8,
                column: "password",
                value: "AQAAAAIAAYagAAAAENlc6OGihyIufEHmE62YBvOXkJOEvR+5dbDm5huvvqyb7PyUf/8Ic4/ClTR3Y/k+KQ==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "id",
                keyValue: 9,
                column: "password",
                value: "AQAAAAIAAYagAAAAEIRtQOs/NJ6M2b6PEkjkmTTaGhuN7paWo1/1cYNLqc9tSTgCE+H7MxvpbNJyg9BOWQ==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "id",
                keyValue: 10,
                column: "password",
                value: "AQAAAAIAAYagAAAAEGOvEOkFh7Tq7D2t/d2vGo5Q2GXAR3AAeDGLF0mPE/chCYNTrlxbluiDjjbtkN+9xA==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "created_at", "password" },
                values: new object[] { new DateOnly(2025, 12, 20), "AQAAAAIAAYagAAAAEIH7WwEUVF6sQsfmlByGS3NY4lZFavrVrD/tU3t9cVRPqvhoCe+DcCVZ9m6GCQfbyw==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "id",
                keyValue: 2,
                columns: new[] { "created_at", "password" },
                values: new object[] { new DateOnly(2025, 12, 20), "AQAAAAIAAYagAAAAEJYk5AEze9PgFyyKPyiV0ZRAVAxIHwIHceO3g1SjtpVn1L9j/pMhk5wMN+iQ0ili9A==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "id",
                keyValue: 3,
                columns: new[] { "created_at", "password" },
                values: new object[] { new DateOnly(2025, 12, 20), "AQAAAAIAAYagAAAAED6h0xTT/85E4V18XlKihUyCzZSX8TM+XMZ2UFM4/P7l2BLQrbr1YZs1dmgNd7AcGA==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "id",
                keyValue: 4,
                column: "password",
                value: "AQAAAAIAAYagAAAAEOUi4QlJsZQB4OBTkR54KZdv6tjbdbn4j6zcAvGcAZmIMnaYP5mA3cPasGybNuNaQw==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "id",
                keyValue: 5,
                column: "password",
                value: "AQAAAAIAAYagAAAAEMJ24EPxnC89E2/dvVzIWUVOxyfoWIPCxqzngYKNLbaQlf4jzPDCpS6YqRG5SArQ+w==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "id",
                keyValue: 6,
                column: "password",
                value: "AQAAAAIAAYagAAAAEFCtXe6A1b+/SncT8EmtxIX+dsnlXRizj7xQVQt/7KOENrtO6f6g62kWaPVx7SIgfg==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "id",
                keyValue: 7,
                column: "password",
                value: "AQAAAAIAAYagAAAAELmnA/3KL6FKMFQTxAttAo2wArsmWhcBtVfxLhWUV41GnHlxl1UpYT+/LGYXJvxRXg==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "id",
                keyValue: 8,
                column: "password",
                value: "AQAAAAIAAYagAAAAEKK+GdShjnVkleYU0Ddg4O1/5Vx4dyrL5GUwKVPokracL1fYDl1M8DYoAWMPUc1SnQ==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "id",
                keyValue: 9,
                column: "password",
                value: "AQAAAAIAAYagAAAAEL4A3iOTqHtnB1thXrqRMfCcxza894KPhrbHS0fuP6aOZzjDMOcPE6snBr4N/WCccg==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "id",
                keyValue: 10,
                column: "password",
                value: "AQAAAAIAAYagAAAAEA/859sR5c9z3uaztmh+pOy7f+iGTzCUVaj/jyzffnrcWZr6b0k5a740vfS6bT/5bg==");
        }
    }
}
