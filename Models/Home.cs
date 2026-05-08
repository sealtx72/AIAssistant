using Microsoft.AspNetCore.Mvc;

namespace AIAssistant.Models
{
    public class Home
    {
        public string? Request { get; set; }
        public IFormFile? UploadedFile { get; set; }

        public string? Message { get; set; }
        [BindProperty]
        public List<string>? MessagesOut { get; set; } = new List<string>();
        [BindProperty]
        public List<string>? MessagesIn { get; set; } = new List<string>();
    }
}
