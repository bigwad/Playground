using System;
using Starcounter;

namespace Chatter.Database
{
    [Database]
    public class Message
    {
        public User Sender { get; set; }
        public User Receiver { get; set; }
        public DateTime DateUtc { get; set; }
        public string Text { get; set; }
    }
}
