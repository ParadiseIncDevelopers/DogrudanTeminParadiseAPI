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

            // Backup mappings
            CreateMap<CreateBackupInspectionAcceptanceCertificateDto, BackupInspectionAcceptanceCertificate>();
            CreateMap<BackupInspectionAcceptanceCertificate, BackupInspectionAcceptanceCertificateDto>();
            CreateMap<InspectionAcceptanceCertificate, BackupInspectionAcceptanceCertificate>();

            CreateMap<CreateBackupInspectionAcceptanceJuryDto, BackupInspectionAcceptanceJury>();
            CreateMap<BackupInspectionAcceptanceJury, BackupInspectionAcceptanceJuryDto>();
            CreateMap<InspectionAcceptanceJury, BackupInspectionAcceptanceJury>();

            CreateMap<CreateBackupAdditionalInspectionAcceptanceCertificateDto, BackupAdditionalInspectionAcceptanceCertificate>();
            CreateMap<BackupAdditionalInspectionAcceptanceCertificate, BackupAdditionalInspectionAcceptanceCertificateDto>();
            CreateMap<AdditionalInspectionAcceptanceCertificate, BackupAdditionalInspectionAcceptanceCertificate>();

            CreateMap<CreateBackupOfferLetterDto, BackupOfferLetter>();
            CreateMap<BackupOfferLetter, BackupOfferLetterDto>();
            CreateMap<OfferLetter, BackupOfferLetter>();
            
            CreateMap<CreateBackupProcurementEntryDto, BackupProcurementEntry>();
            CreateMap<BackupProcurementEntry, BackupProcurementEntryDto>();
            CreateMap<ProcurementEntry, BackupProcurementEntry>();

            CreateMap<CreateBackupProcurementEntryEditorDto, BackupProcurementEntryEditor>();
            CreateMap<BackupProcurementEntryEditor, BackupProcurementEntryEditorDto>();
            CreateMap<ProcurementEntryEditor, BackupProcurementEntryEditor>();
            
            CreateMap<OfferItemDto, SelectedOfferItem>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id == Guid.Empty ? Guid.NewGuid() : s.Id))
                .ForMember(d => d.Name, o => o.MapFrom(s => s.Name))
                .ForMember(d => d.Features, o => o.MapFrom(s => s.Features))
                .ForMember(d => d.Quantity, o => o.MapFrom(s => s.Quantity))
                .ForMember(d => d.UnitId, o => o.MapFrom(s => s.UnitId))
                .ForMember(d => d.UnitPrice, o => o.MapFrom(s => s.UnitPrice));

            CreateMap<CreateProcurementEntryEditorDto, ProcurementEntryEditor>();
            CreateMap<UpdateProcurementEntryEditorDto, ProcurementEntryEditor>();
            CreateMap<ProcurementEntryEditor, ProcurementEntryEditorDto>();

            CreateMap<SuperAdminUser, SuperAdminDto>()
                .ForMember(dest => dest.UserType, opt => opt.MapFrom(src => src.UserType));
            CreateMap<CreateSuperAdminDto, SuperAdminUser>()
                .ForMember(dst => dst.UserType, opt => opt.Ignore())
                .ForMember(dst => dst.ActivePassiveUsers, opt => opt.Ignore())
                .ForMember(dst => dst.AssignPermissionToAdmin, opt => opt.Ignore());

            CreateMap<CreateInspectionAcceptanceNoteDto, InspectionAcceptanceNote>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ProcurementEntryId, opt => opt.MapFrom(src => src.ProcurementEntryId.ToString()));
            CreateMap<UpdateInspectionAcceptanceNoteDto, InspectionAcceptanceNote>()
                .ForMember(dest => dest.ProcurementEntryId, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<InspectionAcceptanceNote, InspectionAcceptanceNoteDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.Parse(src.Id)))
                .ForMember(dest => dest.ProcurementEntryId, opt => opt.MapFrom(src => Guid.Parse(src.ProcurementEntryId)));

            CreateMap<CreateUserOwnFeaturesListDto, UserOwnFeaturesList>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId.ToString()));
            CreateMap<CreateUserFeaturesDto, UserFeatures>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<FeatureDto, Feature>();
            CreateMap<UpdateUserOwnFeaturesListDto, UserOwnFeaturesList>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore());
            CreateMap<UpdateUserFeaturesDto, UserFeatures>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()));
            CreateMap<UserOwnFeaturesList, UserOwnFeaturesListDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.Parse(src.Id)))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => Guid.Parse(src.UserId)));
            CreateMap<UserFeatures, UserFeaturesDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.Parse(src.Id)));
            CreateMap<Feature, FeatureDto>();

            CreateMap<CreateDecisionNumbersDto, DecisionNumbers>();
            CreateMap<UpdateDecisionNumbersDto, DecisionNumbers>();
            CreateMap<DecisionNumbers, DecisionNumbersDto>();

            CreateMap<CreateSharedProcurementEntryDto, SharedProcurementEntry>();
            CreateMap<SharedProcurementEntry, SharedProcurementEntryDto>();

            CreateMap<CreateUserNotificationDto, UserNotification>();
            CreateMap<UpdateUserNotificationDto, UserNotification>();
            CreateMap<UserNotification, UserNotificationDto>();
        }
    }
}
