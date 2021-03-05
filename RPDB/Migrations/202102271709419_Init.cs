namespace RPDB.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AppSettings",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Databases",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Alias = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RegisteredScripts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Path = c.String(),
                        Name = c.String(),
                        Text = c.String(),
                        FileTime = c.DateTime(),
                        FileSize = c.Int(),
                        Executed = c.Boolean(nullable: false),
                        Ignore = c.Boolean(nullable: false),
                        ExecutedTime = c.DateTime(),
                        LastExecutionError = c.String(),
                        DatabaseId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Databases", t => t.DatabaseId)
                .Index(t => t.DatabaseId);
            
            CreateTable(
                "dbo.SearchFolders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Path = c.String(),
                        IncludeSubfolders = c.Boolean(nullable: false),
                        DatabaseId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Databases", t => t.DatabaseId)
                .Index(t => t.DatabaseId);
            
            CreateTable(
                "dbo.ServerSettings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        IsWindowsAuth = c.Boolean(nullable: false),
                        UserName = c.String(),
                        Password = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SearchFolders", "DatabaseId", "dbo.Databases");
            DropForeignKey("dbo.RegisteredScripts", "DatabaseId", "dbo.Databases");
            DropIndex("dbo.SearchFolders", new[] { "DatabaseId" });
            DropIndex("dbo.RegisteredScripts", new[] { "DatabaseId" });
            DropTable("dbo.ServerSettings");
            DropTable("dbo.SearchFolders");
            DropTable("dbo.RegisteredScripts");
            DropTable("dbo.Databases");
            DropTable("dbo.AppSettings");
        }
    }
}
