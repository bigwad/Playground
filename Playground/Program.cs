using System;
using System.Text;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using Starcounter;
using Starcounter.Linq;
using Database;

namespace Playground
{
    class Program
    {
        static void Main()
        {
            Db.Transact(() =>
            {
                DbLinq.Objects<Database.Item>().DeleteAll();

                for (int i = 0; i < 10; i++)
                {
                    new Database.Item() { Name = "Item #" + i, Index = i };
                }
            });

            Application.Current.Use(new HtmlFromJsonProvider());
            Application.Current.Use(new PartialToStandaloneHtmlProvider());

            Handle.GET("/json-linq-bind", () => new IndexPage());
        }
    }
}