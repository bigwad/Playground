using System;
using System.Text;
using System.Diagnostics;
using System.Linq;
using Starcounter;
using Playground.Database;

namespace Playground {
    class Program {
        static void Main() {
            Hook<Item>.BeforeDelete += (sender, entity) => {
                Session.ForAll((s, id) => {
                    if (s == null || !(s.Data is IndexPage)) {
                        return;
                    }

                    IndexPage page = s.Data as IndexPage;

                    page.MessageText = "Item " + entity.Name + " removed!";
                    s.CalculatePatchAndPushOnWebSocket();
                });
            };

            Hook<Item>.CommitDelete += (sender, entity) => {
                Session.ForAll((s, id) => {
                    if (s == null || !(s.Data is IndexPage))
                    {
                        return;
                    }

                    IndexPage page = s.Data as IndexPage;

                    page.MessageText = "Item " + entity + " removed!";
                    s.CalculatePatchAndPushOnWebSocket();
                });
            };

            Hook<Item>.CommitInsert += (sender, entity) => {
                Session.ForAll((s, id) => {
                    if (s == null || !(s.Data is IndexPage)) {
                        return;
                    }

                    IndexPage page = s.Data as IndexPage;

                    page.MessageText = "Item " + entity.Name + " inserted!";
                    page.LastItem.Data = entity;
                    s.CalculatePatchAndPushOnWebSocket();
                });
            };

            Hook<Item>.CommitUpdate += (sender, entity) => {
                Session.ForAll((s, id) => {
                    if (s == null || !(s.Data is IndexPage)) {
                        return;
                    }

                    IndexPage page = s.Data as IndexPage;

                    page.MessageText = "Item " + entity.Name + " updated!";
                    page.LastItem.Data = entity;
                    s.CalculatePatchAndPushOnWebSocket();
                });
            };

            Handle.GET("/playground", () => {
                if (Session.Current == null) {
                    Session.Current = new Session(SessionOptions.PatchVersioning);
                }

                return Db.Scope<IndexPage>(() => new IndexPage() {
                    Session = Session.Current,
                    Data = null
                });
            });

            Handle.GET("/playground/reset", () => {
                if (Session.Current != null && Session.Current.Data is IndexPage) {
                    IndexPage page = Session.Current.Data as IndexPage;

                    page.Data = null;

                    return page;
                } else {
                    return Self.GET("/playground");
                }
            });

            Handle.GET("/playground/change", () => {
                if (Session.Current != null && Session.Current.Data is IndexPage) {
                    IndexPage page = Session.Current.Data as IndexPage;

                    page.MessageText = "/playground/change was opened";

                    return page;
                } else {
                    return Self.GET("/playground");
                }
            });

            Handle.GET("/playground/sub", () => new SubPage() {
                Data = null
            });

            Handle.GET("/playground/details/{?}", (string id) => new SubDetailsPage() {
                Data = id
            });

            Handle.GET("/playground/ractive", () => new RactivePage() {
                Session = new Session(SessionOptions.PatchVersioning),
                Data = null
            });

            Handle.GET("/playground/ractive/{?}", (string value) => new RactiveSubPage() {
                Name = value.Split(".".ToCharArray())[0],
                Price = decimal.Parse(value.Split(".".ToCharArray())[1])
            });

            Handle.GET("/playground/insert", () => {
                Data data = new Data();

                data.Insert();

                return "Data inserted";
            });

            Handle.GET("/playground/delete", () => {
                Data data = new Data();

                data.Delete();

                return "Data deleted";
            });

            Handle.GET("/playground/benchmark/{?}", (int times) => {
                StringBuilder html = new StringBuilder();
                Stopwatch watch = new Stopwatch();

                html.Append("<p>Times: ").Append(times)
                    .Append("<br/>Motherboards: ").Append(Db.SQL("SELECT COUNT(m) FROM Playground.Motherboard m").First)
                    .Append("<br/>CPUs: ").Append(Db.SQL("SELECT COUNT(c) FROM Playground.Cpu c").First)
                    .Append("<br/>MotherboardCPUs: ").Append(Db.SQL("SELECT COUNT(c) FROM Playground.MotherboardCpu c").First)
                    .Append("</p>");

                //watch.Start();
                //for (int i = 0; i < times; i++) {
                //    var motherboards = Db.SQL<Motherboard>("SELECT m FROM Playground.Motherboard m WHERE m.MotherboardCpuNames LIKE ?", "%i7-5%").ToList();
                //}
                //watch.Stop();
                //html.Append("<p>").Append("MotherboardCpuNames LIKE: <b>").Append(watch.ElapsedMilliseconds).Append("</b> ms</p>");

                watch.Reset();
                watch.Start();
                for (int i = 0; i < times; i++) {
                    var motherboards = Db.SQL<Motherboard>("SELECT c.Motherboard FROM Playground.MotherboardCpu c WHERE c.Cpu.Name LIKE ? GROUP BY c.Motherboard", "%i7-5%").ToList();
                }
                watch.Stop();
                html.Append("<p>").Append("MotherboardCpu GROUP: <b>").Append(watch.ElapsedMilliseconds).Append("</b> ms</p>");

                watch.Reset();
                watch.Start();
                for (int i = 0; i < times; i++) {
                    var motherboards = Db.SQL<MotherboardCpu>("SELECT c FROM Playground.MotherboardCpu c WHERE c.Cpu.Name LIKE ?", "%i7-5%").Select(x => x.Motherboard).Distinct().ToList();
                }
                watch.Stop();
                html.Append("<p>").Append("MotherboardCpu Distinct: <b>").Append(watch.ElapsedMilliseconds).Append("</b> ms</p>");

                watch.Reset();
                watch.Start();
                for (int i = 0; i < times; i++) {
                    var motherboards = Db.SQL<Motherboard>("SELECT c.Motherboard FROM Playground.MotherboardCpu c WHERE c.Cpu.Name LIKE ?", "%i7-5%").Distinct().ToList();
                }
                watch.Stop();
                html.Append("<p>").Append("Motherboard Distinct: <b>").Append(watch.ElapsedMilliseconds).Append("</b> ms</p>");

                //watch.Reset();
                //watch.Start();
                //for (int i = 0; i < times; i++) {
                //    var motherboards = Db.SQL<Motherboard>("SELECT m FROM Playground.Motherboard m").Where(x => x.MotherboardCpus.Any(c => c.Name.Contains("i7-5"))).ToList();
                //}
                //watch.Stop();
                //html.Append("<p>").Append("MotherboardCpu Any Contains: <b>").Append(watch.ElapsedMilliseconds).Append("</b> ms</p>");

                return html.ToString();
            });
        }
    }
}