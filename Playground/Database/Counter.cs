using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Starcounter;

namespace Playground.Database {
    [Database]
    public class Counter {
        static Counter() {
            if (Db.SQL(@"SELECT i FROM Starcounter.Metadata.""Index"" i WHERE Name = ?", "UK_Counter_Name").First == null) {
                Db.SQL(@"CREATE UNIQUE INDEX UK_Counter_Name ON Playground.Database.Counter (Name ASC)");
            }
        }

        public string Name { get; set; }
        public ulong Value { get; set; }

        public static ulong GetNextValue(string CounterName) {
            ulong value = Db.Transact<ulong>(() => {
                Counter counter = Db.SQL<Counter>("SELECT c FROM Playground.Database.Counter c WHERE c.Name = ?", CounterName).First;

                if (counter == null) {
                    counter = new Counter() {
                        Name = CounterName,
                        Value = 0
                    };
                }

                counter.Value++;

                return counter.Value;
            });

            return value;
        }
    }
}
