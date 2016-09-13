using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using journals.commons.Model.Entities;
using journals.commons.SimpleInjector;
using NHibernate;

namespace journals.commons.Model.dao {

    public interface IGenericDao : ISingletonComponent {
        /// <summary>
        /// Saves and returns the object
        /// </summary>
        /// <typeparam name="T">Type of Object to persist, must extend IBaseEntity</typeparam>
        /// <param name="objectToPersist"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        Task<T> Save<T>(T objectToPersist, ISession session=null) where T : class, IBaseEntity;

        Task<ICollection<T>> BulkSave<T>(ICollection<T> items) where T : class, IBaseEntity;

        Task Delete(object ob);

        Task<T> FindByPk<T>(object id, params string[] toEager);

        Task<int> CountByQuery(string queryst, params object[] parameters);

        Task<T> FindSingleByQuery<T>(string queryst, params object[] parameters);

        Task<IList<T>> FindByQuery<T>(string queryst, PaginationData paginationData=null, params object[] parameters) where T : class;

        Task<IList<T>> FindByNativeQuery<T>(string queryst, PaginationData paginationData = null, params object[] parameters) where T : class;


        Task<int> ExecuteSql(string sql, params object[] parameters);

        Task<int> ExecuteSql(string sql,ISession session, params object[] parameters);

        ISession GetSession();
    }


}