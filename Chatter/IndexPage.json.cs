using System;
using System.Linq;
using System.Collections.Generic;
using Starcounter;
using Starcounter.Linq;
using Chatter.Database;

namespace Chatter
{
    partial class IndexPage : Json
    {
        protected void Handle(Input.SubmitTrigger action)
        {
            action.Cancel();

            if (string.IsNullOrEmpty(this.Username))
            {
                return;
            }

            ulong no = 0;

            Db.Transact(() =>
            {
                User user = DbLinq.Objects<User>().FirstOrDefault(x => x.Name == this.Username);

                if (user == null)
                {
                    user = new User()
                    {
                        Name = this.Username
                    };
                }

                no = user.GetObjectNo();
            });

            this.RedirectUrl = "/chatter/" + no;
        }
    }
}
