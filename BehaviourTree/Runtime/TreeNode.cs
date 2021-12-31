using XNode;

namespace US.BT
{
    public abstract class TreeNode : Node
    {
        static int static_tree_node_id = 0;
        static int GetTreeNodeID()
        {
            static_tree_node_id++;
            return static_tree_node_id;
        }

        /// <summary>
        /// 运行状态
        /// </summary>
        protected BTStatus btStatus = BTStatus.IDLE;
        /// <summary>
        /// 节点名字
        /// </summary>
        protected string nodeName = string.Empty;
        /// <summary>
        /// 节点ID
        /// </summary>
        protected int nodeID = -1;
        /// <summary>
        /// 节点类型
        /// </summary>
        protected BehaviourNodeType behaviourNodeType;

        [Input] public TreeNode parentNode;

        public TreeNode()
        {
            nodeName = GetNodeName();
            nodeID = GetTreeNodeID();
        }

        public string GetNodeName()
        {
            return GetType().Name;
        }

        /// <summary>
        /// 进入当前节点
        /// </summary>
        public virtual void OnEnter()
        {
            btStatus = BTStatus.Running;
        }

        /// <summary>
        /// 执行当前节点
        /// </summary>
        /// <returns></returns>
        public virtual BTStatus OnExcute()
        {
            return BTStatus.Success;
        }

        /// <summary>
        /// 推出当前节点
        /// </summary>
        public virtual void OnLeave()
        {
            btStatus = BTStatus.IDLE;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <returns></returns>
        public virtual BTStatus Tick()
        {
            BTStatus status;
            if (btStatus == BTStatus.IDLE)
                OnEnter();
            status = OnExcute();
            if (btStatus != BTStatus.Running)
                OnLeave();
            return status;
        }

        public override object GetValue(NodePort port)
        {
            return this;
        }
    }
}
