
using System.Collections;
using System.Collections.Generic;

namespace behaviac
{
    public class Event : ConditionBase
    {

        protected override BehaviorTask createTask()
        {
            EventTask pTask = new EventTask();

            return pTask;
        }


        protected CMethodBase m_event;

        //an event can be configured to stop being checked if triggered
        protected bool m_bTriggeredOnce;


        // ============================================================================
        public class EventTask : AttachmentTask
        {

            public bool TriggeredOnce()
            {
                Event pEventNode = this.GetNode() as Event;
                return pEventNode.m_bTriggeredOnce;
            }

            public string GetEventName()
            {
                Event pEventNode = this.GetNode() as Event;
                return pEventNode.m_event.Name;
            }

        }
    }

}