using System;
using Starcounter;

namespace Playground {
    [Database]
    public class Cpu {
        public string Name;
        public string Description;
        public string Socket;

        public ReviewCollection Reviews {
            get {
                return new ReviewCollection(this);
            }
        }
    }
}
