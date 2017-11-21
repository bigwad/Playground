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

            Handle.GET("/test/sync", () =>
            {
                Scheduling.RunTask(() => throw new Exception("Sync exception")).Wait();
                return "Done";
            });

            Handle.GET("/test/async", () =>
            {
                Scheduling.RunTask(() => throw new Exception("Async exception"));
                return "Done";
            });

            Handle.GET("/test/async-session", () =>
            {
                if (Session.Current == null)
                {
                    Session.Current = new Session(SessionOptions.PatchVersioning);
                }

                Session.RunTask(Session.Current.SessionId, (s, id) => throw new Exception("Sync exception"));
                return "Done";
            });

            Handle.GET("/index", () =>
            {
                if (Session.Current == null)
                {
                    Session.Current = new Session(SessionOptions.PatchVersioning);
                }

                return new IndexPage();
            });
        }
    }
}