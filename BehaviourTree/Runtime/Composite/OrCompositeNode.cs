namespace US.BT
{
    public class OrCompositeNode : CompositeNode
    {
        public OrCompositeNode() : base() { }

        public override BTStatus OnExcute()
        {
            if (mChildren.Count <= 0)
                return BTStatus.Success;
            foreach (var child in mChildren)
            {
                if (child.Tick() == BTStatus.Success)
                {
                    return BTStatus.Success;
                }
            }
            return BTStatus.Failure;
        }
    }
}
