using Exercise5.Models;

namespace Exercise5.Interfaces;

public interface IWarehouseRepository
{
   Task<bool> DoesProductExist(int id);
   Task<bool> DoesWarehouseExist(int id);
   Task<bool> DoesOrderExist(ProductWarehouseDTO productWarehouse);
   Task<int> GetOrderId(ProductWarehouseDTO productWarehouse);
   Task<int> AddProductToWarehouse(ProductWarehouseDTO productWarehouseDto);
   Task<int> AddProductToWarehouse_Procedure(int IdProduct,int IdWarehouse,int Amount);
   Task UpdateOrderFulfilledAt(int orderId);
  public IEnumerable<ProductWarehouseDTO> GetProductWarehouse();

  public Task<bool> WasFullfilled(int orderId);
  public Task<bool> IsOrderInProduct_Warehouse(int orderId);


}