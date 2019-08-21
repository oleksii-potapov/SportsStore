namespace SportsStore.Domain.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddImageData : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.Products");
            CreateTable(
                "dbo.Products",
                c => new
                {
                    ProductId = c.Int(nullable: false, identity: true),
                    Name = c.String(nullable: false),
                    Description = c.String(nullable: false),
                    Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                    Category = c.String(nullable: false),
                    ImageData = c.Binary(),
                    ImageMimeType = c.String(maxLength: 50),
                })
                .PrimaryKey(t => t.ProductId);
        }

        public override void Down()
        {
            DropTable("dbo.Products");
        }
    }
}