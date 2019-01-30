using System;
using Starcounter;

namespace Chatter.Database
{
    [Database]
    public class UserSession
    {
        public User User { get; set; }
        public string SessionId { get; set; }
        public DateTime LastAccessDateUtc { get; set; }
    }
}
