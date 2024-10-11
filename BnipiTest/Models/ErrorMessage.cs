using BnipiTest.Models.InterfacesLib;

namespace BnipiTest.Models
{
    public class ErrorMessage 
    {
        public ErrorMessage(string message, string? stackTrace = null) 
        {
            Message = message;
            StackTrace = stackTrace;
        }
        public string Message { get ; set ; }
        public string? StackTrace { get ; set ; }
    }
}
