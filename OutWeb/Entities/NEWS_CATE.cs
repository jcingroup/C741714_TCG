//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace OutWeb.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class NEWS_CATE
    {
        public int ID { get; set; }
        public string BD_ID { get; set; }
        public Nullable<System.DateTime> BD_DT { get; set; }
        public string UPD_ID { get; set; }
        public Nullable<System.DateTime> UPD_DT { get; set; }
        public string CATE_NAME { get; set; }
        public string CATE_DESC { get; set; }
        public string PIC { get; set; }
        public string LANG_ID { get; set; }
        public Nullable<int> SORT { get; set; }
        public string STATUS { get; set; }
    }
}
