using System;
using Starcounter;

namespace Playground {
    class Program {
        static void Main() {
            Handle.GET("/playground", () => new IndexPage() { Session = new Session(SessionOptions.PatchVersioning), Data = null });
        }
    }
}