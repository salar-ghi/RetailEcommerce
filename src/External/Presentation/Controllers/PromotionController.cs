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

    [HttpGet("{promotionId:int}")]
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

    [HttpGet("discounts")]
    public async Task<ActionResult<IEnumerable<PromotionDiscountDto>>> GetDiscounts()
    {
        var discounts = await _promotionService.GetAllDiscountsAsync();
        return Ok(discounts);
    }

    [HttpGet("discounts/{discountId:int}")]
    public async Task<ActionResult<PromotionDiscountDto>> GetDiscount(int discountId)
    {
        var discount = await _promotionService.GetDiscountAsync(discountId);
        return Ok(discount);
    }

    [HttpPost("discounts")]
    public async Task<ActionResult<PromotionDiscountDto>> CreateDiscount([FromBody] CreateDiscountRequestDto request)
    {
        var discount = await _promotionService.CreateDiscountAsync(request);
        return CreatedAtAction(nameof(GetDiscount), new { discountId = discount.Id }, discount);
    }

    [HttpPut("discounts/{discountId:int}")]
    public async Task<ActionResult<PromotionDiscountDto>> UpdateDiscount(int discountId, [FromBody] UpdateDiscountRequestDto request)
    {
        var discount = await _promotionService.UpdateDiscountAsync(discountId, request);
        return Ok(discount);
    }

    [HttpDelete("discounts/{discountId:int}")]
    public async Task<IActionResult> DeleteDiscount(int discountId)
    {
        await _promotionService.DeleteDiscountAsync(discountId);
        return NoContent();
    }

    [HttpPost("discounts/calculate")]
    public async Task<ActionResult<DiscountCalculationResultDto>> CalculateDiscount([FromBody] DiscountCalculationRequestDto request)
    {
        var result = await _promotionService.CalculateDiscountAsync(request);
        return Ok(result);
    }
}
