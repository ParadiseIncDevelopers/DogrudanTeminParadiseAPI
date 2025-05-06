using AutoMapper;
using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Helpers;
using DogrudanTeminParadiseAPI.Models;

namespace DogrudanTeminParadiseAPI.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateUserDto, AdminUser>();
            CreateMap<AdminUser, AdminUserDto>();

            CreateMap<User, UserDto>();
            CreateMap<CreateUserDto, User>();

            CreateMap<CreateEntrepriseDto, Entreprise>();
            CreateMap<Entreprise, EntrepriseDto>();

            CreateMap<CreateProductItemDto, ProductItem>();
            CreateMap<ProductItem, ProductItemDto>();

            CreateMap<CreateProductDto, Product>();
            CreateMap<Product, ProductDto>();

            CreateMap<CreateAdministrationUnitDto, AdministrationUnit>();
            CreateMap<AdministrationUnit, AdministrationUnitDto>();

            CreateMap<CreateSubAdministrationUnitDto, SubAdministrationUnit>();
            CreateMap<SubAdministrationUnit, SubAdministrationUnitDto>();

            CreateMap<CreateThreeSubAdministrationUnitDto, ThreeSubAdministrationUnit>();
            CreateMap<ThreeSubAdministrationUnit, ThreeSubAdministrationUnitDto>();

            CreateMap<CreateProcurementEntryDto, ProcurementEntry>();
            CreateMap<ProcurementEntry, ProcurementEntryDto>();

            CreateMap<CreateTitleDto, Title>();
            CreateMap<Title, TitleDto>();
            CreateMap<UpdateTitleDto, Title>();

            CreateMap<CreateProcurementListItemDto, ProcurementListItem>();
            CreateMap<ProcurementListItem, ProcurementListItemDto>();

            CreateMap<CreateUnitDto, Unit>();
            CreateMap<Unit, UnitDto>();
            CreateMap<UpdateUnitDto, Unit>();

            CreateMap<CreateOfferItemDto, OfferItem>();
            CreateMap<OfferItem, OfferItemDto>();

            CreateMap<CreateOfferLetterDto, OfferLetter>();
            CreateMap<OfferLetter, OfferLetterDto>();

            CreateMap<CreateCategoryDto, Category>();
            CreateMap<UpdateCategoryDto, Category>();
            CreateMap<Category, CategoryDto>();

            CreateMap<CreateBudgetRecordDto, BudgetRecord>();
            CreateMap<BudgetRecord, BudgetRecordDto>();
            CreateMap<CreateBudgetItemDto, BudgetItem>();
            CreateMap<UpdateBudgetItemDto, BudgetItem>();
            CreateMap<BudgetItem, BudgetItemDto>();
        }
    }
}
