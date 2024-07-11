using System.ComponentModel;

namespace ProgramGuard.Enums
{
    /// <summary>
    /// 操作功能權限
    /// </summary>
    [Flags]
    public enum OPERATE_PRIVILEGE : uint
    {
        [Description("預設值, 代表無權限")]
        UNKNOWN = 0,

        [Description("新增檔案")]
        ADD_FILELIST = 1,

        [Description("編輯檔案")]
        MODIFY_FILELIST = 2,

        [Description("刪除檔案")]
        DELETE_FILELIST = 4,

        [Description("檔案異動確認")]
        CONFIRM_CHANGE_LOG = 8,

        [Description("新增帳號")]
        ADD_ACCOUNT = 16,

        [Description("刪除帳號")]
        DELETE_ACCOUNT = 32,

        [Description("啟用帳號")]
        ENABLE_ACCOUNT = 64,

        [Description("停用帳號")]
        DISABLE_ACCOUNT = 128,

        [Description("變更帳號權限")]
        MODIFY_PRIVILEGE = 256,

        [Description("重置其他人密碼")]
        RESET_PASSWORD = 512,

        [Description("調整權限規則")]
        MODIFY_PRIVILEGE_RULE = 1024,

    }
}
