using AutoMapper;
using AutoMapper.QueryableExtensions;
using Infocenter.Application.Interfaces;
using Infocenter.Application.ViewModels;
using Infocenter.Domain.Commands;
using Infocenter.Domain.Core.Bus;
using Infocenter.Domain.ExternalModels;
using Infocenter.Domain.Interfaces;
namespace Infocenter.Application.Services
{
    public class IOperationApplicationService : IBusinessPartnerAppService
    {
        private readonly IMapper _mapper;
        private readonly IBusinessPartnerRepository _businessPartnerRepository;
        private readonly ICreditLimitExtService _creditLimitExtService;
        private readonly IRegionRepository _regionRepository;
        private readonly ICountryRepository _countryRepository;
        private readonly ILogImportErrorAppService _logImportErrorAppService;
        private readonly ICompanyRepository _companyRepository;
        private readonly IBusinessPartnerAccountGroupRepository _businessPartnerAccountGroupRepository;
        private readonly IStatusBusinessPartnerRepository _statusBusinessPartnerRepository;
        private readonly ITransportationZoneRepository _transportationZoneRepository;
        private readonly ISalesOrderRepository _salesOrderRepository;
        private readonly ISalesManagementRepository _salesManagementRepository;
        private readonly IOpenInvoicesRepository _openInvoicesRepository;
        private readonly IBusinessPartnerPremiumServiceRepository _businessPartnerPremiumServiceRepository;
        private readonly ICompanyAppService _companyAppService;
        private readonly IBusinessPartnerBrRepository _businessPartnerBrRepository;
        private readonly IUser _loggedUser;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly IUser _user;
        private readonly ISalesOrganizationRepository _salesOrganizationRepository;
        private readonly IBusinessPartnerSalesPartnerAppService _businessPartnerSalesPartnerAppService;


        private List<Country> _listCountry;
        private List<Region> _listRegion;
        private List<BusinessPartner> _listBusinessPartners;
        private Company _company;
        private BusinessPartnerAccountGroup _businessPartnerAccountGroup;
        private StatusBusinessPartner _statusBusinessPartner;
        private List<TransportationZone> _listTransportationZone;
        private readonly ILogger _logger;

        private readonly IMediatorHandler Bus;

        public BusinessPartnerAppService(IMapper mapper,
                                  IBusinessPartnerRepository businessPartnerRepository,
                                  IMediatorHandler bus,
                                  ICreditLimitExtService creditLimitExtService,
                                  IRegionRepository regionRepository,
                                  ICountryRepository countryRepository,
                                  ICompanyRepository companyRepository,
                                  IBusinessPartnerAccountGroupRepository businessPartnerAccountGroupRepository,
                                  IStatusBusinessPartnerRepository statusBusinessPartnerRepository,
                                  ILogImportErrorAppService logImportErrorAppService,
                                  ILoggerFactory loggerFactory,
                                  ITransportationZoneRepository transportationZoneRepository,
                                  ISalesOrderRepository salesOrderRepository,
                                  ISalesManagementRepository salesManagementRepository,
                                  IOpenInvoicesRepository openInvoicesRepository,
                                  IStringLocalizer<SharedResource> localizer,
                                  IUser user,
								  ICompanyAppService companyAppService,
								  IUser loggedUser,
                                  IBusinessPartnerPremiumServiceRepository businessPartnerPremiumServiceRepository,
                                  IBusinessPartnerBrRepository businessPartnerBrRepository,
                                  ISalesOrganizationRepository salesOrganizationRepository,
                                  IBusinessPartnerSalesPartnerAppService businessPartnerSalesPartnerAppService
            )
        {
            _mapper = mapper;
            _businessPartnerRepository = businessPartnerRepository;
            Bus = bus;
            _creditLimitExtService = creditLimitExtService;
            _regionRepository = regionRepository;
            _countryRepository = countryRepository;
            _logImportErrorAppService = logImportErrorAppService;
            _businessPartnerAccountGroupRepository = businessPartnerAccountGroupRepository;
            _statusBusinessPartnerRepository = statusBusinessPartnerRepository;
            _statusBusinessPartner = null;
            _companyRepository = companyRepository;
            _listCountry = new List<Country>();
            _listRegion = new List<Region>();
            _listBusinessPartners = new List<BusinessPartner>();
            _company = null;
            _businessPartnerAccountGroup = null;
            _logger = loggerFactory.CreateLogger<BusinessPartnerAppService>();
            _transportationZoneRepository = transportationZoneRepository;
            _salesOrderRepository = salesOrderRepository;
            _salesManagementRepository = salesManagementRepository;
            _openInvoicesRepository = openInvoicesRepository;
            _localizer = localizer;
            _user = user;
            _loggedUser = loggedUser;
            _companyAppService = companyAppService;
            _businessPartnerPremiumServiceRepository = businessPartnerPremiumServiceRepository;
            _businessPartnerBrRepository = businessPartnerBrRepository;
            _salesOrganizationRepository = salesOrganizationRepository;
            _businessPartnerSalesPartnerAppService = businessPartnerSalesPartnerAppService;
        }

        public IQueryable<BusinessPartnerViewModel> GetAll()
        {
            return _businessPartnerRepository.GetAll().ProjectTo<BusinessPartnerViewModel>(_mapper.ConfigurationProvider);
        }

        public IEnumerable<BusinessPartnerViewModel> GetBusinessPartnerByPurchaseGroupId(Guid purchaseGroupid)
        {
            var data = _businessPartnerRepository.GetBusinessPartnerByPurchaseGroupId(purchaseGroupid).AsEnumerable();
            return _mapper.Map<IEnumerable<BusinessPartnerViewModel>>(data);
        }

        public IEnumerable<BusinessPartnerViewModel> GetBusinessPartnersCommision()
        {
            var data = _businessPartnerRepository.GetBusinessPartnersCommision();
            return _mapper.Map<IEnumerable<BusinessPartnerViewModel>>(data).OrderBy(x => x.Name);
        }

        public BusinessPartnerViewModel GetBusinessPartnerHeaderCommisionById(Guid SalesRepresentativeId)
        {
            return _mapper.Map<BusinessPartnerViewModel>(_businessPartnerRepository.GetBusinessPartnerHeaderCommisionById(SalesRepresentativeId));
        }

        public BusinessPartnerViewModel GetById(Guid id)
        {
            var bp = _businessPartnerRepository.GetAllAsNoTracking()
                .Include(c => c.SalesArea)
                .ThenInclude(c => c.Division)
                .Include(c => c.SalesArea)
                .ThenInclude(c => c.SalesOrganization)
                .Include(c => c.SalesArea)
                .ThenInclude(c => c.DistributionChannel)
                .Include(c => c.CustomerGroup1)
                .Include(c => c.CustomerGroup2)
                .Include(c => c.CustomerGroup3)
                .Include(c => c.SalesOffice)
                .Include(c => c.SalesGroup)
                .Include(c => c.SalesDistrict)
                .Include(c => c.PaymentTerm)
                .Include(c => c.Region)
                .Include(c => c.Country)
                .Include(c => c.TransportationZone)
                .Include(c => c.InfocenterChannel)
                .Include(c => c.PurchaseGroup)
                .Include(c => c.CustomerTieringTier)
                .Where(c => c.Id == id).FirstOrDefault();
            return _mapper.Map<BusinessPartnerViewModel>(bp);
        }

        public BusinessPartnerViewModel FirstOrDefaultByCode(string code)
        {
            return _mapper.Map<BusinessPartnerViewModel>(_businessPartnerRepository.FirstOrDefault(c => c.Code == code));
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public BusinessPartnerViewModel GetBusinessPartnerByCode(string code)
        {
            return _mapper.Map<BusinessPartnerViewModel>(_businessPartnerRepository.GetBusinessPartnerByCode(code));
        }

        public BusinessPartnerViewModel GetUserProfileBusinessPartnerByCode(string code, Guid userId)
        {
            return _mapper.Map<BusinessPartnerViewModel>(_businessPartnerRepository.GetUserProfileBusinessPartnerByCode(code, userId));
        }

        public IQueryable<BusinessPartnerViewModel> GetAllSoldToByUserSalesman(Guid id)
        {
            var bps = _mapper.Map<IEnumerable<BusinessPartnerViewModel>>(_businessPartnerRepository.GetAllSoldToByUserSalesman(id));
            return bps.AsQueryable();
        }

        public IQueryable<BusinessPartnerViewModel> GetAllSoldToProfileByUserSalesman(Guid id)
        {
            var bps = _mapper.Map<IEnumerable<BusinessPartnerViewModel>>(_businessPartnerRepository.GetAllSoldToProfileByUserSalesman(id));
            return bps.AsQueryable();
        }

        public BusinessPartnerViewModel GetSoldToByUserSalesmanAndCode(Guid userId, string code)
        {
            return _mapper.Map<BusinessPartnerViewModel>(_businessPartnerRepository.GetSoldToByUserSalesmanAndCode(userId, code));
        }

        public BusinessPartnerViewModel GetBusinessPartnersCreditLimitByPurchaseGroupId(Guid PurchaseGroupId, string Code)
        {
            return _mapper.Map<BusinessPartnerViewModel>(_businessPartnerRepository.GetBusinessPartnersCreditLimitByPurchaseGroupId(PurchaseGroupId, Code));
        }

        public BusinessPartnerViewModel GetBusinessPartnersCreditLimitByBusinessPartnerId(Guid SoldToPartyId, string Code)
        {
            return _mapper.Map<BusinessPartnerViewModel>(_businessPartnerRepository.GetBusinessPartnersCreditLimitByBusinessPartnerId(SoldToPartyId, Code));
        }

        public IQueryable<BusinessPartnerViewModel> GetConsumerAddressByPurchaseGroupId(Guid PurchaseGroupId)
        {
            return _businessPartnerRepository.GetConsumerAddressByPurchaseGroupId(PurchaseGroupId).ProjectTo<BusinessPartnerViewModel>(_mapper.ConfigurationProvider);
        }

        public IQueryable<BusinessPartnerViewModel> GetConsumerAddressByBusinessPartnerId(Guid BusinessPartnerId)
        {
            return _businessPartnerRepository.GetConsumerAddressByBusinessPartnerId(BusinessPartnerId).ProjectTo<BusinessPartnerViewModel>(_mapper.ConfigurationProvider);
        }

        public IQueryable<BusinessPartnerViewModel> GetAllByBusinessPartnerAcountGroupCode(string businessPartnerAccountGroupCode)
        {
            return _businessPartnerRepository.GetAllByBusinessPartnerAcountGroupCode(businessPartnerAccountGroupCode).ProjectTo<BusinessPartnerViewModel>(_mapper.ConfigurationProvider);
        }

        public CreditLimitInquireModel GetCreditLimitByCustomer(string customerCode, string segment)
        {
            var iphostentry = Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
            string iporigem = iphostentry?.ToString();
            CreditLimitInquireModel byCustomer;


            if (iporigem == "10.98.240.7" || segment == "US01")
            {
                byCustomer = _creditLimitExtService.GetCreditLimitByCustomer_prd(customerCode, segment);
            }
            else
            {
                byCustomer = _creditLimitExtService.GetCreditLimitByCustomer_dev(customerCode, segment);
            }

            CreditLimitInquireModel creditLimitInquireModel = new CreditLimitInquireModel();

            creditLimitInquireModel.PreviousLimit = byCustomer.PreviousLimit;
            creditLimitInquireModel.Balance = byCustomer.Balance;
            creditLimitInquireModel.Overdue = byCustomer.Overdue;
            creditLimitInquireModel.NotOverdue = byCustomer.NotOverdue;

            return (creditLimitInquireModel);
        }
        public double GetInfocenterCreditLimitCommited(Guid BusinessPartnerId)
        {
            var businessPartnerCreditLimitId = _businessPartnerRepository.GetById(BusinessPartnerId).BusinessPartnerCreditLimitId;

            var bps = new List<Guid>() { BusinessPartnerId };

            if (businessPartnerCreditLimitId != null)
            {
                bps = _businessPartnerRepository.GetBpsByBpCreditLimitId(businessPartnerCreditLimitId.GetValueOrDefault());
            }

            var openOrdersValue = _salesManagementRepository.GetGrossValueOpenOrdersSap(bps);     // Busca do Gestão os pedidos no status P e A de todos os codigos desses bp.
            var infocenterOrders = _salesOrderRepository.GetGrossValueOpenInfocenterOrders(bps);  // Ordens de vendas que não foram enviadas para o SAP.
            var todayOrders = _salesOrderRepository.GetGrossValueInfocenterOrdersSentToday(bps);  // Ordens de vendas que foram enviadas hoje e já tenham código sap
            var openDuplicates = _openInvoicesRepository.GetValueOpenDuplicates(bps);             // Duplicatas em aberto.

            var total = openOrdersValue + infocenterOrders + todayOrders + openDuplicates;

            return total;
        }


        public IQueryable<BusinessPartnerViewModel> GetAllByAcountGroupId(Guid businessPartnerAccountGroupId)
        {
            return _businessPartnerRepository.GetWhere(c => c.BusinessPartnerAccountGroupId == businessPartnerAccountGroupId).ProjectTo<BusinessPartnerViewModel>(_mapper.ConfigurationProvider);
        }

        public IQueryable<Guid> GetSalesRepresentative()
        {
            return _businessPartnerRepository.GetSalesRepresentative();
        }

        public IQueryable<Guid> GetAllSoldTo()
        {
            return _businessPartnerRepository.GetAllSoldTo();
        }

        public IQueryable<Guid> GetBusinessPartnerBySoldTo(Guid salesRepresentative, Guid businessPartnerRoleId)
        {
            return _businessPartnerRepository.GetBusinessPartnerBySoldTo(salesRepresentative, businessPartnerRoleId);
        }

        public IEnumerable<BusinessPartnerFilterViewModel> GetAllByCodeFiscalIdNameType(string name, string code, string countryCode, string type, string fiscalId, bool canSeeAllCustomers, string legacyCode, string city, string zipCode, string infocenterChannel, Guid regionId, Guid countryId, bool empty = true) => 
            _mapper.Map<IEnumerable<BusinessPartnerFilterViewModel>>(_businessPartnerRepository.GetAllByCodeFiscalIdNameType(name, code, countryCode, type, fiscalId, canSeeAllCustomers, legacyCode, city, zipCode, infocenterChannel, regionId, countryId, empty));

        public IEnumerable<BusinessPartnerFilterViewModel> GetAllSoldToByCodeFiscalIdNameType(string name, string code, string countryCode, string type, string fiscalId, bool canSeeAllCustomers, string legacyCode, string city, string zipCode, string infocenterChannel, Guid regionId, Guid countryId) =>
            _mapper.Map<IEnumerable<BusinessPartnerFilterViewModel>>(_businessPartnerRepository.GetAllSoldToByCodeFiscalIdNameType(name, code, countryCode, type, fiscalId, canSeeAllCustomers, legacyCode, city, zipCode, infocenterChannel, regionId, countryId));

        public IEnumerable<BusinessPartnerFilterViewModel> GetAllByCodeFiscalIdNameTypeAR10(string name, string code, string countryCode, string type, string fiscalId, bool canSeeAllCustomers, string legacyCode, string city, string zipCode, string infocenterChannel, Guid regionId, Guid countryId, bool empty = true) => 
            _mapper.Map<IEnumerable<BusinessPartnerFilterViewModel>>(_businessPartnerRepository.GetAllByCodeFiscalIdNameTypeAR10(name, code, countryCode, type, fiscalId, canSeeAllCustomers, legacyCode, city, zipCode, infocenterChannel, regionId, countryId, empty));

        public IQueryable<BusinessPartnerFilterViewModel> GetAllBusinessPartnerEmployee(string name, string code, string countryCode, string type, string fiscalId, bool canSeeAllCustomers, string legacyCode, string city, string zipCode, string infocenterChannel, Guid regionId, Guid countryId, bool empty = true) =>
            _businessPartnerRepository.GetAllBusinessPartnerEmployee(name, code, countryCode, type, fiscalId, canSeeAllCustomers, legacyCode, city, zipCode, infocenterChannel, regionId, countryId, empty).ProjectTo<BusinessPartnerFilterViewModel>(_mapper.ConfigurationProvider);

        public List<BusinessPartnerFilterViewModel> GetAllBusinessPartnerFilterByUserCompany(string name, string code, string countryCode, string type, string fiscalId, bool canSeeAllCustomers, string legacyCode, string city, string zipCode, string infocenterChannel, Guid regionId, Guid countryId, bool empty = true)
        {
            return _mapper.Map<List<BusinessPartnerFilterViewModel>>(_businessPartnerRepository.GetAllBusinessPartnerFilterByUserCompany(name, code, countryCode, type, fiscalId, canSeeAllCustomers, legacyCode, city, zipCode, infocenterChannel, regionId, countryId, empty));

        }
        public IQueryable<BusinessPartnerViewModel> GetAllByCodeFiscalIdName(string name, string code, string countryCode, string type, string fiscalId, bool canSeeAllCustomers, string legacyCode, string city, string zipCode, string infocenterChannel, Guid regionId, Guid countryId)
        {
            return _businessPartnerRepository.GetAllByCodeFiscalIdName(name, code, countryCode, type, fiscalId, canSeeAllCustomers, legacyCode, city, zipCode, infocenterChannel, regionId, countryId).ProjectTo<BusinessPartnerViewModel>(_mapper.ConfigurationProvider);
        }


        public IQueryable<BusinessPartnerViewModel> GetFiltered(string name, string code, string countryCode, string city, string zipCode, string infocenterChannel, string fiscalIdentification, bool customerOnly = false)
        {
            return _businessPartnerRepository.GetFiltered(name, code, countryCode, city, zipCode, infocenterChannel, fiscalIdentification, customerOnly).ProjectTo<BusinessPartnerViewModel>(_mapper.ConfigurationProvider);
        }

        public void Update(BusinessPartnerViewModel businessPartner)
        {
            var updateCommand = _mapper.Map<UpdateBusinessPartnerCommand>(businessPartner);
            Bus.SendCommand(updateCommand);
        }

        public void UpdateBusinessPartnerPurchaseGroup(List<BusinessPartnerViewModel> businessPartnerList, Guid? purchaseGroupId)
        {
            if (businessPartnerList == null)
            {
                throw new ArgumentNullException(nameof(businessPartnerList));
            }

            //Update all businesPartnerList for the purchaseGroupId
            foreach (var businessPartnerFiscalParent in businessPartnerList)
            {
                businessPartnerFiscalParent.PurchaseGroupId = purchaseGroupId;
                var updateCommand = _mapper.Map<UpdateBusinessPartnerCommand>(businessPartnerFiscalParent);
                Bus.SendCommand(updateCommand);
            }
        }

        public IQueryable<BusinessPartnerViewModel> GetAllByAcountGroupIdFiltered(List<Guid> businessPartnerAccountGroup, string code, string legacyCode, string name, bool canSeeAllBusinessPartner, List<Guid> listBusinessPartner)
        {
            return _businessPartnerRepository.GetAllByAcountGroupIdFiltered(businessPartnerAccountGroup, code, legacyCode, name, canSeeAllBusinessPartner, listBusinessPartner).ProjectTo<BusinessPartnerViewModel>(_mapper.ConfigurationProvider);
        }

        //public IQueryable<CustomersViewModel> GetAllByAcountGroupIdFilteredForContacts(List<Guid> businessPartnerAccountGroup, string code, string name, string fiscalIdentity, string channelCode, string salesRepresentativeCode, string city, Guid statusBusinessPartnerId, bool canSeeAllBusinessPartner, List<Guid> listBusinessPartner, string companyCode, string legacyCode, Guid countryId, Guid regionId)
        //{
        //    return _businessPartnerRepository.GetAllByAcountGroupIdFilteredForContacts(businessPartnerAccountGroup, code, name, fiscalIdentity, channelCode, salesRepresentativeCode, city, statusBusinessPartnerId, canSeeAllBusinessPartner, listBusinessPartner, companyCode, legacyCode,  countryId, regionId).ProjectTo<CustomersViewModel>(_mapper.ConfigurationProvider);
        //}

        public List<BusinessPartnerContactListViewModel> GetAllByAcountGroupIdFilteredForContacts(List<Guid> businessPartnerAccountGroup, string code, string name, string fiscalIdentity, string channelCode, string salesRepresentativeCode, string city, Guid statusBusinessPartnerId, bool canSeeAllBusinessPartner, List<Guid> listBusinessPartner, string companyCode, string legacyCode, Guid countryId, Guid regionId, Guid customerTieringTierId, Guid classificationId, Guid infocenterChannelId, Guid purchaseGroupId, int onlyContract)
        {
            return _mapper.Map<List<BusinessPartnerContactListViewModel>>(_businessPartnerRepository.GetAllByAcountGroupIdFilteredForContacts(businessPartnerAccountGroup, code, name, fiscalIdentity, channelCode, salesRepresentativeCode, city, statusBusinessPartnerId, canSeeAllBusinessPartner, listBusinessPartner, companyCode, legacyCode, countryId, regionId, customerTieringTierId, classificationId, infocenterChannelId, purchaseGroupId, onlyContract));
        }

        public BusinessPartnerViewModel GetByIdAllRelationsShip(Guid id)
        {
            return _mapper.Map<BusinessPartnerViewModel>(_businessPartnerRepository.GetById(id));
        }

        public IQueryable<BusinessPartnerViewModel> GetAllSoldToBySalesRepresentative(string salesRepresentativeCode)
        {
            return _businessPartnerRepository.GetAllSoldToBySalesRepresentative(salesRepresentativeCode).ProjectTo<BusinessPartnerViewModel>(_mapper.ConfigurationProvider);
        }

        public IQueryable<BusinessPartnerViewModel> GetAllSoldToByAreaManagerCode(string areaManagerCode)
        {
            return _businessPartnerRepository.GetAllSoldToByAreaManagerCode(areaManagerCode).ProjectTo<BusinessPartnerViewModel>(_mapper.ConfigurationProvider);
        }
        public IQueryable<BusinessPartnerViewModel> GetAllSoldToByNationalManagerCode(string nationalManagerCode)
        {
            return _businessPartnerRepository.GetAllSoldToByNationalManagerCode(nationalManagerCode).ProjectTo<BusinessPartnerViewModel>(_mapper.ConfigurationProvider);
        }

        public IQueryable<Guid> GetEmailApprovalCreditLimit(string code, string bpRoleCode)
        {
            return _businessPartnerRepository.GetEmailApprovalCreditLimit(code, bpRoleCode);
        }

        public IQueryable<BusinessPartnerViewModel> GetSalesRepresentativeExternal()
        {
            return _businessPartnerRepository.GetSalesRepresentativeExternal().ProjectTo<BusinessPartnerViewModel>(_mapper.ConfigurationProvider);
        }


        public IQueryable<BusinessPartnerSelectViewModel> GetAtiveSalesRepresentative(string companyCode)
        {
            var ativeSalesRepresentative = _businessPartnerRepository.GetAtiveSalesRepresentative(companyCode);
            var _return = ativeSalesRepresentative.ProjectTo<BusinessPartnerSelectViewModel>(_mapper.ConfigurationProvider);
            return _return;
        }

        public IEnumerable<BusinessPartnerViewModel> GetListSalesRepresentativeByListId(List<Guid> listId)
        {
            return _businessPartnerRepository.GetListSalesRepresentativeByListId(listId).ProjectTo<BusinessPartnerViewModel>(_mapper.ConfigurationProvider);
        }

        public IEnumerable<BusinessPartnerViewModel> GetAllSalesRepresentative(List<Guid> userProfileCompanyList)
        {
            //return _businessPartnerRepository.GetAllSalesRepresentative(userProfileCompanyList).ProjectTo<BusinessPartnerViewModel>(_mapper.ConfigurationProvider);
            return _mapper.Map<IEnumerable<BusinessPartnerViewModel>>(_businessPartnerRepository.GetAllSalesRepresentative(userProfileCompanyList));
        }

        public IQueryable<BusinessPartnerViewModel> GetAllPartnerMembers(string headquarter)
        {
            return _businessPartnerRepository.GetAllPartnerMembers(headquarter).ProjectTo<BusinessPartnerViewModel>(_mapper.ConfigurationProvider);
        }

        public IQueryable<BusinessPartnerViewModel> GetBusinessGetOutOfMembers()
        {
            return _businessPartnerRepository.GetBusinessGetOutOfMembers().ProjectTo<BusinessPartnerViewModel>(_mapper.ConfigurationProvider);
        }

        public IQueryable<BusinessPartnerViewModel> GetAllSoldToByUserAreaManager(List<Guid> listSalesPartnerId)
        {
            return _businessPartnerRepository.GetAllSoldToByUserAreaManager(listSalesPartnerId).ProjectTo<BusinessPartnerViewModel>(_mapper.ConfigurationProvider);
        }

        public void PreProcess(ImportParams importParams)
        {
            if (importParams == null)
            {
                throw new ArgumentNullException(nameof(importParams));
            }

            _listCountry = _countryRepository.GetAllAsNoTracking().ToList();
            _listRegion = _regionRepository.GetAllAsNoTracking().ToList();
            _company = _companyRepository.GetById(importParams.CompanyId);
            _businessPartnerAccountGroup = _businessPartnerAccountGroupRepository.GetAllAsNoTracking().Where(c => c.Code == "FLVN").SingleOrDefault();
            _statusBusinessPartner = _statusBusinessPartnerRepository.GetAllAsNoTracking().Where(c => c.Code == "A").SingleOrDefault();

            if (importParams.Type == ImportInfocenterType.VendorBR10)
            {
                _listTransportationZone = _transportationZoneRepository.GetAllAsNoTracking().Where(c => c.CountryId == CountryViewModel.Brazil).ToList();
                _listBusinessPartners = _businessPartnerRepository.GetAllByAcountGroupId(_businessPartnerAccountGroup.Id).ToList();
            }

        }

        public BusinessPartnerViewModel SaveItem(ImportParams importParams)
        {
            if (importParams == null)
            {
                throw new ArgumentNullException(nameof(importParams));
            }

            //if (importParams?.Line.Substring(0, 1) != "|") return null;
            string[] payload = importParams.Line.Split("|");
            Guid? id = null;

            BusinessPartnerViewModel bp = null;

            if (importParams.Type == ImportInfocenterType.VendorBR10) bp = GetImportVendorBR10(importParams, payload, out id);

            return bp;
        }

        private BusinessPartnerViewModel GetImportVendorBR10(ImportParams importParams, string[] payload, out Guid? vendorId)
        {
            var code = ImportValidations.getString(payload[0]);
            var identification = ImportValidations.getString(payload[1]);
            var name = ImportValidations.getString(payload[2]);
            var nameAbbr = ImportValidations.getString(payload[3]);
            var countryCode = ImportValidations.getString(payload[4]);
            var regionCode = ImportValidations.getString(payload[5]);
            var ibge = countryCode != "BR" ? "" : ImportValidations.getString(payload[6]).Substring(3);
            var country = _listCountry.Where(c => c.Iso == countryCode).FirstOrDefault();
            var city = _listTransportationZone.Where(c => c.Code == countryCode + ibge).FirstOrDefault()?.Description;

            var bp = _listBusinessPartners.Where(c => c.Code == code).FirstOrDefault();
            var newBp = new BusinessPartnerViewModel();

            if (bp == null)
            {
                bp = _businessPartnerRepository.GetAllAsNoTracking().Where(c => c.Code == code).SingleOrDefault();

                if (bp == null)
                {
                    newBp.Id = Guid.NewGuid();
                    newBp.SalesAreaId = SalesAreaViewModel.BR10;
                    newBp.SalesGroupId = SalesGroupViewModel.Undefined;
                    newBp.SalesOfficeId = SalesOfficeViewModel.Others;
                    newBp.SalesDistrictId = SalesDistrictViewModel.Undefined;
                    newBp.OnlyContract = false;
                    newBp.Update = false;
                }
                else
                {
                    newBp = _mapper.Map<BusinessPartner, BusinessPartnerViewModel>(bp);
                    newBp.Id = bp.Id;
                    newBp.Update = true;
                    newBp.District = "";
                }
            }
            else
            {
                newBp = _mapper.Map<BusinessPartner, BusinessPartnerViewModel>(bp);
                newBp.Id = bp.Id;
                newBp.Update = true;
                newBp.District = "";
            }

            vendorId = newBp.Id;
            Guid countryId = Guid.Empty;

            if (country != null)
            {
                countryId = country.Id;
            }
            else
            {
                var log = new LogImportErrorViewModel()
                {
                    Id = Guid.NewGuid(),
                    Company = _company.Code,
                    Date = DateTime.UtcNow,
                    FileOrRotine = importParams.FileName,
                    Texto = importParams.Line,
                    Error = "Country not found"
                };

                _logImportErrorAppService.Register(log);
            }

            var region = _listRegion.Where(c => c.Code == regionCode && c.CountryId == countryId).SingleOrDefault();

            Guid regionId = Guid.Empty;

            if (region != null)
            {
                regionId = region.Id;
            }
            else
            {
                region = _listRegion.Where(c => c.Code == "ZZ" && c.CountryId == countryId).SingleOrDefault();
                if (region != null)
                {
                    regionId = region.Id;
                }

                var log = new LogImportErrorViewModel()
                {
                    Id = Guid.NewGuid(),
                    Company = _company.Code,
                    Date = DateTime.UtcNow,
                    FileOrRotine = importParams.FileName,
                    Texto = importParams.Line,
                    Error = "Region not found"
                };

                _logImportErrorAppService.Register(log);
            }

            newBp.RegionId = regionId;
            newBp.BusinessPartnerAccountGroupId = _businessPartnerAccountGroup.Id;
            newBp.CountryId = countryId;
            newBp.FiscalIdentification = identification;
            newBp.StatusBusinessPartnerId = _statusBusinessPartner.Id;
            newBp.Name = name;
            newBp.Code = code;
            newBp.PostalCode = ibge;

            if (city == null)
            {
                newBp.City = "";
            }
            else
            {
                newBp.City = city;
            }

            if (newBp.District == null)
            {
                newBp.District = "";
            }

            if (newBp.Street == null)
            {
                newBp.Street = "";
            }

            return newBp;
        }

        public void Register(BusinessPartnerViewModel businessPartner)
        {
            var RegisterCommand = _mapper.Map<RegisterNewBusinessPartnerCommand>(businessPartner);
            Bus.SendCommand(RegisterCommand);
        }

        public void PosProcess(ImportParams importParams)
        {

        }

        public void UpdateMany(BusinessPartnerViewModel[] obj)
        {
            BusinessPartner[] list = _mapper.Map<BusinessPartnerViewModel[], BusinessPartner[]>(obj);
            _businessPartnerRepository.UpdateMany(list);
        }

        public void RegisterMany(BusinessPartnerViewModel[] obj)
        {
            BusinessPartner[] list = _mapper.Map<BusinessPartnerViewModel[], BusinessPartner[]>(obj);
            _businessPartnerRepository.RegisterMany(list);
        }

        public IQueryable<BusinessPartnerViewModel> GetAllSoldToBySoldToChildOfGroup(string code)
        {
            return _businessPartnerRepository.GetAllSoldToBySoldToChildOfGroup(code).ProjectTo<BusinessPartnerViewModel>(_mapper.ConfigurationProvider);
        }

        public List<Guid> GetSalesRepresentativeBySalesPartner(Guid salesPartnerId)
        {

            return _businessPartnerRepository.GetSalesRepresentativeBySalesPartner(salesPartnerId);
        }
        public IEnumerable<BusinessPartnerViewModel> GetSalesRepresentativeBySalesOrderTypeId(Guid salesOrderTypeId, string status)
        {
            return _mapper.Map<IEnumerable<BusinessPartnerViewModel>>(_businessPartnerRepository.GetSalesRepresentativeBySalesOrderTypeId(salesOrderTypeId, status));
        }
        public BusinessPartnerViewModel GetSalesRepresentativeByBusinessPartner(Guid businessPartnerId)
        {
            return _mapper.Map<BusinessPartnerViewModel>(_businessPartnerRepository.GetSalesRepresentativeByBusinessPartner(businessPartnerId));
        }
        public BusinessPartnerViewModel GetRegionalManagerByBusinessPartner(Guid businessPartnerId)
        {
            return _mapper.Map<BusinessPartnerViewModel>(_businessPartnerRepository.GetRegionalManagerByBusinessPartner(businessPartnerId));
        }
        public BusinessPartnerViewModel GetNacionalManagerByBusinessPartner(Guid businessPartnerId)
        {
            return _mapper.Map<BusinessPartnerViewModel>(_businessPartnerRepository.GetNacionalManagerByBusinessPartner(businessPartnerId));
        }

        public IQueryable<BusinessPartnerViewModel> GetSalesBusinessPartner(Guid userId)
        {
            return _businessPartnerRepository.GetSalesBusinessPartner(userId).ProjectTo<BusinessPartnerViewModel>(_mapper.ConfigurationProvider);
        }

        public IEnumerable<BusinessPartnerSelectViewModel> GetSelectShipTo()
        {
            var countryId = _user.GetUserCountryId();

            var data = _businessPartnerRepository
                .GetWhere(c => c.BusinessPartnerAccountGroupId == BusinessPartnerAccountGroupViewModel.ShipToParty && c.CountryId == countryId)
                .Select(c => new BusinessPartnerSelectViewModel() { Id = c.Id, Name = c.Code + " - " + c.Name})
                .ToList()
                .OrderBy(c => c.Name)
                .ToList();

            data.Insert(0, new BusinessPartnerSelectViewModel() { Id = Guid.Empty, Name = _localizer["Select"] });

            return data;
        }

        public IEnumerable<BusinessPartnerViewModel> GetAllShipToBySoldTo(string code, string search)
        {
            var list = _businessPartnerSalesPartnerAppService.GetAllSalesPartnerByBusinessPartnerCodeByBusinessPartnerRoleCode(code, "WE").OrderBy(x => x.Name);

            if(search != "")
            {
                list = list.Where(c => c.Name.Contains(search) || c.Code.Contains(search)).OrderBy(x => x.Name);
            }

            return list;
        }

        public BusinessPartnerViewModel GetBusinessPartnerByCodeAndCompanyAR(string code)
        {
            return _mapper.Map<BusinessPartnerViewModel>(_businessPartnerRepository.GetBusinessPartnerByCode(code));
        }

        public List<BusinessPartnerReportViewModel> GetBusinessPartnetReport(Guid? companyId, string businessPartnerCode, string salesRepresentativeCode, Guid? infocenterChannelId, Guid? statusBusinessPartnerId)
        {
            var companyCode = _companyRepository.GetAllAsNoTracking().Where(w=> w.Id == companyId).Select(s=> s.Code).FirstOrDefault();
            var salesOrganizationId = _salesOrganizationRepository.GetAllAsNoTracking().Where(w => w.Code == companyCode).Select(s=> s.Id).ToList();

            var list = (from bp in _businessPartnerRepository.GetAllAsNoTracking()
                        .Include(c => c.StatusBusinessPartner)
                        .Include(c => c.Channel)
                        .Include(c => c.PriceCustomerGroup)
                        .Include(c => c.InfocenterChannel)
                        .Include(c => c.PaymentTerm)
                        .Include(c => c.Region)
                        .Include(c => c.SalesOffice)
                        .Include(c => c.Plant)
                        .Include(c => c.Incoterms)
                        .Include(c => c.Region)
                        .Include(c => c.SalesArea)
                        .Where(w => w.SalesArea.SalesOrganizationId != Guid.Parse("9201DD4B-702B-4E03-AE64-0325079F4DFE") /*Diferente de: BR10*/&&
                               salesOrganizationId.Contains(w.SalesArea.SalesOrganizationId))
                       select new BusinessPartnerReportViewModel
                       {
                           Id = bp.Id,
                           Code = bp.Code,
                           Name = bp.Name,
                           Status = bp.StatusBusinessPartner.Description,
                           FiscalIdentification = bp.FiscalIdentification,
                           ChannelCode = bp.InfocenterChannel.ChannelCode,
                           ChannelDescription = bp.InfocenterChannel.ChannelDescription,
                           TypeCode = bp.InfocenterChannel.TypeCode,
                           TypeDescription = bp.InfocenterChannel.TypeDescription,
                           PriceCustomerGroupDescription = bp.PriceCustomerGroup.Description,
                           PaymentTermCode = bp.PaymentTerm.Code,
                           PaymentTermDescription = bp.PaymentTerm.Description,
                           CreditLimit = bp.CreditLimit,
                           Street = bp.Street,
                           PostalCode = bp.PostalCode,
                           City = bp.City,
                           Region = bp.Region.Description,
                           SalesOffice = bp.SalesOffice.Code,
                           CreateDate = bp.CreateDate,
                           PlantCode = bp.Plant.Code,
                           PlantDescription = bp.Plant.Description,
                           IncotermsCode = bp.Incoterms.Code,
                           IncotermsDescription = bp.Incoterms.Description,
                           InfocenterChannelId = bp.InfocenterChannelId,
                           CompanyId = bp.InfocenterChannel.CompanyId,
                           StatusBusinessPartnerId = bp.StatusBusinessPartnerId
                       }).ToList();


            if (infocenterChannelId != Guid.Empty && infocenterChannelId != null)
            {
                list = list.Where(c => c.InfocenterChannelId == infocenterChannelId).ToList();
            }

            if (statusBusinessPartnerId != Guid.Empty && statusBusinessPartnerId != null)
            {
                list = list.Where(c => c.StatusBusinessPartnerId == statusBusinessPartnerId).ToList();
            }

            if (!string.IsNullOrWhiteSpace(businessPartnerCode))
            {
                list = list.Where(c => c.Code == businessPartnerCode).ToList();
            }

            foreach (var item in list)
            {
                var salesRepresentative = GetSalesRepresentativeByBusinessPartner(item.Id);
                var regionalManager = GetRegionalManagerByBusinessPartner(item.Id);

                if (salesRepresentative != null)
                {
                    item.SalesRepresentativeCode = salesRepresentative.Code;
                    item.SalesRepresentativeName = salesRepresentative.Name;
                }

                if (regionalManager != null)
                {
                    item.SalesManagerCode = regionalManager.Code;
                    item.SalesManagerOffice = regionalManager.Name;
                }
            }

            if (!string.IsNullOrWhiteSpace(salesRepresentativeCode))
            {
                list = list.Where(c => c.SalesRepresentativeCode == salesRepresentativeCode).ToList();
            }


            return list.OrderBy(x => x.Name).ToList();
        }

        public MemoryStream GetExcelBusinessPartnetReport(Guid? companyId, string businessPartnerCode, string salesRepresentativeCode, Guid? infocenterChannelId, Guid? statusBusinessPartnerId)
        {

            var memory = new MemoryStream();

            var BusinessPartnetReport = GetBusinessPartnetReport(companyId, businessPartnerCode, salesRepresentativeCode, infocenterChannelId, statusBusinessPartnerId).ToList();

            //Create file excel
            IWorkbook workbook = new XSSFWorkbook();
            var SheetName = ("BusinessPartnetReport");
            ISheet sheet = workbook.CreateSheet(SheetName);

            int rowNumer = 0;

            IRow row = sheet.CreateRow(rowNumer);
            ICell cell;

            ICellStyle styleHeader = workbook.CreateCellStyle();
            styleHeader.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Grey25Percent.Index;
            styleHeader.FillPattern = FillPattern.SolidForeground;

            ICellStyle cellDateStyle = workbook.CreateCellStyle();
            //DateTimeFormatInfo dtfi = CultureInfo.InstalledUICulture.DateTimeFormat;
            DateTimeFormatInfo dtfi = CultureInfo.CreateSpecificCulture(CultureInfo.CurrentUICulture.Name).DateTimeFormat;
            IDataFormat dataFormatCustom = workbook.CreateDataFormat();
            cellDateStyle.DataFormat = dataFormatCustom.GetFormat(dtfi.ShortDatePattern);

            cell = row.CreateCell(0);
            cell.SetCellValue("Code");
            cell.CellStyle = styleHeader;

            cell = row.CreateCell(1);
            cell.SetCellValue("Name");
            cell.CellStyle = styleHeader;

            cell = row.CreateCell(2);
            cell.SetCellValue("Status");
            cell.CellStyle = styleHeader;

            cell = row.CreateCell(3);
            cell.SetCellValue("Fiscal Identification");
            cell.CellStyle = styleHeader;

            cell = row.CreateCell(4);
            cell.SetCellValue("Channel Code");
            cell.CellStyle = styleHeader;

            cell = row.CreateCell(5);
            cell.SetCellValue("Channel Description");
            cell.CellStyle = styleHeader;

            cell = row.CreateCell(6);
            cell.SetCellValue("Type Code");
            cell.CellStyle = styleHeader;

            cell = row.CreateCell(7);
            cell.SetCellValue("Type Description");
            cell.CellStyle = styleHeader;

            cell = row.CreateCell(8);
            cell.SetCellValue("Grupo de Precios");
            cell.CellStyle = styleHeader;

            cell = row.CreateCell(9);
            cell.SetCellValue("Payment Term Code");
            cell.CellStyle = styleHeader;

            cell = row.CreateCell(10);
            cell.SetCellValue("Payment Term Description");
            cell.CellStyle = styleHeader;

            cell = row.CreateCell(11);
            cell.SetCellValue("Credit Limit");
            cell.CellStyle = styleHeader;

            cell = row.CreateCell(12);
            cell.SetCellValue("Sales Representative Code");
            cell.CellStyle = styleHeader;

            cell = row.CreateCell(13);
            cell.SetCellValue("Sales Representative Name");
            cell.CellStyle = styleHeader;

            cell = row.CreateCell(14);
            cell.SetCellValue("Sales Manager Code");
            cell.CellStyle = styleHeader;

            cell = row.CreateCell(15);
            cell.SetCellValue("Sales Manager Office");
            cell.CellStyle = styleHeader;

            cell = row.CreateCell(16);
            cell.SetCellValue("Street");
            cell.CellStyle = styleHeader;

            cell = row.CreateCell(17);
            cell.SetCellValue("Postal Code");
            cell.CellStyle = styleHeader;

            cell = row.CreateCell(18);
            cell.SetCellValue("City");
            cell.CellStyle = styleHeader;

            cell = row.CreateCell(19);
            cell.SetCellValue("Provincia");
            cell.CellStyle = styleHeader;

            cell = row.CreateCell(20);
            cell.SetCellValue("Sales Office");
            cell.CellStyle = styleHeader;

            cell = row.CreateCell(21);
            cell.SetCellValue("Create Date");
            cell.CellStyle = styleHeader;

            cell = row.CreateCell(22);
            cell.SetCellValue("Plant Code");
            cell.CellStyle = styleHeader;

            cell = row.CreateCell(23);
            cell.SetCellValue("Plant Description");
            cell.CellStyle = styleHeader;

            cell = row.CreateCell(24);
            cell.SetCellValue("Inconterms Code");
            cell.CellStyle = styleHeader;

            cell = row.CreateCell(25);
            cell.SetCellValue("Inconterms Description");
            cell.CellStyle = styleHeader;


            foreach (var itens in BusinessPartnetReport)
            {

                rowNumer++;
                row = sheet.CreateRow(rowNumer);
                row.CreateCell(0).SetCellValue(!string.IsNullOrWhiteSpace(itens.Code) ? itens.Code : "");
                row.CreateCell(1).SetCellValue(!string.IsNullOrWhiteSpace(itens.Name) ? itens.Name : "");
                row.CreateCell(2).SetCellValue(!string.IsNullOrWhiteSpace(itens.Status) ? itens.Status : "");
                row.CreateCell(3).SetCellValue(!string.IsNullOrWhiteSpace(itens.FiscalIdentification) ? itens.FiscalIdentification : "");
                row.CreateCell(4).SetCellValue(!string.IsNullOrWhiteSpace(itens.ChannelCode) ? itens.ChannelCode : "");
                row.CreateCell(5).SetCellValue(!string.IsNullOrWhiteSpace(itens.ChannelDescription) ? itens.ChannelDescription : "");
                row.CreateCell(6).SetCellValue(!string.IsNullOrWhiteSpace(itens.TypeCode) ? itens.TypeCode : "");
                row.CreateCell(7).SetCellValue(!string.IsNullOrWhiteSpace(itens.TypeDescription) ? itens.TypeDescription : "");
                row.CreateCell(8).SetCellValue(!string.IsNullOrWhiteSpace(itens.PriceCustomerGroupDescription) ? itens.PriceCustomerGroupDescription : "");
                row.CreateCell(9).SetCellValue(!string.IsNullOrWhiteSpace(itens.PaymentTermCode) ? itens.PaymentTermCode : "");
                row.CreateCell(10).SetCellValue(!string.IsNullOrWhiteSpace(itens.PaymentTermDescription) ? itens.PaymentTermDescription : "");
                row.CreateCell(11).SetCellValue(Math.Round(itens.CreditLimit.GetValueOrDefault(), 2));
                row.CreateCell(12).SetCellValue(!string.IsNullOrWhiteSpace(itens.SalesRepresentativeCode) ? itens.SalesRepresentativeCode : "");
                row.CreateCell(13).SetCellValue(!string.IsNullOrWhiteSpace(itens.SalesRepresentativeName) ? itens.SalesRepresentativeName : "");
                row.CreateCell(14).SetCellValue(!string.IsNullOrWhiteSpace(itens.SalesManagerCode) ? itens.SalesManagerCode : "");
                row.CreateCell(15).SetCellValue(!string.IsNullOrWhiteSpace(itens.SalesManagerOffice) ? itens.SalesManagerOffice : "");
                row.CreateCell(16).SetCellValue(!string.IsNullOrWhiteSpace(itens.Street) ? itens.Street : "");
                row.CreateCell(17).SetCellValue(!string.IsNullOrWhiteSpace(itens.PostalCode) ? itens.PostalCode : "");
                row.CreateCell(18).SetCellValue(!string.IsNullOrWhiteSpace(itens.City) ? itens.City : "");
                row.CreateCell(19).SetCellValue(!string.IsNullOrWhiteSpace(itens.Region) ? itens.Region : "");
                row.CreateCell(20).SetCellValue(!string.IsNullOrWhiteSpace(itens.SalesOffice) ? itens.SalesOffice : "");
                row.CreateCell(21).SetCellValue(itens.CreateDate.HasValue ? itens.CreateDate.Value.ToString("dd/MM/yyyy") : "");
                row.CreateCell(22).SetCellValue(!string.IsNullOrWhiteSpace(itens.PlantCode) ? itens.PlantCode : "");
                row.CreateCell(23).SetCellValue(!string.IsNullOrWhiteSpace(itens.PlantDescription) ? itens.PlantDescription : "");
                row.CreateCell(24).SetCellValue(!string.IsNullOrWhiteSpace(itens.IncotermsCode) ? itens.IncotermsCode : "");
                row.CreateCell(25).SetCellValue(!string.IsNullOrWhiteSpace(itens.IncotermsDescription) ? itens.IncotermsDescription : "");

            }

            sheet.SetColumnWidth(0, 10 * 256);
            sheet.SetColumnWidth(1, 30 * 256);
            sheet.SetColumnWidth(2, 10 * 256);
            sheet.SetColumnWidth(3, 30 * 256);
            sheet.SetColumnWidth(4, 20 * 256);
            sheet.SetColumnWidth(5, 30 * 256);
            sheet.SetColumnWidth(6, 20 * 256);
            sheet.SetColumnWidth(7, 20 * 256);
            sheet.SetColumnWidth(8, 30 * 256);
            sheet.SetColumnWidth(9, 20 * 256);
            sheet.SetColumnWidth(10, 30 * 256);
            sheet.SetColumnWidth(11, 30 * 256);
            sheet.SetColumnWidth(12, 30 * 256);
            sheet.SetColumnWidth(13, 30 * 256);
            sheet.SetColumnWidth(14, 30 * 256);
            sheet.SetColumnWidth(15, 30 * 256);
            sheet.SetColumnWidth(16, 30 * 256);
            sheet.SetColumnWidth(17, 30 * 256);
            sheet.SetColumnWidth(18, 30 * 256);
            sheet.SetColumnWidth(19, 30 * 256);
            sheet.SetColumnWidth(20, 30 * 256);
            sheet.SetColumnWidth(21, 30 * 256);
            sheet.SetColumnWidth(22, 30 * 256);
            sheet.SetColumnWidth(23, 30 * 256);
            sheet.SetColumnWidth(24, 30 * 256);
            sheet.SetColumnWidth(25, 30 * 256);

            workbook.Write(memory);

            return memory;

        }

        public List<BusinessPartnerViewModel> GetSelect(Guid? companyId)
        {
            var companyCode = _companyRepository.GetWhere(c => c.Id.Equals(companyId)).First().Code;

            var data = _businessPartnerRepository
                .GetAll()
                .Include(x => x.SalesArea)
                .Where(c => c.BusinessPartnerAccountGroupId == BusinessPartnerAccountGroupViewModel.SoldToParty && c.SalesArea.Code == companyCode && c.StatusBusinessPartnerId == StatusBusinessPartnerViewModel.Active)
                .Select(c => new BusinessPartnerViewModel()
                {
                    Id = c.Id,
                    Name = c.Code + " - " + c.Name,
                })
                .ToList();

            data.Insert(0, new BusinessPartnerViewModel { Id = Guid.Empty, Name = _localizer["Select"] });

            return data;
        }

        public List<BusinessPartnerViewModel> GetSelect(Guid? companyId, string channelCode)
        {
            var companyCode = _companyRepository.GetWhere(c => c.Id.Equals(companyId)).First().Code;

            var data = _businessPartnerRepository
                .GetAll()
                .Include(x => x.SalesArea)
                .Where(c => c.BusinessPartnerAccountGroupId == BusinessPartnerAccountGroupViewModel.SoldToParty && c.SalesArea.Code == companyCode && c.InfocenterChannel.ChannelCode == channelCode && c.StatusBusinessPartnerId == StatusBusinessPartnerViewModel.Active)
                .Select(c => new BusinessPartnerViewModel()
                {
                    Id = c.Id,
                    Name = c.Code + " - " + c.Name,
                })
                .ToList();

            data.Insert(0, new BusinessPartnerViewModel { Id = Guid.Empty, Name = _localizer["Select"] });

            return data;
        }

        public List<BusinessPartnerViewModel> GetSelect(string search = "", string companyCode = "BR10")
        {
            var userId = _loggedUser.GetUserId();
            var businessPartnersIds = _businessPartnerRepository.GetAllSoldToByUserSalesman(userId).Select(c => c.Id).ToList();
            var permissionSalesRepresentative = _user.HasPermission("CanSeeAllSalesRepresentativeData") || _user.HasPermission("CanSeeAllCustomers");

            var data = _businessPartnerRepository
                .GetAll()
                .Include(x => x.SalesArea)
                .Where(c => 
                    c.BusinessPartnerAccountGroupId == BusinessPartnerAccountGroupViewModel.SoldToParty 
                    && c.SalesArea.Code == companyCode
                    && (!string.IsNullOrEmpty(search) ? (c.Code.Contains(search) || c.Name.Contains(search) || c.FiscalIdentification.Contains(search)) : 1 == 1)
                    && c.StatusBusinessPartnerId == StatusBusinessPartnerViewModel.Active
                    )                
                .Select(c => new BusinessPartnerViewModel()
                {
                    Id = c.Id,
                    Name = c.Name,
                    Code = c.Code,
                    FiscalIdentification = c.FiscalIdentification,
                    City = c.City
                });

            if (!permissionSalesRepresentative)
            {
                data = data.Where(c => businessPartnersIds.Contains(c.Id));
            }

            var dataViewModel = data.OrderBy(c => c.Name).ToList();

            dataViewModel.Insert(0, new BusinessPartnerViewModel { Id = Guid.Empty, Name = _localizer["Select"] });

            return dataViewModel;
        }

        public List<BusinessPartnerViewModel> GetSelectByAllCompanies(string search = "")
        {
            var userId = _loggedUser.GetUserId();
            var listCompanys = _companyAppService.GetCompaniesByUserId(userId).Select(x => x.Code).ToList();

            var businessPartnersIds = _businessPartnerRepository.GetAllSoldToByUserSalesman(userId).Select(c => c.Id).ToList();
            var permissionSalesRepresentative = _user.HasPermission("CanSeeAllSalesRepresentativeData") || _user.HasPermission("CanSeeAllCustomers");

            var data = _businessPartnerRepository
                .GetAll()
                .Include(x => x.SalesArea)
                .Where(c =>
                    c.BusinessPartnerAccountGroupId == BusinessPartnerAccountGroupViewModel.SoldToParty
                    && listCompanys.Contains(c.SalesArea.Code)
                    && (!string.IsNullOrEmpty(search) ? (c.Code.Contains(search) || c.Name.Contains(search) || c.FiscalIdentification.Contains(search)) : 1 == 1)
                    && c.StatusBusinessPartnerId == StatusBusinessPartnerViewModel.Active
                    )
                .Select(c => new BusinessPartnerViewModel()
                {
                    Id = c.Id,
                    Name = c.Name,
                    Code = c.Code,
                    FiscalIdentification = c.FiscalIdentification,
                    City = c.City
                });

            if (!permissionSalesRepresentative)
            {
                data = data.Where(c => businessPartnersIds.Contains(c.Id));
            }

            var dataViewModel = data.OrderBy(c => c.Name).ToList();

            dataViewModel.Insert(0, new BusinessPartnerViewModel { Id = Guid.Empty, Name = _localizer["Select"] });

            return dataViewModel;
        }

        public List<BusinessPartnerViewModel> GetAllCompanyCRM(string search = "")
        {
            var userId = _loggedUser.GetUserId();
            var companyCodes = _companyAppService.GetCompaniesByUserId(userId).Select(x => x.Code).ToList();
            var data = _businessPartnerRepository
                .GetAll()
                .Include(x => x.SalesArea)
                .Where(c =>
                    c.BusinessPartnerAccountGroupId == BusinessPartnerAccountGroupViewModel.SoldToParty
                    && companyCodes.Contains(c.SalesArea.Code)
                    && (string.IsNullOrEmpty(search) ||
                        c.Code.Contains(search) ||
                        c.Name.Contains(search) ||
                        c.FiscalIdentification.Contains(search)))
                .Select(c => new BusinessPartnerViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Code = c.Code,
                    FiscalIdentification = c.FiscalIdentification,
                    City = c.City
                });

            var dataViewModel = data.OrderBy(c => c.Name).ToList();
            dataViewModel.Insert(0, new BusinessPartnerViewModel { Id = Guid.Empty, Name = _localizer["Select"] });
            return dataViewModel;
        }

        public List<BusinessPartnerViewModel> GetSelectAllCompany(string search = "")
        {
            var userId = _loggedUser.GetUserId();
            var companyCodes = _companyAppService.GetCompaniesByUserId(userId).Select(x=> x.Code).ToList();

            var data = _businessPartnerRepository
                .GetAll()
                .Include(x => x.SalesArea)
                .Where(c =>
                    c.BusinessPartnerAccountGroupId == BusinessPartnerAccountGroupViewModel.SoldToParty
                    && companyCodes.Contains(c.SalesArea.Code)
                    && (!string.IsNullOrEmpty(search) ? (c.Code.Contains(search) || c.Name.Contains(search) || c.FiscalIdentification.Contains(search)) : 1 == 1)
                    && c.StatusBusinessPartnerId == StatusBusinessPartnerViewModel.Active
                    )
                .Select(c => new BusinessPartnerViewModel()
                {
                    Id = c.Id,
                    Name = c.Name,
                    Code = c.Code,
                    FiscalIdentification = c.FiscalIdentification
                });


            var dataViewModel = data.OrderBy(c => c.Name).ToList();

            dataViewModel.Insert(0, new BusinessPartnerViewModel { Id = Guid.Empty, Name = _localizer["Select"] });

            return dataViewModel;
        }

        public List<BusinessPartnerViewModel> GetSelectExport(string search = "")
        {
            var userId = _loggedUser.GetUserId();
            var companyMain = _companyAppService.GetCompanyMainByUserId(userId).FirstOrDefault();

            var data = _businessPartnerRepository
                .GetAll()
                .Where(c =>
                    c.BusinessPartnerAccountGroupId == BusinessPartnerAccountGroupViewModel.SoldToParty
                    && c.SalesAreaId == SalesAreaViewModel.BR20
                    && (!string.IsNullOrEmpty(search) ? (c.Code.Contains(search) || c.Name.Contains(search)) : 1 == 1)
                    && c.StatusBusinessPartnerId == StatusBusinessPartnerViewModel.Active
                    )
                .Select(c => new BusinessPartnerViewModel()
                {
                    Id = c.Id,
                    Name = string.IsNullOrEmpty(c.Code) ? c.Name : c.Code + " - " + c.Name,
                }).ToList();

            data.Insert(0, new BusinessPartnerViewModel { Id = Guid.Empty, Name = _localizer["Select"] });

            return data;
        }

        public List<BusinessPartnerViewModel> GetVendorSelect2(string search = "")
        {
            var userId = _loggedUser.GetUserId();
            var listCompanys = _companyAppService.GetCompaniesByUserId(userId).Select(x => x.Code).ToList();

            var data = _businessPartnerRepository
                .GetAllAsNoTracking()
                .Where(c =>
                    c.BusinessPartnerAccountGroupId == BusinessPartnerAccountGroupViewModel.Vendor
                    && listCompanys.Contains(c.SalesArea.Code)
                    && (!string.IsNullOrEmpty(search) ? (c.Code.Contains(search) || c.Name.Contains(search)) : 1 == 1)
                    && c.StatusBusinessPartnerId == StatusBusinessPartnerViewModel.Active
                    )
                .Select(c => new BusinessPartnerViewModel()
                {
                    Id = c.Id,
                    Name = string.IsNullOrEmpty(c.Code) ? c.Name : c.Code + " - " + c.Name,
                }).ToList();

            data.Insert(0, new BusinessPartnerViewModel { Id = Guid.Empty, Name = _localizer["Select"] });

            return data;
        }

        public List<BusinessPartnerViewModel> GetVendorSelectById(Guid vendorId)
        {
            var userId = _loggedUser.GetUserId();
            var listCompanys = _companyAppService.GetCompaniesByUserId(userId).Select(x => x.Code).ToList();

            var data = _businessPartnerRepository
                .GetAllAsNoTracking()
                .Where(c =>
                    c.BusinessPartnerAccountGroupId == BusinessPartnerAccountGroupViewModel.Vendor
                    && listCompanys.Contains(c.SalesArea.Code)
                    && c.Id == vendorId
                    && c.StatusBusinessPartnerId == StatusBusinessPartnerViewModel.Active
                    )
                .Select(c => new BusinessPartnerViewModel()
                {
                    Id = c.Id,
                    Name = string.IsNullOrEmpty(c.Code) ? c.Name : c.Code + " - " + c.Name,
                }).ToList();

            data.Insert(0, new BusinessPartnerViewModel { Id = Guid.Empty, Name = _localizer["Select"] });

            return data;
        }

        public List<BusinessPartnerViewModel> GetSelectLai(string salesAreaCode, string channelCode)
        {
            var data = _businessPartnerRepository
                .GetAll()
                .Include(x => x.SalesArea)
                .Where(c => c.BusinessPartnerAccountGroupId == BusinessPartnerAccountGroupViewModel.SoldToParty && c.SalesArea.Code == salesAreaCode && c.InfocenterChannel.ChannelCode == channelCode)
                .Select(c => new BusinessPartnerViewModel()
                {
                    Id = c.Id,
                    Name = c.Code + " - " + c.Name,
                })
                .ToList();

            data.Insert(0, new BusinessPartnerViewModel { Id = Guid.Empty, Name = _localizer["Select"] });

            return data;
        }

        public List<BusinessPartnerViewModel> GetSelectSupplier(string search = "", Guid? id = null)
        {
            var userId = _loggedUser.GetUserId();
            var company = _companyAppService.GetCompanyMainByUserId(userId);
            var companyId = Guid.Empty;

            if (company.Any())
            {
                companyId = company.FirstOrDefault().Id;
            }
            else
            {
                companyId = _companyAppService.GetCompanyIdByUserCountryId(userId);
            }

            var companyCode = _companyRepository.GetWhere(c => c.Id.Equals(companyId)).First().Code;

            var data = _businessPartnerRepository
                .GetAll()
                .Include(x => x.SalesArea)
                .Where(c =>
                    c.BusinessPartnerAccountGroupId == BusinessPartnerAccountGroupViewModel.Vendor
                    && c.SalesArea.Code == companyCode
                    && (!string.IsNullOrEmpty(search) ? (c.Code.Contains(search) || c.Name.Contains(search)) : 1 == 1)
                    && c.StatusBusinessPartnerId == StatusBusinessPartnerViewModel.Active
                    )
                .Select(c => new BusinessPartnerViewModel()
                {
                    Id = c.Id,
                    Name = string.IsNullOrEmpty(c.Code) ? c.Name : c.Code + " - " + c.Name,
                });

            if(id != null && id != Guid.Empty)
            {
                data = data.Where(c => c.Id == id);
            }

            var dataList = data.ToList();

            dataList.Insert(0, new BusinessPartnerViewModel { Id = Guid.Empty, Name = _localizer["Select"] });

            return dataList;
        }

		public List<BusinessPartnerViewModel> GetSelectDistributors(Guid? companyId = null, Guid? distributorId = null)
		{
            var data = _businessPartnerRepository
				.GetAll()
				.Include(x => x.SalesArea)
				.Where(c => c.BusinessPartnerAccountGroupId == BusinessPartnerAccountGroupViewModel.SoldToParty)
				.Select(c => new BusinessPartnerViewModel()
				{
					Id = c.Id,
					Name = c.Code + " - " + c.Name,
                    CompanyCode = c.SalesArea.Code
				});

            if(distributorId != null && distributorId != Guid.Empty)
            {
                data = data.Where(c => c.Id == distributorId);
            }
            else
            {
                var companyCode = string.Empty;
                if (companyId is null)
                {
                    var userId = _loggedUser.GetUserId();
                    var companyMain = _companyAppService.GetCompanyMainByUserId(userId).FirstOrDefault();
                    companyCode = companyMain?.Code;
                }
                else
                {
                    companyCode = _companyRepository.GetWhere(c => c.Id.Equals(companyId)).First().Code;
                }

                data = data.Where(c => c.CompanyCode == companyCode);
            }

            var dataList = data.ToList();

            dataList.Insert(0, new BusinessPartnerViewModel { Id = Guid.Empty, Name = _localizer["Select"] });

			return dataList;
		}

		public List<BusinessPartnerViewModel> GetSelectDistributors(Guid? companyId = null, Guid? distributorId = null, string search = "")
		{
			var data = _businessPartnerRepository
				.GetAll()
				.Include(x => x.SalesArea)
                .Include(x => x.InfocenterChannel)
                .ThenInclude(x => x.Company)
				.Where(c => c.BusinessPartnerAccountGroupId == BusinessPartnerAccountGroupViewModel.SoldToParty
				        && (!string.IsNullOrEmpty(search) ? (c.Code.Contains(search) || c.Name.Contains(search)) : 1 == 1))
				.Select(c => new BusinessPartnerViewModel()
				{
					Id = c.Id,
					Name = c.Code + " - " + c.Name,
					CompanyCode = c.SalesArea.Code,
                    CompanyName = c.InfocenterChannel.Company.Name

				});

			if (distributorId != null && distributorId != Guid.Empty)
			{
				data = data.Where(c => c.Id == distributorId);
			}
			else
			{
				var companyCode = string.Empty;
				if (companyId is null)
				{
					var userId = _loggedUser.GetUserId();
					var companyMain = _companyAppService.GetCompanyMainByUserId(userId).FirstOrDefault();
					companyCode = companyMain?.Code;
				}
				else
				{
					companyCode = _companyRepository.GetWhere(c => c.Id.Equals(companyId)).First().Code;
				}

				data = data.Where(c => c.CompanyCode == companyCode);
			}

			var dataList = data.ToList();

			dataList.Insert(0, new BusinessPartnerViewModel { Id = Guid.Empty, Name = _localizer["Select"] });

			return dataList;
		}

		public List<BusinessPartnerViewModel> GetSelectBusinessPartner(Guid? businessPartnerId = null)
        {
            var userId = _loggedUser.GetUserId();
            var companyMain = _companyAppService.GetCompanyMainByUserId(userId).FirstOrDefault();

            var data = _businessPartnerRepository
                .GetAll()
                .Include(x => x.SalesArea)
                .Where(c => c.BusinessPartnerAccountGroupId == BusinessPartnerAccountGroupViewModel.SoldToParty && c.SalesArea.Code == companyMain.Code)
                .Select(c => new BusinessPartnerViewModel()
                {
                    Id = c.Id,
                    Name = c.Code + " - " + c.Name,
                });

            if (businessPartnerId != null & businessPartnerId != Guid.Empty)
            {
                data = data.Where(c => c.Id == businessPartnerId);
            }

            var dataList = data.ToList();

            dataList.Insert(0, new BusinessPartnerViewModel { Id = Guid.Empty, Name = _localizer["Select"] });

            return dataList;
        }

        public IEnumerable<BusinessPartnerFilterViewModel> GetAllByPremiumService(string name, string code, string countryCode, string type, string fiscalId, bool canSeeAllCustomers, string legacyCode, string city, string zipCode, string infocenterChannel, Guid regionId, Guid countryId, bool empty = true) =>
            _mapper.Map<IEnumerable<BusinessPartnerFilterViewModel>>(_businessPartnerRepository.GetAllByPremiumService(name, code, countryCode, type, fiscalId, canSeeAllCustomers, legacyCode, city, zipCode, infocenterChannel, regionId, countryId, empty));

        public List<BusinessPartnerViewModel> GetAllPremiumService(string search = "", bool checkCompany = true)
        {
            var businessPartnerBrPS = _businessPartnerPremiumServiceRepository.GetAll().Select(s => s.BusinessPartnerId).Distinct().ToList();

            var businessPartnerBr = _businessPartnerBrRepository.GetAll().Where(x => x.PremiumService == true && businessPartnerBrPS.Contains(x.BusinessPartnerId)).Select(x => x.BusinessPartnerId).ToList();

            var companyCode = "";

            if (checkCompany)
            {
                var userId = _loggedUser.GetUserId();
                var comapany = _companyAppService.GetCompanyMainByUserId(userId);
                var companyId = Guid.Empty;

                if (comapany.Any())
                {
                    companyId = comapany.FirstOrDefault().Id;
                }
                else
                {
                    companyId = _companyAppService.GetCompanyIdByUserCountryId(userId);
                }

                companyCode = _companyRepository.GetWhere(c => c.Id.Equals(companyId)).First().Code;
            }

            var data = _businessPartnerRepository
                .GetAll()
                .Include(x => x.SalesArea)
                .Where(c =>
                    c.BusinessPartnerAccountGroupId == BusinessPartnerAccountGroupViewModel.SoldToParty
                    && (checkCompany ? c.SalesArea.Code == companyCode : 1 == 1)
                    && (!string.IsNullOrEmpty(search) ? (c.Code.Contains(search) || c.Name.Contains(search)) : 1 == 1)
                    && c.StatusBusinessPartnerId == StatusBusinessPartnerViewModel.Active
                    && businessPartnerBr.Contains(c.Id)
                    )
                .Select(c => new BusinessPartnerViewModel()
                {
                    Id = c.Id,
                    Name = string.IsNullOrEmpty(c.Code) ? c.Name : c.Code + " - " + c.Name + " - " + c.City,
                }).ToList();

            return data;
        }

        public List<BusinessPartnerViewModel> GetSelectPremiumService(string search = "", bool checkCompany = true)
        {
            var businessPartnerBr = _businessPartnerBrRepository.GetAll().Where(x => x.PremiumService == true).Select(x => x.BusinessPartnerId).ToList();

            var companyCode = "";

            if (checkCompany)
            {
                var userId = _loggedUser.GetUserId();
                var comapany = _companyAppService.GetCompanyMainByUserId(userId);
                var companyId = Guid.Empty;

                if (comapany.Any())
                {
                    companyId = comapany.FirstOrDefault().Id;
                }
                else
                {
                    companyId = _companyAppService.GetCompanyIdByUserCountryId(userId);
                }

                companyCode = _companyRepository.GetWhere(c => c.Id.Equals(companyId)).First().Code;
            }

            var data = _businessPartnerRepository
                .GetAll()
                .Include(x => x.SalesArea)
                .Where(c =>
                    c.BusinessPartnerAccountGroupId == BusinessPartnerAccountGroupViewModel.SoldToParty
                    && (checkCompany ? c.SalesArea.Code == companyCode : 1 == 1)
                    && (!string.IsNullOrEmpty(search) ? (c.Code.Contains(search) || c.Name.Contains(search)) : 1 == 1)
                    && c.StatusBusinessPartnerId == StatusBusinessPartnerViewModel.Active
                    && businessPartnerBr.Contains(c.Id)
                    )
                .Select(c => new BusinessPartnerViewModel()
                {
                    Id = c.Id,
                    Name = string.IsNullOrEmpty(c.Code) ? c.Name : c.Code + " - " + c.Name,
                }).ToList();

            data.Insert(0, new BusinessPartnerViewModel { Id = Guid.Empty, Name = _localizer["Select"] });

            return data;
        }

        public List<BusinessPartnerViewModel> GetSelectBusinesPartnerByCompany(Guid? companyId = null, string search = "")
        {
            var data = _businessPartnerRepository
                .GetAllAsNoTracking()
                .Include(x => x.SalesArea)
                .ThenInclude(x => x.SalesOrganization)
                .Where(c => string.IsNullOrEmpty(search) ||
                c.Code.Contains(search) ||
                c.Name.Contains(search) &&
                c.StatusBusinessPartnerId == StatusBusinessPartnerViewModel.Active &&
                c.BusinessPartnerAccountGroupId == BusinessPartnerAccountGroupViewModel.SoldToParty)
                .Select(c => new BusinessPartnerViewModel()
                {
                    Id = c.Id,
                    Name = c.Code + " - " + c.Name,
                    CompanyCode = c.SalesArea.Code,
                    CompanyName = c.SalesArea.SalesOrganization.Description
                });

            var companyCode = string.Empty;

            if (companyId == null)
            {
                var userId = _loggedUser.GetUserId();
                var companyMain = _companyAppService.GetCompanyMainByUserId(userId).FirstOrDefault();
                companyCode = companyMain?.Code;
            }
            else
            {
                companyCode = _companyRepository.GetWhere(c => c.Id.Equals(companyId)).First().Code;
            }

            data = data.Where(c => c.CompanyCode == companyCode);

            var dataList = data.ToList();

            dataList.Insert(0, new BusinessPartnerViewModel { Id = Guid.Empty, Name = _localizer["Select"] });

            return dataList;
        }


        public List<BusinessPartnerViewModel> GetSelectPersonBusinesPartnerByCompany(Guid? companyId = null, string search = "")
        {
            var data = _businessPartnerRepository
                .GetAllAsNoTracking()
                .Include(x => x.SalesArea)
                .ThenInclude(x => x.SalesOrganization)
                .Where(c => string.IsNullOrEmpty(search) ||
                c.Code.Contains(search) ||
                c.Name.Contains(search) &&
                c.StatusBusinessPartnerId == StatusBusinessPartnerViewModel.Active &&
                c.BusinessPartnerAccountGroupId == BusinessPartnerAccountGroupViewModel.Vendor)
                .Select(c => new BusinessPartnerViewModel()
                {
                    Id = c.Id,
                    Name = c.Code + " - " + c.Name,
                    CompanyCode = c.SalesArea.Code,
                    CompanyName = c.SalesArea.SalesOrganization.Description
                });

            var companyCode = string.Empty;

            if (companyId == null)
            {
                var userId = _loggedUser.GetUserId();
                var companyMain = _companyAppService.GetCompanyMainByUserId(userId).FirstOrDefault();
                companyCode = companyMain?.Code;
            }
            else
            {
                companyCode = _companyRepository.GetWhere(c => c.Id.Equals(companyId)).First().Code;
            }

            data = data.Where(c => c.CompanyCode == companyCode);

            var dataList = data.ToList();

            dataList.Insert(0, new BusinessPartnerViewModel { Id = Guid.Empty, Name = _localizer["Select"] });

            return dataList;
        }

        public List<BusinessPartnerViewModel> GetSelectResponsibleByCompany(Guid companyId)
		{
            var companyCode = _companyAppService.GetById(companyId)?.Code;

            var data = _businessPartnerRepository
                .GetAllAsNoTracking()
                .Include(x => x.SalesArea)
                .ThenInclude(x => x.SalesOrganization)
                .Where(x => x.SalesArea.SalesOrganization.Code == companyCode
                        && x.BusinessPartnerAccountGroupId == Guid.Parse("3C71F1D8-B0CB-4D3F-8797-CB6F3A27D047")
                        && x.StatusBusinessPartnerId == StatusBusinessPartnerViewModel.Active)
                .OrderBy(x => x.Name)
                .Select(c => new BusinessPartnerViewModel()
				{
					Id = c.Id,
					Name = c.Code + " - " + c.Name
				});


			var dataList = data.ToList();

			dataList.Insert(0, new BusinessPartnerViewModel { Id = Guid.Empty, Name = _localizer["Select"] });

			return dataList;
		}
		

		public List<BusinessPartnerViewModel> GetSelectSoldToByCompany(Guid companyId)
		{
			var companyCode = _companyAppService.GetById(companyId)?.Code;

			var data = _businessPartnerRepository
				.GetAllAsNoTracking()
				.Include(x => x.SalesArea)
				.ThenInclude(x => x.SalesOrganization)
				.Where(x => x.SalesArea.SalesOrganization.Code == companyCode
						&& x.BusinessPartnerAccountGroupId == Guid.Parse("B6102261-3D89-4E2A-B322-3D14825D6131"))
                .OrderBy(x => x.Name)
				.Select(c => new BusinessPartnerViewModel()
				{
					Id = c.Id,
					Name = c.Code + " - " + c.Name
				});


			var dataList = data.ToList();

			dataList.Insert(0, new BusinessPartnerViewModel { Id = Guid.Empty, Name = _localizer["Select"] });

			return dataList;
		}

        public IQueryable<string> GetEmailsByBusinessPartnerId(string code)
        {
            return _businessPartnerRepository.GetEmailsByBusinessPartnerCode(code);
        }
    }
}
