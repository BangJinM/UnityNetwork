namespace US.BT
{
    public class InverseDecoratorNode : DecoratorNode
    {
        public InverseDecoratorNode() : base() { }

        public override BTStatus OnExcute()
        {
            if (childNode == null)
                return BTStatus.Success;
            var status = childNode.Tick();
            var result = BTStatus.Running;
            switch (status)
            {
                case BTStatus.Success:
                    result = BTStatus.Failure;
                    break;
                case BTStatus.Failure:
                    result = BTStatus.Success;
                    break;
                default:
                    break;
            }
            return result;
        }
    }
}
