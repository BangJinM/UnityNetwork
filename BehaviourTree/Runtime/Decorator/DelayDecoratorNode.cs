using UnityEngine;

namespace US.BT
{
    public class DelayDecoratorNode : DecoratorNode
    {
        protected float curTime = 0;
        protected float maxTime = 0;

        public DelayDecoratorNode(float maxTime) : base()
        {
            this.maxTime = maxTime;
            this.curTime = 0;
        }

        public override BTStatus OnExcute()
        {
            while (curTime < maxTime)
            {
                BTStatus bTStatus = childNode.Tick();
                switch (bTStatus)
                {
                    case BTStatus.Success:

                        break;
                    case BTStatus.Failure:
                        curTime = 0;
                        return BTStatus.Failure;
                    case BTStatus.Running:
                        return BTStatus.Running;
                    default:
                        break;
                }
            }

            return BTStatus.Success;
        }
    }
}
