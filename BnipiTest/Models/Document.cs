using BnipiTest.Extensions;
using BnipiTest.Models.InterfacesLib;

namespace BnipiTest.Models
{
    public class Document
    {
        public int Id { get; set; }
        public int MarkId { get; set; }
        public Mark? Mark { get; set; }
        public int Number { get; set; }
        public int DesignObjectId {  get; set; }
    }
}
