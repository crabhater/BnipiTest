using BnipiTest.Models.InterfacesLib;

namespace BnipiTest.Models
{
    public class DesignObject 
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string FullCode { get 
            { return ParentDesignObject != null ? string.Format("{0}.{1}", new string[] { ParentDesignObject.Code, Code }) : Code; } }
        public string Name { get; set; }
        public int ProjectId { get; set; }
        public int? ParentDesignObjectId { get; set; }
        public DesignObject? ParentDesignObject { get; set; }
        public List<Document> Documents { get; set ; }
    }
}
