using System;
using Starcounter;

namespace Subground {
    class Program {
        static void Main() {
            Application.Current.Use(new HtmlFromJsonProvider());
            Application.Current.Use(new PartialToStandaloneHtmlProvider());

            Hook<Simplified.Ring5.SystemUserSession>.CommitDelete += (sender, entity) => {
                Session.ForAll((s, id) => {
                    if (s == null || !(s.Data is SubPage)) {
                        return;
                    }

                    SubPage page = s.Data as SubPage;

                    page.Date = "CommitDelete: " + DateTime.Now.ToString();
                    s.CalculatePatchAndPushOnWebSocket();
                });
            };

            Hook<Simplified.Ring5.SystemUserSession>.CommitInsert += (sender, entity) => {
                Session.ForAll((s, id) => {
                    if (s == null || !(s.Data is SubPage)) {
                        return;
                    }

                    SubPage page = s.Data as SubPage;

                    page.Date = "CommitInsert: " + DateTime.Now.ToString();
                    s.CalculatePatchAndPushOnWebSocket();
                });
            };

            Hook<Simplified.Ring5.SystemUserSession>.CommitUpdate += (sender, entity) => {
                Session.ForAll((s, id) => {
                    if (s == null || !(s.Data is SubPage)) {
                        return;
                    }

                    SubPage page = s.Data as SubPage;

                    page.Date = "CommitUpdate: " + DateTime.Now.ToString();
                    s.CalculatePatchAndPushOnWebSocket();
                });
            };

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

            Handle.GET("/subground/user", () => {
                if (Session.Current == null) {
                    Session.Current = new Session(SessionOptions.PatchVersioning);
                }

                SubPage page = new SubPage() {
                    Html = "/Subground/UserPage.html",
                    Data = null,
                    Session = Session.Current
                };

                return page;
            });

            Handle.GET("/subground/anonymous", () => {
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
            UriMapping.Map("/subground/anonymous", UriMapping.MappingUriPrefix + "/playground-anonymous-page");
            UriMapping.Map("/subground/user", UriMapping.MappingUriPrefix + "/userform");
        }
    }
}