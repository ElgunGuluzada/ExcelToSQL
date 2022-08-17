using Microsoft.AspNetCore.Http;

namespace ExcelToSQL.Interface
{
    public interface IEmailService
    {
        bool SendEmail(string email,string subject, string message,string fileName,byte[] bytes);
    }
}
