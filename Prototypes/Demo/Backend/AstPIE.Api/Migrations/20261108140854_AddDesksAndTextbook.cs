using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AstPIE.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddDesksAndTextbook : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "explore_entry_id",
                table: "topics",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "explore_line",
                table: "topics",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "law_equation",
                table: "topics",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "law_statement",
                table: "topics",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "predict_answer",
                table: "topics",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<List<string>>(
                name: "predict_options",
                table: "topics",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "predict_question",
                table: "topics",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "question",
                table: "topics",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "read_aloud",
                table: "topics",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<List<string>>(
                name: "recap_points",
                table: "topics",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "lab_intro",
                table: "simulations",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<List<string>>(
                name: "limits",
                table: "simulations",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "model_text",
                table: "simulations",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "step_code",
                table: "simulations",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "photo_credit",
                table: "people",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "url",
                table: "media_items",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "author",
                table: "media_items",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "chapter_id",
                table: "media_items",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "chart_key",
                table: "media_items",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "data",
                table: "media_items",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "date_label",
                table: "media_items",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "length_label",
                table: "media_items",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "person_id",
                table: "media_items",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "poster_url",
                table: "media_items",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "style",
                table: "media_items",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "text",
                table: "media_items",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "country",
                table: "entries",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "cover_caption",
                table: "entries",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "cover_credit",
                table: "entries",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "desk",
                table: "entries",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "radio_max_m_hz",
                table: "entries",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "radio_min_m_hz",
                table: "entries",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "status_note",
                table: "entries",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "tagline",
                table: "entries",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "margin_note",
                table: "chapters",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "place",
                table: "chapters",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "section",
                table: "chapters",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "shows_radio_hint",
                table: "chapters",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "tab_label",
                table: "chapters",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "declination",
                table: "catalogue_objects",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "image_caption",
                table: "catalogue_objects",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "image_credit",
                table: "catalogue_objects",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "image_is_dark",
                table: "catalogue_objects",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "right_ascension",
                table: "catalogue_objects",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "type_label",
                table: "catalogue_objects",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "catalogue_facts",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    catalogue_object_id = table.Column<int>(type: "integer", nullable: false),
                    label = table.Column<string>(type: "text", nullable: false),
                    value = table.Column<string>(type: "text", nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_catalogue_facts", x => x.id);
                    table.ForeignKey(
                        name: "fk_catalogue_facts_catalogue_objects_catalogue_object_id",
                        column: x => x.catalogue_object_id,
                        principalTable: "catalogue_objects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "check_questions",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    topic_id = table.Column<int>(type: "integer", nullable: false),
                    after_section = table.Column<int>(type: "integer", nullable: false),
                    question = table.Column<string>(type: "text", nullable: false),
                    answer = table.Column<string>(type: "text", nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_check_questions", x => x.id);
                    table.ForeignKey(
                        name: "fk_check_questions_topics_topic_id",
                        column: x => x.topic_id,
                        principalTable: "topics",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "entry_facts",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    entry_id = table.Column<int>(type: "integer", nullable: false),
                    label = table.Column<string>(type: "text", nullable: false),
                    value = table.Column<string>(type: "text", nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_entry_facts", x => x.id);
                    table.ForeignKey(
                        name: "fk_entry_facts_entries_entry_id",
                        column: x => x.entry_id,
                        principalTable: "entries",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "explanations",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    entry_id = table.Column<int>(type: "integer", nullable: false),
                    chapter_id = table.Column<int>(type: "integer", nullable: true),
                    title = table.Column<string>(type: "text", nullable: false),
                    proposed_by = table.Column<string>(type: "text", nullable: true),
                    for_text = table.Column<string>(type: "text", nullable: false),
                    against_text = table.Column<string>(type: "text", nullable: false),
                    verdict = table.Column<string>(type: "text", nullable: false),
                    stamp = table.Column<string>(type: "text", nullable: false),
                    image_url = table.Column<string>(type: "text", nullable: true),
                    image_credit = table.Column<string>(type: "text", nullable: true),
                    image_fit = table.Column<string>(type: "text", nullable: true),
                    sort_order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_explanations", x => x.id);
                    table.ForeignKey(
                        name: "fk_explanations_chapters_chapter_id",
                        column: x => x.chapter_id,
                        principalTable: "chapters",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_explanations_entries_entry_id",
                        column: x => x.entry_id,
                        principalTable: "entries",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "instruments",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    photo_url = table.Column<string>(type: "text", nullable: false),
                    photo_credit = table.Column<string>(type: "text", nullable: true),
                    photo_is_dark = table.Column<bool>(type: "boolean", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_instruments", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "key_words",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    slug = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    other_forms = table.Column<List<string>>(type: "text[]", nullable: true),
                    definition = table.Column<string>(type: "text", nullable: false),
                    symbol = table.Column<string>(type: "text", nullable: true),
                    unit = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_key_words", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "lab_tasks",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    simulation_id = table.Column<int>(type: "integer", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    task = table.Column<string>(type: "text", nullable: false),
                    hint = table.Column<string>(type: "text", nullable: false),
                    answer = table.Column<string>(type: "text", nullable: false),
                    answers_predict = table.Column<bool>(type: "boolean", nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_lab_tasks", x => x.id);
                    table.ForeignKey(
                        name: "fk_lab_tasks_simulations_simulation_id",
                        column: x => x.simulation_id,
                        principalTable: "simulations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "observation_log_entries",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    catalogue_object_id = table.Column<int>(type: "integer", nullable: false),
                    date_label = table.Column<string>(type: "text", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    summary = table.Column<string>(type: "text", nullable: false),
                    place = table.Column<string>(type: "text", nullable: true),
                    body = table.Column<List<string>>(type: "text[]", nullable: true),
                    facts = table.Column<List<string>>(type: "text[]", nullable: true),
                    note = table.Column<string>(type: "text", nullable: true),
                    plate_kind = table.Column<string>(type: "text", nullable: false),
                    plate_url = table.Column<string>(type: "text", nullable: true),
                    plate_chart_key = table.Column<string>(type: "text", nullable: true),
                    plate_poster_url = table.Column<string>(type: "text", nullable: true),
                    plate_title = table.Column<string>(type: "text", nullable: true),
                    plate_caption = table.Column<string>(type: "text", nullable: true),
                    plate_credit = table.Column<string>(type: "text", nullable: true),
                    plate_is_dark = table.Column<bool>(type: "boolean", nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_observation_log_entries", x => x.id);
                    table.ForeignKey(
                        name: "fk_observation_log_entries_catalogue_objects_catalogue_object_",
                        column: x => x.catalogue_object_id,
                        principalTable: "catalogue_objects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "topic_symbols",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    topic_id = table.Column<int>(type: "integer", nullable: false),
                    symbol = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    meaning = table.Column<string>(type: "text", nullable: false),
                    unit = table.Column<string>(type: "text", nullable: true),
                    equation_part = table.Column<string>(type: "text", nullable: true),
                    sort_order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_topic_symbols", x => x.id);
                    table.ForeignKey(
                        name: "fk_topic_symbols_topics_topic_id",
                        column: x => x.topic_id,
                        principalTable: "topics",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "catalogue_object_instruments",
                columns: table => new
                {
                    catalogue_object_id = table.Column<int>(type: "integer", nullable: false),
                    instrument_id = table.Column<int>(type: "integer", nullable: false),
                    role = table.Column<string>(type: "text", nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_catalogue_object_instruments", x => new { x.catalogue_object_id, x.instrument_id });
                    table.ForeignKey(
                        name: "fk_catalogue_object_instruments_catalogue_objects_catalogue_ob",
                        column: x => x.catalogue_object_id,
                        principalTable: "catalogue_objects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_catalogue_object_instruments_instruments_instrument_id",
                        column: x => x.instrument_id,
                        principalTable: "instruments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "topic_key_words",
                columns: table => new
                {
                    topic_id = table.Column<int>(type: "integer", nullable: false),
                    key_word_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_topic_key_words", x => new { x.topic_id, x.key_word_id });
                    table.ForeignKey(
                        name: "fk_topic_key_words_key_words_key_word_id",
                        column: x => x.key_word_id,
                        principalTable: "key_words",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_topic_key_words_topics_topic_id",
                        column: x => x.topic_id,
                        principalTable: "topics",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_topics_explore_entry_id",
                table: "topics",
                column: "explore_entry_id");

            migrationBuilder.CreateIndex(
                name: "ix_media_items_chapter_id",
                table: "media_items",
                column: "chapter_id");

            migrationBuilder.CreateIndex(
                name: "ix_media_items_person_id",
                table: "media_items",
                column: "person_id");

            migrationBuilder.CreateIndex(
                name: "ix_catalogue_facts_catalogue_object_id",
                table: "catalogue_facts",
                column: "catalogue_object_id");

            migrationBuilder.CreateIndex(
                name: "ix_catalogue_object_instruments_instrument_id",
                table: "catalogue_object_instruments",
                column: "instrument_id");

            migrationBuilder.CreateIndex(
                name: "ix_check_questions_topic_id",
                table: "check_questions",
                column: "topic_id");

            migrationBuilder.CreateIndex(
                name: "ix_entry_facts_entry_id",
                table: "entry_facts",
                column: "entry_id");

            migrationBuilder.CreateIndex(
                name: "ix_explanations_chapter_id",
                table: "explanations",
                column: "chapter_id");

            migrationBuilder.CreateIndex(
                name: "ix_explanations_entry_id",
                table: "explanations",
                column: "entry_id");

            migrationBuilder.CreateIndex(
                name: "ix_key_words_slug",
                table: "key_words",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_lab_tasks_simulation_id",
                table: "lab_tasks",
                column: "simulation_id");

            migrationBuilder.CreateIndex(
                name: "ix_observation_log_entries_catalogue_object_id",
                table: "observation_log_entries",
                column: "catalogue_object_id");

            migrationBuilder.CreateIndex(
                name: "ix_topic_key_words_key_word_id",
                table: "topic_key_words",
                column: "key_word_id");

            migrationBuilder.CreateIndex(
                name: "ix_topic_symbols_topic_id",
                table: "topic_symbols",
                column: "topic_id");

            migrationBuilder.AddForeignKey(
                name: "fk_media_items_chapters_chapter_id",
                table: "media_items",
                column: "chapter_id",
                principalTable: "chapters",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_media_items_people_person_id",
                table: "media_items",
                column: "person_id",
                principalTable: "people",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_topics_entries_explore_entry_id",
                table: "topics",
                column: "explore_entry_id",
                principalTable: "entries",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_media_items_chapters_chapter_id",
                table: "media_items");

            migrationBuilder.DropForeignKey(
                name: "fk_media_items_people_person_id",
                table: "media_items");

            migrationBuilder.DropForeignKey(
                name: "fk_topics_entries_explore_entry_id",
                table: "topics");

            migrationBuilder.DropTable(
                name: "catalogue_facts");

            migrationBuilder.DropTable(
                name: "catalogue_object_instruments");

            migrationBuilder.DropTable(
                name: "check_questions");

            migrationBuilder.DropTable(
                name: "entry_facts");

            migrationBuilder.DropTable(
                name: "explanations");

            migrationBuilder.DropTable(
                name: "lab_tasks");

            migrationBuilder.DropTable(
                name: "observation_log_entries");

            migrationBuilder.DropTable(
                name: "topic_key_words");

            migrationBuilder.DropTable(
                name: "topic_symbols");

            migrationBuilder.DropTable(
                name: "instruments");

            migrationBuilder.DropTable(
                name: "key_words");

            migrationBuilder.DropIndex(
                name: "ix_topics_explore_entry_id",
                table: "topics");

            migrationBuilder.DropIndex(
                name: "ix_media_items_chapter_id",
                table: "media_items");

            migrationBuilder.DropIndex(
                name: "ix_media_items_person_id",
                table: "media_items");

            migrationBuilder.DropColumn(
                name: "explore_entry_id",
                table: "topics");

            migrationBuilder.DropColumn(
                name: "explore_line",
                table: "topics");

            migrationBuilder.DropColumn(
                name: "law_equation",
                table: "topics");

            migrationBuilder.DropColumn(
                name: "law_statement",
                table: "topics");

            migrationBuilder.DropColumn(
                name: "predict_answer",
                table: "topics");

            migrationBuilder.DropColumn(
                name: "predict_options",
                table: "topics");

            migrationBuilder.DropColumn(
                name: "predict_question",
                table: "topics");

            migrationBuilder.DropColumn(
                name: "question",
                table: "topics");

            migrationBuilder.DropColumn(
                name: "read_aloud",
                table: "topics");

            migrationBuilder.DropColumn(
                name: "recap_points",
                table: "topics");

            migrationBuilder.DropColumn(
                name: "lab_intro",
                table: "simulations");

            migrationBuilder.DropColumn(
                name: "limits",
                table: "simulations");

            migrationBuilder.DropColumn(
                name: "model_text",
                table: "simulations");

            migrationBuilder.DropColumn(
                name: "step_code",
                table: "simulations");

            migrationBuilder.DropColumn(
                name: "photo_credit",
                table: "people");

            migrationBuilder.DropColumn(
                name: "author",
                table: "media_items");

            migrationBuilder.DropColumn(
                name: "chapter_id",
                table: "media_items");

            migrationBuilder.DropColumn(
                name: "chart_key",
                table: "media_items");

            migrationBuilder.DropColumn(
                name: "data",
                table: "media_items");

            migrationBuilder.DropColumn(
                name: "date_label",
                table: "media_items");

            migrationBuilder.DropColumn(
                name: "length_label",
                table: "media_items");

            migrationBuilder.DropColumn(
                name: "person_id",
                table: "media_items");

            migrationBuilder.DropColumn(
                name: "poster_url",
                table: "media_items");

            migrationBuilder.DropColumn(
                name: "style",
                table: "media_items");

            migrationBuilder.DropColumn(
                name: "text",
                table: "media_items");

            migrationBuilder.DropColumn(
                name: "country",
                table: "entries");

            migrationBuilder.DropColumn(
                name: "cover_caption",
                table: "entries");

            migrationBuilder.DropColumn(
                name: "cover_credit",
                table: "entries");

            migrationBuilder.DropColumn(
                name: "desk",
                table: "entries");

            migrationBuilder.DropColumn(
                name: "radio_max_m_hz",
                table: "entries");

            migrationBuilder.DropColumn(
                name: "radio_min_m_hz",
                table: "entries");

            migrationBuilder.DropColumn(
                name: "status_note",
                table: "entries");

            migrationBuilder.DropColumn(
                name: "tagline",
                table: "entries");

            migrationBuilder.DropColumn(
                name: "margin_note",
                table: "chapters");

            migrationBuilder.DropColumn(
                name: "place",
                table: "chapters");

            migrationBuilder.DropColumn(
                name: "section",
                table: "chapters");

            migrationBuilder.DropColumn(
                name: "shows_radio_hint",
                table: "chapters");

            migrationBuilder.DropColumn(
                name: "tab_label",
                table: "chapters");

            migrationBuilder.DropColumn(
                name: "declination",
                table: "catalogue_objects");

            migrationBuilder.DropColumn(
                name: "image_caption",
                table: "catalogue_objects");

            migrationBuilder.DropColumn(
                name: "image_credit",
                table: "catalogue_objects");

            migrationBuilder.DropColumn(
                name: "image_is_dark",
                table: "catalogue_objects");

            migrationBuilder.DropColumn(
                name: "right_ascension",
                table: "catalogue_objects");

            migrationBuilder.DropColumn(
                name: "type_label",
                table: "catalogue_objects");

            migrationBuilder.AlterColumn<string>(
                name: "url",
                table: "media_items",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
