using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EnrollmentService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Enrollments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CourseId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    EnrolledAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "timezone('utc', now())"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "timezone('utc', now())"),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enrollments", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_CourseId_UserId",
                table: "Enrollments",
                columns: new[] { "CourseId", "UserId" },
                unique: true);

            migrationBuilder.Sql("""
                CREATE OR REPLACE FUNCTION set_created_and_updated_at()
                RETURNS TRIGGER AS $$
                BEGIN
                    NEW."EnrolledAt" := timezone('utc', now());
                    NEW."UpdatedAt"  := timezone('utc', now());
                    RETURN NEW;
                END;
                $$ LANGUAGE plpgsql;
            """);

            // Before Update entity auto update UpdatedAt
            migrationBuilder.Sql("""
                CREATE OR REPLACE FUNCTION set_updated_at()
                RETURNS TRIGGER AS $$
                BEGIN
                    NEW."UpdatedAt" := timezone('utc', now());
                    RETURN NEW;
                END;
                $$ LANGUAGE plpgsql;
            """);

            // Drop triggers if exists
            migrationBuilder.Sql("""
                DROP TRIGGER IF EXISTS trg_set_created_updated_at ON "Enrollments";
                CREATE TRIGGER trg_set_created_updated_at
                BEFORE INSERT ON "Enrollments"
                FOR EACH ROW
                EXECUTE FUNCTION set_created_and_updated_at();
            """);

            migrationBuilder.Sql("""
                DROP TRIGGER IF EXISTS trg_set_updated_at ON "Enrollments";
                CREATE TRIGGER trg_set_updated_at
                BEFORE UPDATE ON "Enrollments"
                FOR EACH ROW
                EXECUTE FUNCTION set_updated_at();
            """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Enrollments");

            migrationBuilder.Sql(@"DROP TRIGGER IF EXISTS trg_set_updated_at ON ""Enrollments"";");
            migrationBuilder.Sql(@"DROP TRIGGER IF EXISTS trg_set_created_updated_at ON ""Enrollments"";");
            migrationBuilder.Sql(@"DROP FUNCTION IF EXISTS set_updated_at();");
            migrationBuilder.Sql(@"DROP FUNCTION IF EXISTS set_created_and_updated_at();");
        }
    }
}
