namespace Application.Common.DTOs.Email
{
    public class EmailDto
    {
        public string? Subject { get; set; }
        public string? Body { get; set; }
        public string? From { get; set; }
        public DateTime? Date { get; set; }
    }
}
