using Application.Interfaces;

namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PromotionController : ControllerBase
{
    private readonly IPromotionService _promotionService;

    public PromotionController(IPromotionService promotionService)
    {
        _promotionService = promotionService;
    }

    [HttpGet("{promotionId}")]
    public async Task<ActionResult<PromotionDto>> GetPromotion(int promotionId)
    {
        var promotion = await _promotionService.GetPromotionAsync(promotionId);
        return Ok(promotion);
    }

    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<PromotionDto>>> GetActivePromotions()
    {
        var promotions = await _promotionService.GetActivePromotionsAsync();
        return Ok(promotions);
    }

}
