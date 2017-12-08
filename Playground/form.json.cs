using System;
using Starcounter;
using Database;

namespace A
{
    partial class form : Json
    {
    }

    [form_json.P]
    partial class formP : Json, IBound<P>
    {
        protected void Handle(Input.v action)
        {
            this.Data.v = action.Value;
            this.Transaction.Commit();
        }
    }
}
