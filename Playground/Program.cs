using System;
using System.Text;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using Starcounter;
using Starcounter.XSON;
using Starcounter.Templates;
using Playground.Database;

namespace Playground
{
    class Program
    {
        static void Main()
        {
            Application.Current.Use(new HtmlFromJsonProvider());
            Application.Current.Use(new PartialToStandaloneHtmlProvider());

            Handle.GET("/Playground/product", () =>
            {
                return Self.GET("/Playground/product-apples");
            });

            /* -- */
            Handle.GET("/Playground/product-apples", () =>
            {
                return Db.Scope<IndexPage>(() =>
                {
                    IndexPage page = new IndexPage();

                    page.Product.Data = GetProduct();

                    return page;
                });
            });
            /* -- */

            /* -- */
            Handle.GET("/Playground/product-oranges", () =>
            {
                return Db.Scope<IndexPage>(() =>
                {
                    IndexPage page = new IndexPage();

                    page.Product.Data = GetProduct();

                    return page;
                });
            });
            /* -- */

            Blender.MapUri(typeof(Product).FullName, "/Playground/product-apples");
            Blender.MapUri(typeof(Product).FullName, "/Playground/product-oranges");
        }

        static Product GetProduct()
        {
            Product p = Db.SQL<Product>($"SELECT p FROM {typeof(Product)} p").FirstOrDefault();

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