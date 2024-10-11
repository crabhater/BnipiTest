namespace BnipiTest.Models.InterfacesLib
{
    public interface IProject
    {
        int ProjectId { get; set; }
        string Code { get; set; }
        string Name { get; set; }
        IList<IDesignObject> DesignObjects { get; set; }
    }
}
