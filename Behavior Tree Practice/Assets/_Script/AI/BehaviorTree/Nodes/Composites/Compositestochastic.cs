
using System;
using System.Collections;
using System.Collections.Generic;

namespace behaviac
{
    public abstract class CompositeStochastic : BehaviorNode
    {
        public CompositeStochastic()
        {
		}

        ~CompositeStochastic()
        {
            m_method = null;
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);

            foreach (property_t p in properties)
            {
                if (p.name == "RandomGenerator")
                {
                    if (p.value[0] != '\0')
                    {
                        this.m_method = Action.LoadMethod(p.value);
                    }//if (p.value[0] != '\0')
                }
                else
                {
                    //Debug.Check(0, "unrecognised property %s", p.name);
                }
            }
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            if (!(pTask.GetNode() is CompositeStochastic))
            {
                return false;
            }

            return base.IsValid(pAgent, pTask);
        }

        protected CMethodBase m_method;

        public class CompositeStochasticTask : CompositeTask
        {
            public CompositeStochasticTask()
            {
			}

            ~CompositeStochasticTask()
            {
            }

			//generate a random float value between 0 and 1.
            public static float GetRandomValue(CMethodBase method, Agent pAgent)
            {
				float value = 0;
				if (method != null)
                {
					value = (float)method.Invoke(pAgent);
                }
                else
                {
					value = RandomGenerator.GetInstance().GetRandom();
                }

				Debug.Check(value >= 0.0f && value < 1.0f);
				return value;
			}

            protected override bool onenter(Agent pAgent)
            {
                Debug.Check(this.m_children.Count > 0);

                this.random_child(pAgent);

                this.m_activeChildIndex = 0;
                return true;
            }

            protected override void onexit(Agent pAgent, EBTStatus s)
            {
            }

            private void random_child(Agent pAgent)
            {
                Debug.Check(this.GetNode() == null || this.GetNode() is CompositeStochastic);
                CompositeStochastic pNode = (CompositeStochastic)(this.GetNode());

				int n = this.m_children.Count;
                if (this.m_set.Count != n)
                {
					this.m_set.Clear();

					for (int i = 0; i < n; ++i)
					{
						this.m_set.Add(i);
					}
                }

                for (int i = 0; i < n; ++i)
                {
					int index1 = (int)(n * GetRandomValue(pNode != null ? pNode.m_method : null, pAgent));
                    Debug.Check(index1 < n);

					int index2 = (int)(n * GetRandomValue(pNode != null ? pNode.m_method : null, pAgent));
                    Debug.Check(index2 < n);

                    //swap
                    if (index1 != index2)
                    {
                        int old = this.m_set[index1];
                        this.m_set[index1] = this.m_set[index2];
                        this.m_set[index2] = old;
                    }
                }
            }

            protected List<int> m_set = new List<int>();
        }
    }
}
