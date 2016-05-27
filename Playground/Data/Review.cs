using System;
using Starcounter;

namespace Playground {
    [Database]
    public class Review {
        public string Name { get; set; }
        public int Score { get; set; }
        public Cpu Cpu { get; set; }
    }
}
