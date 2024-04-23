using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Database.Repository;
using DG.Tweening;
using NekoOdyssey.Scripts.Database.Domains;
using NekoOdyssey.Scripts.Database.Domains.Sites;
using NekoOdyssey.Scripts.Database.Domains.Sites.Entities.SiteEntity.Repo;
using NekoOdyssey.Scripts.IO.FileStore.Nintendo;
using SpatiumInteractive.Libraries.Unity.GRU.Base;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SpatiumInteractive.Libraries.Unity.GRU.Domain;
using SQLite4Unity3d;
using UnityEngine;

namespace NekoOdyssey.Scripts.Database
{
    public class PersonDbContext : IDbContext<SQLiteConnection>
    {
        private SQLiteConnection _connection;

        public PersonDbContext(SQLiteConnection connection)
        {
            _connection = connection;
        }

        public SQLiteConnection Context => _connection;
    }

    public class Person : EntityBase<int>, IAggregateRoot
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public int Age { get; set; }

        public Person()
        {
        }

        public Person(string name, string surname, int age)
        {
            Name = name;
            Surname = surname;
            Age = age;
        }

        public override string ToString()
        {
            return $">>person<< id: {Id} name: {Name} surname: {Surname} age: {Age}";
        }
    }

    public class PersonRepo : Repository<Person, int, SQLiteConnection>
    {
        public PersonRepo(IDbContext<SQLiteConnection> dbContext) : base(dbContext)
        {
        }

        public void TestInsert()
        {
            Add(new Person("Chanan", "G", 40));
        }

        public int GetTotalCount()
        {
            var totalCount = _dbContext.Context.TableAbstract<Person>().Count();
            return totalCount;
        }

        public IEnumerable<Person> List()
        {
            return _dbContext.Context.Table<Person>()
                // .Where(p => p.Age >= 40)
                .OrderByDescending(x => x.Id)
                .ToList();
            // var query = new QueryExpression<Person>();
            // query.AddWhereExpression(x => true);
            // query.AddOrderExpression(x => x.Id, OrderDirection.Descending);
            // return FindAll(query);
        }
    }

    public class DatabaseInitializer : MonoBehaviour
    {
        private List<string> _logs = new();

        private static readonly string[] DatabaseNames = new[]
        {
            "existing.db",
        };

        private void Start()
        {
//             foreach (var databaseName in DatabaseNames)
//                 Debug.Log($">>database_name<< {databaseName}");
//
// #if UNITY_SWITCH && !UNITY_EDITOR
//             foreach (var databaseName in DatabaseNames)
//             {
//                 var filepath = $"{Application.streamingAssetsPath}/{databaseName}";
//                 var dbData = File.ReadAllBytes(filepath);
//                 if (NintendoFileHandler.Exists(databaseName))
//                 {
//                     Debug.Log($">>file<< {databaseName} exists");
//                     continue;
//                 }
//
//                 Debug.Log($">>database_name<< copy {databaseName} {dbData.Length}");
//                 NintendoFileHandler.SaveRaw(dbData, databaseName);
//             }
// #endif
//             var connector = new DataConnector("existing",
//                 "D:/neko-odyssey/neko-odyssey-development/Assets/StreamingAssets");
//             var connection = connector.GetConnection();
//             var context = new PersonDbContext(connection);
//             var repo = new PersonRepo(context);
//             repo.TestInsert();
//             _logs.Add($">>total<< {repo.GetTotalCount()}");
//             _logs.Add($">>persons<<");
//             var people = repo.List();
//             _logs.AddRange(people.Select(person => person.ToString()));
//             connection.Dispose();
// #if UNITY_SWITCH && !UNITY_EDITOR
//             NintendoFileHandler.Commit();
// #endif

            using (var siteDbContext = new SitesDbContext(new() { ReadOnly = true, CopyMode = DbCopyMode.ForceCopy }))
            {
                var siteRepo = new SiteRepo(siteDbContext);
                var site = siteRepo.FindByName("Prologue");
                _logs.Add(site.ToString());
                site = siteRepo.FindByName("GamePlayZone4");
                _logs.Add(site.ToString());
            }

            using (var siteDbContext = new SitesDbContext(new() { ReadOnly = true, CopyMode = DbCopyMode.ForceCopy }))
            {
                var siteRepo = new SiteRepo(siteDbContext);
                var site = siteRepo.FindByName("Prologue");
                _logs.Add(site.ToString());
            }
        }

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
            GUILayout.Label("\n" + string.Join("\n", _logs.ToArray()));
            GUILayout.EndArea();
        }
    }
}