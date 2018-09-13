using System;
using System.Collections.Generic;
using System.Linq;
using Starcounter;
using Database;

namespace Subground
{
    class Program
    {
        static void Main()
        {
            Application.Current.Use(new HtmlFromJsonProvider());
            Application.Current.Use(new PartialToStandaloneHtmlProvider());

            Handle.GET("/subground/items/partial/{?}", (string id) =>
            {
                ItemPage page = new ItemPage();
                Item item = Db.FromId<Item>(id);

                page.Item.Data = item;

                return page;
            }, new HandlerOptions() { SelfOnly = true });

            Starcounter.Advanced.Blender.MapUri<Item>("/subground/items/partial/{?}", new string[0]);
        }
    }
}