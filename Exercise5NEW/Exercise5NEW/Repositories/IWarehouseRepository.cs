using Exercise5NEW.Models;

namespace Exercise5NEW.Repositories;

public interface IWarehouseRepository
{
    Task<bool> DoesWarehouseExist(ProductWarehouseDTO product);
    Task<bool> DoesProductExist(ProductWarehouseDTO product);
    Task<bool> DoesOrderExist(ProductWarehouseDTO product);
    Task<bool> WasFullfilled(int orderId);
    Task<bool> IsOrderInProduct_Warehouse(int orderId);
    Task<int> GetOrderId(ProductWarehouseDTO product);
    Task UpdateOrderFulfilledAt(int orderId);
    Task<decimal> GetPrice(ProductWarehouseDTO product);
    Task<int> AddProductWarehouse(ProductWarehouseDTO product);
    Task<int> AddProductToWarehouse_Procedure(ProductWarehouseDTO product);

}