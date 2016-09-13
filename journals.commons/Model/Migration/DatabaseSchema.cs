using FluentMigrator;

namespace journals.commons.Model.Migration {

    [Migration(201609101200)]
    public class DatabaseSchema : FluentMigrator.Migration {
        public override void Up() {

            Create.Table("USER").
                WithColumn("id").AsInt32().PrimaryKey().Identity().
                WithColumn("login").AsString(50).NotNullable().Unique().
                WithColumn("email").AsString(255).NotNullable().
                WithColumn("password").AsString(255).NotNullable().
                WithColumn("publisher").AsBoolean().WithDefaultValue(false);


            Create.Table("JOURNAL").
                WithColumn("id").AsInt32().PrimaryKey().Identity().
                WithColumn("name").AsString(255).NotNullable().
                WithColumn("description").AsString(4000).NotNullable().
                WithColumn("registerdate").AsDateTime().NotNullable();


            Create.Table("PUBLICATION").
                WithColumn("id").AsInt32().PrimaryKey().Identity().
                WithColumn("title").AsString(255).NotNullable().
                WithColumn("description").AsString(4000).NotNullable().
                WithColumn("registerdate").AsDateTime().NotNullable().
                WithColumn("volume").AsInt32().NotNullable().
                WithColumn("issue").AsInt32().NotNullable()
                .WithColumn("journal_id").AsInt32().NotNullable().ForeignKey("FK_PUB_JOURNAL", "JOURNAL", "id");

            Create.Table("PUBLICATION_DATA").
                WithColumn("id").AsInt32().PrimaryKey().Identity().
                WithColumn("binarycontent").AsBinary().NotNullable()
                .WithColumn("publication_id").AsInt32().NotNullable().ForeignKey("FK_PUB_DATA_PUB", "PUBLICATION", "id");



            Create.Table("SUBSCRIPTION").
                WithColumn("id").AsInt32().PrimaryKey().Identity().
                WithColumn("subscriptiondate").AsDateTime().NotNullable()
                .WithColumn("user_id").AsInt32().NotNullable().ForeignKey("FK_SUB_USER", "USER", "id")
                .WithColumn("journal_id").AsInt32().NotNullable().ForeignKey("FK_SUB_JOURNAL", "JOURNAL", "id");

            Execute.Sql(@"create or replace view subsjournals as
                         select j.id,name,description,s.subscriptiondate,s.user_id,(select count(id) from publication p where p.journal_id = j.id) as publication_count from journal j
                         left join subscription s on j.id = s.journal_id
                         order by user_id is not null desc");

        }

        public override void Down() {
        }
    }
}
