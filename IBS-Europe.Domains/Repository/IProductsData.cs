namespace IBS_Europe.Domains;

public interface IProductsData
{
    public Task<List<Product>> GetAllProducts(string culture);
    
    public Task<Product> GetProduct(string name);
    
    public Task EditProduct(Product product, string actualName);
    
    public Task<bool> ProductExists(string name, string actualName);
    
    public Task UpdateImage(string name, string path);

    public Task<string> GetPath(string name);
    
    public Task<string> AddProduct(Product product);
    
    public Task DeleteProduct(string name);
    
    public Task SwitchPriority(int priority, string direction);
    
    public Task<Dictionary<string,string>> GetBrokers(string productName);
    
    public Task<Dictionary<string,string>> GetProductsList();
}