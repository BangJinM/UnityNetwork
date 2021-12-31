namespace US.BT
{
    public class AndCompositeNode : CompositeNode
    {
        public AndCompositeNode() : base() { }

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
