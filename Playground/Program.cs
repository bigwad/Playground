using System;
using System.Text;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using Starcounter;
using Starcounter.XSON;
using Starcounter.Templates;

namespace Playground
{
    class Program
    {
        static void Main()
        {
            Application.Current.Use(new HtmlFromJsonProvider());
            Application.Current.Use(new PartialToStandaloneHtmlProvider());

            Handle.GET("/playground", () =>
            {
                IndexPage page = new IndexPage()
                {
                    Session = new Session(SessionOptions.PatchVersioning),
                    Data = null
                };

                return page;
            });

            Handle.GET("/playground/dynamic", () =>
            {
                List<DataItem> dataItems = new List<DataItem>()
                {
                    new DataItem() { Id = 0, Priority = 0, Name = "John", Age = 15 },
                    new DataItem() { Id = 1, Priority = 1, Name = "Peter", Age = 20 },
                    new DataItem() { Id = 2, Priority = 2, Name = "Susan", Age = 25 },
                    new DataItem() { Id = 3, Priority = 3, Name = "Kelly", Age = 30 },
                    new DataItem() { Id = 4, Priority = 4, Name = "Sherlock", Age = 35 }
                };

                TObject pageTemplate = new TObject();

                pageTemplate.Add<TString>("Html").DefaultValue = "/IndexPage.html";

                TObjArr itemsTemplate = pageTemplate.Add<TObjArr>("Items");
                TObject itemTemplate = new TObject();

                itemTemplate.Add<TLong>("Id");
                itemTemplate.Add<TLong>("Priority");
                itemTemplate.Add<TString>("Name").SetCustomBoundAccessors(j => (j.Data as DataItem).Name, (j, s) => { });
                itemTemplate.Add<TLong>("Age");
                itemsTemplate.ElementType = itemTemplate;
                itemsTemplate.SetCustomBoundAccessors(j => { return dataItems; }, (j, d) => { });

                TLong trigger = pageTemplate.Add<TLong>("SwipeRowsTrigger$");

                trigger.Editable = true;
                trigger.AddHandler((j, prop, value) => new Input<Json, Property<long>, long>()
                {
                    App = j,
                    Template = prop,
                    Value = value
                },
                (j, a) =>
                {
                    a.Cancel();

                    List<DataItem> newItems = dataItems.Select(x => new DataItem(x)).ToList();
                    DataItem item = newItems.FirstOrDefault(x => x.Id == 0);

                    if (newItems.IndexOf(item) == item.Priority)
                    {
                        item.Name = "John White";
                        newItems.Remove(item);
                        newItems.Insert(1, item);
                    }
                    else
                    {
                        item.Name = "John";
                        newItems.Remove(item);
                        newItems.Insert(0, item);
                    }

                    dataItems = newItems;
                });

                Json page = new Json() { Template = pageTemplate };
                page.Session = new Session(SessionOptions.PatchVersioning);

                return page;
            });
        }
    }
}