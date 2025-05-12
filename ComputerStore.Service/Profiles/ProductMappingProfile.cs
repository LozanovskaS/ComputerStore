using AutoMapper;
using ComputerStore.Data.Entities;
using ComputerStore.Service.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerStore.Service.Profiles
{
    public class ProductMappingProfile : Profile
    {
        public ProductMappingProfile()
        {
            CreateMap<Product, ProductDTO>()
               .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.Category))
               .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));

            CreateMap<ProductDTO, Product>()
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Categories))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));


            CreateMap<StockImportDTO, Product>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src =>
                    src.Categories.Select(c => new Category { Name = c }).ToList()));
        }
    }
 }
