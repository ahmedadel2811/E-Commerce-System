using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ECommerce.Core.DTOs.CartItem;
using ECommerce.Core.DTOs.category;
using ECommerce.Core.DTOs.order;
using ECommerce.Core.DTOs.product;
using ECommerce.Core.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ECommerce.Core.Profiles
{
    public class MappingProfile : Profile
    {
        //public MappingProfile()
        //{
        //    // Product Mappings
        //    CreateMap<Product, ProductDto>()
        //        .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
        //        .ForMember(dest => dest.VendorShopName, opt => opt.MapFrom(src => src.Vendor.ShopName))
        //        .ForMember(dest => dest.FinalPrice, opt => opt.Ignore());



        //    CreateMap<Product, ProductDetailDto>()
        //        .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
        //        .ForMember(dest => dest.VendorShopName, opt => opt.MapFrom(src => src.Vendor.ShopName));

        //    CreateMap<CreateProductDto, Product>();
        //    CreateMap<UpdateProductDto, Product>();

        //    //// ProductImage Mappings
        //    //CreateMap<ProductImage, ProductImageDto>();

        //    //// ProductVariant Mappings
        //    //CreateMap<ProductVariant, ProductVariantDto>();

        //    //// ProductReview Mappings
        //    //CreateMap<ProductReview, ProductReviewDto>()
        //    //    .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.FullName));




        //    // Existing Product mappings...

        //    // Category Mappings
        //    CreateMap<Category, CategoryDto>()
        //        .ForMember(dest => dest.ParentCategoryName, opt => opt.MapFrom(src => src.ParentCategory.Name))
        //        .ForMember(dest => dest.ProductsCount, opt => opt.MapFrom(src => src.Products.Count))
        //        .ForMember(dest => dest.SubCategoriesCount, opt => opt.MapFrom(src => src.SubCategories.Count));

        //    CreateMap<Category, CategoryDetailDto>()
        //        .ForMember(dest => dest.ParentCategoryName, opt => opt.MapFrom(src => src.ParentCategory.Name));

        //    CreateMap<Category, CategoryWithSubCategoriesDto>();

        //    CreateMap<CreateCategoryDto, Category>();
        //    CreateMap<UpdateCategoryDto, Category>();


        //    // Order Mappings
        //    CreateMap<Order, OrderDto>()
        //        .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.FullName))
        //        .ForMember(dest => dest.StatusDisplay, opt => opt.MapFrom(src => src.Status.ToString()))
        //        .ForMember(dest => dest.ItemsCount, opt => opt.MapFrom(src => src.OrderItems.Count));

        //    CreateMap<Order, OrderDetailDto>()
        //        .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.FullName))
        //        .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User.Email))
        //        .ForMember(dest => dest.StatusDisplay, opt => opt.MapFrom(src => src.Status.ToString()));

        //    CreateMap<OrderItem, OrderItemDto>()
        //        .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
        //        .ForMember(dest => dest.ProductImage, opt => opt.MapFrom(src => src.Product.ImageUrl));

        //    CreateMap<CreateOrderDto, Order>();
        //    CreateMap<UpdateOrderDto, Order>();
        //    CreateMap<CreateOrderItemDto, OrderItem>();


        //    // Cart Mappings
        //    CreateMap<CartItem, CartItemDto>()
        //        .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
        //        .ForMember(dest => dest.ProductImage, opt => opt.MapFrom(src => src.Product.ImageUrl))
        //        .ForMember(dest => dest.ProductPrice, opt => opt.MapFrom(src => src.Product.Price))
        //        .ForMember(dest => dest.FinalPrice, opt => opt.MapFrom(src => src.Product.FinalPrice))
        //        .ForMember(dest => dest.VendorShopName, opt => opt.MapFrom(src => src.Product.Vendor.ShopName))
        //        .ForMember(dest => dest.StockQuantity, opt => opt.MapFrom(src => src.Product.StockQuantity))
        //        .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.Product.IsActive));
        //}
    }
}