using System;
using System.Text;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Globalization;
using System.Threading;
using Starcounter;
using Starcounter.Linq;
using Database;

namespace Playground
{
    class Program
    {
        static void Main()
        {
            //CultureInfo culture = CultureInfo.CreateSpecificCulture("tr-TR");
            //Thread.CurrentThread.CurrentCulture = culture;
            //Thread.CurrentThread.CurrentUICulture = culture;

            Application.Current.Use(new HtmlFromJsonProvider());
            Application.Current.Use(new PartialToStandaloneHtmlProvider());

            Handle.GET("/semantic-table", () =>
            {
                Session.Ensure();
                SemanticTablePage page = new SemanticTablePage();
                return page;
            });

            Handle.GET("/playground/table-items", () => new TableItemsJson());
            Handle.GET("/playground/table-columns-items", () => new TableColumnsItemsJson());
            Handle.GET("/playground/table-row-item/{?}", (string id) => new TableRowItemJson(id));
            Starcounter.Advanced.Blender.MapUri<Database.Person>("/playground/table-row-item/{?}", new string[0]);
            Starcounter.Advanced.Blender.MapUri("/playground/table-columns-items", $"table-columns-{typeof(Database.Person)}", new string[0]);

            Handle.GET("/index", () =>
            {
                Session.Ensure();

                IndexPage page = new IndexPage();
                Session.Current.Store[nameof(IndexPage)] = page;

                page.Init();

                return page;
            });

            Handle.GET("/items/{?}", (string id) => 
            {
                Session.Ensure();

                ItemPage page = Session.Current.Store[nameof(ItemPage)] as ItemPage;
                Starcounter.XSON.IScopeContext scope = page?.AttachedScope;

                if (scope != null)
                {
                    return scope.Scope(() =>
                    {
                        page = Self.GET<ItemPage>($"/items/partial/{id}");
                        Session.Current.Store[nameof(ItemPage)] = page;
                        return page;
                    });
                }

                return Db.Scope<Json>(() =>
                {
                    page = Self.GET<ItemPage>($"/items/partial/{id}");
                    Session.Current.Store[nameof(ItemPage)] = page;
                    return page;
                });
            });

            Handle.GET("/items/partial/{?}", (string id) =>
            {
                ItemPage page = new ItemPage();
                Person item = Db.FromId<Person>(id);

                page.Item.Data = item;

                return page;
            }, new HandlerOptions() { SelfOnly = true });

            //Starcounter.Advanced.Blender.MapUri<Item>("/items/partial/{?}", new string[0]);
        }
    }
}