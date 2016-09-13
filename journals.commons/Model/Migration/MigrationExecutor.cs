using System;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using FluentMigrator;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Announcers;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors;
using FluentMigrator.Runner.Processors.MySql;
using journals.commons.Util;
using JournalsForm.Commons.Model.Migration;
using log4net;

namespace journals.commons.Model.Migration {

    /// <summary>
    /// Responsible for running all available migrators of the solution, generating the database schema and any eventual initialization data
    /// </summary>
    public class MigratorExecutor {

        private static readonly ILog Log = LogManager.GetLogger(typeof(MigratorExecutor));

        private readonly string _connectionString;
        private readonly DatabaseUtil.DBMS _serverType;

        public MigratorExecutor() {

            var connString = ConfigurationManager.ConnectionStrings["journal"];
            if (connString == null) {
                throw new InvalidOperationException("Please setup a connection string called \"journal\"");
            }

            _connectionString = connString.ConnectionString;
            _serverType = DatabaseUtil.DiscoverDBMS(connString.ProviderName);


        }

        private class MigrationOptions : IMigrationProcessorOptions {
            public bool PreviewOnly {
                get; set;
            }
            public int Timeout {
                get; set;
            }
            public string ProviderSwitches {
                get; private set;
            }
        }

        public void Migrate(Action<IMigrationRunner> runnerAction) {

            var stopWatch = Stopwatch.StartNew();

            var options = new MigrationOptions { PreviewOnly = false, Timeout = 0 };
            var factory = GetFactory();
            if (factory == null) {
                Log.WarnFormat("choosen database {0} is not supported for automatic Migration", _serverType);
                return;
            }

            var assembly = Assembly.GetExecutingAssembly();

            //using (var announcer = new NullAnnouncer())
            var announcer = new TextWriterAnnouncer(s => Debug.WriteLine(s));
            var migrationContext = new RunnerContext(announcer);

            var processor = factory.Create(_connectionString, announcer, options);
            var runner = new MigrationRunner(assembly, migrationContext, processor);

            var migratorAssemblies = AssemblyUtil.GetJournalAssemblies();
            runner.MigrationLoader = new MultiAssemblyMigrationLoader(runner.Conventions, migratorAssemblies, migrationContext.Namespace, migrationContext.NestedNamespaces, migrationContext.Tags);
            runner.MigrateUp(true);
            runnerAction(runner);

            stopWatch.Stop();
            Log.InfoFormat("Migration execution finished in {0}", stopWatch.ElapsedMilliseconds.ToString(CultureInfo.InvariantCulture) + " ms");
        }

        private MigrationProcessorFactory GetFactory() {

            if (_serverType.Equals(DatabaseUtil.DBMS.MSSQL)) {
                return new FluentMigrator.Runner.Processors.SqlServer.SqlServerProcessorFactory();
            }
            if (_serverType.Equals(DatabaseUtil.DBMS.MYSQL)) {
                return new MySqlProcessorFactory();
            }

            if (_serverType.Equals(DatabaseUtil.DBMS.SQLLITE)) {
                return new FluentMigrator.Runner.Processors.SQLite.SQLiteProcessorFactory();
            }

            return null;
        }
    }
}
