using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AstPIE.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddMediaItemPeople : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "media_item_people",
                columns: table => new
                {
                    media_item_id = table.Column<int>(type: "integer", nullable: false),
                    person_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_media_item_people", x => new { x.media_item_id, x.person_id });
                    table.ForeignKey(
                        name: "fk_media_item_people_media_items_media_item_id",
                        column: x => x.media_item_id,
                        principalTable: "media_items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_media_item_people_people_person_id",
                        column: x => x.person_id,
                        principalTable: "people",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_media_item_people_person_id",
                table: "media_item_people",
                column: "person_id");

            // Photos that named their person in media_items.person_id (meant only for personnel files) move here
            migrationBuilder.Sql("insert into media_item_people (media_item_id, person_id) select id, person_id from media_items where person_id is not null and type <> 'PersonFile'");
            migrationBuilder.Sql("update media_items set person_id = null where type <> 'PersonFile'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Back to one person per photo (a photo of several keeps one of them)
            migrationBuilder.Sql("update media_items m set person_id = l.person_id from media_item_people l where l.media_item_id = m.id and m.type <> 'PersonFile'");

            migrationBuilder.DropTable(
                name: "media_item_people");
        }
    }
}
