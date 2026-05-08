using AIAssistant.Models;
using Google.GenAI;
using Google.GenAI.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using static Google.Apis.Requests.BatchRequest;

namespace AIAssistant.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;

        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View(new Home());
        }

        [HttpPost]
        public async Task<IActionResult> Index([Bind("Request,UploadedFile")] Home home)
        {
            home.Message = "Processing your request...";



            //var AI = "Gemini";
            var AI = "Local";
            if (AI == "Gemini")
            {
                ViewBag.ValueMessage = "";
                ViewBag.ValueMessageVisible = false;
                var action = Request.Form["action"];
                if (action == "upload")
                {
                    if (home.UploadedFile != null && home.UploadedFile.Length > 0)
                    {
                        // Process the uploaded file here
                        // For example, save it to a temporary location or analyze it
                        var filePath = Path.Combine(Path.GetTempPath(), home.UploadedFile.FileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await home.UploadedFile.CopyToAsync(stream);
                        }
                        // Add logic to analyze the file

                        // Read the file content as a string and pass it to the AI method
                        using (var reader = new StreamReader(home.UploadedFile.OpenReadStream()))
                        {
                            var fileContent = await reader.ReadToEndAsync();
                            var aiResponse = await GenerateContentSimpleText.GetAIResponse(fileContent);
                            ViewBag.ValueMessage = aiResponse;
                            ViewBag.ValueMessageVisible = true;
                        }

                        //ViewBag.FileMessage = $"File {home.UploadedFile.FileName} uploaded successfully.";
                        //var aiResponse = await GenerateContentSimpleText.GetAIResponse(home.UploadedFile.);
                        //ViewBag.ValueMessage = aiResponse;

                    }
                    else
                    {
                        ModelState.AddModelError("UploadedFile", "Please select a file to upload.");
                    }
                }
                else if (action == "text")
                {
                    if (ModelState.IsValid && !string.IsNullOrEmpty(home.Request))
                    {
                        // Process the text request with AI
                        var aiResponse = await GenerateContentSimpleText.GetAIResponse(home.Request);
                        ViewBag.ValueMessage = aiResponse;
                        ViewBag.ValueMessageVisible = true;
                    }
                }
            }
            else if (AI == "Local")
            {
                ViewBag.ValueMessage = "";
                ViewBag.ValueMessageVisible = false;
                ViewBag.ValueMessage = "Local AI response: This is a placeholder response from the local AI model.";
                ViewBag.ValueMessageVisible = true;
            }
            return View(home);
        }

        public IActionResult ChatBot()
        {
            List<string> messages = new List<string>();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChatBot([Bind("Request")] Home home)
        {
            //home.MessagesIn = new List<string>();
            //home.MessagesOut = new List<string>();

            var action = Request.Form["action"];
            if (action == "text")
            {
                //var AI = "Gemini";
                var AI = "Local";
                if (AI == "Gemini")
                {


                    ViewBag.ValueMessage = "";
                    ViewBag.ValueMessageVisible = false;

                    if (ModelState.IsValid && !string.IsNullOrEmpty(home.Request))
                    {
                        // Process the text request with AI
                        var aiResponse = await GenerateContentSimpleText.GetAIResponse(home.Request);
                        ViewBag.ValueMessage = aiResponse;
                        ViewBag.ValueMessageVisible = true;
                    }

                }
                else if (AI == "Local")
                {
                    ViewBag.ValueMessage = "";
                    ViewBag.ValueMessageVisible = false;
                    if (!string.IsNullOrEmpty(home.Request)) home.MessagesOut.Add(home.Request);
                    
                    ViewBag.ValueMessage = "Local AI response: This is a placeholder response from the local AI model.";
                    ViewBag.ValueMessageVisible = true;
                }
            }
            return View(home);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public class GenerateContentSimpleText
        {
            public static async Task<string> GetAIResponse(string prompt)
            {
                // The client gets the API key from the provided parameter.
                try
                {
                    var client = new Client();
                    var response = await client.Models.GenerateContentAsync(
                      model: "gemini-3-flash-preview", contents: prompt);
                    return response.Candidates[0].Content.Parts[0].Text;

                }
                catch (Exception ex)
                {
                    return $"Error: {ex.Message}";
                }
            }
        }
    }
}
