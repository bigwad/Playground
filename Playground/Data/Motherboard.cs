using System;
using System.Collections.Generic;
using System.Linq;
using Starcounter;

namespace Playground {
    [Database]
    public class Motherboard {
        public string Name;
        public string Socket;

        public IEnumerable<Cpu> MotherboardCpus {
            get {
                return Db.SQL<Cpu>("SELECT c.Cpu FROM Playground.MotherboardCpu c WHERE c.Motherboard = ?", this);
            }
        }

        public string MotherboardCpuNames {
            get {
                return string.Join(", ", this.MotherboardCpus.Select(x => x.Name).ToArray());
            }
        }
    }
}
