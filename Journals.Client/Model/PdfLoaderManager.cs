using System;
using System.IO;
using System.Threading.Tasks;
using journals.commons.Model.dao;
using journals.commons.Model.Entities;
using journals.commons.Model.Manager;
using journals.commons.SimpleInjector;
using journals.commons.Util;
using Journals.Client.Util;
using Newtonsoft.Json.Linq;
using PdfiumViewer;

namespace Journals.Client.Model {
    public class PdfLoaderManager : ISingletonComponent {

        private readonly IGenericDao _dao;
        private JournalViewModel _jvm;
        private readonly RestService _restService;
        private readonly PublicationManager _publicationManager;

        public PdfLoaderManager(IGenericDao dao, JournalViewModel jvm, RestService restService, PublicationManager publicationManager) {
            _dao = dao;
            _jvm = jvm;
            _restService = restService;
            _publicationManager = publicationManager;
        }

        public async Task<PdfDocument> LoadPdfData(int publicationId) {
            var pubData = await _dao.FindSingleByQuery<PublicationData>(PublicationData.ByPublication, publicationId);
            byte[] bytes;

            if (pubData != null) {
                bytes = CompressionUtil.Decompress(pubData.BinaryContent);
                var stream = new MemoryStream(bytes);
                return PdfDocument.Load(stream);
            }
            var response = await _restService.Get("Publication/DownloadPublication", "publicationid={0}".Fmt(publicationId));
            var job = JObject.Parse(response.Item2);
            var base64Data = job.StringValue("base64String");
            bytes = _publicationManager.GetByteContent(base64Data);
            var memstream = new MemoryStream(bytes);
            var pdf = PdfDocument.Load(memstream);
            await _publicationManager.SavePublicationData(publicationId, base64Data);
            return pdf;

        }
    }
}
