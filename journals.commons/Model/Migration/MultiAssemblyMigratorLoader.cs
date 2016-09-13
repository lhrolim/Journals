using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentMigrator;
using FluentMigrator.Exceptions;
using FluentMigrator.Infrastructure;
using FluentMigrator.Runner;

//took from http://stackoverflow.com/questions/25239992/using-multiple-fluentmigrator-assemblies-on-same-database
namespace JournalsForm.Commons.Model.Migration {


    public class MultiAssemblyMigrationLoader : IMigrationInformationLoader {
        public MultiAssemblyMigrationLoader(IMigrationConventions conventions, IEnumerable<Assembly> assemblies, string @namespace, IEnumerable<string> tagsToMatch)
            : this(conventions, assemblies, @namespace, false, tagsToMatch) {
        }

        public MultiAssemblyMigrationLoader(IMigrationConventions conventions, IEnumerable<Assembly> assemblies, string @namespace, bool loadNestedNamespaces, IEnumerable<string> tagsToMatch) {
            Conventions = conventions;
            Assemblies = assemblies;
            Namespace = @namespace;
            LoadNestedNamespaces = loadNestedNamespaces;
            TagsToMatch = tagsToMatch ?? new string[0];
        }

        public IMigrationConventions Conventions {
            get;
        }

        public IEnumerable<Assembly> Assemblies {
            get;
        }

        public string Namespace {
            get;
        }

        public bool LoadNestedNamespaces {
            get;
        }

        public IEnumerable<string> TagsToMatch {
            get;
        }

        public SortedList<long, IMigrationInfo> LoadMigrations() {
            var sortedList = new SortedList<long, IMigrationInfo>();

            IEnumerable<Type> migrations = FindMigrationTypes();
            if (migrations == null)
                return sortedList;

            foreach (Type migration in migrations) {
                IMigrationInfo migrationInfo = Conventions.GetMigrationInfo(migration);
                if (sortedList.ContainsKey(migrationInfo.Version))
                    throw new DuplicateMigrationException(string.Format("Duplicate migration version {0}.", migrationInfo.Version));
                sortedList.Add(migrationInfo.Version, migrationInfo);
            }
            return sortedList;
        }

        private IEnumerable<Type> FindMigrationTypes() {
            IEnumerable<Type> types = new Type[] { };
            foreach (var assembly in Assemblies) {
                types = types.Concat(assembly.GetExportedTypes());
            }
            IEnumerable<Type> matchedTypes = types.Where(t => Conventions.TypeIsMigration(t)
                                                                 &&
                                                                 (Conventions.TypeHasMatchingTags(t, TagsToMatch) ||
                                                                  !Conventions.TypeHasTags(t)));

            if (!string.IsNullOrEmpty(Namespace)) {
                Func<Type, bool> shouldInclude = t => t.Namespace == Namespace;
                if (LoadNestedNamespaces) {
                    string matchNested = Namespace + ".";
                    shouldInclude = t => t.Namespace == Namespace || t.Namespace.StartsWith(matchNested);
                }

                matchedTypes = matchedTypes.Where(shouldInclude);
            }

            return matchedTypes;
        }
    }
}