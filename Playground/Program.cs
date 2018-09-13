using System;
using System.Text;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
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
        }
    }
}