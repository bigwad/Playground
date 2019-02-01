using System;
using Starcounter;

namespace Chatter
{
    partial class TransientPage : Json
    {
        protected void Handle(Input.SendTrigger action)
        {
            action.Cancel();

            string username = this.Username;
            string text = this.Text;

            Session.RunTaskForAll((s, id) =>
            {
                if (s == null || !s.IsAlive())
                {
                    return;
                }

                TransientPage page = s.Store[nameof(Json)] as TransientPage;

                if (page == null)
                {
                    return;
                }

                var m = page.Messages.Add();

                m.Username = username;
                m.Text = text;

                s.CalculatePatchAndPushOnWebSocket();
            });

            this.Text = null;
        }
    }
}
