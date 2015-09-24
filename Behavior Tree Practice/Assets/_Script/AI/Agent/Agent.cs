using UnityEngine;
using System.Collections;
using System.Collections.Generic;



namespace behavior
{
    public class Agent : MonoBehaviour
    {

        class BehaviorTreeStackItem_t
        {
            public BehaviorTreeTask bt;
            public TriggerMode triggerMode;
            public bool triggerByEvent;

            public BehaviorTreeStackItem_t(BehaviorTreeTask bt_, TriggerMode tm, bool bByEvent)
            {
                bt = bt_;
                triggerMode = tm;
                triggerByEvent = bByEvent;
            }
        }


        bool m_referencetree = false;

        BehaviorTreeTask m_currentBT;

        List<BehaviorTreeStackItem_t> m_btStack;
        List<BehaviorTreeStackItem_t> BTStack
        {
            get
            {
                if (m_btStack == null)
                {
                    m_btStack = new List<BehaviorTreeStackItem_t>();
                }

                return m_btStack;
            }
        }

        List<BehaviorTreeTask> m_behaviorTreeTasks;
        List<BehaviorTreeTask> BehaviorTreeTasks
        {
            get
            {
                if (m_behaviorTreeTasks == null)
                {
                    m_behaviorTreeTasks = new List<BehaviorTreeTask>();
                }

                return m_behaviorTreeTasks;
            }
        }



        public bool btload(string relativePath, bool bForce /*= false*/)
        {
            bool bOk = Workspace.Load(relativePath, bForce);

            if (bOk)
            {
                Workspace.RecordBTAgentMapping(relativePath, this);
            }

            return bOk;
        }

        public bool btload(string relativePath)
        {
            bool bOk = this.btload(relativePath, false);

            return bOk;
        }

        /**
        before set the found one as the current bt, 
        it aborts the current one and pushes it on the stack so that it will be continued later when the 
        new one is finished.
        */
        public void btsetcurrent(string relativePath)
        {
            _btsetcurrent(relativePath, TriggerMode.TM_Transfer, false);
        }

        public void btreferencetree(string relativePath)
        {
            this._btsetcurrent(relativePath, TriggerMode.TM_Return, false);
            this.m_referencetree = true;
        }

        private void _btsetcurrent(string relativePath, TriggerMode triggerMode, bool bByEvent)
        {
            bool bEmptyPath = string.IsNullOrEmpty(relativePath);

            if (!bEmptyPath)
            {
                bool bLoaded = Workspace.Load(relativePath);

                if (!bLoaded)
                {
                    string agentName = this.GetType().FullName;
                    agentName += "::";
                    agentName += this.name;

                    Debug.LogError(string.Format("{0} is not a valid loaded behavior tree of {1}", relativePath, agentName));

                    //Debug.Check(false);
                    //System.Diagnostics.Debug.WriteLine(string.Format("{0} is not a valid loaded behavior tree of {1}", relativePath, agentName));
                }
                else
                {
                    Workspace.RecordBTAgentMapping(relativePath, this);

                    if (this.m_currentBT != null)
                    {
                        //if trigger mode is 'return', just push the current bt 'oldBt' on the stack and do nothing more
                        //'oldBt' will be restored when the new triggered one ends
                        if (triggerMode == TriggerMode.TM_Return)
                        {
                            BehaviorTreeStackItem_t item = new BehaviorTreeStackItem_t(this.m_currentBT, triggerMode, bByEvent);
                            Debug.Check(this.BTStack.Count < 200, "recursive?");

                            this.BTStack.Add(item);
                        }
                        else if (triggerMode == TriggerMode.TM_Transfer)
                        {
                            //don't use the bt stack to restore, we just abort the current one.
                            //as the bt node has onenter/onexit, the abort can make them paired
                            //Debug.Check (this.m_currentBT.GetName() != relativePath);

                            this.m_currentBT.abort(this);
                            this.m_currentBT.reset(this);
                        }
                    }

                    //BehaviorTreeTask pTask = this.BehaviorTreeTasks.Find(delegate (BehaviorTreeTask task) {return task.GetName() == relativePath;});
                    BehaviorTreeTask pTask = null;
                    for (int i = 0; i < this.BehaviorTreeTasks.Count; ++i)
                    {
                        BehaviorTreeTask t = this.BehaviorTreeTasks[i];
                        if (t.GetName() == relativePath)
                        {
                            pTask = t;
                            break;
                        }
                    }

                    bool bRecursive = false;
                    if (pTask != null && this.BTStack.Count > 0)
                    {
                        //bRecursive = this.BTStack.FindIndex(delegate (BehaviorTreeStackItem_t item){return item.bt.GetName() == relativePath;}) > -1;
                        for (int i = 0; i < this.BTStack.Count; ++i)
                        {
                            BehaviorTreeStackItem_t item = this.BTStack[i];
                            if (item.bt.GetName() == relativePath)
                            {
                                bRecursive = true;
                                break;
                            }
                        }
                    }

                    if (pTask == null || bRecursive)
                    {
                        pTask = Workspace.CreateBehaviorTreeTask(relativePath);
                        Debug.Check(pTask != null);
                        this.BehaviorTreeTasks.Add(pTask);
                    }

                    this.m_currentBT = pTask;
                }
            }
        }



        public virtual EBTStatus btexec()
        {
            if (this.m_currentBT != null)
            {
                //the following might modify this.m_currentBT if the invoked function called btsetcurrent/FireEvent
                BehaviorTreeTask currentBT = this.m_currentBT;
                EBTStatus s = this.m_currentBT.exec(this);
                Debug.Check(s == EBTStatus.BT_RUNNING || currentBT == this.m_currentBT, "btsetcurrent/FireEvent is not allowed in the invoked function.");

                while (s != EBTStatus.BT_RUNNING)
                {
                    this.m_currentBT.reset(this);
                    if (this.BTStack.Count > 0)
                    {
                        //get the last one
                        BehaviorTreeStackItem_t lastOne = this.BTStack[this.BTStack.Count - 1];
                        this.m_currentBT = lastOne.bt;
                        this.BTStack.RemoveAt(this.BTStack.Count - 1);

                        if (lastOne.triggerMode == TriggerMode.TM_Return)
                        {
                            //string newBT = this.m_currentBT.GetName();
                            //LogManager.Log(this, newBT, EActionResult.EAR_none, LogMode.ELM_return);

                            if (!lastOne.triggerByEvent)
                            {
                                /*EBTStatus s0 = */
                                this.m_currentBT.resume(this, s);
                                s = this.m_currentBT.exec(this);
                            }
                        }
                        else
                        {
                            s = this.m_currentBT.exec(this);
                            break;
                        }

                    }
                    else
                    {
                        //this.m_currentBT = null;
                        break;
                    }
                }

                return s;
            }
            else
            {
                //behaviac.Debug.LogWarning("NO ACTIVE BT!\n");
            }

            return EBTStatus.BT_INVALID;
        }
        
        private EBTStatus btexec_()
        {

            EBTStatus s = this.btexec_();

            while (this.m_referencetree && s == EBTStatus.BT_RUNNING)
            {
                this.m_referencetree = false;
                s = this.btexec_();
            }

            return EBTStatus.BT_INVALID;
        }


    }
}