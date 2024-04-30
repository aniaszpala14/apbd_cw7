using System.Data;
using Exercise5.Interfaces;
using Exercise5.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
namespace Exercise5.Repositories;
public class WarehouseRepository : IWarehouseRepository
{ 
    public readonly IConfiguration _configuration;

    public WarehouseRepository(IConfiguration configuration)
    { _configuration = configuration; }
    
    public IEnumerable<ProductWarehouseDTO> GetProductWarehouse()
    {
        using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default")))
        {
            connection.Open();
            using (SqlCommand command = new SqlCommand("SELECT * FROM Product_Warehouse;", connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    var product_warehouses = new List<ProductWarehouseDTO>();
                    int IdProductWarehouse = reader.GetOrdinal("IdProductWarehouse");
                    int IdProduct = reader.GetOrdinal("IdProduct");
                    int IdWarehouse = reader.GetOrdinal("IdWarehouse");
                    int IdOrder = reader.GetOrdinal("IdOrder");
                    int Amount = reader.GetOrdinal("Amount");
                    int Price = reader.GetOrdinal("Price");
                    int CreatedAt = reader.GetOrdinal("CreatedAt");

                    while (reader.Read())
                    {
                        product_warehouses.Add(new ProductWarehouseDTO
                        {
                            IdProductWarehouse = reader.GetInt32(IdProductWarehouse),
                            IdProduct = reader.GetInt32(IdProduct),
                            IdWarehouse = reader.GetInt32(IdWarehouse),
                            IdOrder=reader.GetInt32(IdOrder),
                            Amount = reader.GetInt32(Amount),
                            Price = reader.GetDecimal(Price),
                            CreatedAt = reader.GetDateTime(CreatedAt)
                        });
                    }
                    return product_warehouses;
                }
            }
        }
    }

    public async Task<bool> DoesWarehouseExist(int id)
    {
        var query = $"SELECT 1 FROM Warehouse WHERE IdWarehouse = @Id";
    
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand();
    
        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@Id", id);
    
        await connection.OpenAsync();
        var result = await command.ExecuteScalarAsync();
        
        return result is not null;
    }
    public async Task<bool> DoesProductExist(int id)
    {
        var query = $"SELECT 1 FROM Product WHERE IdProduct = @Id";
    
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand();
    
        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@Id", id);
    
        await connection.OpenAsync();
        var result = await command.ExecuteScalarAsync();
    
        return result is not null;
    }
    
    public async Task<bool> DoesOrderExist(int idproduct,int amount,DateTime datarequesta){
        var query = $"SELECT 1 FROM Order WHERE IdProduct = @ID and Amount=@AMOUN AND CreatedAt < @CreatedAtT";
    
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand();
    
        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@Id", idproduct);
        command.Parameters.AddWithValue("@AMOUNT", amount);
    
        await connection.OpenAsync();
        var result = await command.ExecuteScalarAsync();
    
        return result is not null;
    }
    

    //////////////// DOTAD JEST OK /////////////////////////////
    
    public async Task<int> AddProductToWarehouse_Procedure(int IdProduct, int IdWarehouse, int Amount)
    {
        int newId = 0;
        using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default")))
        {
            using (SqlCommand command = new SqlCommand("AddProductToWarehouse", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@IdProduct", IdProduct);
                command.Parameters.AddWithValue("@IdWarehouse", IdWarehouse);
                command.Parameters.AddWithValue("@Amount", Amount);
                command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

                // Parametr zwracający nowe Id
                SqlParameter returnParameter = command.Parameters.Add("@NewId", SqlDbType.Int);
                returnParameter.Direction = ParameterDirection.Output;

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();

                // Pobranie wartości nowego Id
                newId = (int)returnParameter.Value;
            }
        }

        return newId;
    }
    
    public async Task UpdateOrderFulfilledAt(Order order)
    {
        string connectionString = _configuration.GetConnectionString("Default");
        string query = @" UPDATE Orders SET FulfilledAt = @CurrentDateTime WHERE IdOrder = @order.IdOrder";
    
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@CurrentDateTime", DateTime.Now);
                command.Parameters.AddWithValue("@OrderId", order.IdOrder);
    
                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
        }
    }
    
    public async Task<int> AddProductToWarehouse(ProductWarehouseDTO productWarehouse)
    {
        string connectionString = _configuration.GetConnectionString("Default");

        string query = @"
    INSERT INTO Product_Warehouse (IdWarehouse, IdProduct, Amount, CreatedAt)
    VALUES (@IdWarehouse, @IdProduct, @Amount, @CreatedAt);
    SELECT SCOPE_IDENTITY();";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@IdWarehouse", productWarehouse.IdWarehouse);
                command.Parameters.AddWithValue("@IdProduct", productWarehouse.IdProduct);
                command.Parameters.AddWithValue("@Amount", productWarehouse.Amount);
                command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

                await connection.OpenAsync();
                int newId = Convert.ToInt32(await command.ExecuteScalarAsync());
                return newId;
            }
        }
    }

}
    