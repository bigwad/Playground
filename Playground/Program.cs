using System;
using System.Text;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using Starcounter;
using Starcounter.Linq;
using Database;

namespace A
{
    class Program
    {
        static void Main()
        {
            Application.Current.Use(new HtmlFromJsonProvider());
            Application.Current.Use(new PartialToStandaloneHtmlProvider());

            Handle.GET("/a", () =>
            {
                return Db.Scope(() =>
                {
                    mp json = new mp();
                    json.cp = Self.GET("/a/form");
                    return json;
                });
            });
        }
    }
}