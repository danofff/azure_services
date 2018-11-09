namespace MyFaceIdApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial_migration : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.UserModels", "Photo", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.UserModels", "Photo", c => c.Binary());
        }
    }
}
