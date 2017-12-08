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

            Handle.GET("/a/form", () =>
            {
                return Db.Scope(() =>
                {
                    form json = new form();
                    json.P.Data = GetP();
                    return json;
                });
            });

            Handle.GET("/a/link", () =>
            {
                return Db.Scope(() =>
                {
                    return new link();
                });
            });

            Blender.MapUri("/a/form", $"A.{typeof(P).FullName}");
            Blender.MapUri("/a/link", $"B.{typeof(P).FullName}");
        }

        static P GetP()
        {            
            P p = DbLinq.Objects<P>().FirstOrDefault();

            if (p == null)
            {
                Db.Transact(() =>
                {
                    p = new P() { v = Guid.NewGuid().ToString() };
                });
            }

            return p;
        }
    }
}