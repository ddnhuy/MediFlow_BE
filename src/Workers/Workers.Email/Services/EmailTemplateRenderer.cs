using RazorLight;

namespace Workers.Email.Services
{
    public interface IEmailTemplateRenderer
    {
        Task<string> RenderAsync(string templateName, Dictionary<string, string> model);
    }
    public class RazorEmailTemplateRenderer : IEmailTemplateRenderer
    {
        private readonly RazorLightEngine _engine;

        public RazorEmailTemplateRenderer()
        {
            _engine = new RazorLightEngineBuilder()
                .UseFileSystemProject(Path.Combine(AppContext.BaseDirectory, "Templates"))
                .UseMemoryCachingProvider()
                .Build();
        }

        public async Task<string> RenderAsync(string templateName, Dictionary<string, string> model)
        {
            string templatePath = $"{templateName}.cshtml";
            return await _engine.CompileRenderAsync(templatePath, model);
        }
    }
}
