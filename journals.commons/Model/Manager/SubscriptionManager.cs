using System.Collections.Generic;
using System.Threading.Tasks;
using journals.commons.Model.dao;
using journals.commons.Model.Dao.Internal;
using journals.commons.Model.Entities;
using journals.commons.Security;
using journals.commons.SimpleInjector;

namespace journals.commons.Model.Manager {
    public class SubscriptionManager : ISingletonComponent {

        private readonly IGenericDao _dao;

        public SubscriptionManager(IGenericDao dao) {
            _dao = dao;
        }

        public async Task<IList<JournalDto>> LookupJournals(UserIdentity user, PaginationData paginationData) {
            using (var session = _dao.GetSession()) {
                using (session.BeginTransaction()) {
                    var query = session.CreateQuery(JournalDto.ByUser);
                    session.EnableFilter("UserFilter").SetParameter("id", user.UserId);
                    if (paginationData != null) {
                        var pageSize = paginationData.PageSize;
                        query.SetMaxResults(pageSize);
                        query.SetFirstResult((paginationData.PageNumber - 1) * pageSize);
                    }
                    return await query.ListAsync<JournalDto>();
                }
            }
        }


    }
}
