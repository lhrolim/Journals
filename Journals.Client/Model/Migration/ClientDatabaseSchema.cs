using FluentMigrator;

namespace Journals.Client.Model.Migration {

    [Migration(201609111300)]
    public class ClientDatabaseSchema : FluentMigrator.Migration {
        public override void Up() {

            Create.Table("CONFIGURATION").
                   WithColumn("id").AsInt32().PrimaryKey().Identity().
                   WithColumn("url").AsString(4000).NotNullable().
                   WithColumn("token").AsString(2500).NotNullable().
                   WithColumn("syncdate").AsDateTime().Nullable();
        }

        public override void Down() {
        }
    }
}
