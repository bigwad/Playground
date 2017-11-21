using System;
using Starcounter;

namespace Playground
{
    partial class IndexPage : Json
    {
        protected void Handle(Input.ScheduleExceptionTaskTrigger action)
        {
            action.Cancel();
            Session.RunTask(Session.Current.SessionId, (s, id) => { throw new Exception("ScheduleTaskTrigger"); });
        }

        protected void Handle(Input.ScheduleDateTimeTaskTrigger action)
        {
            action.Cancel();

            Session.RunTask(Session.Current.SessionId, (s, id) =>
            {
                this.DateTime = System.DateTime.Now.ToString();
                s.CalculatePatchAndPushOnWebSocket();
            });
        }
    }
}
