using API.Models;
using API.Services;
using Messages;
using Microsoft.AspNetCore.Mvc;
using Monitoring;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TextController : ControllerBase
{
    [HttpGet]
    public IActionResult Get(string languageCode)
    {
        using var activity = MonitorService.ActivitySource.StartActivity("TextController.Get");
        MonitorService.Log.Debug("Entered 'Get' in TextController");

        try
        {

            

            var greeting = GreetingService.Instance.Greet(new GreetingRequest { LanguageCode = languageCode });
            var planet = PlanetService.Instance.GetPlanet();
        
            var response = new GetGreetingModel.Response
            {
                Greeting = greeting.Greeting,
                Planet = planet.Planet
            };
            return Ok(response);
        }
        catch (Exception e)
        {
            // Added an error log here so the exception details appear in Seq 
            MonitorService.Log.Error($"An error occurred in 'Get' method of TextController: {e.Message}");
            return StatusCode(500, "An error occurred");
        }
    }
}