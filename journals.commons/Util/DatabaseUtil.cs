namespace journals.commons.Util {
    internal class DatabaseUtil {

        public enum DBMS {
            MSSQL, MYSQL, SQLLITE
        }

        public static string HibernateDialect(string driverName) {
            var dbms = DiscoverDBMS(driverName);

            switch (dbms) {
                case DBMS.SQLLITE:
                return typeof(NHibernate.Dialect.SQLiteDialect).FullName;
                case DBMS.MSSQL:
                return typeof(NHibernate.Dialect.MsSql2008Dialect).FullName;
                case DBMS.MYSQL:
                return typeof(NHibernate.Dialect.MySQLDialect).FullName;
                default:
                return typeof(NHibernate.Dialect.MsSql2008Dialect).FullName;
            }

        }


        internal static DBMS DiscoverDBMS(string baseProviderName) {
            switch (baseProviderName) {
                case "System.Data.SQL":
                return DBMS.MSSQL;
                case "System.Data.SqlClient":
                return DBMS.MSSQL;
                case "System.Data.SQLite":
                return DBMS.SQLLITE;

            }
            return DBMS.MYSQL;
        }


        public static string HibernateDriverName(string baseProviderName) {
            var dbms = DiscoverDBMS(baseProviderName);
            switch (dbms) {
                case DBMS.SQLLITE:
                return typeof(NHibernate.Driver.SQLite20Driver).FullName;
                case DBMS.MSSQL:
                return typeof(NHibernate.Driver.SqlClientDriver).FullName;
                case DBMS.MYSQL:
                return typeof(NHibernate.Driver.MySqlDataDriver).FullName;
                default:
                return typeof(NHibernate.Driver.SqlClientDriver).FullName;
            }

        }
    }
}
