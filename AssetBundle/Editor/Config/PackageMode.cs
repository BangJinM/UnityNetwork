namespace US
{
    public enum PackageMode
    {
        /// <summary>
        /// 孙子文件夹打为一个包
        /// </summary>
        SUB_DIR_FOREACH2PACKAGE = 1,
        /// <summary>
        /// 所有文件打为一个包
        /// </summary>
        All_FILE2PACKAGE,
        /// <summary>
        /// 一个文件一个包
        /// </summary>
        ONE_FILE2PACKAGE,
        /// <summary>
        /// 每个文件夹打成一个包
        /// </summary>
        SUB_DIR2PACKAGE,
        /// <summary>
        /// 每个场景达成一个包
        /// </summary>
        SCENE2PACKAGE,
        /// <summary>
        /// 引用超过2的达成一个包
        /// </summary>
        REF2PACKAGE,
    }
}