namespace SWMSU.Interface
{
    public interface IMail
    {
        Task<dynamic> SendEmailAsync(string toEmail, string subject, string body);

    }
}
