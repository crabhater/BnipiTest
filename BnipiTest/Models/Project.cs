using BnipiTest.Models.InterfacesLib;

namespace BnipiTest.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public List<DesignObject> DesignObjects { get; set; }
    }
}
