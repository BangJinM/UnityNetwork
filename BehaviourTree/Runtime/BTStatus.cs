
namespace US.BT
{
    public enum BTStatus
    {
        ///初始状态
        IDLE,
        ///运行
        Running,
        ///成功
        Success,
        ///失败
        Failure,
    }

    public enum BehaviourNodeType
    {
        /// <summary>
        /// 组合节点
        /// </summary>
        CompositeNode,
        /// <summary>
        /// 叶子节点
        /// </summary>
        LeafNode,
        /// <summary>
        /// 条件节点
        /// </summary>
        ConditionNode,
        /// <summary>
        /// 修饰节点
        /// </summary>
        DecoratorNode,
    }
}
