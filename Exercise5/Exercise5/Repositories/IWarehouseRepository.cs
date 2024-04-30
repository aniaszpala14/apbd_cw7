using Exercise5.Models;

namespace Exercise5.Interfaces;

public interface IWarehouseRepository
{
   Task<bool> DoesProductExist(int id);
   Task<bool> DoesWarehouseExist(int id);
   Task<bool> DoesOrderExist(int IdProduct,int amount,DateTime createdat);
   Task<int> AddProductToWarehouse(ProductWarehouseDTO productWarehouseDto);
   Task<int> AddProductToWarehouse_Procedure(int IdProduct,int IdWarehouse,int Amount);
  // Task UpdateOrderFulfilledAt(Order order);
  public IEnumerable<ProductWarehouseDTO> GetProductWarehouse();
  

}