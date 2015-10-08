
using System;
using System.Collections;
using System.Collections.Generic;

namespace behaviac
{
    public enum EComputeOperator
    {
        E_INVALID,
        E_ADD,
        E_SUB,
        E_MUL,
        E_DIV
    }

    public class Compute : BehaviorNode
    {
        protected override BehaviorTask createTask()
        {
            return new ComputeTask();
        }



        class ComputeTask : LeafTask
        {
            public ComputeTask()
            {
            }

            ~ComputeTask()
            {
            }
        }

    }
}
