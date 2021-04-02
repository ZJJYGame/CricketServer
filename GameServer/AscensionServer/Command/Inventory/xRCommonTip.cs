using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    /// <summary>
    /// 全局的错误提示
    /// </summary>
    public class xRCommonTip
    {
        public const string xR_err_ReLogin = "数据验证失败请重新登录";
        public const string xR_err_Account = "账号错误";
        public const string xR_err_Verify = "数据验证错误";
        public const string xR_err_VerifyProp = "背包物品數量不匹配";
        public const string xR_err_VerifyCricket = "请检查小屋中的蛐蛐";
        public const string xR_err_VerifyAssets= "请检查个人资产";
        public const string xR_tip_InputSpreaCode = "邀请码输入成功请查收";
        public const string xR_err_VerifySpreaCode = "输入失败请确认邀请码";
        public const string xR_err_VerifyAward = "已领取该奖励";
        public const string xR_err_VerifyAwardType = "奖励领取错误";
        public const string xR_err_RemoveSkill = "没有可以删除的技能";
    }
}
