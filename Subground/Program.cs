using System;
using System.Collections.Generic;
using System.Linq;
using Starcounter;
using Starcounter.Linq;
using Playground.Database;

namespace Subground
{
    class Program
    {
        static void Main()
        {
            Application.Current.Use(new HtmlFromJsonProvider());
            Application.Current.Use(new PartialToStandaloneHtmlProvider());

            Handle.GET("/b", () =>
            {
                MasterPage master = new MasterPage();

                master.CurrentPage = Db.Scope(() => Self.GET("/b/form"));

                return master;
            });

            Handle.GET("/b/form", () =>
            {
                return Db.Scope(() =>
                {
                    IndexPage page = new IndexPage();
                    page.Product.Data = GetProduct();
                    return page;
                });
            });

            Handle.GET("/b/link", () =>
            {
                return new LinkPage();
            });

            Blender.MapUri("/b/form", $"{nameof(Subground)}.{typeof(Product).FullName}");
            Blender.MapUri("/b/link", $"Playground.{typeof(Product).FullName}");
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