using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Database {
    public class LockCounter {
        private static object locker = new object();
        private static ulong value = 1;

        public static ulong GetNextValue() {
            lock (locker) {
                return value++;
            }
        }
    }
}
