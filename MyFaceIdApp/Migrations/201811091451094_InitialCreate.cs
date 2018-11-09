namespace MyFaceIdApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserModels",
                c => new
                    {
                        ChatId = c.String(nullable: false, maxLength: 128),
                        UserName = c.String(),
                        Gender = c.Boolean(),
                        Age = c.Int(),
                        Photo = c.Binary(),
                    })
                .PrimaryKey(t => t.ChatId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.UserModels");
        }
    }
}
