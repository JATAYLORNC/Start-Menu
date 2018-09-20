namespace Start.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IconPath : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Programs", "IconPath", c => c.String());
            AddColumn("dbo.Programs", "Count", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Programs", "Count");
            DropColumn("dbo.Programs", "IconPath");
        }
    }
}
