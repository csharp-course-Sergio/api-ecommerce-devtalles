
using ApiEcommerce.Models;
using ApiEcommerce.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace ApiEcommerce.Repository;

public class ProductRepository(ApplicationDbContext db) : IProductRepository
{
    private readonly ApplicationDbContext _db = db;

    public ICollection<Product> GetProducts()
    {
        return [.. _db.Products.Include(product => product.Category).OrderBy(product => product.Name)];
    }

    public ICollection<Product> GetProductsForCategory(int categoryId)
    {
        if (categoryId <= 0) return Array.Empty<Product>();
        return [.. _db.Products.Where(product => product.CategoryId == categoryId).OrderBy(product => product.Name)];
    }

    public ICollection<Product> SearchProduct(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return Array.Empty<Product>();
        return [.. _db.Products.Where(product => product.Name.ToLower().Trim() == name.ToLower().Trim()).OrderBy(product => product.Name)];
    }

    public Product? GetProduct(int id)
    {
        if (id <= 0) return null;
        return _db.Products.Include(product => product.Category).FirstOrDefault(product => product.Id == id);
    }

    public bool BuyProduct(string name, int quantity)
    {
        if (string.IsNullOrWhiteSpace(name) || quantity <= 0) return false;

        var product = _db.Products.FirstOrDefault(p => p.Name.ToLower().Trim() == name.ToLower().Trim());
        if (product == null || product.Stock < quantity) return false;

        product.Stock -= quantity;
        _db.Products.Update(product);
        return Save();
    }

    public bool ProductExists(int id)
    {
        return _db.Products.Any(product => product.Id == id);
    }

    public bool ProductExists(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return false;
        return _db.Products.Any(product => product.Name.ToLower().Trim() == name.ToLower().Trim());
    }

    public bool CreateProduct(Product product)
    {
        product.CreationDate = DateTime.Now;
        product.UpdateDate = DateTime.Now;

        _db.Products.Add(product);
        return Save();
    }

    public bool UpdateProduct(Product product)
    {
        product.UpdateDate = DateTime.Now;
        _db.Products.Update(product);
        return Save();
    }

    public bool DeleteProduct(Product product)
    {
        _db.Products.Remove(product);
        return Save();
    }

    public bool Save()
    {
        return _db.SaveChanges() >= 0;
    }


}
