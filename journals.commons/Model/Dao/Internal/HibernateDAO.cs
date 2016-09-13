using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using journals.commons.Model.dao;
using journals.commons.Model.Entities;
using journals.commons.Util;
using log4net;
using NHibernate;
using NHibernate.Type;

namespace journals.commons.Model.Dao.Internal {

    /// <summary>
    ///     Base Hibernate implementation of the IGenericDao Instance where all methods are transactional.
    /// </summary>
    public class HibernateDao : IGenericDao {

        private static readonly ILog HibernateLog = LogManager.GetLogger("QUERY_LOGGER");

        private readonly ISessionManager _sessionManager;

        public HibernateDao() {
            _sessionManager = SessionManagerWrapper.GetInstance();
        }


      

        public ISession GetSession() {
            return _sessionManager.OpenSession();
        }

        public virtual async Task<T> Save<T>(T ob, ISession session = null) where T : class, IBaseEntity {
            if (session != null) {
                //transaction is controlled outside of DAO scope in that scenario
                ob = await DoSave(ob, session);
                return ob;

            }
            using (session = GetSession()) {

                using (var transaction = session.BeginTransaction()) {
                    ob = await DoSave(ob, session);
                    transaction.Commit();
                    return ob;
                }
            }
        }

        public async Task<ICollection<T>> BulkSave<T>(ICollection<T> items) where T : class, IBaseEntity {
            if (items == null || !items.Any()) {
                return items;
            }
            var result = new List<T>(items.Count);
            using (var session = GetSession()) {
                using (var transaction = session.BeginTransaction()) {
                    // adding the saved items to a new collection 
                    // because they can't replace the original's in an iteration 
                    result.AddRange(await Task.WhenAll(items.Select(item =>DoSave(item, session))));
                    transaction.Commit();
                }
            }
            return result;
        }


        private async Task<T> DoSave<T>(T ob, ISession session) where T : class, IBaseEntity {
            var b = ob as IBaseEntity;
            b.OnSave();
            var errors = b.Validate();
            if (errors != null && errors.Any()) {
                throw ModelValidationException.Instance(b, errors);
            }

            if ((b.Id == 0 || b.Id == null)) {
                await session.SaveAsync(ob);
            } else {
                ob = await session.MergeAsync(ob);
            }
            return ob;
        }

        public async Task Delete(object ob) {
            using (var session = GetSession()) {
                using (var transaction = session.BeginTransaction()) {
                    await session.DeleteAsync(ob);
                    transaction.Commit();
                }
            }
        }

        public async Task<T> FindByPk<T>(object id, params string[] toEager) {
            using (var session = GetSession()) {
                using (session.BeginTransaction()) {
                    var ob = await session.LoadAsync(typeof(T), id);
                    for (var i = 0; i < toEager.Length; i++) {
                        var property = ReflectionUtil.GetProperty(ob, toEager[i]);
                        NHibernateUtil.Initialize(property);
                    }
                    return (T)ob;
                }
            }
        }

        public async Task<int> CountByQuery(string queryst, params object[] parameters) {
            using (var session = GetSession()) {
                using (session.BeginTransaction()) {
                    var query = BuildQuery(queryst, parameters, session);
                    var result =  Convert.ToInt32(await query.UniqueResultAsync());
                    return result;
                }
            }
        }

        public async Task<T> FindSingleByQuery<T>(string queryst, params object[] parameters) {
            using (var session = GetSession()) {
                using (session.BeginTransaction()) {
                    var query = BuildQuery(queryst, parameters, session);
                    return (T) await query.UniqueResultAsync();
                }
            }
        }

        public async Task<IList<T>> FindByQuery<T>(string queryst, PaginationData paginationData = null, params object[] parameters) where T : class {
            using (var session = GetSession()) {
                using (session.BeginTransaction()) {
                    var query = BuildQuery(queryst, parameters, session);
                    if (paginationData != null) {
                        var pageSize = paginationData.PageSize;
                        query.SetMaxResults(pageSize);
                        query.SetFirstResult((paginationData.PageNumber - 1) * pageSize);
                    }
                    return await query.ListAsync<T>();
                }
            }
        }

        public Task<IList<T>> FindByNativeQuery<T>(string queryst, PaginationData paginationData = null, params object[] parameters) where T : class
        {
            return null;
        }

        public async Task<int> ExecuteSql(string sql, params object[] parameters) {
            using (var session = GetSession()) {
                using (var transaction = session.BeginTransaction()) {
                    var query = session.CreateSQLQuery(sql);
                    if (parameters != null) {
                        for (int i = 0; i < parameters.Length; i++) {
                            query.SetParameter(i, parameters[i]);
                        }
                    }
                    var result = await query.ExecuteUpdateAsync();
                    transaction.Commit();

                    return result;
                }
            }
        }

        public async Task<int> ExecuteSql(string sql, ISession session, params object[] parameters) {
            using (var transaction = session.BeginTransaction()) {

                var query = session.CreateSQLQuery(sql);
                if (parameters != null) {
                    for (int i = 0; i < parameters.Length; i++) {
                        query.SetParameter(i, parameters[i]);
                    }
                }
                var result = await query.ExecuteUpdateAsync();
                transaction.Commit();

                return result;
            }
        }


        #region internalMethods


        private IQuery BuildQuery(string queryst, object[] parameters, ISession session) {

            var query = session.CreateQuery(queryst);
            query.SetFlushMode(FlushMode.Never);
            HibernateLog.Debug(queryst);

            if (parameters == null || !parameters.Any()) {
                return query;
            }
            for (var i = 0; i < parameters.Length; i++) {
                if (queryst.Contains(":p" + i)) {
                    if (parameters[i] == null) {
                        query.SetParameter("p" + i, parameters[i], NHibernateUtil.String);
                    } else {
                        var asEnumerable = parameters[i] as IEnumerable;
                        if (asEnumerable != null) {
                            query.SetParameterList("p" + i, (IEnumerable)parameters[i]);
                        } else {
                            query.SetParameter("p" + i, parameters[i]);
                        }
                    }
                } else {
                    if (parameters[i] != null) {
                        if (parameters[i] is Int32) {
                            query.SetParameter(i, parameters[i], new Int32Type());
                        } else {
                            query.SetParameter(i, parameters[i]);
                        }
                    }

                }

            }

            return query;
        }

        #endregion
    }


}
