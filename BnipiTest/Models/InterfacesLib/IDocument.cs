namespace BnipiTest.Models.InterfacesLib
{
    public interface IDocument
    {
        int DocumentId { get; set; }
        string Mark { get; set; }
        int Number { get; set; }
        int Index { get; }
        int DesignObjectId { get; set; }
        IDesignObject DesignObject { get; set; }
    }
}
