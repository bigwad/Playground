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

            Handle.GET("/subground/table-row-item/{?}", (string id) => new TableRowItemJson(id));
            Handle.GET("/subground/table-columns-items", () => new TableColumnsItemsJson());
            Starcounter.Advanced.Blender.MapUri<Database.Person>("/subground/table-row-item/{?}", new string[0]);
            Starcounter.Advanced.Blender.MapUri("/subground/table-columns-items", $"table-columns-{typeof(Database.Person)}", new string[0]);

            Handle.GET("/subground/items/partial/{?}", (string id) =>
            {
                Person item = Db.FromId<Person>(id);
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

            //Starcounter.Advanced.Blender.MapUri<Item>("/subground/items/partial/{?}", new string[0]);
        }
    }
}