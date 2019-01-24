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
            Application.Current.Use(new HtmlFromJsonProvider());
            Application.Current.Use(new PartialToStandaloneHtmlProvider());
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
        }
    }
}