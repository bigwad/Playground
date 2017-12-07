using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Starcounter;

namespace Playground.Database
{
    [Database]
    public class Product
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        public decimal TotalCost
        {
            get
            {
                return this.Price * this.Quantity;
            }
        }
    }
}
