namespace Application.Common.DTOs.Email
{
    public class DraftEmailDto
    {
        public string ToName { get; set; }
        public string ToEmail { get; set; }

        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
