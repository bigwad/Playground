using System;
using Starcounter;

namespace Playground.Database {
    [Database]
    public class Item {
        public Item() {
            this.Date = DateTime.Now;
        }

        public DateTime Date { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }
    }
}
