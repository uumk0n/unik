using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "lab3");

            migrationBuilder.CreateTable(
                name: "Instituts",
                schema: "lab3",
                columns: table => new
                {
                    RegEduDoc = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Instituts", x => x.RegEduDoc);
                });

            migrationBuilder.CreateTable(
                name: "Works",
                schema: "lab3",
                columns: table => new
                {
                    RegOrgNum = table.Column<string>(type: "text", nullable: false),
                    NameOrg = table.Column<string>(type: "text", nullable: false),
                    ItnOrg = table.Column<string>(type: "text", nullable: false),
                    OrgAddress = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Works", x => x.RegOrgNum);
                });

            migrationBuilder.CreateTable(
                name: "EduDocs",
                schema: "lab3",
                columns: table => new
                {
                    Inila = table.Column<string>(type: "text", nullable: false),
                    SndEduDoc = table.Column<string>(type: "text", nullable: false),
                    DateEnd = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RegEduDoc = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EduDocs", x => x.Inila);
                    table.ForeignKey(
                        name: "FK_EduDocs_Instituts_RegEduDoc",
                        column: x => x.RegEduDoc,
                        principalSchema: "lab3",
                        principalTable: "Instituts",
                        principalColumn: "RegEduDoc",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HfInfos",
                schema: "lab3",
                columns: table => new
                {
                    Inila = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    IdInfo = table.Column<int>(type: "integer", nullable: true),
                    DateOrd = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RegOrgNum = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HfInfos", x => x.Inila);
                    table.ForeignKey(
                        name: "FK_HfInfos_Works_RegOrgNum",
                        column: x => x.RegOrgNum,
                        principalSchema: "lab3",
                        principalTable: "Works",
                        principalColumn: "RegOrgNum",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonalDatas",
                schema: "lab3",
                columns: table => new
                {
                    Fcs = table.Column<string>(type: "text", nullable: false),
                    Itn = table.Column<string>(type: "text", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    SnPassport = table.Column<string>(type: "text", nullable: false),
                    Married = table.Column<bool>(type: "boolean", nullable: true),
                    Kids = table.Column<int>(type: "integer", nullable: true),
                    Inila = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonalDatas", x => x.Fcs);
                    table.ForeignKey(
                        name: "FK_PersonalDatas_HfInfos_Inila",
                        column: x => x.Inila,
                        principalSchema: "lab3",
                        principalTable: "HfInfos",
                        principalColumn: "Inila",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EduDocs_RegEduDoc",
                schema: "lab3",
                table: "EduDocs",
                column: "RegEduDoc");

            migrationBuilder.CreateIndex(
                name: "IX_HfInfos_RegOrgNum",
                schema: "lab3",
                table: "HfInfos",
                column: "RegOrgNum");

            migrationBuilder.CreateIndex(
                name: "IX_PersonalDatas_Inila",
                schema: "lab3",
                table: "PersonalDatas",
                column: "Inila");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EduDocs",
                schema: "lab3");

            migrationBuilder.DropTable(
                name: "PersonalDatas",
                schema: "lab3");

            migrationBuilder.DropTable(
                name: "Instituts",
                schema: "lab3");

            migrationBuilder.DropTable(
                name: "HfInfos",
                schema: "lab3");

            migrationBuilder.DropTable(
                name: "Works",
                schema: "lab3");
        }
    }
}
