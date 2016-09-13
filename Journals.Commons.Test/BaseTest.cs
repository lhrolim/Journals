using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Threading.Tasks;
using journals.commons.Model.Dao.Internal;
using journals.commons.Model.Entities;
using journals.commons.Model.Migration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JournalsForm.Commons.Test {

    [TestClass]
    public class BaseTest {


        protected static User AdminUser;
        protected static Journal BaseJournal;

        protected static IList<User> Users = new List<User>();
        protected static IList<Journal> Journals = new List<Journal>();


        [AssemblyInitialize]
        public static async void AssemblyInit(TestContext context) {
            SQLiteConnection.CreateFile("journals.sqlite");

            var connection = new SQLiteConnection("Data Source=Journals.test.db;Version=3;");
            connection.Open();

            new MigratorExecutor().Migrate(runner => runner.MigrateUp());
            SessionManagerWrapper.GetInstance();

            var dao = new HibernateDao();
            await dao.ExecuteSql("delete from User");
            await dao.ExecuteSql("delete from Publication_Data");
            await dao.ExecuteSql("delete from Publication");
            await dao.ExecuteSql("delete from Journal");
            await dao.ExecuteSql("delete from Subscription");

            var user = new User {
                Login = "admin",
                Password = "testpass",
                Email = "user@a.com",
                Publisher = true
            };

            AdminUser = await dao.Save(user);


            var journal = new Journal {
                Name = "my journal",
                Description = "test description",
                RegisterDate = DateTime.Now
            };

            BaseJournal = await dao.Save(journal);

            for (int i = 0; i < 10; i++) {
                user = new User {
                    Login = "user" + i,
                    Password = "testpass" + i,
                    Email = "user@a.com",
                    Publisher = true
                };

                Users.Add(await dao.Save(user));


                journal = new Journal {
                    Name = "my journal " + i,
                    Description = "test description" + i,
                    RegisterDate = DateTime.Now
                };
                Journals.Add(await dao.Save(journal));
            }


        }
//
//        [TestMethod]
//        public void TestMethod1() {
//            //placeholder empty test to assure database initialization only once per assembly
//        }
    }
}
