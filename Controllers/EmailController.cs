using Microsoft.AspNetCore.Mvc;
using tangti.Services;
using tangti.DTOs;

[Route("api/[controller]")]
[ApiController]
public class EmailController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly EmailService _emailService;

    public EmailController(IConfiguration configuration, EmailService emailService)
    {
        _configuration = configuration;
        _emailService = emailService;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendEmail([FromBody] EmailDto request)
    {
        // Extract email details from the request
        string toAddress = request.ToAddress;
        string subject = request.Subject;
        string body = request.Body;

        // Send email using the EmailService asynchronously
        try
        {
            await _emailService.SendEmail(toAddress, subject, body);
            return Ok("Email sent successfully!");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Failed to send email. Error: {ex.Message}");
        }
    }

}