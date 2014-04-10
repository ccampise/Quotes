namespace Quotes.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class createcategory : DbMigration
    {
        public override void Up()
        {
            Sql("UPDATE [dbo].[Categories] SET Name = 'No Name' WHERE Name IS NULL");
            AlterColumn("dbo.Categories", "Name", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Categories", "Name", c => c.String());
        }
    }
}
