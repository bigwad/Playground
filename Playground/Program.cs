using System;
using System.Text;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using Starcounter;
using Starcounter.Linq;
using Playground.Database;

namespace Playground
{
    class Program
    {
        static void Main()
        {
            Application.Current.Use(new HtmlFromJsonProvider());
            Application.Current.Use(new PartialToStandaloneHtmlProvider());

            Handle.GET("/a", () =>
            {
                MasterPage master = new MasterPage();

                master.CurrentPage = Db.Scope(() => Self.GET("/a/form"));

                return master;
            });

            Handle.GET("/a/form", () =>
            {
                return Db.Scope(() =>
                {
                    IndexPage page = new IndexPage();
                    page.Product.Data = GetProduct();
                    return page;
                });
            });

            Handle.GET("/a/link", () =>
            {
                return new LinkPage();
            });

            Blender.MapUri("/a/form", $"{nameof(Playground)}.{typeof(Product).FullName}");
            Blender.MapUri("/a/link", $"Subground.{typeof(Product).FullName}");
        }

        static Product GetProduct()
        {            
            Product p = DbLinq.Objects<Product>().FirstOrDefault();

            if (p == null)
            {
                Db.Transact(() =>
                {
                    p = new Product() { Name = Guid.NewGuid().ToString() };
                });
            }

            return p;
        }
    }
}