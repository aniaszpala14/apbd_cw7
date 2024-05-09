using System.Transactions;
using Exercise5NEW.Models;
using Exercise5NEW.Repositories;

namespace Exercise5.Controllers;
using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("aniapi/[controller]")]
public class WarehouseController : Controller
{
    private readonly IWarehouseRepository _warehouseRepository;

    public WarehouseController(IWarehouseRepository warehouseRepository)
    {
        _warehouseRepository = warehouseRepository;
    }

    [HttpPost("")]
    public async Task<IActionResult> AddProductWarehouse(ProductWarehouseDTO productWarehouse)
    {
        if (!await _warehouseRepository.DoesProductExist(productWarehouse))
            return NotFound("No product under this ID");
        if (!await _warehouseRepository.DoesWarehouseExist(productWarehouse))
            return NotFound("No warehouse under this ID");
        if (productWarehouse.Amount <= 0)
            return Conflict("Wrong amount");
        if (!await _warehouseRepository.DoesOrderExist(productWarehouse))
            return Conflict("Wrong order");
        int orderId = await _warehouseRepository.GetOrderId(productWarehouse);
        if (!await _warehouseRepository.WasFullfilled(orderId))
            return Conflict("Already a fullfillment date");
        if (await _warehouseRepository.IsOrderInProduct_Warehouse(orderId))
            return Conflict("This order exsist in ProductWarehouse");
        await _warehouseRepository.UpdateOrderFulfilledAt(orderId);

        int? res = await _warehouseRepository.AddProductWarehouse(productWarehouse);
        return Ok(res);
    }
    [HttpPost("Procedure")]
    public async Task<IActionResult> AddToWarehouse_Procedure(ProductWarehouseDTO product)
    {
        var result = await _warehouseRepository.AddProductToWarehouse_Procedure(product);
        return Ok(result);
    }


}