using System;
using Starcounter;
using Starcounter.Linq;
using Chatter.Database;

namespace Chatter
{
    class Program
    {
        const string UQ_User_Name = nameof(UQ_User_Name);
        const string IX_Message_Sender_Receiver_DateUtc = nameof(IX_Message_Sender_Receiver_DateUtc);
        const string UQ_UserSession_User_Session = nameof(UQ_UserSession_User_Session);

        static void Main()
        {
            EnsureIndexes();

            Application.Current.Use(new HtmlFromJsonProvider());
            Application.Current.Use(new PartialToStandaloneHtmlProvider());

            Db.Transact(() =>
            {
                DbLinq.Objects<UserSession>().DeleteAll();
            });

            Handle.GET("/chatter", () =>
            {
                Session.Ensure();
                IndexPage page = new IndexPage();
                Session.Current.Store[nameof(Json)] = page;
                return page;
            });

            Handle.GET("/chatter/{?}", (ulong no) =>
            {
                Session.Ensure();
                UserPage page = new UserPage(no);
                Session.Current.Store[nameof(Json)] = page;
                return page;
            });

            Handle.GET("/chatter/transient", () =>
            {
                Session.Ensure();
                TransientPage page = new TransientPage();
                Session.Current.Store[nameof(Json)] = page;
                return page;
            });
        }

        static void EnsureIndexes()
        {
            {
                Starcounter.Metadata.Index index = DbLinq.Objects<Starcounter.Metadata.Index>().FirstOrDefault(x => x.Name == UQ_User_Name);

                if (index == null)
                {
                    Db.SQL($"CREATE UNIQUE INDEX {UQ_User_Name} ON {typeof(User)} ({nameof(User.Name)})");
                }
            }

            {
                Starcounter.Metadata.Index index = DbLinq.Objects<Starcounter.Metadata.Index>().FirstOrDefault(x => x.Name == IX_Message_Sender_Receiver_DateUtc);

                if (index == null)
                {
                    Db.SQL($"CREATE INDEX {IX_Message_Sender_Receiver_DateUtc} ON {typeof(Message)} ({nameof(Message.Sender)}, {nameof(Message.Receiver)}, {nameof(Message.DateUtc)})");
                }
            }

            {
                Starcounter.Metadata.Index index = DbLinq.Objects<Starcounter.Metadata.Index>().FirstOrDefault(x => x.Name == UQ_UserSession_User_Session);

                if (index == null)
                {
                    Db.SQL($"CREATE UNIQUE INDEX {UQ_UserSession_User_Session} ON {typeof(UserSession)} ({nameof(UserSession.User)}, {nameof(UserSession.SessionId)})");
                }
            }
        }
    }
}