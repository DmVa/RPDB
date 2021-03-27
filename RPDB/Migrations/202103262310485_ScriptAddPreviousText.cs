namespace RPDB.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ScriptAddPreviousText : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RegisteredScripts", "PreviousText", c => c.String());
            AddColumn("dbo.RegisteredScripts", "PreviousFileTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RegisteredScripts", "PreviousFileTime");
            DropColumn("dbo.RegisteredScripts", "PreviousText");
        }
    }
}
