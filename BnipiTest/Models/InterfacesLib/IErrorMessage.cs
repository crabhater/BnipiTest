namespace BnipiTest.Models.InterfacesLib
{
    public interface IErrorMessage
    {
        string Message { get; set; }
        string StackTrace { get; set; }
    }
}
