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
        }
    }
}
