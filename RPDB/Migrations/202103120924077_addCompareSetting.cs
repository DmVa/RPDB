namespace RPDB.Migrations
{
	using RPDB.Data;
	using System;
    using System.Data.Entity.Migrations;
    
    public partial class addCompareSetting : DbMigration
    {
        public override void Up()
        {
			string command = "WinMergeU.exe /u /dl registered \"%1\" /dr \"%2\" \"%2\"";
			Sql($@"IF NOT EXISTS (SELECT ID FROM AppSettings WHERE ID = {(int)AppSettingEnum.CompareCommand} )
BEGIN
INSERT INTO AppSettings(Id, Value)
VALUES( {(int)AppSettingEnum.CompareCommand}, '{command}')
END
");

		}
        
        public override void Down()
        {
        }
    }
}
