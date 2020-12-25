namespace AscensionProtocol
{
    /// <summary>
    ///对象数据参数码
    /// </summary>
    public enum ParameterCode : byte
    {
        HeartBeat = 0,
        ForcedOffline = 1,
        /// <summary>
        /// 服务器会话ID；
        /// </summary>
        ClientPeer=2,
        /// <summary>
        /// 包含账户信息以及设备号的数据
        /// </summary>
        UserInfo = 3,
        Token = 4,
        ///////////////////前16为系统预留////////////////////////
        #region 蟋蟀数据
        /// <summary>
        /// 蟋蟀数据
        /// </summary>
        Cricket=5,
        /// <summary>
        /// 蟋蟀属性
        /// </summary>
        CricketStatus = 6,
        /// <summary>
        /// 蟋蟀资质
        /// </summary>
        CricketAptitude = 7,
        /// <summary>
        /// 蟋蟀加点
        /// </summary>
        CricketPoint = 8,
        /// <summary>
        /// 角色所有蟋蟀
        /// </summary>
        RoleCricket =9,
        #endregion
        /// <summary>
        /// 玩家金币
        /// </summary>
        RoleAsset =10,
        User = 17,
        RoleStatus = 18,
        /// <summary>
        /// 角色状态的集合
        /// </summary>
        RoleStatusSet = 19,
        Role = 20,
        RoleInventory =21,
        RoleTask = 22,
        /// <summary>
        /// 背包使用物品
        /// </summary>
        UseItem=23,
        RoleExploration = 24,
        RoleExplorationUnlock  =25,
        RoleRank = 26,
        SpreaCode=27,
        RoleMatch =28,
    }
}
