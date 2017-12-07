using System;
using Starcounter;

namespace Playground
{
    partial class IndexPage : Json
    {
        protected void Handle(Input.SaveTrigger action)
        {
            action.Cancel();
            this.Transaction.Commit();
        }

        protected void Handle(Input.CancelTrigger action)
        {
            action.Cancel();
            this.Transaction.Rollback();
        }
    }
}
