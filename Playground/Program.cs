﻿using System;
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
            //RegisterDatabaseHooks();

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