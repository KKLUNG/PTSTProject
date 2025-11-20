# PTSD.Utility 工具類別使用說明

> **建立日期**: 2025-11-20  
> **來源**: Bia 專案 Bia.Utility.cs  
> **狀態**: ✅ 已完成 (排除 PDF 浮水印功能)

---

## 📋 目錄

1. [功能概述](#功能概述)
2. [列舉定義](#列舉定義)
3. [功能分類](#功能分類)
4. [使用範例](#使用範例)
5. [注意事項](#注意事項)

---

## 功能概述

`PTSD.Utility` 提供以下核心功能：

- ✅ **權限管理列舉** (ACLID)
- ✅ **工作流列舉** (FlowStatus, FlowMemberType, FlowStepType)
- ✅ **JSON/DataTable 檢查**
- ✅ **資料轉換** (DataTable → SQL INSERT)
- ✅ **多語系處理**
- ✅ **HTML 處理** (去除標籤、轉純文字)
- ✅ **DataSet 處理** (移除時區)
- ✅ **JSON 驗證**
- ✅ **檔案加密/解密** (AES-256)
- ✅ **金額轉英文文字**
- ❌ **PDF 浮水印** (已排除)

---

## 列舉定義

### 1. ACLID (權限識別碼)

```csharp
public enum ACLID
{
    NONE = 0,       // 無權限
    SHOW = 1,       // 顯示
    READ = 2,       // 瀏覽
    EXPORT = 4,     // 匯出
    INSERT = 8,     // 新增
    UPDATE = 16,    // 修改
    DELETE = 32     // 刪除
}
```

**使用範例**:
```csharp
// 檢查使用者是否有新增權限
if ((userACL & (int)Utility.ACLID.INSERT) > 0)
{
    // 允許新增
}

// 組合多個權限
int fullPermission = (int)Utility.ACLID.READ | 
                     (int)Utility.ACLID.INSERT | 
                     (int)Utility.ACLID.UPDATE;
```

---

### 2. FlowStatus (工作流狀態)

```csharp
public enum FlowStatus
{
    A_ActionStart = 0,                  // 待處理
    H_Handled_Approve = 1,              // 送件
    C_Handled_Reject = 2,               // 退件
    Z_NotHandle_UserWithDrawn = 3,      // 申請者撤回
    // ... 共 26 種狀態
}
```

**主要狀態說明**:
- `A_ActionStart`: 待處理 (新簽核任務)
- `H_Handled_Approve`: 已核准送件
- `C_Handled_Reject`: 已退件
- `Y_IsEnd`: 流程結束

---

### 3. FlowMemberType (工作流成員類型)

```csharp
public enum FlowMemberType
{
    User = 1,                           // 使用者
    DeptAdmin = 2,                      // 某部門主管
    Group = 6,                          // 群組成員
    DeptMember = 7,                     // 某部門成員
    // ... 等
}
```

---

### 4. FlowStepType (工作流步驟類型)

```csharp
public enum FlowStepType
{
    Serial = 1,         // 串簽
    Parallel = 4,       // 並簽
    Notify = 7,         // 核定通知
    Inform = 8,         // 知會
}
```

---

## 功能分類

### 1. JSON/DataTable 欄位檢查

#### CheckObjectKey() - 檢查欄位是否存在

```csharp
// 檢查 JSON 是否包含 Key
System.Text.Json.JsonElement jsonObj = /* ... */;
bool hasKey = Utility.CheckObjectKey(jsonObj, "UserGuid");

// 檢查 DataTable 是否包含欄位
DataTable dt = /* ... */;
bool hasColumn = Utility.CheckObjectKey(dt, "UserName");

// 檢查 Dictionary 是否包含 Key
Dictionary<string, string> dict = /* ... */;
bool hasKey = Utility.CheckObjectKey(dict, "Email");
```

#### CheckColumnExist() - 檢查 DataTable 欄位

```csharp
DataTable dt = /* ... */;
if (Utility.CheckColumnExist("CreatedDate", dt))
{
    // 欄位存在
}
```

---

### 2. 資料轉換 (DataTable → SQL)

#### GetSQL() - 產生 INSERT 語句

```csharp
DataTable dt = new DataTable();
dt.Columns.Add("UserGuid", typeof(Guid));
dt.Columns.Add("UserName", typeof(string));
dt.Columns.Add("CreatedDate", typeof(DateTime));

// 加入資料
DataRow row = dt.NewRow();
row["UserGuid"] = Guid.NewGuid();
row["UserName"] = "John Doe";
row["CreatedDate"] = DateTime.Now;
dt.Rows.Add(row);

// 產生 SQL
string sql = Utility.GetSQL("CMSUser", dt);

// 輸出:
// INSERT INTO CMSUser (UserGuid, UserName, CreatedDate) 
// VALUES (N'xxx-xxx-xxx', N'John Doe', '2025-11-20 10:30:00');
```

**特殊處理**:
- `CreatedDate`, `ModifiedDate` → 自動格式化為 ISO 8601
- `CreatedUserID`, `ModifiedUserID` → 空值轉為 `null`
- 一般字串 → 自動跳脫單引號 (`'` → `''`)

---

### 3. 多語系處理

#### GetCaption() - 取得多語系標題

```csharp
string defaultTitle = "使用者管理";
string langJson = "{\"enUS\":\"User Management\",\"vi\":\"Quản lý người dùng\"}";
string language = "enUS";

string title = Utility.GetCaption(defaultTitle, langJson, language);
// 輸出: "User Management"
```

**語言代碼**:
- `zhTW`: 繁體中文 (預設)
- `enUS`: 英文
- `vi`: 越南文
- `th`: 泰文
- `id`: 印尼文
- `zhCN`: 簡體中文

---

### 4. HTML 處理

#### StripTags() - 移除 HTML 標籤

```csharp
string html = "<div><p>Hello <strong>World</strong></p></div>";
string[] reservedTags = { "strong" }; // 保留 <strong>

string result = Utility.StripTags(html, reservedTags);
// 輸出: "Hello <strong>World</strong>"
```

#### ReplaceHtml() - HTML 轉純文字

```csharp
string html = "<p>第一段</p><br/><p>第二段</p>";
string plainText = Utility.ReplaceHtml(html);
// 輸出: 
// 第一段
// 
// 第二段
```

**支援轉換**:
- `<p>` → 移除
- `</p>` → 換行
- `<br>`, `<br/>`, `<br />` → 換行
- HTML 實體 → 解碼 (例如 `&nbsp;` → 空格)

#### EscapeValue() - 跳脫 SQL LIKE 特殊字元

```csharp
string searchText = "10%折扣[優惠]";
string escaped = Utility.EscapeValue(searchText);
// 輸出: "10[%]折扣[[]優惠[]]"

// SQL 查詢
string sql = $"SELECT * FROM Products WHERE Name LIKE '%{escaped}%'";
```

---

### 5. DataSet 處理

#### RemoveTimezoneForDataSet() - 移除時區

```csharp
DataSet ds = /* 從資料庫取得 */;
Utility.RemoveTimezoneForDataSet(ds);

// 避免 JSON 序列化時出現時區格式
string json = JsonConvert.SerializeObject(ds);
```

#### NF() - 數字格式化

```csharp
decimal amount = 1234567.89m;
string formatted = Utility.NF(amount, "N2");
// 輸出: "1,234,567.89"
```

---

### 6. JSON 驗證

#### IsValidJson() - 驗證 JSON 格式

```csharp
string jsonStr = "{\"name\":\"John\"}";
bool isValid = Utility.IsValidJson(jsonStr);
// 輸出: true

string invalidJson = "{name:John}"; // 缺少引號
bool isValid2 = Utility.IsValidJson(invalidJson);
// 輸出: false
```

---

### 7. 檔案處理

#### SaveFile() - 下載檔案

```csharp
string fileUrl = "https://example.com/document.pdf";
string savePath = "C:\\Temp\\document.pdf";

await Utility.SaveFile(fileUrl, savePath);
```

---

### 8. 金額轉英文文字

#### AmountToWords.GetWords() - 金額轉文字

```csharp
decimal amount = 1234.56m;
string words = Utility.AmountToWords.GetWords(amount);
// 輸出: "One Thousand Two Hundred Thirty Four And Cents Fifty Six Only."

decimal amount2 = 100m;
string words2 = Utility.AmountToWords.GetWords(amount2);
// 輸出: "One Hundred Only."
```

**支援範圍**:
- 整數部分: 0 ~ 999,999,999,999 (Billion)
- 小數部分: 2 位 (Cents)

---

### 9. 檔案加密/解密 (AES-256)

#### EncryptFile() - 加密檔案

```csharp
string sourceFile = @"C:\Documents\secret.docx";
string outputFolder = @"C:\Encrypted";

Utility.EncryptFile(sourceFile, outputFolder);
// 輸出: C:\Encrypted\secret.aes (包含原始副檔名資訊)
```

#### DecryptFile() - 解密檔案

```csharp
string encryptedFile = @"C:\Encrypted\secret.aes";
string outputFolder = @"C:\Decrypted";

Utility.DecryptFile(encryptedFile, outputFolder);
// 輸出: C:\Decrypted\secret.docx (自動還原副檔名)
```

**加密規格**:
- 演算法: AES-256
- 模式: CBC
- Key: 256-bit (從 EncryptKey 轉換)
- IV: 128-bit (從 EncryptKey 子字串取得)
- Buffer: 1 MB (大檔案友善)

**注意事項**:
- ⚠️ `EncryptKey` 是硬編碼在程式碼中，建議移到 `appsettings.json`
- ✅ 原始副檔名會被儲存在加密檔案開頭
- ✅ 支援任意大小的檔案 (串流處理)

---

## 使用範例

### 完整範例 1: 工作流狀態檢查

```csharp
using PTSDProject;

public class WorkflowService
{
    public bool CanApprove(int flowStatus)
    {
        // 只有待處理的任務可以核准
        return flowStatus == (int)Utility.FlowStatus.A_ActionStart;
    }

    public string GetStatusName(int flowStatus)
    {
        return flowStatus switch
        {
            (int)Utility.FlowStatus.A_ActionStart => "待處理",
            (int)Utility.FlowStatus.H_Handled_Approve => "已核准",
            (int)Utility.FlowStatus.C_Handled_Reject => "已退件",
            (int)Utility.FlowStatus.Y_IsEnd => "已結束",
            _ => "未知狀態"
        };
    }
}
```

---

### 完整範例 2: DataTable 匯出為 SQL

```csharp
public class DataExporter
{
    public string ExportToSQL(DataTable data, string tableName)
    {
        // 檢查必要欄位
        if (!Utility.CheckColumnExist("UserGuid", data))
        {
            throw new Exception("缺少 UserGuid 欄位");
        }

        // 移除時區資訊
        DataSet ds = new DataSet();
        ds.Tables.Add(data);
        Utility.RemoveTimezoneForDataSet(ds);

        // 產生 SQL
        string sql = Utility.GetSQL(tableName, data);
        
        return sql;
    }
}
```

---

### 完整範例 3: 多語系標題處理

```csharp
public class MenuService
{
    public string GetMenuTitle(string defaultTitle, string langJson, string userLang)
    {
        return Utility.GetCaption(defaultTitle, langJson, userLang);
    }

    // 範例資料
    public void Example()
    {
        string title = "系統管理";
        string langJson = @"{
            ""enUS"": ""System Admin"",
            ""vi"": ""Quản trị hệ thống""
        }";

        string zhTW = GetMenuTitle(title, langJson, "zhTW");  // "系統管理"
        string enUS = GetMenuTitle(title, langJson, "enUS");  // "System Admin"
        string vi = GetMenuTitle(title, langJson, "vi");      // "Quản trị hệ thống"
    }
}
```

---

### 完整範例 4: 檔案加密/解密

```csharp
public class FileSecurityService
{
    public void EncryptSensitiveFile(string filePath)
    {
        string encryptFolder = Path.Combine(
            Path.GetDirectoryName(filePath)!, 
            "Encrypted"
        );
        
        Directory.CreateDirectory(encryptFolder);
        Utility.EncryptFile(filePath, encryptFolder);
        
        // 刪除原始檔案 (可選)
        File.Delete(filePath);
    }

    public string DecryptAndGetPath(string encryptedFile)
    {
        string decryptFolder = Path.Combine(
            Path.GetDirectoryName(encryptedFile)!, 
            "Decrypted"
        );
        
        Directory.CreateDirectory(decryptFolder);
        Utility.DecryptFile(encryptedFile, decryptFolder);
        
        // 回傳解密後的檔案路徑
        string fileName = Path.GetFileNameWithoutExtension(encryptedFile);
        return Directory.GetFiles(decryptFolder, $"{fileName}.*")[0];
    }
}
```

---

### 完整範例 5: 金額轉文字 (發票、合約)

```csharp
public class InvoiceService
{
    public string GenerateAmountInWords(decimal totalAmount)
    {
        string amountInWords = Utility.AmountToWords.GetWords(totalAmount);
        
        // 範例輸出格式
        return $"Amount: ${totalAmount:N2}\n" +
               $"In Words: {amountInWords}";
    }

    // 範例
    public void Example()
    {
        decimal amount = 12345.67m;
        string result = GenerateAmountInWords(amount);
        
        /* 輸出:
        Amount: $12,345.67
        In Words: Twelve Thousand Three Hundred Forty Five And Cents Sixty Seven Only.
        */
    }
}
```

---

## 注意事項

### ⚠️ 安全性建議

1. **加密金鑰管理**
   ```csharp
   // ❌ 不建議 (硬編碼)
   private static readonly string EncryptKey = "466A9A1C...";

   // ✅ 建議 (從設定檔讀取)
   // appsettings.json:
   {
     "Encryption": {
       "Key": "466A9A1C2D054805BF3AE7E5C45B838B66F1418AAC3E4340B3E7F4DAD361FE2C"
     }
   }
   ```

2. **SQL Injection 防護**
   - `GetSQL()` 已對單引號進行跳脫，但仍建議使用參數化查詢
   - `EscapeValue()` 用於 LIKE 查詢，但請小心使用

3. **檔案加密**
   - AES-256 是安全的，但金鑰管理很重要
   - 建議使用 Key Vault 或類似服務

### 💡 效能建議

1. **大型 DataTable 轉 SQL**
   - `GetSQL()` 會產生很長的字串，大量資料建議分批處理
   - 考慮使用 Bulk Insert 而非 INSERT 語句

2. **檔案加密**
   - 使用 1 MB Buffer，適合大檔案
   - 非常大的檔案 (>1GB) 建議顯示進度條

3. **JSON 驗證**
   - `IsValidJson()` 會完整解析 JSON，大型 JSON 會較慢

### 🐛 已知限制

1. **金額轉文字**
   - 僅支援到 Billion (十億)
   - 僅支援英文輸出

2. **HTML 處理**
   - `ReplaceHtml()` 僅處理常見標籤
   - 複雜的 HTML 建議使用 HTML Agility Pack

3. **多語系**
   - `GetCaption()` 依賴 JSON 格式的語系資料
   - 不支援複數形式、性別等複雜規則

---

## 📝 變更記錄

| 日期 | 版本 | 變更內容 | 作者 |
|------|------|----------|------|
| 2025-11-20 | 1.0 | 從 Bia 專案移植，排除 PDF 浮水印 | AI Assistant |

---

## ✅ 下一步

1. **建議改進**
   - [ ] 將 EncryptKey 移到 appsettings.json
   - [ ] 加入單元測試
   - [ ] 支援更多語言的金額轉文字

2. **文檔**
   - [x] 建立使用說明
   - [ ] 加入 XML 註解 (Intellisense)
   - [ ] 建立 API 文檔

---

**文檔狀態**: 🟢 Complete  
**最後更新**: 2025-11-20  
**版本**: 1.0

