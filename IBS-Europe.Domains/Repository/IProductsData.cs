namespace IBS_Europe.Domains;

public interface IProductsData
{
    public HashSet<Product> GetAllProducts(int id, string culture);
    
    public Product GetProduct(int id);
    
    public Task EditProduct(Product product);
    
    public bool ProductExists(string name, int id);
    
    public void UpdateImage(int id, string path);

    public string GetName(int id);
    
    public Task<int> AddProduct(Product product);
    
    public void DeleteProduct(int id);
}