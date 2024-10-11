namespace BnipiTest.Models.InterfacesLib
{
    public interface IDesignObject
    {
        int DesignObjectId { get; set; }
        string Code { get; set; }
        string FullCode { get; }
        string Name { get; set; }
        int ProjectId { get; set; }
        int? ParentDesignObjectId { get; set; }
        IDesignObject ParentDesignObject { get; set; }
        IProject Project { get; set; }
        IList<IDesignObject> ChildDesignObjects { get; set; }
        IList<IDocument> Documents { get; set; }
    }
}
