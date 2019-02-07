using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Playground.Nova.Models
{
    [Table("Items")]
    public partial class Item
    {
        public long Id { get; set; }
        public string Guid { get; set; }
        public DateTime Date { get; set; }
        public int Thread { get; set; }
        public int Index { get; set; }
    }
}
