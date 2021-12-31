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
        SUB_DIR2PACKAGE,
        SCENE2PACKAGE,
        REF2PACKAGE,
    }
}