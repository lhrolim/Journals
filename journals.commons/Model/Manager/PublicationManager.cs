using System;
using System.Threading.Tasks;
using journals.commons.Model.dao;
using journals.commons.Model.Dao.Internal;
using journals.commons.Model.Entities;
using journals.commons.SimpleInjector;
using journals.commons.Util;

namespace journals.commons.Model.Manager {
    public class PublicationManager : ISingletonComponent {

        private readonly IGenericDao _dao;

        public PublicationManager(IGenericDao dao) {
            _dao = dao;
        }

        public async Task<Publication> SavePublication(Publication publication, string base64Data) {
            var content = CompressionUtil.GetB64PartOnly(base64Data);
            var binaryData = CompressionUtil.Compress(Convert.FromBase64String(content));
            var pubData = new PublicationData() {
                BinaryContent = binaryData
            };
            using (var session = _dao.GetSession()) {
                using (var tx = session.BeginTransaction()) {
                    var savedPub = await _dao.Save(publication, session);
                    pubData.Publication = savedPub;
                    await _dao.Save(pubData, session);
                    tx.Commit();
                    return savedPub;
                }
            }
        }


        public async Task<byte[]> SavePublicationData(int publicationId, string base64Data) {
            var byteContent = GetByteContent(base64Data);
            var binaryData = CompressionUtil.Compress(byteContent);

            var pubData = new PublicationData() {
                Publication = new Publication() { Id = publicationId },
                BinaryContent = binaryData
            };

            await _dao.Save(pubData);
            return byteContent;

        }

        public byte[] GetByteContent(string base64Data) {
            var content = CompressionUtil.GetB64PartOnly(base64Data);
            var byteContent = Convert.FromBase64String(content);
            return byteContent;
        }
    }
}
