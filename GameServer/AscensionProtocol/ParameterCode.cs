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

        User = 17,
        RoleStatus = 18,
        /// <summary>
        /// 角色状态的集合
        /// </summary>
        RoleStatusSet = 19,
        Role = 20,
    }
}
