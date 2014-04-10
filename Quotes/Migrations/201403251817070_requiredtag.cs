namespace Quotes.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class requiredtag : DbMigration
    {
        public override void Up()
        {
            Sql("UPDATE [dbo].[Quotations] SET Quote = 'No Quote' WHERE Quote IS NULL");
            Sql("UPDATE [dbo].[Quotations] SET Author = 'No Author' WHERE Author IS NULL");
            AlterColumn("dbo.Quotations", "Quote", c => c.String(nullable: false));
            AlterColumn("dbo.Quotations", "Author", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Quotations", "Author", c => c.String());
            AlterColumn("dbo.Quotations", "Quote", c => c.String());
        }
    }
}
