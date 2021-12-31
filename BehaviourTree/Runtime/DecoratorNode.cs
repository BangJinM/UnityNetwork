using XNode;

namespace US.BT
{
    public class DecoratorNode : TreeNode
    {
        [Output] public TreeNode childNode;

        public DecoratorNode() : base()
        {
            behaviourNodeType = BehaviourNodeType.DecoratorNode;
        }

        /// <summary>
        /// 添加修饰节点
        /// </summary>
        /// <param name="treeNode"></param>
        public void AddDecoratorNode(TreeNode treeNode)
        {
            childNode = treeNode;
        }

        /// <summary>
        /// 移除修饰节点
        /// </summary>
        public void RemoveDecoratorNode()
        {
            childNode = null;
        }
    }
}
