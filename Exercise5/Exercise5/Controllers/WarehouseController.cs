using System.Transactions;
using Exercise5.Interfaces;
using Exercise5.Models;
namespace Exercise5.Controllers;
using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("aniapi/warehouse")]
public class WarehouseController : Controller
{
    private readonly IWarehouseRepository _warehouseRepository;

    public WarehouseController(IWarehouseRepository warehouseRepository)
    { _warehouseRepository = warehouseRepository; }

    [HttpGet("getall")]
    public IActionResult GetAnimals()
    {
        var product_warehouse = _warehouseRepository.GetProductWarehouse();
        return Ok(product_warehouse);
    }
    
    [HttpPost("Normal")]
    public async Task<IActionResult> AddToWarehouse(int IdWarehouse,int IdProduct,int Amount,DateTime CreatedAt)
    {
        
        if (!await _warehouseRepository.DoesWarehouseExist(IdWarehouse))
            return NotFound("Warehouse not found");
        if (!await _warehouseRepository.DoesProductExist(IdProduct))
            return NotFound("Product not found");
        if (Amount <= 0)
            return NotFound("Amount is too low");
        if (!await _warehouseRepository.DoesOrderExist(IdProduct,Amount,CreatedAt))
            return NotFound("There is no order for this id or amount isn't right or wrong data");
        
        int orderId = await _warehouseRepository.GetOrderId(IdProduct);
        
        if (!await _warehouseRepository.WasFullfilled(orderId))
            return NotFound("Already fullfilled");
        if (await _warehouseRepository.IsOrderInProduct_Warehouse(orderId))
            return NotFound("There is already an order for that");
        
        int newProductId = await _warehouseRepository.AddProductToWarehouse(IdWarehouse,IdProduct,Amount,CreatedAt);
        await _warehouseRepository.UpdateOrderFulfilledAt(orderId);

        return Ok(newProductId);
    }
    
    [HttpPost("Procedure")]
    public async Task<IActionResult> AddToWarehouse_Procedure(int IdProduct, int IdWarehouse, int Amount)
    {
        var result = await _warehouseRepository.AddProductToWarehouse_Procedure(IdProduct, IdWarehouse, Amount);
        return Ok(result);
    }
    
}

