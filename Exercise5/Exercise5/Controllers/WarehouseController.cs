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
    {
        _warehouseRepository = warehouseRepository;
    }
    
    [HttpGet("getall")]
    public IActionResult GetAnimals(){
        var product_warehouse = _warehouseRepository.GetProductWarehouse();
        return Ok(product_warehouse);
    }
    //////////////// DOTAD JEST OK /////////////////////////////
    ///
    ///
    /// 

    [HttpPost("Procedure")]
    public async Task<IActionResult> AddToWarehouse_Procedure(int IdProduct, int IdWarehouse, int Amount)
    {
        var result = await _warehouseRepository.AddProductToWarehouse_Procedure(IdProduct, IdWarehouse, Amount);
        return Ok(result);
    }
    
    [HttpPost("Normal")]
    public async Task<IActionResult> AddToWarehouse([FromBody] ProductWarehouseDTO productWarehouse)
    {
        //
        // if ((!await _warehouseRepository.DoesWarehouseExist(ProductWarehouse.IdWarehouse)) ||
        //     (!await _warehouseRepository.DoesProductExist(ProductWarehouse.IdProduct)) ||
        //     (!await _warehouseRepository.DoesOrderExist(ProductWarehouse.IdProduct, ProductWarehouse.Amount,ProductWarehouse.CreatedAt)))
        //     return NotFound();
        //
        // var ware = await _warehouseRepository.AddProductToWarehouse(ProductWarehouse);
        //
        // return Ok(ware);
        //
        if (!await _warehouseRepository.DoesWarehouseExist(productWarehouse.IdWarehouse))
            return NotFound("Warehouse not found");
        if (!await _warehouseRepository.DoesProductExist(productWarehouse.IdProduct))
            return NotFound("Product not found");
      

        int newProductId = await _warehouseRepository.AddProductToWarehouse(productWarehouse);

        return Ok(newProductId);
    }
    
}

