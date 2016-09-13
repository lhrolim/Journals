using System;
using System.Threading.Tasks;
using journals.commons.Model.Dao.Internal;
using journals.commons.Model.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JournalsForm.Commons.Test.Model {

    [TestClass]
    public class SubscriptionMappingTest  : BaseTest{

        private readonly HibernateDao _dao = new HibernateDao();

        [TestInitialize]
        public async void Init() {

            var user = new User {
                Login = "subscriptionuser",
                Password = "testpass",
                Email = "user@a.com",
                Publisher = true
            };
            AdminUser = await _dao.Save(user);

        }


        [TestMethod]
        public async Task LookupSubscriptionsOfUser() {
            var subscriptions = await _dao.FindByQuery<Subscription>(Subscription.ByUser, null, AdminUser.Id);
            Assert.AreEqual(0, subscriptions.Count);

            var subscription = new Subscription() {
                Journal = BaseJournal,
                User = AdminUser
            };

            //create a new subscription
            var savedSubscription = await _dao.Save(subscription);

            Assert.IsNotNull(savedSubscription.SubscriptionDate);

            subscriptions = await _dao.FindByQuery<Subscription>(Subscription.ByUser, null, AdminUser.Id);
            Assert.AreEqual(1, subscriptions.Count);

        }


        [TestMethod]
        public async Task Subscribe() {
            
            var subscription = new Subscription() {
                Journal = Journals[0],
                User = Users[5]
            };

            await _dao.Save(subscription);

            var subscription2 = new Subscription() {
                Journal = Journals[0],
                User = Users[5]
            };

            await _dao.Save(subscription2);
            //create a new subscription


            var subscriptions = await _dao.FindByQuery<Subscription>(Subscription.ByUser, null, Users[5].Id);
            Assert.AreEqual(2, subscriptions.Count);


        }



    }
}
