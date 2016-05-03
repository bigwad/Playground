using System;
using Starcounter;

namespace Subground {
    class Program {
        static void Main() {
            Handle.GET("/subground/sub", () => {
                if (Session.Current == null) {
                    Session.Current = new Session(SessionOptions.PatchVersioning);
                }

                SubPage page = new SubPage() {
                    Data = null,
                    Session = Session.Current
                };

                return page;
            });

            UriMapping.Map("/subground/sub", UriMapping.MappingUriPrefix + "/playground-sub-page");
        }
    }
}