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
                Item item = Db.FromId<Item>(id);
                Json page;

                if (item.Index >= 0)
                {
                    ItemPage ip = new ItemPage();

                    ip.Item.Data = item;
                    page = ip;
                }
                else
                {
                    ItemNegativePage inp = new ItemNegativePage();

                    inp.Item.Data = item;
                    page = inp;
                }

                return page;
            }, new HandlerOptions() { SelfOnly = true });

            Starcounter.Advanced.Blender.MapUri<Item>("/subground/items/partial/{?}", new string[0]);
        }
    }
}