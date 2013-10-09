using Rhino.Mocks;
using SSD.Domain;
using System.Linq;

namespace SSD.Repository
{
    public class TestRepositories
    {
        public IUserRepository MockUserRepository { get; private set; }
        public IProviderRepository MockProviderRepository { get; private set; }
        public IProgramRepository MockProgramRepository { get; private set; }
        public ISchoolRepository MockSchoolRepository { get; private set; }
        public IRoleRepository MockRoleRepository { get; private set; }
        public IUserRoleRepository MockUserRoleRepository { get; private set; }
        public ICustomDataOriginRepository MockCustomDataOriginRepository { get; private set; }
        public ICustomFieldCategoryRepository MockCustomFieldCategoryRepository { get; private set; }
        public ICustomFieldValueRepository MockCustomFieldValueRepository { get; private set; }
        public ICustomFieldRepository MockCustomFieldRepository { get; private set; }
        public ICustomFieldTypeRepository MockCustomFieldTypeRepository { get; private set; }
        public IStudentRepository MockStudentRepository { get; private set; }
        public IServiceTypeRepository MockServiceTypeRepository { get; private set; }
        public IPriorityRepository MockPriorityRepository { get; private set; }
        public IFulfillmentStatusRepository MockFulfillmentStatusRepository { get; private set; }
        public ISubjectRepository MockSubjectRepository { get; private set; }
        public IPropertyRepository MockPropertyRepository { get; private set; }
        public IServiceOfferingRepository MockServiceOfferingRepository { get; private set; }
        public IServiceAttendanceRepository MockServiceAttendanceRepository { get; private set; }
        public IStudentAssignedOfferingRepository MockStudentAssignedOfferingRepository { get; private set; }
        public IServiceTypeCategoryRepository MockCategoryRepository { get; private set; }
        public IServiceRequestRepository MockServiceRequestRepository { get; private set; }
        public IServiceRequestFulfillmentRepository MockServiceRequestFulfillmentRepository { get; private set; }
        public IUserAccessChangeEventRepository MockUserAccessChangeEventRepository { get; private set; }
        public IEulaAgreementRepository MockEulaAgreementRepository { get; private set; }
        public IEulaAcceptanceRepository MockEulaAcceptanceRepository { get; private set; }
        public IPrivateHealthDataViewEventRepository MockPrivateHealthDataViewEventRepository { get; private set; }
        public ILoginEventRepository MockLoginEventRepository { get; private set; }

        public IRepositoryContainer MockRepositoryContainer { get; private set; }

        public TestRepositories()
        {
            InitializeRepositoryMocks();
            InitializeEducationContainer();
        }

        public TestRepositories(TestData testData)
            : this()
        {
            MockWithTestData(testData);
        }

        private void InitializeEducationContainer()
        {
            MockRepositoryContainer = MockRepository.GenerateMock<IRepositoryContainer>();
            MockRepositoryContainer.Expect(m => m.Obtain<IUserRepository>()).Return(MockUserRepository);
            MockRepositoryContainer.Expect(m => m.Obtain<ISchoolRepository>()).Return(MockSchoolRepository);
            MockRepositoryContainer.Expect(m => m.Obtain<IRoleRepository>()).Return(MockRoleRepository);
            MockRepositoryContainer.Expect(m => m.Obtain<IUserRoleRepository>()).Return(MockUserRoleRepository);
            MockRepositoryContainer.Expect(m => m.Obtain<IProviderRepository>()).Return(MockProviderRepository);
            MockRepositoryContainer.Expect(m => m.Obtain<IProgramRepository>()).Return(MockProgramRepository);
            MockRepositoryContainer.Expect(m => m.Obtain<ICustomDataOriginRepository>()).Return(MockCustomDataOriginRepository);
            MockRepositoryContainer.Expect(m => m.Obtain<ICustomFieldCategoryRepository>()).Return(MockCustomFieldCategoryRepository);
            MockRepositoryContainer.Expect(m => m.Obtain<ICustomFieldValueRepository>()).Return(MockCustomFieldValueRepository);
            MockRepositoryContainer.Expect(m => m.Obtain<ICustomFieldTypeRepository>()).Return(MockCustomFieldTypeRepository);
            MockRepositoryContainer.Expect(m => m.Obtain<ICustomFieldRepository>()).Return(MockCustomFieldRepository);
            MockRepositoryContainer.Expect(m => m.Obtain<IStudentRepository>()).Return(MockStudentRepository);
            MockRepositoryContainer.Expect(m => m.Obtain<IPropertyRepository>()).Return(MockPropertyRepository);
            MockRepositoryContainer.Expect(m => m.Obtain<IServiceTypeRepository>()).Return(MockServiceTypeRepository);
            MockRepositoryContainer.Expect(m => m.Obtain<ISubjectRepository>()).Return(MockSubjectRepository);
            MockRepositoryContainer.Expect(m => m.Obtain<IPriorityRepository>()).Return(MockPriorityRepository);
            MockRepositoryContainer.Expect(m => m.Obtain<IFulfillmentStatusRepository>()).Return(MockFulfillmentStatusRepository);
            MockRepositoryContainer.Expect(m => m.Obtain<IStudentAssignedOfferingRepository>()).Return(MockStudentAssignedOfferingRepository);
            MockRepositoryContainer.Expect(m => m.Obtain<IServiceOfferingRepository>()).Return(MockServiceOfferingRepository);
            MockRepositoryContainer.Expect(m => m.Obtain<IServiceAttendanceRepository>()).Return(MockServiceAttendanceRepository);
            MockRepositoryContainer.Expect(m => m.Obtain<IServiceTypeCategoryRepository>()).Return(MockCategoryRepository);
            MockRepositoryContainer.Expect(m => m.Obtain<IServiceRequestRepository>()).Return(MockServiceRequestRepository);
            MockRepositoryContainer.Expect(m => m.Obtain<IServiceRequestFulfillmentRepository>()).Return(MockServiceRequestFulfillmentRepository);
            MockRepositoryContainer.Expect(m => m.Obtain<IUserAccessChangeEventRepository>()).Return(MockUserAccessChangeEventRepository);
            MockRepositoryContainer.Expect(m => m.Obtain<IEulaAgreementRepository>()).Return(MockEulaAgreementRepository);
            MockRepositoryContainer.Expect(m => m.Obtain<IEulaAcceptanceRepository>()).Return(MockEulaAcceptanceRepository);
            MockRepositoryContainer.Expect(m => m.Obtain<IPrivateHealthDataViewEventRepository>()).Return(MockPrivateHealthDataViewEventRepository);
            MockRepositoryContainer.Expect(m => m.Obtain<ILoginEventRepository>()).Return(MockLoginEventRepository);
        }

        private void InitializeRepositoryMocks()
        {
            MockUserRepository = MockRepository.GenerateMock<IUserRepository>();
            MockProviderRepository = MockRepository.GenerateMock<IProviderRepository>();
            MockProgramRepository = MockRepository.GenerateMock<IProgramRepository>();
            MockSchoolRepository = MockRepository.GenerateMock<ISchoolRepository>();
            MockRoleRepository = MockRepository.GenerateMock<IRoleRepository>();
            MockUserRoleRepository = MockRepository.GenerateMock<IUserRoleRepository>();
            MockCustomDataOriginRepository = MockRepository.GenerateMock<ICustomDataOriginRepository>();
            MockCustomFieldCategoryRepository = MockRepository.GenerateMock<ICustomFieldCategoryRepository>();
            MockCustomFieldValueRepository = MockRepository.GenerateMock<ICustomFieldValueRepository>();
            MockCustomFieldRepository = MockRepository.GenerateMock<ICustomFieldRepository>();
            MockCustomFieldTypeRepository = MockRepository.GenerateMock<ICustomFieldTypeRepository>();
            MockStudentRepository = MockRepository.GenerateMock<IStudentRepository>();
            MockPropertyRepository = MockRepository.GenerateMock<IPropertyRepository>();
            MockPriorityRepository = MockRepository.GenerateMock<IPriorityRepository>();
            MockFulfillmentStatusRepository = MockRepository.GenerateMock<IFulfillmentStatusRepository>();
            MockSubjectRepository = MockRepository.GenerateMock<ISubjectRepository>();
            MockServiceTypeRepository = MockRepository.GenerateMock<IServiceTypeRepository>();
            MockServiceOfferingRepository = MockRepository.GenerateMock<IServiceOfferingRepository>();
            MockServiceAttendanceRepository = MockRepository.GenerateMock<IServiceAttendanceRepository>();
            MockStudentAssignedOfferingRepository = MockRepository.GenerateMock<IStudentAssignedOfferingRepository>();
            MockCategoryRepository = MockRepository.GenerateMock<IServiceTypeCategoryRepository>();
            MockServiceRequestRepository = MockRepository.GenerateMock<IServiceRequestRepository>();
            MockServiceRequestFulfillmentRepository = MockRepository.GenerateMock<IServiceRequestFulfillmentRepository>();
            MockUserAccessChangeEventRepository = MockRepository.GenerateMock<IUserAccessChangeEventRepository>();
            MockEulaAgreementRepository = MockRepository.GenerateMock<IEulaAgreementRepository>();
            MockEulaAcceptanceRepository = MockRepository.GenerateMock<IEulaAcceptanceRepository>();
            MockPrivateHealthDataViewEventRepository = MockRepository.GenerateMock<IPrivateHealthDataViewEventRepository>();
            MockLoginEventRepository = MockRepository.GenerateMock<ILoginEventRepository>();
        }

        private void MockWithTestData(TestData testData)
        {
            MockUserRepository.Expect(m => m.Items).Return(testData.Users.AsQueryable());
            MockProviderRepository.Expect(m => m.Items).Return(testData.Providers.AsQueryable());
            MockProgramRepository.Expect(m => m.Items).Return(testData.Programs.AsQueryable());
            MockSchoolRepository.Expect(m => m.Items).Return(testData.Schools.AsQueryable());
            MockRoleRepository.Expect(m => m.Items).Return(testData.Roles.AsQueryable());
            MockUserRoleRepository.Expect(m => m.Items).Return(testData.UserRoles.AsQueryable());
            MockCustomDataOriginRepository.Expect(m => m.Items).Return(testData.CustomDataOrigins.AsQueryable());
            MockCustomFieldCategoryRepository.Expect(m => m.Items).Return(testData.CustomFieldCategories.AsQueryable());
            MockCustomFieldValueRepository.Expect(m => m.Items).Return(testData.CustomFieldValues.AsQueryable());
            MockCustomFieldRepository.Expect(m => m.Items).Return(testData.CustomFields.AsQueryable());
            MockCustomFieldTypeRepository.Expect(m => m.Items).Return(testData.CustomFieldTypes.AsQueryable());
            MockStudentRepository.Expect(m => m.Items).Return(testData.Students.AsQueryable());
            MockPropertyRepository.Expect(m => m.Items).Return(testData.Properties.AsQueryable());
            MockPriorityRepository.Expect(m => m.Items).Return(testData.Priorities.AsQueryable());
            MockFulfillmentStatusRepository.Expect(m => m.Items).Return(testData.FulfillmentStatuses.AsQueryable());
            MockSubjectRepository.Expect(m => m.Items).Return(testData.Subjects.AsQueryable());
            MockServiceTypeRepository.Expect(m => m.Items).Return(testData.ServiceTypes.AsQueryable());
            MockServiceOfferingRepository.Expect(m => m.Items).Return(testData.ServiceOfferings.AsQueryable());
            MockServiceAttendanceRepository.Expect(m => m.Items).Return(testData.ServiceAttendances.AsQueryable());
            MockStudentAssignedOfferingRepository.Expect(m => m.Items).Return(testData.StudentAssignedOfferings.AsQueryable());
            MockCategoryRepository.Expect(m => m.Items).Return(testData.Categories.AsQueryable());
            MockServiceRequestRepository.Expect(m => m.Items).Return(testData.ServiceRequests.AsQueryable());
            MockServiceRequestFulfillmentRepository.Expect(m => m.Items).Return(testData.ServiceRequestFulfillments.AsQueryable());
            MockUserAccessChangeEventRepository.Expect(m => m.Items).Return(testData.UserAccessChangeEvents.AsQueryable());
            MockEulaAgreementRepository.Expect(m => m.Items).Return(testData.Eulas.AsQueryable());
            MockEulaAcceptanceRepository.Expect(m => m.Items).Return(testData.EulaAcceptances.AsQueryable());
            MockPrivateHealthDataViewEventRepository.Expect(m => m.Items).Return(testData.PrivateHealthDataViewEvents.AsQueryable());
            MockLoginEventRepository.Expect(m => m.Items).Return(testData.LoginEvents.AsQueryable());
        }
    }
}
