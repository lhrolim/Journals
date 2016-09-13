using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using journals.commons.Model.dao;
using journals.commons.Model.Dao.Internal;
using journals.commons.Model.Entities;
using journals.commons.Model.Manager;
using journals.commons.Util;
using JournalsForm.Commons.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Journals.Commons.Test.Model {

    [TestClass]
    public class PublicationMappingTest : BaseTest {


        private readonly IGenericDao _genericDao = new HibernateDao();

        private PublicationManager _pubManager;

        [TestInitialize]
        public void init() {
            _pubManager = new PublicationManager(_genericDao);
        }

        [TestMethod]
        public async Task AddPublicationToJournal() {

            var publication = new Publication() {
                Title = "my publication",
                Description = "my publication description",
                JournalId = BaseJournal.Id.Value
            };


            var savedData = await _pubManager.SavePublication(publication, "test");
            var content = await _genericDao.FindSingleByQuery<PublicationData>(PublicationData.ByPublication, savedData.Id);
            Assert.IsNotNull(content);


        }


        [TestMethod]
        public async Task ByJournalTest() {

            var publication = new Publication() {
                Title = "my publication",
                Description = "my publication description",
                JournalId = BaseJournal.Id.Value
            };


            var savedData = await _pubManager.SavePublication(publication, "test");

            var publications = await _genericDao.FindByQuery<Publication>(Publication.ByJournalId, null, BaseJournal.Id);
            Assert.IsTrue(publications.Any());
        }

        [TestMethod]
        public async Task Base64Test() {

            var publication = new Publication() {
                Title = "my publication",
                Description = "my publication description",
                JournalId = BaseJournal.Id.Value
            };

            var bytes = File.ReadAllBytes("a.pdf");
            var fileB64 = Convert.ToBase64String(bytes);


            var savedData = await _pubManager.SavePublication(publication, fileB64);
            var content = await _genericDao.FindSingleByQuery<PublicationData>(PublicationData.ByPublication, savedData.Id);

            var base64String = Convert.ToBase64String(content.UnCompressed());
            Assert.AreEqual(fileB64, base64String);
        }



    }
}
