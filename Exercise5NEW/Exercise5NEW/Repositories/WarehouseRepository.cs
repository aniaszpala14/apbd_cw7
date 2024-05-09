using System.Data;
using System.Data.Common;
using Exercise5NEW.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.SqlClient;

namespace Exercise5NEW.Repositories;

public class WarehouseRepository : IWarehouseRepository
{
    public readonly IConfiguration _configuration;

    public WarehouseRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public async Task<bool> DoesWarehouseExist(ProductWarehouseDTO product)
    {
        var query = $"SELECT 1 FROM Warehouse WHERE IdWarehouse = @Id";
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@Id", product.IdWarehouse);
        await connection.OpenAsync();
        var result = await command.ExecuteScalarAsync();
        return result is not null;
    }
    public async Task<bool> DoesProductExist(ProductWarehouseDTO product)
    {
        var query = $"SELECT 1 FROM Product WHERE IdProduct = @Id";
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@Id",product.IdProduct);
        await connection.OpenAsync();
        var result = await command.ExecuteScalarAsync();
        return result is not null;
    }
    public async Task<bool> DoesOrderExist(ProductWarehouseDTO product){
        var query = $"SELECT 1 FROM [Order] WHERE IdProduct = @ID and Amount=@AMOUNT AND CreatedAt < @DATAREQUEST";
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@Id", product.IdProduct);
        command.Parameters.AddWithValue("@AMOUNT", product.Amount);
        command.Parameters.AddWithValue("@DATAREQUEST", product.CreatedAt);
        await connection.OpenAsync();
        var result = await command.ExecuteScalarAsync();
        return result is not null;
    }
    public async Task<int> GetOrderId(ProductWarehouseDTO product)
    {
        var query = $"SELECT IdOrder FROM [Order] WHERE IdProduct = @ID";
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@Id", product.IdProduct);
        await connection.OpenAsync();
        var result = await command.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }
    public async Task<bool> WasFullfilled(int orderId)
    {
        var query = $"SELECT FulfilledAt FROM [Order] WHERE IdProduct = @ID";
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@Id", orderId);
        await connection.OpenAsync();
        var result = await command.ExecuteScalarAsync();
        return result is not null;
    }
    public async Task<bool> IsOrderInProduct_Warehouse(int orderId)
    {
        var query = $"SELECT 1 FROM Product_Warehouse WHERE IdOrder= @ID";
        await  using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@Id", orderId);
        await connection.OpenAsync();
        var result = await command.ExecuteScalarAsync();
        return result is not null;
    }
  
    public async Task UpdateOrderFulfilledAt(int orderId)
    {
        string query = @" UPDATE [Order] SET FulfilledAt = @CurrentDateTime WHERE IdOrder = @IdOrder";
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@CurrentDateTime", DateTime.Now);
        command.Parameters.AddWithValue("@IdOrder", orderId);

        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();
    }
    public async Task<decimal> GetPrice(ProductWarehouseDTO product)
    {
        var query = $"SELECT Price FROM Product WHERE IdProduct = @ID";
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@Id", product.IdProduct);
        await connection.OpenAsync();
        var result = await command.ExecuteScalarAsync();
        return Convert.ToDecimal(result);
    }

    //bez transakcji
    // public async Task<int> AddProductWarehouse(ProductWarehouseDTO product)
    // {
    //     var insert = @"INSERT INTO Product_Warehouse(IdProduct,IdWarehouse,Amount,CreatedAt) VALUES (@IDPRODUCT,@IDWAREHOUSE,@AMOUNT,@CREATEDAT)";
    //     await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
    //     await using SqlCommand command = new SqlCommand();
    //     command.Connection = connection;
    //     command.CommandText = insert;
    //  
    //     decimal Price = await GetPrice(product);
    //     command.Parameters.AddWithValue("@IDPRODUCT", product.IdProduct);
    //     command.Parameters.AddWithValue("@IDWAREHOUSE", product.IdWarehouse);
    //     command.Parameters.AddWithValue("@AMOUNT", product.Amount );
    //     command.Parameters.AddWithValue("@Price", product.Amount * Price);
    //     command.Parameters.AddWithValue("@CREATEDAT", DateTime.Now);
    //     await connection.OpenAsync();
    //     int newId = Convert.ToInt32(await command.ExecuteScalarAsync());
    //     return newId;
    //     
    // }
    //z transakcja
    public async Task<int> AddProductWarehouse(ProductWarehouseDTO product)
    {
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        
        command.CommandText = "SELECT Price FROM Product WHERE IdProduct = @IdProduct";
        command.Parameters.AddWithValue("@IdProduct", product.IdProduct);
        
        await connection.OpenAsync();
        DbTransaction transaction = connection.BeginTransaction();
        command.Transaction = (SqlTransaction)transaction;
        try
        {
            var price = await command.ExecuteScalarAsync();
            command.Parameters.Clear();
            
            if (price == null)
            { return -1; }
            
            command.CommandText = "INSERT INTO Product_Warehouse (IdWarehouse, IdProduct, IdOrder, Amount, Price, CreatedAt) " +
                                  "OUTPUT INSERTED.IdProductWarehouse VALUES (@IdWarehouse, @IdProduct, @IdOrder, @Amount, @Price, GETDATE())";
            command.Parameters.AddWithValue("@IdWarehouse", product.IdWarehouse);
            command.Parameters.AddWithValue("@IdProduct", product.IdProduct);
            command.Parameters.AddWithValue("@IdOrder", (await GetOrderId(product)));
            command.Parameters.AddWithValue("@Amount", product.Amount);
            command.Parameters.AddWithValue("@Price", (decimal)price * product.Amount);

            var idProductWarehouse = await command.ExecuteScalarAsync();
            if (idProductWarehouse == null)
            {return -1; }
            await transaction.CommitAsync();
            return (int) idProductWarehouse;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            await transaction.RollbackAsync();
        }
        return -1;
    }   
    public async Task<int> AddProductToWarehouse_Procedure(ProductWarehouseDTO product)
    {
        int newId = 0;
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand("AddProductToWarehouse", connection);
            
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.AddWithValue("@IdProduct",product.IdProduct);
        command.Parameters.AddWithValue("@IdWarehouse", product.IdWarehouse);
        command.Parameters.AddWithValue("@Amount", product.Amount);
        command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
        await connection.OpenAsync();
        newId = Convert.ToInt32(await command.ExecuteScalarAsync());
        return newId;
    }


}
