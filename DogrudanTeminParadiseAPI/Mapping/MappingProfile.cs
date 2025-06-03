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

            CreateMap<UpdateOfferItemDto, OfferItem>();
            CreateMap<OfferItem, OfferItemDto>();

            CreateMap<CreateOfferLetterDto, OfferLetter>();
            CreateMap<OfferLetter, OfferLetterDto>();

            CreateMap<UpdateOfferLetterDto, OfferLetter>();
            CreateMap<OfferLetter, OfferLetterDto>();

            CreateMap<CreateCategoryDto, Category>();
            CreateMap<UpdateCategoryDto, Category>();
            CreateMap<Category, CategoryDto>();

            CreateMap<CreateBudgetRecordDto, BudgetRecord>();
            CreateMap<BudgetRecord, BudgetRecordDto>();
            CreateMap<CreateBudgetItemDto, BudgetItem>();
            CreateMap<UpdateBudgetItemDto, BudgetItem>();
            CreateMap<BudgetItem, BudgetItemDto>();

            CreateMap<CreateMarketResearchJuryDto, MarketResearchJury>();
            CreateMap<MarketResearchJury, MarketResearchJuryDto>();
            CreateMap<UpdateMarketResearchJuryDto, MarketResearchJury>();

            CreateMap<CreateApproximateCostJuryDto, ApproximateCostJury>();
            CreateMap<ApproximateCostJury, ApproximateCostJuryDto>();
            CreateMap<UpdateApproximateCostJuryDto, ApproximateCostJury>();

            CreateMap<CreateInspectionAcceptanceJuryDto, InspectionAcceptanceJury>();
            CreateMap<InspectionAcceptanceJury, InspectionAcceptanceJuryDto>();
            CreateMap<UpdateInspectionAcceptanceJuryDto, InspectionAcceptanceJury>();

            CreateMap<CreateInspectionAcceptanceCertificateDto, InspectionAcceptanceCertificate>();
            CreateMap<UpdateInspectionAcceptanceCertificateDto, InspectionAcceptanceCertificate>();
            CreateMap<InspectionAcceptanceCertificate, InspectionAcceptanceCertificateDto>();

            CreateMap<CreateAdditionalInspectionAcceptanceCertificateDto, AdditionalInspectionAcceptanceCertificate>();
            CreateMap<UpdateAdditionalInspectionAcceptanceCertificateDto, AdditionalInspectionAcceptanceCertificate>();
            CreateMap<AdditionalInspectionAcceptanceCertificate, AdditionalInspectionAcceptanceCertificateDto>();

            CreateMap<CreateProcurementEntryEditorDto, ProcurementEntryEditor>();
            CreateMap<UpdateProcurementEntryEditorDto, ProcurementEntryEditor>();
            CreateMap<ProcurementEntryEditor, ProcurementEntryEditorDto>();

            // 1. Entity → DTO (decrypt / expose)
            CreateMap<SuperAdminUser, SuperAdminDto>()
                .ForMember(dest => dest.Name,
                           opt => opt.MapFrom(src => Crypto.Decrypt(src.Name)))
                .ForMember(dest => dest.Surname,
                           opt => opt.MapFrom(src => Crypto.Decrypt(src.Surname)))
                .ForMember(dest => dest.Email,
                           opt => opt.MapFrom(src => Crypto.Decrypt(src.Email)))
                .ForMember(dest => dest.Tcid,
                           opt => opt.MapFrom(src => Crypto.Decrypt(src.Tcid)))
                .ForMember(dest => dest.UserType,
                           opt => opt.MapFrom(src => src.UserType));

            // 2. DTO → Entity (encrypt / hash, Id ataması)
            CreateMap<CreateSuperAdminDto, SuperAdminUser>()
                .ForMember(dest => dest.Id,
                           opt => opt.MapFrom(_ => Guid.NewGuid()))
                .ForMember(dest => dest.Name,
                           opt => opt.MapFrom(src => Crypto.Encrypt(src.Name)))
                .ForMember(dest => dest.Surname,
                           opt => opt.MapFrom(src => Crypto.Encrypt(src.Surname)))
                .ForMember(dest => dest.Email,
                           opt => opt.MapFrom(src => Crypto.Encrypt(src.Email)))
                .ForMember(dest => dest.Tcid,
                           opt => opt.MapFrom(src => Crypto.Encrypt(src.Tcid)))
                .ForMember(dest => dest.Password,
                           opt => opt.MapFrom(src => Crypto.HashSha512(src.Password)))
                .ForMember(dest => dest.UserType,
                           opt => opt.MapFrom(_ => "SUPER_ADMIN"));

            // 3. Güncelleme DTO’su → Mevcut Entity 
            //    (UpdateAsync metodunda manuel güncelleme yaptığımız için burada map kullanmayacağız; 
            //     yine de bir örnek vermek gerekirse aşağıdaki gibi tanımlanabilir:)
            CreateMap<UpdateSuperAdminDto, SuperAdminUser>()
                .ForMember(dest => dest.Name,
                           opt => opt.MapFrom(src => Crypto.Encrypt(src.Name)))
                .ForMember(dest => dest.Surname,
                           opt => opt.MapFrom(src => Crypto.Encrypt(src.Surname)))
                // E-posta veya TC boş gelirse var olan kaydın değerini koru:
                .ForMember(dest => dest.Email,
                           opt => opt.Condition(src => !string.IsNullOrEmpty(src.Email)))
                .ForMember(dest => dest.Email,
                           opt => opt.MapFrom(src => Crypto.Encrypt(src.Email)))
                .ForMember(dest => dest.Tcid,
                           opt => opt.Condition(src => !string.IsNullOrEmpty(src.Tcid)))
                .ForMember(dest => dest.Tcid,
                           opt => opt.MapFrom(src => Crypto.Encrypt(src.Tcid)));
        }
    }
}
