namespace RPDB.Migrations
{
    using RPDB.Data;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addAutoSyncFolderSetting : DbMigration
    {
        public override void Up()
        {
            Sql($@"IF NOT EXISTS (SELECT ID FROM AppSettings WHERE ID = {(int)AppSettingEnum.AutosyncFoldersFrom} )
BEGIN
INSERT INTO AppSettings(Id, Value)
VALUES( {(int)AppSettingEnum.AutosyncFoldersFrom}, '\\azwesv0025\Tools\folders.rpdb')
END
");
        }
        
        public override void Down()
        {
        }
    }
}
