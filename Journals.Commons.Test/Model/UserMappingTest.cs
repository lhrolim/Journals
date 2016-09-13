using System.Threading.Tasks;
using journals.commons.Model.Dao.Internal;
using journals.commons.Model.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JournalsForm.Commons.Test.Model {

    [TestClass]
    public class UserMappingTest :BaseTest {

        private readonly HibernateDao _dao = new HibernateDao();


        [TestMethod]
        public async Task TestUserCreation() {

            var user = new User {
                Login = "test",
                Password = "testpass",
                Email = "user@a.com",
                Publisher = true
            };

            var retrievedUser = await _dao.FindSingleByQuery<User>(User.ByUserName, "test");
            Assert.IsNull(retrievedUser);

            await _dao.Save(user);

            retrievedUser = await _dao.FindSingleByQuery<User>(User.ByUserName, "test");
            Assert.IsNotNull(retrievedUser);

        }



        [TestMethod]
        public async Task TestUserUpdate() {

            AdminUser.Email = "newemail@a.com";

            var newAdminUser =await _dao.Save(AdminUser);
            Assert.AreEqual("newemail@a.com", newAdminUser.Email);


        }

    }
}
