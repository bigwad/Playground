using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using Starcounter;

namespace Playground {
    public class ReviewCollection : IEnumerable<Review>, ICollection<Review> {
        public QueryResultRows<Review> Query { get; protected set; }
        public Cpu Cpu { get; protected set; }

        public ReviewCollection(Cpu Cpu) {
            this.Cpu = Cpu;
            this.Query = Db.SQL<Review>("SELECT r FROM Playground.Review r WHERE r.Cpu = ?", this.Cpu);
        }

        public IEnumerator<Review> GetEnumerator() {
            return this.Query.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.Query.GetEnumerator();
        }

        public int Count {
            get {
                return this.Query.Count();
            }
        }

        public bool IsReadOnly {
            get {
                return false;
            }
        }

        public void Add(Review item) {
            item.Cpu = this.Cpu;
        }

        public void Clear() {
            this.Query.ToList().ForEach(x => x.Delete());
        }

        public bool Contains(Review item) {
            return item.Cpu == this.Cpu;
        }

        public void CopyTo(Review[] array, int arrayIndex) {
            var items = this.Query.ToList();

            for (int i = arrayIndex; i < items.Count; i++) {
                array[i] = items[i];
            }
        }

        public bool Remove(Review item) {
            if (item.Cpu != this.Cpu) {
                return false;
            }

            item.Cpu = null;
            return true;
        }
    }
}
