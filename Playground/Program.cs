using System;
using System.Text;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.IO;
using Starcounter;
using Starcounter.Linq;
using Database.Tree;

namespace Playground
{
    class Program
    {
        static void Main()
        {
            Application.Current.Use(new HtmlFromJsonProvider());
            Application.Current.Use(new PartialToStandaloneHtmlProvider());

            Handle.GET("/insert/{?}", (int count) =>
            {
                List<Sibling> siblings = new List<Sibling>();
                Random rand = new Random();

                Db.Transact(() =>
                {
                    for (int i = 0; i < count; i++)
                    {
                        siblings.Add(new Sibling() { Index = i, Name = $"Sibling {i}" });
                    }

                    for (int i = 0; i < count; i++)
                    {
                        new Parent() { Index = i, Name = $"Parent {i}", ParentSibling = siblings[rand.Next(0, siblings.Count)] };
                        new Child() { Index = i, Name = $"Child {i}", ParentSibling = siblings[rand.Next(0, siblings.Count)], Date = DateTime.Now, Value = $"Child value {i}", ChildSibling = siblings[rand.Next(0, siblings.Count)] };
                    }
                });

                return $"{count} item(s) in each table inserted.";
            });

            Handle.GET("/export", () =>
            {
                ImportExport.Container container = new ImportExport.Container();

                container.PopulateFromDatabase();

                return container.ToJson();
            });

            Handle.GET("/import/{?}", (string path) =>
            {
                TextReader reader = new StreamReader(path);
                string json = reader.ReadToEnd();

                reader.Dispose();

                ImportExport.Container container = ImportExport.Container.FromJson(json);

                Db.Transact(() =>
                {
                    container.PopulateToDatabase();
                });

                return $"The {path} file has been imported into the database.";
            });
        }
    }
}