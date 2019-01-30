using System;
using System.Linq;
using System.Collections.Generic;
using Starcounter;
using Starcounter.Linq;
using Chatter.Database;

namespace Chatter
{
    partial class UserPage : Json
    {
        public UserPage(ulong userNo)
        {
            this.User.Data = Db.FromId<User>(userNo) ?? throw new ArgumentException(nameof(userNo));
            this.RefreshMessages();

            User user = this.User.Data;
            string sessionId = Session.Current.SessionId;

            Db.Transact(() => 
            {
                UserSession userSession = DbLinq.Objects<UserSession>().FirstOrDefault(x => x.User == user && x.SessionId == sessionId);

                if (userSession == null)
                {
                    userSession = new UserSession()
                    {
                        User = user,
                        SessionId = sessionId
                    };
                }

                userSession.LastAccessDateUtc = DateTime.UtcNow;
            });
        }

        public void RefreshMessages()
        {
            User user = this.User.Data;

            this.Messages.Clear();
            this.Messages.Data = DbLinq.Objects<Message>().Where(x => x.Sender == user || x.Receiver == user).OrderByDescending(x => x.DateUtc).Take(100).ToArray();
        }

        public void NotifyOtherUser(User sender, User receiver)
        {
            this.NotifyOtherUser(sender);
            this.NotifyOtherUser(receiver);
        }

        public void NotifyOtherUser(User user)
        {
            if (user == null || user.Equals(this.User.Data))
            {
                return;
            }

            DateTime date = DateTime.UtcNow.AddHours(-1);
            string[] sessionIds = DbLinq.Objects<UserSession>().Where(x => x.User == user && x.LastAccessDateUtc >= date).ToArray().Select(x => x.SessionId).ToArray();

            foreach (string sessionId in sessionIds)
            {
                Session.RunTask(sessionId, (s, id) =>
                {
                    if (s == null)
                    {
                        return;
                    }

                    UserPage page = s.Store[nameof(Json)] as UserPage;

                    if (page != null)
                    {
                        page.RefreshMessages();
                        s.CalculatePatchAndPushOnWebSocket();
                    }
                });
            }
        }

        [UserPage_json.User]
        partial class UserPage_json_User : Json, IBound<User>
        {
        }

        [UserPage_json.Messages]
        partial class UserPage_json_Messages : Json, IBound<Message>
        {
            public UserPage ParentPage => this.Parent.Parent as UserPage;
            public string DateUtcStr => this.Data?.DateUtc.ToString();

            protected void Handle(Input.DeleteTrigger action)
            {
                User sender = this.Data.Sender;
                User receiver = this.Data.Receiver;

                Db.Transact(() =>
                {
                    this.Data.Delete();
                });

                this.ParentPage.NotifyOtherUser(sender, receiver);
                this.ParentPage.RefreshMessages();
            }
        }

        [UserPage_json.Send]
        partial class UserPage_json_Send : Json
        {
            public UserPage ParentPage => this.Parent as UserPage;

            protected void Handle(Input.SubmitTrigger action)
            {
                action.Cancel();

                bool sent = false;
                User receiver = null;

                Db.Transact(() =>
                {
                    receiver = DbLinq.Objects<User>().FirstOrDefault(x => x.Name == this.Username);

                    if (receiver == null)
                    {
                        return;
                    }

                    new Message()
                    {
                        Sender = this.ParentPage.User.Data,
                        Receiver = receiver,
                        DateUtc = DateTime.UtcNow,
                        Text = this.Text
                    };

                    sent = true;
                });

                if (!sent)
                {
                    return;
                }

                this.Text = null;
                this.ParentPage.RefreshMessages();
                this.ParentPage.NotifyOtherUser(receiver);
            }
        }
    }
}
