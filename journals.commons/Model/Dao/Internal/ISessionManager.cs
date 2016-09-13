using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using journals.commons.Model.Entities;
using journals.commons.Util;
using NHibernate;
using NHibernate.Engine;
using NHibernate.Mapping.Attributes;
using NHibernate.Type;

namespace journals.commons.Model.Dao.Internal {


    public interface ISessionManager {


        ISession OpenSession();

        ISession GetOpenendSession();

        void Restart();

    }


    public class SessionManagerWrapper : ISessionManager {

        private readonly string _connectionString;
        private readonly string _driverName;
        private readonly string _dialect;
        private readonly IEnumerable<Assembly> _assembliesToLookupForMappings;


        private ISessionFactory _sessionFactory;

        private static bool _useAssignedGenerator = false;

        private static SessionManagerWrapper _instance;


        public static SessionManagerWrapper GetInstance() {
            if (_instance == null) {

                var connString = ConfigurationManager.ConnectionStrings["journal"];

                _useAssignedGenerator = ConfigurationManager.AppSettings["assignedgenerator"].Equals("true");

                if (connString == null) {
                    throw new InvalidOperationException("Please setup a connection string called \"journal\"");
                }

                _instance = new SessionManagerWrapper(connString.ConnectionString, DatabaseUtil.HibernateDriverName(connString.ProviderName),
                DatabaseUtil.HibernateDialect(connString.ProviderName), AssemblyUtil.GetJournalAssemblies());
            }

            return _instance;
        }


        private SessionManagerWrapper(string connectionString, string driverName, string dialect, IEnumerable<Assembly> assembliesToLookupForMappings) {
            _connectionString = connectionString;
            _driverName = driverName;
            _dialect = dialect;
            _assembliesToLookupForMappings = assembliesToLookupForMappings;
            _sessionFactory = InitSessionFactory();
            _useAssignedGenerator = ConfigurationManager.AppSettings["assignedgenerator"].Equals("true");
        }


        private ISessionFactory InitSessionFactory() {
            var configuration = new NHibernate.Cfg.Configuration();

            var properties = new Dictionary<string, string> {
                [NHibernate.Cfg.Environment.ConnectionString] = _connectionString
            };

            configuration.AddFilterDefinition(new FilterDefinition("UserFilter", "user_id = :id", new Dictionary<string, IType>() { { "id", new Int32Type() } }, false));

            properties.Add(NHibernate.Cfg.Environment.ConnectionDriver, _driverName);
            properties.Add(NHibernate.Cfg.Environment.Dialect, _dialect);
            properties.Add(NHibernate.Cfg.Environment.ShowSql, "true");
            properties.Add(NHibernate.Cfg.Environment.ConnectionProvider, "NHibernate.Connection.DriverConnectionProvider");
            properties.Add(NHibernate.Cfg.Environment.ProxyFactoryFactoryClass, "NHibernate.Bytecode.DefaultProxyFactoryFactory, NHibernate");
            //            properties.Add(NHibernate.Cfg.Environment.CurrentSessionContextClass, "thread_static");
            configuration.SetProperties(properties);

            var hibernateTypes = AttributeUtil.FindTypesAnnotattedWith(_assembliesToLookupForMappings, typeof(ClassAttribute), typeof(JoinedSubclassAttribute));

            foreach (var nHibernateType in hibernateTypes) {
                configuration.AddInputStream(HbmSerializer.Default.Serialize(nHibernateType));

                if (_useAssignedGenerator && nHibernateType != typeof(PublicationData)) {
                    var keyValue = configuration.GetClassMapping(nHibernateType).Key as NHibernate.Mapping.SimpleValue;
                    if (keyValue != null) {
                        // set GeneratorStrategy (the same string you would put in generator class="..." in the hbm file)
                        keyValue.IdentifierGeneratorStrategy = "assigned";
                    }
                }
            }


            return configuration.BuildSessionFactory();
        }

        public ISession OpenSession() {
            return _sessionFactory.OpenSession();
        }

        public ISession GetOpenendSession() {
            return _sessionFactory.GetCurrentSession();
        }

        public void Restart() {
            _sessionFactory = InitSessionFactory();
        }
    }
}
