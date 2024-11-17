namespace PetShop.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddOwnerIdToPet : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Pets", name: "Owner_Id", newName: "OwnerId");
            RenameIndex(table: "dbo.Pets", name: "IX_Owner_Id", newName: "IX_OwnerId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Pets", name: "IX_OwnerId", newName: "IX_Owner_Id");
            RenameColumn(table: "dbo.Pets", name: "OwnerId", newName: "Owner_Id");
        }
    }
}
