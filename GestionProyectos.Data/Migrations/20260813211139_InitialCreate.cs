using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GestionProyectos.Data.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "TaskManagement");

            migrationBuilder.EnsureSchema(
                name: "Security");

            migrationBuilder.CreateTable(
                name: "BacklogStatus",
                schema: "TaskManagement",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    BadgeColor = table.Column<string>(type: "text", nullable: false),
                    IsClosed = table.Column<bool>(type: "boolean", nullable: false),
                    RowStatus = table.Column<short>(type: "smallint", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BacklogStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Enterprise",
                schema: "TaskManagement",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "text", nullable: false),
                    RowStatus = table.Column<short>(type: "smallint", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enterprise", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HourType",
                schema: "TaskManagement",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Scope = table.Column<string>(type: "text", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    BadgeColor = table.Column<string>(type: "text", nullable: false),
                    RowStatus = table.Column<short>(type: "smallint", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HourType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Menu",
                schema: "Security",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Icon = table.Column<int>(type: "integer", nullable: false),
                    Page = table.Column<string>(type: "text", nullable: false),
                    Parent = table.Column<long>(type: "bigint", nullable: true),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    RowStatus = table.Column<short>(type: "smallint", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Menu", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Priority",
                schema: "TaskManagement",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "text", nullable: false),
                    BadgeColor = table.Column<string>(type: "text", nullable: false),
                    RowStatus = table.Column<short>(type: "smallint", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Priority", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProjectStatus",
                schema: "TaskManagement",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    BadgeColor = table.Column<string>(type: "text", nullable: false),
                    IsClosed = table.Column<bool>(type: "boolean", nullable: false),
                    RowStatus = table.Column<short>(type: "smallint", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RequirementStatus",
                schema: "TaskManagement",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    BadgeColor = table.Column<string>(type: "text", nullable: false),
                    IsClosed = table.Column<bool>(type: "boolean", nullable: false),
                    RowStatus = table.Column<short>(type: "smallint", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequirementStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                schema: "Security",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "text", nullable: false),
                    RowStatus = table.Column<short>(type: "smallint", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaskBugStatus",
                schema: "TaskManagement",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    BadgeColor = table.Column<string>(type: "text", nullable: false),
                    RowStatus = table.Column<short>(type: "smallint", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskBugStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaskDevelopmentPhase",
                schema: "TaskManagement",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    BadgeColor = table.Column<string>(type: "text", nullable: false),
                    RowStatus = table.Column<short>(type: "smallint", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskDevelopmentPhase", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaskStatus",
                schema: "TaskManagement",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    BadgeColor = table.Column<string>(type: "text", nullable: false),
                    RowStatus = table.Column<short>(type: "smallint", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                schema: "Security",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    JobTitle = table.Column<string>(type: "text", nullable: false),
                    AvatarFileName = table.Column<string>(type: "text", nullable: true),
                    AvatarFilePath = table.Column<string>(type: "text", nullable: true),
                    RowStatus = table.Column<short>(type: "smallint", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Customer",
                schema: "TaskManagement",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "text", nullable: false),
                    EnterpriseId = table.Column<long>(type: "bigint", nullable: false),
                    RowStatus = table.Column<short>(type: "smallint", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Customer_Enterprise_EnterpriseId",
                        column: x => x.EnterpriseId,
                        principalSchema: "TaskManagement",
                        principalTable: "Enterprise",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoleMenu",
                schema: "Security",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<long>(type: "bigint", nullable: false),
                    MenuId = table.Column<long>(type: "bigint", nullable: false),
                    CanView = table.Column<bool>(type: "boolean", nullable: false),
                    CanCreate = table.Column<bool>(type: "boolean", nullable: false),
                    CanEdit = table.Column<bool>(type: "boolean", nullable: false),
                    CanDelete = table.Column<bool>(type: "boolean", nullable: false),
                    CanRegisterHours = table.Column<bool>(type: "boolean", nullable: false),
                    CanFinalize = table.Column<bool>(type: "boolean", nullable: false),
                    RowStatus = table.Column<short>(type: "smallint", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleMenu", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleMenu_Menu_MenuId",
                        column: x => x.MenuId,
                        principalSchema: "Security",
                        principalTable: "Menu",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoleMenu_Role_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "Security",
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRole",
                schema: "Security",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    RoleId = table.Column<long>(type: "bigint", nullable: false),
                    RowStatus = table.Column<short>(type: "smallint", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRole", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRole_Role_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "Security",
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRole_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "Security",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserTimeOff",
                schema: "Security",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Type = table.Column<short>(type: "smallint", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Hours = table.Column<decimal>(type: "numeric", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    RowStatus = table.Column<short>(type: "smallint", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTimeOff", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserTimeOff_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "Security",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BacklogItem",
                schema: "TaskManagement",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    BacklogStatusId = table.Column<long>(type: "bigint", nullable: false),
                    CustomerId = table.Column<long>(type: "bigint", nullable: true),
                    RowStatus = table.Column<short>(type: "smallint", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BacklogItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BacklogItem_BacklogStatus_BacklogStatusId",
                        column: x => x.BacklogStatusId,
                        principalSchema: "TaskManagement",
                        principalTable: "BacklogStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BacklogItem_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalSchema: "TaskManagement",
                        principalTable: "Customer",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Project",
                schema: "TaskManagement",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "text", nullable: false),
                    CustomerId = table.Column<long>(type: "bigint", nullable: false),
                    ProjectStatusId = table.Column<long>(type: "bigint", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ActualEndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RowStatus = table.Column<short>(type: "smallint", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Project", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Project_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalSchema: "TaskManagement",
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Project_ProjectStatus_ProjectStatusId",
                        column: x => x.ProjectStatusId,
                        principalSchema: "TaskManagement",
                        principalTable: "ProjectStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Requirement",
                schema: "TaskManagement",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "text", nullable: false),
                    ProjectId = table.Column<long>(type: "bigint", nullable: false),
                    Scope = table.Column<string>(type: "text", nullable: false),
                    RequirementStatusId = table.Column<long>(type: "bigint", nullable: false),
                    PriorityId = table.Column<long>(type: "bigint", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ActualEndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RequesterName = table.Column<string>(type: "text", nullable: false),
                    RequestDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ImpactedSystems = table.Column<string>(type: "text", nullable: false),
                    FreshDeskTicketNumber = table.Column<string>(type: "text", nullable: true),
                    IsWithinOriginalScope = table.Column<bool>(type: "boolean", nullable: false),
                    ScopeChangeReason = table.Column<short>(type: "smallint", nullable: true),
                    IsProductionReprocess = table.Column<bool>(type: "boolean", nullable: false),
                    RowStatus = table.Column<short>(type: "smallint", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Requirement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Requirement_Priority_PriorityId",
                        column: x => x.PriorityId,
                        principalSchema: "TaskManagement",
                        principalTable: "Priority",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Requirement_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "TaskManagement",
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Requirement_RequirementStatus_RequirementStatusId",
                        column: x => x.RequirementStatusId,
                        principalSchema: "TaskManagement",
                        principalTable: "RequirementStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Attachment",
                schema: "TaskManagement",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RequirementId = table.Column<long>(type: "bigint", nullable: false),
                    FileName = table.Column<string>(type: "text", nullable: false),
                    FilePath = table.Column<string>(type: "text", nullable: false),
                    RowStatus = table.Column<short>(type: "smallint", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attachment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Attachment_Requirement_RequirementId",
                        column: x => x.RequirementId,
                        principalSchema: "TaskManagement",
                        principalTable: "Requirement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Task",
                schema: "TaskManagement",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RequirementId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: true),
                    TimeEstimationHours = table.Column<decimal>(type: "numeric", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    PriorityId = table.Column<long>(type: "bigint", nullable: false),
                    TaskStatusId = table.Column<long>(type: "bigint", nullable: false),
                    DevelopmentPhaseId = table.Column<long>(type: "bigint", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ActualEndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    QaEnteredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsWithinOriginalScope = table.Column<bool>(type: "boolean", nullable: false),
                    ScopeChangeReason = table.Column<short>(type: "smallint", nullable: true),
                    RowStatus = table.Column<short>(type: "smallint", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Task", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Task_Priority_PriorityId",
                        column: x => x.PriorityId,
                        principalSchema: "TaskManagement",
                        principalTable: "Priority",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Task_Requirement_RequirementId",
                        column: x => x.RequirementId,
                        principalSchema: "TaskManagement",
                        principalTable: "Requirement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Task_TaskDevelopmentPhase_DevelopmentPhaseId",
                        column: x => x.DevelopmentPhaseId,
                        principalSchema: "TaskManagement",
                        principalTable: "TaskDevelopmentPhase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Task_TaskStatus_TaskStatusId",
                        column: x => x.TaskStatusId,
                        principalSchema: "TaskManagement",
                        principalTable: "TaskStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Task_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "Security",
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TaskBug",
                schema: "TaskManagement",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RequirementId = table.Column<long>(type: "bigint", nullable: false),
                    TaskId = table.Column<long>(type: "bigint", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: false),
                    TaskBugStatusId = table.Column<long>(type: "bigint", nullable: false),
                    ReportedByUserId = table.Column<long>(type: "bigint", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RowStatus = table.Column<short>(type: "smallint", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskBug", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskBug_Requirement_RequirementId",
                        column: x => x.RequirementId,
                        principalSchema: "TaskManagement",
                        principalTable: "Requirement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskBug_Task_TaskId",
                        column: x => x.TaskId,
                        principalSchema: "TaskManagement",
                        principalTable: "Task",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TaskBug_TaskBugStatus_TaskBugStatusId",
                        column: x => x.TaskBugStatusId,
                        principalSchema: "TaskManagement",
                        principalTable: "TaskBugStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskBug_User_ReportedByUserId",
                        column: x => x.ReportedByUserId,
                        principalSchema: "Security",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskNote",
                schema: "TaskManagement",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TaskId = table.Column<long>(type: "bigint", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    AuthorUserId = table.Column<long>(type: "bigint", nullable: false),
                    RowStatus = table.Column<short>(type: "smallint", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskNote", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskNote_Task_TaskId",
                        column: x => x.TaskId,
                        principalSchema: "TaskManagement",
                        principalTable: "Task",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskNote_User_AuthorUserId",
                        column: x => x.AuthorUserId,
                        principalSchema: "Security",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskStatusHistory",
                schema: "TaskManagement",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TaskId = table.Column<long>(type: "bigint", nullable: false),
                    TaskStatusId = table.Column<long>(type: "bigint", nullable: false),
                    PreviousTaskStatusId = table.Column<long>(type: "bigint", nullable: true),
                    Reason = table.Column<string>(type: "text", nullable: false),
                    ChangedByUserId = table.Column<long>(type: "bigint", nullable: false),
                    RowStatus = table.Column<short>(type: "smallint", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskStatusHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskStatusHistory_Task_TaskId",
                        column: x => x.TaskId,
                        principalSchema: "TaskManagement",
                        principalTable: "Task",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskStatusHistory_TaskStatus_PreviousTaskStatusId",
                        column: x => x.PreviousTaskStatusId,
                        principalSchema: "TaskManagement",
                        principalTable: "TaskStatus",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TaskStatusHistory_TaskStatus_TaskStatusId",
                        column: x => x.TaskStatusId,
                        principalSchema: "TaskManagement",
                        principalTable: "TaskStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskStatusHistory_User_ChangedByUserId",
                        column: x => x.ChangedByUserId,
                        principalSchema: "Security",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TimeLog",
                schema: "TaskManagement",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    ExecutionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UsedHours = table.Column<decimal>(type: "numeric", nullable: false),
                    ProgressPercent = table.Column<decimal>(type: "numeric", nullable: false),
                    TaskId = table.Column<long>(type: "bigint", nullable: false),
                    HourTypeId = table.Column<long>(type: "bigint", nullable: false),
                    RowStatus = table.Column<short>(type: "smallint", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimeLog_HourType_HourTypeId",
                        column: x => x.HourTypeId,
                        principalSchema: "TaskManagement",
                        principalTable: "HourType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TimeLog_Task_TaskId",
                        column: x => x.TaskId,
                        principalSchema: "TaskManagement",
                        principalTable: "Task",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TimeLog_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "Security",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskBugAttachment",
                schema: "TaskManagement",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TaskBugId = table.Column<long>(type: "bigint", nullable: false),
                    FileName = table.Column<string>(type: "text", nullable: false),
                    FilePath = table.Column<string>(type: "text", nullable: false),
                    RowStatus = table.Column<short>(type: "smallint", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskBugAttachment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskBugAttachment_TaskBug_TaskBugId",
                        column: x => x.TaskBugId,
                        principalSchema: "TaskManagement",
                        principalTable: "TaskBug",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Attachment_RequirementId",
                schema: "TaskManagement",
                table: "Attachment",
                column: "RequirementId");

            migrationBuilder.CreateIndex(
                name: "IX_BacklogItem_BacklogStatusId",
                schema: "TaskManagement",
                table: "BacklogItem",
                column: "BacklogStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_BacklogItem_CustomerId",
                schema: "TaskManagement",
                table: "BacklogItem",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_EnterpriseId",
                schema: "TaskManagement",
                table: "Customer",
                column: "EnterpriseId");

            migrationBuilder.CreateIndex(
                name: "IX_Project_CustomerId",
                schema: "TaskManagement",
                table: "Project",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Project_ProjectStatusId",
                schema: "TaskManagement",
                table: "Project",
                column: "ProjectStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Requirement_PriorityId",
                schema: "TaskManagement",
                table: "Requirement",
                column: "PriorityId");

            migrationBuilder.CreateIndex(
                name: "IX_Requirement_ProjectId",
                schema: "TaskManagement",
                table: "Requirement",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Requirement_RequirementStatusId",
                schema: "TaskManagement",
                table: "Requirement",
                column: "RequirementStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleMenu_MenuId",
                schema: "Security",
                table: "RoleMenu",
                column: "MenuId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleMenu_RoleId",
                schema: "Security",
                table: "RoleMenu",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Task_DevelopmentPhaseId",
                schema: "TaskManagement",
                table: "Task",
                column: "DevelopmentPhaseId");

            migrationBuilder.CreateIndex(
                name: "IX_Task_PriorityId",
                schema: "TaskManagement",
                table: "Task",
                column: "PriorityId");

            migrationBuilder.CreateIndex(
                name: "IX_Task_RequirementId",
                schema: "TaskManagement",
                table: "Task",
                column: "RequirementId");

            migrationBuilder.CreateIndex(
                name: "IX_Task_TaskStatusId",
                schema: "TaskManagement",
                table: "Task",
                column: "TaskStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Task_UserId",
                schema: "TaskManagement",
                table: "Task",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskBug_ReportedByUserId",
                schema: "TaskManagement",
                table: "TaskBug",
                column: "ReportedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskBug_RequirementId",
                schema: "TaskManagement",
                table: "TaskBug",
                column: "RequirementId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskBug_TaskBugStatusId",
                schema: "TaskManagement",
                table: "TaskBug",
                column: "TaskBugStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskBug_TaskId",
                schema: "TaskManagement",
                table: "TaskBug",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskBugAttachment_TaskBugId",
                schema: "TaskManagement",
                table: "TaskBugAttachment",
                column: "TaskBugId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskNote_AuthorUserId",
                schema: "TaskManagement",
                table: "TaskNote",
                column: "AuthorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskNote_TaskId",
                schema: "TaskManagement",
                table: "TaskNote",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskStatusHistory_ChangedByUserId",
                schema: "TaskManagement",
                table: "TaskStatusHistory",
                column: "ChangedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskStatusHistory_PreviousTaskStatusId",
                schema: "TaskManagement",
                table: "TaskStatusHistory",
                column: "PreviousTaskStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskStatusHistory_TaskId",
                schema: "TaskManagement",
                table: "TaskStatusHistory",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskStatusHistory_TaskStatusId",
                schema: "TaskManagement",
                table: "TaskStatusHistory",
                column: "TaskStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_TimeLog_HourTypeId",
                schema: "TaskManagement",
                table: "TimeLog",
                column: "HourTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TimeLog_TaskId",
                schema: "TaskManagement",
                table: "TimeLog",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_TimeLog_UserId",
                schema: "TaskManagement",
                table: "TimeLog",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_RoleId",
                schema: "Security",
                table: "UserRole",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_UserId",
                schema: "Security",
                table: "UserRole",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserTimeOff_UserId",
                schema: "Security",
                table: "UserTimeOff",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Attachment",
                schema: "TaskManagement");

            migrationBuilder.DropTable(
                name: "BacklogItem",
                schema: "TaskManagement");

            migrationBuilder.DropTable(
                name: "RoleMenu",
                schema: "Security");

            migrationBuilder.DropTable(
                name: "TaskBugAttachment",
                schema: "TaskManagement");

            migrationBuilder.DropTable(
                name: "TaskNote",
                schema: "TaskManagement");

            migrationBuilder.DropTable(
                name: "TaskStatusHistory",
                schema: "TaskManagement");

            migrationBuilder.DropTable(
                name: "TimeLog",
                schema: "TaskManagement");

            migrationBuilder.DropTable(
                name: "UserRole",
                schema: "Security");

            migrationBuilder.DropTable(
                name: "UserTimeOff",
                schema: "Security");

            migrationBuilder.DropTable(
                name: "BacklogStatus",
                schema: "TaskManagement");

            migrationBuilder.DropTable(
                name: "Menu",
                schema: "Security");

            migrationBuilder.DropTable(
                name: "TaskBug",
                schema: "TaskManagement");

            migrationBuilder.DropTable(
                name: "HourType",
                schema: "TaskManagement");

            migrationBuilder.DropTable(
                name: "Role",
                schema: "Security");

            migrationBuilder.DropTable(
                name: "Task",
                schema: "TaskManagement");

            migrationBuilder.DropTable(
                name: "TaskBugStatus",
                schema: "TaskManagement");

            migrationBuilder.DropTable(
                name: "Requirement",
                schema: "TaskManagement");

            migrationBuilder.DropTable(
                name: "TaskDevelopmentPhase",
                schema: "TaskManagement");

            migrationBuilder.DropTable(
                name: "TaskStatus",
                schema: "TaskManagement");

            migrationBuilder.DropTable(
                name: "User",
                schema: "Security");

            migrationBuilder.DropTable(
                name: "Priority",
                schema: "TaskManagement");

            migrationBuilder.DropTable(
                name: "Project",
                schema: "TaskManagement");

            migrationBuilder.DropTable(
                name: "RequirementStatus",
                schema: "TaskManagement");

            migrationBuilder.DropTable(
                name: "Customer",
                schema: "TaskManagement");

            migrationBuilder.DropTable(
                name: "ProjectStatus",
                schema: "TaskManagement");

            migrationBuilder.DropTable(
                name: "Enterprise",
                schema: "TaskManagement");
        }
    }
}
