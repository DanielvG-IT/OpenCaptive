using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpenCaptive.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RestrictConfigurationDeletes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_networks_site_integrations_SiteIntegrationId",
                table: "networks");

            migrationBuilder.DropForeignKey(
                name: "FK_networks_sites_SiteId",
                table: "networks");

            migrationBuilder.DropForeignKey(
                name: "FK_portals_networks_NetworkId",
                table: "portals");

            migrationBuilder.DropForeignKey(
                name: "FK_site_integrations_sites_SiteId",
                table: "site_integrations");

            migrationBuilder.DropForeignKey(
                name: "FK_sites_organizations_OrganizationId",
                table: "sites");

            migrationBuilder.AddForeignKey(
                name: "FK_networks_site_integrations_SiteIntegrationId",
                table: "networks",
                column: "SiteIntegrationId",
                principalTable: "site_integrations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_networks_sites_SiteId",
                table: "networks",
                column: "SiteId",
                principalTable: "sites",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_portals_networks_NetworkId",
                table: "portals",
                column: "NetworkId",
                principalTable: "networks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_site_integrations_sites_SiteId",
                table: "site_integrations",
                column: "SiteId",
                principalTable: "sites",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_sites_organizations_OrganizationId",
                table: "sites",
                column: "OrganizationId",
                principalTable: "organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_networks_site_integrations_SiteIntegrationId",
                table: "networks");

            migrationBuilder.DropForeignKey(
                name: "FK_networks_sites_SiteId",
                table: "networks");

            migrationBuilder.DropForeignKey(
                name: "FK_portals_networks_NetworkId",
                table: "portals");

            migrationBuilder.DropForeignKey(
                name: "FK_site_integrations_sites_SiteId",
                table: "site_integrations");

            migrationBuilder.DropForeignKey(
                name: "FK_sites_organizations_OrganizationId",
                table: "sites");

            migrationBuilder.AddForeignKey(
                name: "FK_networks_site_integrations_SiteIntegrationId",
                table: "networks",
                column: "SiteIntegrationId",
                principalTable: "site_integrations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_networks_sites_SiteId",
                table: "networks",
                column: "SiteId",
                principalTable: "sites",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_portals_networks_NetworkId",
                table: "portals",
                column: "NetworkId",
                principalTable: "networks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_site_integrations_sites_SiteId",
                table: "site_integrations",
                column: "SiteId",
                principalTable: "sites",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sites_organizations_OrganizationId",
                table: "sites",
                column: "OrganizationId",
                principalTable: "organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
