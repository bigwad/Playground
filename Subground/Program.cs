using System;
using System.Collections.Generic;
using System.Linq;
using Starcounter;
using Starcounter.Linq;
using Database;

namespace B
{
    class Program
    {
        static void Main()
        {
            Application.Current.Use(new HtmlFromJsonProvider());
            Application.Current.Use(new PartialToStandaloneHtmlProvider());

            Handle.GET("/b", () =>
            {
                return Db.Scope(() =>
                {
                    mp json = new mp();
                    json.cp = Self.GET("/b/form");
                    return json;
                });
            });

            Handle.GET("/b/form", () =>
            {
                return Db.Scope(() =>
                {
                    form json = new form();
                    json.P.Data = GetP();
                    return json;
                });
            });

            Handle.GET("/b/link", () =>
            {
                return Db.Scope(() =>
                {
                    return new link();
                });
            });

            Blender.MapUri("/b/form", $"B.{typeof(P).FullName}");
            Blender.MapUri("/b/link", $"A.{typeof(P).FullName}");
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