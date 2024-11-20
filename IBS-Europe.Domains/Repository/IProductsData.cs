namespace IBS_Europe.Domains;

public interface IProductsData
{
    public Task<HashSet<Product>> GetAllProducts(int id, string culture);
    
    public Task<Product> GetProduct(int id);
    
    public Task EditProduct(Product product);
    
    public Task<bool> ProductExists(string name, int id);
    
    public Task UpdateImage(int id, string path);

    public Task<string> GetPath(int id);
    
    public Task<int> AddProduct(Product product);
    
    public Task DeleteProduct(int id);
}