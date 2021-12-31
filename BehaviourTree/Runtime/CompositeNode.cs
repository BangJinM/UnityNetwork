using System.Collections.Generic;
using XNode;

namespace US.BT
{
    public class CompositeNode : TreeNode
    {
        [Output(dynamicPortList = true)] public List<TreeNode> mChildren;

        public CompositeNode() : base()
        {
            behaviourNodeType = BehaviourNodeType.CompositeNode;
            mChildren = new List<TreeNode>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        public void RemoveNode(TreeNode node)
        {
            mChildren.Remove(node);
        }

        /// <summary>
        /// 添加节点
        /// </summary>
        /// <param name="child"></param>
        public void AddChild(TreeNode child)
        {
            mChildren.Add(child);
        }

        /// <summary>
        /// 获取节点
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public TreeNode GetNodeByIndex(int index)
        {
            if (index < mChildren.Count)
                return mChildren[index];
            return null;
        }

        /// <summary>
        /// 获取节点数量
        /// </summary>
        /// <returns></returns>
        public int GetChildrenCount()
        {
            if (this.mChildren != null)
            {
                return this.mChildren.Count;
            }
            return 0;
        }
    }
}
