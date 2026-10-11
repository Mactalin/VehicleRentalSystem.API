using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleRentalSystem.Application.DTOs.Notifications;
using VehicleRentalSystem.Application.Interface_Service;
using VehicleRentalSystem.Authorization;

namespace VehicleRentalSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _service;
    private readonly INotificationTriggerService _trigger;
    private readonly CancellationToken _ct;

    public NotificationsController(
        INotificationService service,
        INotificationTriggerService trigger,
        IHttpContextAccessor accessor)
    {
        _service = service;
        _trigger = trigger;
        _ct = accessor.HttpContext?.RequestAborted ?? CancellationToken.None;
    }

    // ================== CUSTOMER ENDPOINTS ==================

    /// <summary>Get all notifications for the current user.</summary>
    [HttpGet("user/{userId:int}")]
    [Authorize]
    public async Task<IActionResult> GetByUser(int userId)
        => Ok(await _service.GetByUserIdAsync(userId, _ct));

    /// <summary>Get unread notifications for the current user.</summary>
    [HttpGet("user/{userId:int}/unread")]
    [Authorize]
    public async Task<IActionResult> GetUnreadByUser(int userId)
        => Ok(await _service.GetUnreadByUserIdAsync(userId, _ct));

    /// <summary>Get unread count for the badge.</summary>
    [HttpGet("user/{userId:int}/unread/count")]
    [Authorize]
    public async Task<IActionResult> GetUnreadCount(int userId)
        => Ok(new { count = await _service.GetUnreadCountAsync(userId, _ct) });

    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<IActionResult> GetById(int id)
    {
        var dto = await _service.GetByIdAsync(id, _ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPut("{id:int}/read")]
    [Authorize]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        await _service.MarkAsReadAsync(id, _ct);
        return NoContent();
    }

    [HttpPut("user/{userId:int}/read-all")]
    [Authorize]
    public async Task<IActionResult> MarkAllAsRead(int userId)
    {
        await _service.MarkAllAsReadAsync(userId, _ct);
        return NoContent();
    }

    // ================== ADMIN / INTERNAL ENDPOINTS ==================

    [HttpGet]
    [Authorize(Policy = Policies.StaffAndAbove)]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync(_ct));

    [HttpPost]
    [Authorize(Policy = Policies.AdminOrManager)]
    public async Task<IActionResult> Create([FromBody] CreateNotificationDto dto)
    {
        var created = await _service.CreateAsync(dto, _ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>Manual trigger for testing (sends email + SMS via dispatcher).</summary>
    [HttpPost("trigger")]
    [Authorize(Policy = Policies.AdminOrManager)]
    public async Task<IActionResult> Trigger([FromBody] TriggerNotificationDto dto)
    {
        await _trigger.TriggerAsync(
            userId: dto.UserId,
            toEmail: dto.ToEmail,
            toPhone: dto.ToPhone,
            evt: dto.Event,
            customMessage: dto.CustomMessage,
            actionUrl: dto.ActionUrl,
            actionLabel: dto.ActionLabel,
            ct: _ct);

        return Ok(new { message = "Notification dispatched." });
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id, _ct);
        return NoContent();
    }
}