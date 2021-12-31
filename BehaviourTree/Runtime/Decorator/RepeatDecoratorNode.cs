namespace US.BT
{
    public class RepeatDecoratorNode : DecoratorNode
    {
        protected int numCycles = 1;
        protected int tryIndex = 0;

        public RepeatDecoratorNode(int tryCount) : base()
        {
            numCycles = tryCount;
            tryIndex = 0;
        }

        public override BTStatus OnExcute()
        {
            while (tryIndex < numCycles)
            {
                BTStatus bTStatus = childNode.Tick();
                switch (bTStatus)
                {
                    case BTStatus.Success:
                        tryIndex++;
                        break;
                    case BTStatus.Failure:
                        tryIndex = 0;
                        return BTStatus.Failure;
                    case BTStatus.Running:
                        return BTStatus.Running;
                    default:
                        break;
                }
            }

            tryIndex = 0;
            return BTStatus.Success;
        }
    }
}

