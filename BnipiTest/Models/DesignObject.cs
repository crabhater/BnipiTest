using BnipiTest.Models.InterfacesLib;
using System.Text.Json.Serialization;

namespace BnipiTest.Models
{
    public class DesignObject 
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int? ProjectId { get; set; }
        public int? ParentDesignObjectId { get; set; }
        [JsonIgnore]
        public DesignObject? ParentDesignObject { get; set; }
        [JsonIgnore]
        public Project Project { get; set; }
        public List<DesignObject>? ChildDesignObjects { get; set; }
        public List<Document>? Documents { get; set ; }


        public string GetFullCode()
        {
            if (ParentDesignObject == null)
            {
                return Code;
            }
            return $"{ParentDesignObject.GetFullCode()}.{Code}";
        }

        public string GetFullDocumentCode(Document document)
        {
            var fullCode = GetFullCode();
            return $"{Project.Cipher}-{fullCode}-{document.Mark.ShortName}{document.Number}";
        }
    }
}
