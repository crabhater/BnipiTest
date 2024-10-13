using BnipiTest.Models.InterfacesLib;
using System.Text.Json.Serialization;

namespace BnipiTest.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Cipher { get; set; }
        public string Name { get; set; }
        public List<DesignObject>? DesignObjects { get; set; }
    }
}
