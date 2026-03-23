namespace Application.Common.Services.AnswerTemplateManager
{
    public interface IAnswerTemplateService
    {
        Task<string> RenderAsync(string key, object? model = null);
        Task<string[]> GetDefaultRecipientsAsync(string key);
    }
}
