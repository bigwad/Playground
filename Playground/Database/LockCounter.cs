using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Database {
    public class LockCounter {
        private static object locker = new object();
        private static ulong value = 1;
        private static Dictionary<string, ulong> values = new Dictionary<string, ulong>();

        public static ulong GetNextValue() {
            lock (locker) {
                return value++;
            }
        }

        public static ulong GetNextValue(string CounterName) {
            lock (locker) {
                if (!values.ContainsKey(CounterName)) {
                    values.Add(CounterName, 1);
                }

                return values[CounterName]++;
            }
        }
    }
}
