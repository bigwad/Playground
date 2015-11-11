using System;
using Starcounter;

namespace Playground {
    class Program {
        static void Main() {
            Handle.GET("/playground", () => new IndexPage() {
                Session = new Session(SessionOptions.PatchVersioning),
                Data = null
            });

            Handle.GET("/playground/sub", () => new SubPage() {
                Data = null
            });

            Handle.GET("/playground/ractive", () => new RactivePage() {
                Session = new Session(SessionOptions.PatchVersioning),
                Data = null
            });

            Handle.GET("/playground/ractive/{?}", (string value) => new RactiveSubPage() {
                Name = value.Split(".".ToCharArray())[0],
                Price = decimal.Parse(value.Split(".".ToCharArray())[1])
            });
        }
    }
}