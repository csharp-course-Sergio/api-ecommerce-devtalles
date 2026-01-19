using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;
using Mapster;

namespace ApiEcommerce.Mapping;

public static class MapsterConfig
{
    public static void RegisterMappings()
    {
        // Category mappings
        TypeAdapterConfig<Category, CategoryDto>.NewConfig();
        TypeAdapterConfig<CategoryDto, Category>.NewConfig();
        TypeAdapterConfig<Category, CreateCategoryDto>.NewConfig();
        TypeAdapterConfig<CreateCategoryDto, Category>.NewConfig();

        // Product mappings
        TypeAdapterConfig<Product, ProductDto>.NewConfig()
            .Map(dest => dest.CategoryName, src => src.Category.Name);
        TypeAdapterConfig<ProductDto, Product>.NewConfig();
        TypeAdapterConfig<Product, CreateProductDto>.NewConfig();
        TypeAdapterConfig<CreateProductDto, Product>.NewConfig();
        TypeAdapterConfig<Product, UpdateProductDto>.NewConfig();
        TypeAdapterConfig<UpdateProductDto, Product>.NewConfig();

        // User mappings
        TypeAdapterConfig<User, UserDto>.NewConfig();
        TypeAdapterConfig<UserDto, User>.NewConfig();
        TypeAdapterConfig<User, CreateUserDto>.NewConfig();
        TypeAdapterConfig<CreateUserDto, User>.NewConfig();
        TypeAdapterConfig<User, UserLoginDto>.NewConfig();
        TypeAdapterConfig<UserLoginDto, User>.NewConfig();
        TypeAdapterConfig<User, UserLoginResponseDto>.NewConfig();
        TypeAdapterConfig<UserLoginResponseDto, User>.NewConfig();
        TypeAdapterConfig<ApplicationUser, UserDataDto>.NewConfig();
        TypeAdapterConfig<UserDataDto, ApplicationUser>.NewConfig();
        TypeAdapterConfig<ApplicationUser, UserDto>.NewConfig();
        TypeAdapterConfig<UserDto, ApplicationUser>.NewConfig();
    }
}
