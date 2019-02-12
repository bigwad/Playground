using System;
using System.Text;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Globalization;
using System.Threading;
using Newtonsoft.Json;
using Starcounter;
using Starcounter.Linq;
using Database;

namespace Playground
{
    class Program
    {
        static public void CreateIndex(string index_name, string query)
        {
            bool index_exist = false;
            Db.Transact(() => index_exist = Db.SQL("SELECT i FROM Starcounter.Metadata.\"Index\" i WHERE Name = ?", index_name).Any());
            if (!index_exist)
                Db.SQL(query);
        }

        static void Main()
        {
            CreateIndex("Item_Date", $"CREATE INDEX Item_Date ON {nameof(Database.Item)} ({nameof(Database.Item.Date)})");

            //RegisterDatabaseHooks();
            RegisterRestApi();
            //RegisterUi();
        }

        static void RegisterUi()
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

                return Db.Scope<Json>(() =>
                {
                    ItemPage page = new ItemPage();
                    Item item = Db.FromId<Item>(id);

                    page.Item.Data = item;

                    return page;
                });
            });
        }

        static void RegisterRestApi()
        {
            Handle.GET("/rest/list", () =>
            {
                //ItemProxy[] items = DbLinq.Objects<Item>().OrderBy(x => x.Date).ToArray().Select(x => new ItemProxy(x)).ToArray();
                ItemProxy[] items = Db.SQL<Item>("SELECT i FROM Database.Item i ORDER BY i.\"Date\"").Select(x => new ItemProxy(x)).ToArray();
                string json = JsonConvert.SerializeObject(items);

                return json;
            });

            Handle.GET("/rest/view/{?}", (ulong no) =>
            {
                Item item = Db.FromId<Item>(no);

                if (item == null)
                {
                    return $"Item with no {no} does not exist.";
                }

                ItemProxy proxy = new ItemProxy(item);
                string json = JsonConvert.SerializeObject(proxy);

                return json;
            });

            Handle.POST("/rest/insert", (Request request) =>
            {
                ItemProxy item = JsonConvert.DeserializeObject<ItemProxy>(request.Body);
                item.Insert();

                string json = JsonConvert.SerializeObject(item);

                return json;
            });

            Handle.PUT("/rest/update", (Request request) =>
            {
                ItemProxy item = JsonConvert.DeserializeObject<ItemProxy>(request.Body);
                item.Update();

                string json = JsonConvert.SerializeObject(item);

                return json;
            });

            Handle.DELETE("/rest/delete/{?}", (ulong no) =>
            {
                Item item = Db.FromId<Item>(no);

                if (item == null)
                {
                    return $"Item with no {no} does not exist.";
                }

                Db.Transact(() =>
                {
                    item.Delete();
                });

                return 200;
            });
        }

        static void RegisterDatabaseHooks()
        {
            Hook<Item>.AfterCommitInsert += (sender, entityId) => Session.RunTaskForAll((s, id) =>
            {
                if (s == null || s.ActiveWebSocket == null)
                {
                    return;
                }

                IndexPage page = s.Store[nameof(IndexPage)] as IndexPage;

                if (page != null)
                {
                    page.ItemInserted(entityId);
                    s.CalculatePatchAndPushOnWebSocket();
                }
            });

            Hook<Item>.AfterCommitUpdate += (sender, entityId) => Session.RunTaskForAll((s, id) =>
            {
                if (s == null || s.ActiveWebSocket == null)
                {
                    return;
                }

                IndexPage page = s.Store[nameof(IndexPage)] as IndexPage;

                if (page != null)
                {
                    page.ItemUpdated(entityId);
                    s.CalculatePatchAndPushOnWebSocket();
                }
            });

            Hook<Item>.AfterCommitDelete += (sender, entityId) => Session.RunTaskForAll((s, id) =>
            {
                if (s == null || s.ActiveWebSocket == null)
                {
                    return;
                }

                IndexPage page = s.Store[nameof(IndexPage)] as IndexPage;

                if (page != null)
                {
                    page.ItemDeleted(entityId);
                    s.CalculatePatchAndPushOnWebSocket();
                }
            });
        }
    }
}