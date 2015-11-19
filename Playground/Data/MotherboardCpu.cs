using System;
using Starcounter;

namespace Playground {
    [Database]
    public class MotherboardCpu {
        public Motherboard Motherboard;
        public Cpu Cpu;
    }
}
