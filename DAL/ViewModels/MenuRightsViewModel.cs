namespace TransportERP.Models.ViewModels
{
    public class MenuRightsViewModel
    {
        public int UserWiseMenuID { get; set; }
        public int MenuID { get; set; }
        public string MenuDispName { get; set; } = string.Empty;
        public int ParentMenuID { get; set; }
        public bool IsShow { get; set; }
        public bool IsAdd { get; set; }
        public bool IsEdit { get; set; }
        public bool IsDelete { get; set; }
        public bool IsPrint { get; set; }
        public bool IsExport { get; set; }
        public bool IsVerify { get; set; }
    }
}
