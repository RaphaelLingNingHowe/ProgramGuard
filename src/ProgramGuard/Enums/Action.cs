using System.ComponentModel;

namespace ProgramGuard.Enums
{
    public enum ACTION : ushort
    {
        [Description("不明")]
        UNKNOWN = 0,

        [Description("登入")]
        LOGIN = 0001,

        [Description("登出")]
        LOGOUT = 0002,

        [Description("更換密碼")]
        CHANGE_PASSWORD = 0003,

        // 檔案清單
        [Description("[進入] 檔案清單")]
        ACCESS_FILELIST_PAGE = 1000,

        [Description("新增檔案")]
        ADD_FILELIST = 1001,

        [Description("編輯檔案")]
        MODIFY_FILELIST = 1002,

        [Description("刪除檔案")]
        DELETE_FILELIST = 1003,

        // 檔案異動
        [Description("[進入] 檔案異動")]
        ACCESS_CHANGE_LOG_PAGE = 2000,

        [Description("檔案異動確認")]
        CONFIRM_CHANGE_LOG = 2001,

        [Description("查閱異動紀錄")]
        VIEW_CHANGE_LOG = 2002,

        // 帳號管理
        [Description("[進入] 帳號管理")]
        ACCESS_MANAGER_PAGE = 3000,

        [Description("新增帳號")]
        ADD_ACCOUNT = 3001,

        [Description("刪除帳號")]
        DELETE_ACCOUNT = 3002,

        [Description("啟用帳號")]
        ENABLE_ACCOUNT = 3003,

        [Description("停用帳號")]
        DISABLE_ACCOUNT = 3004,

        [Description("解鎖帳號")]
        UNLOCK_ACCOUNT = 3005,

        [Description("重置其他人密碼")]
        RESET_PASSWORD = 3006,

        [Description("變更帳號權限")]
        MODIFY_PRIVILEGE = 3007,

        // 權限管理
        [Description("[進入] 權限管理")]
        ACCESS_PRIVILEGE_PAGE = 4001,

        [Description("新增權限規則")]
        ADD_PRIVILEGE_RULE = 4002,

        [Description("編輯權限規則")]
        MODIFY_PRIVILEGE_RULE = 4003,

        [Description("刪除權限規則")]
        DELETE_PRIVILEGE_RULE = 4004,

        // 操作紀錄
        [Description("[進入] 操作紀錄")]
        ACCESS_ACTION_LOG_PAGE = 5000,

        [Description("查閱操作紀錄")]
        VIEW_ACTION_LOG = 5001,
    }
}
