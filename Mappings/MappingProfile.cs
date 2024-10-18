using AutoMapper;
using krist_server.DTO.AuthDTOs;
using krist_server.DTO.ProductDTOs;
using krist_server.Models;

namespace krist_server.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateProductDto, Product>().ReverseMap();
            CreateMap<UpdateProductDto, Product>().ReverseMap();
            CreateMap<ProductDto, Product>().ReverseMap();
            // CreateMap<Product, ProductDto>();

            CreateMap<RegisterDTO, User>().ReverseMap();
            CreateMap<LoginDTO, User>().ReverseMap();
            CreateMap<UserDTO, User>().ReverseMap();
            // CreateMap<User, UserDTO>();
        }
    }
}