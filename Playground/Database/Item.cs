using System;
using System.Linq;
using Starcounter;

namespace Playground.Database {
    [Database]
    public class Item {
        public Item() {
            int? max = Db.SQL<Item>("SELECT i FROM Playground.Database.Item i").Max(x => (int?)x.Number);

            this.Date = DateTime.Now;
            this.Number = max ?? 1;
        }

        public DateTime Date { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }
    }
}
