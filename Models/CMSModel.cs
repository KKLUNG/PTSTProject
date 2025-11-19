using System;
using System.Collections.Generic;

namespace PTSDProject.Models
{
    /// <summary>
    /// 菜單項目模型
    /// </summary>
    public class MenuItem
    {
        public MenuItem()
        {
            items = new List<MenuItem>();
        }

        /// <summary>
        /// 菜單 GUID
        /// </summary>
        public Guid MenuGuid { get; set; }

        /// <summary>
        /// 父菜單 GUID
        /// </summary>
        public Guid MenuPGuid { get; set; }

        /// <summary>
        /// 菜單標題
        /// </summary>
        public string MenuTitle { get; set; }

        /// <summary>
        /// 路徑（兼容舊版）
        /// </summary>
        public string path { get; set; }

        /// <summary>
        /// 文字（兼容舊版）
        /// </summary>
        public string text { get; set; }

        /// <summary>
        /// 菜單類型
        /// </summary>
        public string MenuType { get; set; }

        /// <summary>
        /// 圖標（小寫是為了前端）
        /// </summary>
        public string icon { get; set; }

        /// <summary>
        /// 路徑（新版）
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 自定義 URL
        /// </summary>
        public string MenuSelfUrl { get; set; }

        /// <summary>
        /// 幫助說明
        /// </summary>
        public string HelpRemark { get; set; }

        /// <summary>
        /// 幫助文檔
        /// </summary>
        public string HelpFiles { get; set; }

        /// <summary>
        /// 幫助視頻
        /// </summary>
        public string HelpVideo { get; set; }

        /// <summary>
        /// ACL 權限成功標識
        /// </summary>
        public string ACLSuccess { get; set; }

        /// <summary>
        /// 子菜單項目
        /// </summary>
        public List<MenuItem> items { get; set; }
    }

    /// <summary>
    /// 上傳信息
    /// </summary>
    public class UploadInfo
    {
        public string uploadPath { get; set; }
    }
}

