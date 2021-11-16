namespace RPDB.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSearchMask : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SearchFolders", "SearchMask", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SearchFolders", "SearchMask");
        }
    }
}
