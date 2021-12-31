namespace US.BT
{
    public class SequenceCompositeNode : CompositeNode
    {
        public SequenceCompositeNode() : base() { }

        public override BTStatus OnExcute()
        {
            if (mChildren.Count <= 0)
                return BTStatus.Success;
            foreach (var child in mChildren)
            {
                if (child.Tick() != BTStatus.Success)
                {
                    return BTStatus.Failure;
                }
            }
            return BTStatus.Success;
        }
    }
}
