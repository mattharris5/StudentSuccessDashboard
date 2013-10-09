using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Domain;
using SSD.Security;
using SSD.Security.Permissions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace SSD.ViewModels.DataTables
{
    [TestClass]
    public class StudentClientDataTableTest
    {
        private TestData TestData { get; set; }
        private HttpContextBase MockContext { get; set; }
        private List<Property> StudentProperties { get; set; }
        private StudentClientDataTable Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            TestData = new TestData();
            StudentProperties = TestData.Properties.Where(p => p.EntityName.Equals(typeof(Student).FullName)).ToList();
            MockContext = MockHttpContextFactory.Create();
            IPermissionFactory mockPermissionFactory = MockRepository.GenerateMock<IPermissionFactory>();
            PermissionFactory.SetCurrent(mockPermissionFactory);
        }

        public EducationSecurityPrincipal CreateUser(bool isAdministrator)
        {
            return CreateUser(isAdministrator, null);
        }

        public EducationSecurityPrincipal CreateUser(bool isAdministrator, IEnumerable<int> associatedSchoolIds)
        {
            User userEntity = new User { Id = 1, UserKey = "1" };
            EducationSecurityPrincipal user = new EducationSecurityPrincipal(userEntity);
            if (isAdministrator)
            {
                userEntity.UserRoles.Add(new UserRole { User = userEntity, UserId = userEntity.Id, Role = new Role { Name = SecurityRoles.DataAdmin, Id = 1 }, RoleId = 1 });
            }
            if (associatedSchoolIds != null)
            {
                UserRole siteCoordinatorRole = new UserRole { User = userEntity, UserId = userEntity.Id, Role = new Role { Name = SecurityRoles.SiteCoordinator, Id = 1 }, RoleId = 1 };
                userEntity.UserRoles.Add(siteCoordinatorRole);
                foreach (int id in associatedSchoolIds)
                {
                    siteCoordinatorRole.Schools.Add(new School { Id = id });
                }
            }
            return user;
        }

        [TestMethod]
        public void GivenUserIsDataAdmin_AndSchoolListRequestParam_WhenIConstruct_ThenSchoolsPopulated()
        {
            string[] expected = { "Middle School", "Other School" };
            PrepareDataTableRequestParameters("0", "asc", null, null, null, "Middle School|Other School", null, null, null, null, null);

            Target = new StudentClientDataTable(MockContext.Request, CreateUser(true), StudentProperties);

            CollectionAssert.AreEqual(expected, Target.Schools.ToList());
        }

        [TestMethod]
        public void GivenSchoolListRequestParam_WhenIConstruct_ThenSchoolsPopulated()
        {
            string[] expected = { "Middle School", "Other School" };
            PrepareDataTableRequestParameters("0", "asc", null, null, null, "Middle School|Other School", null, null, null, null, null);
            var associatedSchoolIds = TestData.Schools.Where(s => s.Name.Equals("Middle School")).Select(s => s.Id).ToList();

            Target = new StudentClientDataTable(MockContext.Request, CreateUser(false, associatedSchoolIds), StudentProperties);

            CollectionAssert.AreEqual(expected, Target.Schools.ToList());
        }

        [TestMethod]
        public void GivenUserIsDataAdmin_AndPriorityListRequestParam_WhenIConstruct_ThenPrioritiesPopulated()
        {
            string[] expected = { "High", "Low" };
            PrepareDataTableRequestParameters("0", "asc", null, null, null, null, null, "High|Low", null, null, null);

            Target = new StudentClientDataTable(MockContext.Request, CreateUser(true), StudentProperties);

            CollectionAssert.AreEqual(expected, Target.Priorities.ToList());
        }

        [TestMethod]
        public void GivenPriorityListRequestParam_WhenIConstruct_ThenPrioritiesPopulated()
        {
            string[] expected = { "High", "Low" };
            PrepareDataTableRequestParameters("0", "asc", null, null, null, null, null, "High|Low", null, null, null);

            Target = new StudentClientDataTable(MockContext.Request, CreateUser(false), StudentProperties);

            CollectionAssert.AreEqual(expected, Target.Priorities.ToList());
        }

        [TestMethod]
        public void GivenUserIsDataAdmin_AndServiceTypeListRequestParam_WhenIConstruct_ThenServiceTypesPopulated()
        {
            string[] expected = { "Tutoring", "Mentoring" };
            PrepareDataTableRequestParameters("0", "asc", null, null, null, null, null, null, null, "Tutoring|Mentoring", null);

            Target = new StudentClientDataTable(MockContext.Request, CreateUser(true), StudentProperties);

            CollectionAssert.AreEqual(expected, Target.ServiceTypes.ToList()); 
        }

        [TestMethod]
        public void GivenServiceTypeListRequestParam_WhenIConstruct_ThenServiceTypesPopulated()
        {
            string[] expected = { "Tutoring", "Mentoring" };
            PrepareDataTableRequestParameters("0", "asc", null, null, null, null, null, null, null, "Tutoring|Mentoring", null);
            Target = new StudentClientDataTable(MockContext.Request, CreateUser(false), StudentProperties);

            CollectionAssert.AreEqual(expected, Target.ServiceTypes.ToList());
        }

        [TestMethod]
        public void GivenUserIsDataAdmin_AndSubjectListRequestParam_WhenIConstruct_ThenSubjectsPopulated()
        {
            string[] expected = { "Math", "Reading" };
            PrepareDataTableRequestParameters("0", "asc", null, null, null, null, null, null, null, null, "Math|Reading");

            Target = new StudentClientDataTable(MockContext.Request, CreateUser(true), StudentProperties);

            CollectionAssert.AreEqual(expected, Target.Subjects.ToList());
        }

        [TestMethod]
        public void GivenSubjectListRequestParam_WhenIConstruct_ThenSubjectsPopulated()
        {
            string[] expected = { "Math", "Reading" };
            PrepareDataTableRequestParameters("0", "asc", null, null, null, null, null, null, null, null, "Math|Reading");

            Target = new StudentClientDataTable(MockContext.Request, CreateUser(false), StudentProperties);

            CollectionAssert.AreEqual(expected, Target.Subjects.ToList());
        }

        [TestMethod]
        public void GivenUserIsDataAdmin_AndServiceRequestStatusParam_AndStudentHasServiceRequestWithStatus_WhenIFilter_ThenStudentIsTaken()
        {
            Student student = new Student
            {
                ServiceRequests = new List<ServiceRequest>
                {
                    new ServiceRequest
                    {
                        FulfillmentDetails = new List<ServiceRequestFulfillment>
                        {
                            new ServiceRequestFulfillment { FulfillmentStatus = new FulfillmentStatus { Name = Statuses.Open } }
                        }
                    }
                }
            };
            PrepareDataTableRequestParameters("0", "asc", null, null, null, null, null, null, "Open", null, null);
            Target = new StudentClientDataTable(MockContext.Request, CreateUser(true), StudentProperties);

            Assert.IsTrue(Target.FilterPredicate.Compile().Invoke(student));
        }

        [TestMethod]
        public void GivenUserIsDataAdmin_AndServiceRequestStatusParam_AndStudentHasNoServiceRequestWithStatus_WhenIFilter_ThenStudentIsNotTaken()
        {
            Student student = new Student
            {
                ServiceRequests = new List<ServiceRequest>
                {
                    new ServiceRequest
                    {
                        FulfillmentDetails = new List<ServiceRequestFulfillment>
                        {
                            new ServiceRequestFulfillment { FulfillmentStatus = new FulfillmentStatus { Name = Statuses.Rejected } }
                        }
                    }
                }
            };
            PrepareDataTableRequestParameters("0", "asc", null, null, null, null, null, null, "Open", null, null);
            Target = new StudentClientDataTable(MockContext.Request, CreateUser(true), StudentProperties);

            Assert.IsFalse(Target.FilterPredicate.Compile().Invoke(student));
        }

        [TestMethod]
        public void GivenUserIsDataAdmin_AndServiceRequestStatusParam_AndStudentHasNoServiceRequestWithStatusAsMostRecentFullfillmentDetail_WhenIFilter_ThenStudentIsNotTaken()
        {
            Student student = new Student
            {
                ServiceRequests = new List<ServiceRequest>
                {
                    new ServiceRequest
                    {
                        FulfillmentDetails = new List<ServiceRequestFulfillment>
                        {
                            new ServiceRequestFulfillment { FulfillmentStatus = new FulfillmentStatus { Name = Statuses.Rejected }, CreateTime = DateTime.Now },
                            new ServiceRequestFulfillment { FulfillmentStatus = new FulfillmentStatus { Name = Statuses.Open }, CreateTime = DateTime.Now.AddDays(-1) }
                        }
                    }
                }
            };
            PrepareDataTableRequestParameters("0", "asc", null, null, null, null, null, null, "Open", null, null);
            Target = new StudentClientDataTable(MockContext.Request, CreateUser(true), StudentProperties);

            Assert.IsFalse(Target.FilterPredicate.Compile().Invoke(student));
        }

        [TestMethod]
        public void GivenUserIsDataAdmin_AndPriorityListRequestParam_AndStudentHasServiceRequestWithPriority_WhenIFilter_ThenStudentIsTaken()
        {
            Student student = new Student
            {
                ServiceRequests = new List<ServiceRequest>
                {
                    new ServiceRequest { Priority = new Priority { Name = "Low" } }
                }
            };
            PrepareDataTableRequestParameters("0", "asc", null, null, null, null, null, "High|Low", null, null, null);
            Target = new StudentClientDataTable(MockContext.Request, CreateUser(true), StudentProperties);

            Assert.IsTrue(Target.FilterPredicate.Compile().Invoke(student));
        }

        [TestMethod]
        public void GivenPriorityListRequestParam_AndStudentHasServiceRequestWithPriority_AndUserIsNotDataAdmin_AndUserHasSchoolAssociation_WhenIFilter_ThenStudentIsTaken()
        {
            Student student = new Student
            {
                SchoolId = 573,
                ServiceRequests = new List<ServiceRequest>
                {
                    new ServiceRequest { Priority = new Priority { Name = "Low" } }
                }
            };
            PrepareDataTableRequestParameters("0", "asc", null, null, null, null, null, "High|Low", null, null, null);
            Target = new StudentClientDataTable(MockContext.Request, CreateUser(false, new[] { 573 }), StudentProperties);

            Assert.IsTrue(Target.FilterPredicate.Compile().Invoke(student));
        }

        [TestMethod]
        public void GivenUserIsDataAdmin_AndPriorityListRequestParam_AndStudentHasNoServiceRequestWithPriority_WhenIFilter_ThenStudentIsNotTaken()
        {
            Student student = new Student
            {
                ServiceRequests = new List<ServiceRequest>
                {
                    new ServiceRequest { Priority = new Priority { Name = "Medium" } }
                }
            };
            PrepareDataTableRequestParameters("0", "asc", null, null, null, null, null, "High,Low", null, null, null);
            Target = new StudentClientDataTable(MockContext.Request, CreateUser(true), StudentProperties);

            Assert.IsFalse(Target.FilterPredicate.Compile().Invoke(student));
        }

        [TestMethod]
        public void GivenPriorityListRequestParam_AndStudentHasNoServiceRequestWithPriority_WhenIFilter_ThenStudentIsNotTaken()
        {
            Student student = new Student
            {
                ServiceRequests = new List<ServiceRequest>
                {
                    new ServiceRequest { Priority = new Priority { Name = "Medium" } }
                }
            };
            PrepareDataTableRequestParameters("0", "asc", null, null, null, null, null, "High|Low", null, null, null);
            Target = new StudentClientDataTable(MockContext.Request, CreateUser(false), StudentProperties);

            Assert.IsFalse(Target.FilterPredicate.Compile().Invoke(student));
        }

        [TestMethod]
        public void GivenUserIsDataAdmin_AndServiceTypeListRequestParam_AndStudentHasServiceRequestWithServiceType_WhenIFilter_ThenStudentIsTaken()
        {
            Student student = new Student
            {
                ServiceRequests = new List<ServiceRequest>
                {
                    new ServiceRequest { ServiceType = new ServiceType { Name = "Mentoring" } }
                }
            };
            PrepareDataTableRequestParameters("0", "asc", null, null, null, null, null, null, null, "Tutoring|Mentoring", null);
            Target = new StudentClientDataTable(MockContext.Request, CreateUser(true), StudentProperties);

            Assert.IsTrue(Target.FilterPredicate.Compile().Invoke(student));
        }

        [TestMethod]
        public void GivenServiceTypeListRequestParam_AndStudentHasServiceRequestWithServiceType_AndUserIsNotDataAdmin_AndUserHasSchoolAssociation_WhenIFilter_ThenStudentIsTaken()
        {
            Student student = new Student
            {
                SchoolId = 738,
                ServiceRequests = new List<ServiceRequest>
                {
                    new ServiceRequest { ServiceType = new ServiceType { Name = "Mentoring" } }
                }
            };
            PrepareDataTableRequestParameters("0", "asc", null, null, null, null, null, null, null, "Tutoring|Mentoring", null);
            Target = new StudentClientDataTable(MockContext.Request, CreateUser(false, new[] { 738 }), StudentProperties);

            Assert.IsTrue(Target.FilterPredicate.Compile().Invoke(student));
        }

        [TestMethod]
        public void GivenUserIsDataAdmin_AndServiceTypeListRequestParam_AndStudentHasNoServiceRequestWithServiceType_WhenIFilter_ThenStudentIsNotTaken()
        {
            Student student = new Student
            {
                ServiceRequests = new List<ServiceRequest>
                {
                    new ServiceRequest { ServiceType = new ServiceType { Name = "Health" } }
                }
            };
            PrepareDataTableRequestParameters("0", "asc", null, null, null, null, null, null, null, "Tutoring|Mentoring", null);
            Target = new StudentClientDataTable(MockContext.Request, CreateUser(true), StudentProperties);

            Assert.IsFalse(Target.FilterPredicate.Compile().Invoke(student));
        }

        [TestMethod]
        public void GivenServiceTypeListRequestParam_AndStudentHasNoServiceRequestWithServiceType_WhenIFilter_ThenStudentIsNotTaken()
        {
            Student student = new Student
            {
                ServiceRequests = new List<ServiceRequest>
                {
                    new ServiceRequest { ServiceType = new ServiceType { Name = "Health" } }
                }
            };
            PrepareDataTableRequestParameters("0", "asc", null, null, null, null, null, null, null, "Tutoring|Mentoring", null);
            Target = new StudentClientDataTable(MockContext.Request, CreateUser(false), StudentProperties);

            Assert.IsFalse(Target.FilterPredicate.Compile().Invoke(student));
        }

        [TestMethod]
        public void GivenUserIsDataAdmin_AndSubjectListRequestParam_AndStudentHasServiceRequestWithSubject_WhenIFilter_ThenStudentIsTaken()
        {
            Student student = new Student
            {
                ServiceRequests = new List<ServiceRequest>
                {
                    new ServiceRequest { Subject = new Subject { Name = "Reading" } }
                }
            };
            PrepareDataTableRequestParameters("0", "asc", null, null, null, null, null, null, null, null, "Math|Reading");
            Target = new StudentClientDataTable(MockContext.Request, CreateUser(true), StudentProperties);

            Assert.IsTrue(Target.FilterPredicate.Compile().Invoke(student));
        }

        [TestMethod]
        public void GivenSubjectListRequestParam_AndStudentHasServiceRequestWithSubject_AndUserIsNotDataAdmin_AndUserHasSchoolAssociation_WhenIFilter_ThenStudentIsTaken()
        {
            Student student = new Student
            {
                SchoolId = 382,
                ServiceRequests = new List<ServiceRequest>
                {
                    new ServiceRequest { Subject = new Subject { Name = "Reading" } }
                }
            };
            PrepareDataTableRequestParameters("0", "asc", null, null, null, null, null, null, null, null, "Math|Reading");
            Target = new StudentClientDataTable(MockContext.Request, CreateUser(false, new[] { 382 }), StudentProperties);

            Assert.IsTrue(Target.FilterPredicate.Compile().Invoke(student));
        }

        [TestMethod]
        public void GivenUserIsDataAdmin_AndSubjectListRequestParam_AndStudentHasNoServiceRequestWithSubject_WhenIFilter_ThenStudentIsNotTaken()
        {
            Student student = new Student
            {
                ServiceRequests = new List<ServiceRequest>
                {
                    new ServiceRequest { Subject = new Subject { Name = "Science" } }
                }
            };
            PrepareDataTableRequestParameters("0", "asc", null, null, null, null, null, null, null, null, "Math|Reading");
            Target = new StudentClientDataTable(MockContext.Request, CreateUser(true), StudentProperties);

            Assert.IsFalse(Target.FilterPredicate.Compile().Invoke(student));
        }

        [TestMethod]
        public void GivenSubjectListRequestParam_AndStudentHasNoServiceRequestWithSubject_WhenIFilter_ThenStudentIsNotTaken()
        {
            Student student = new Student
            {
                ServiceRequests = new List<ServiceRequest>
                {
                    new ServiceRequest { Subject = new Subject { Name = "Science" } }
                }
            };
            PrepareDataTableRequestParameters("0", "asc", null, null, null, null, null, null, null, null, "Math|Reading");
            Target = new StudentClientDataTable(MockContext.Request, CreateUser(false), StudentProperties);

            Assert.IsFalse(Target.FilterPredicate.Compile().Invoke(student));
        }

        [TestMethod]
        public void GivenNoFilterParameters_AndUserIsNotDataAdmin_AndUserHasNoSchoolAssociationWithStudent_WhenIFilter_ThenStudentIsTaken()
        {
            Student student = new Student { SchoolId = 3 };
            PrepareDataTableRequestParameters("0", "asc", null, null, null, null, null, null, null, null, null);
            Target = new StudentClientDataTable(MockContext.Request, CreateUser(false), StudentProperties);

            Assert.IsTrue(Target.FilterPredicate.Compile().Invoke(student));
        }

        [TestMethod]
        public void GivenNoFilterParameters_AndUserIsNotDataAdmin_AndUserHasNoSchoolAssociationWithStudent_AndStudentHasParentalOptOut_WhenIFilter_ThenStudentIsNotTaken()
        {
            Student student = new Student { SchoolId = 3, HasParentalOptOut = true };
            PrepareDataTableRequestParameters("0", "asc", null, null, null, null, null, null, null, null, null);
            Target = new StudentClientDataTable(MockContext.Request, CreateUser(false), StudentProperties);

            Assert.IsFalse(Target.FilterPredicate.Compile().Invoke(student));
        }

        [TestMethod]
        public void GivenNoFilterParameters_AndUserIsNotDataAdmin_AndUserHasSchoolAssociationWithStudent_WhenIFilter_ThenStudentIsTaken()
        {
            Student student = new Student { SchoolId = 3 };
            PrepareDataTableRequestParameters("0", "asc", null, null, null, null, null, null, null, null, null);
            Target = new StudentClientDataTable(MockContext.Request, CreateUser(false, new[] { 3 }), StudentProperties);

            Assert.IsTrue(Target.FilterPredicate.Compile().Invoke(student));
        }

        [TestMethod]
        public void GivenNoFilterParameters_AndUserIsNotDataAdmin_AndUserHasSchoolAssociationWithStudent_AndStudentHasParentalOptOut_WhenIFilter_ThenStudentIsNotTaken()
        {
            Student student = new Student { SchoolId = 3, HasParentalOptOut = true };
            PrepareDataTableRequestParameters("0", "asc", null, null, null, null, null, null, null, null, null);
            Target = new StudentClientDataTable(MockContext.Request, CreateUser(false, new[] { 3 }), StudentProperties);

            Assert.IsTrue(Target.FilterPredicate.Compile().Invoke(student));
        }

        [TestMethod]
        public void GivenPermissionDoesntGrantAccess_WhenInvokeDataSelector_ThenDataIsReturned()
        {
            var expected = string.Format("{0}, {1} {2}", TestData.Students[1].LastName, TestData.Students[1].FirstName, TestData.Students[1].MiddleName);
            var user = CreateUser(false);
            Target = new StudentClientDataTable(MockContext.Request, user, StudentProperties);
            IViewStudentDetailPermission permission = MockRepository.GenerateMock<IViewStudentDetailPermission>();
            permission.Expect(p => p.GrantAccess(user)).Throw(new EntityAccessUnauthorizedException());
            PermissionFactory.Current.Expect(m => m.Create("CreateStudentNameString", TestData.Students[1])).Return(permission);
            PermissionFactory.Current.Expect(m => m.Create("ViewStudentDetail", TestData.Students[1])).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());

            var actual = (string[])Target.DataSelector.Compile().Invoke(TestData.Students[1]);

            Assert.AreEqual(expected, actual[2]);
        }

        [TestMethod]
        public void GivenUserIsDataAdmin_WhenInvokeDataSelector_ThenDataIsReturned()
        {
            var expected = string.Format("{0}, {1} {2}|{3}", TestData.Students[0].LastName, TestData.Students[0].FirstName, TestData.Students[0].MiddleName, TestData.Students[0].Id);
            Target = new StudentClientDataTable(MockContext.Request, CreateUser(true), StudentProperties);
            foreach (var request in TestData.Students[0].ServiceRequests)
            {
                PermissionFactory.Current.Expect(m => m.Create("CreateServiceRequestString", request)).Return(MockRepository.GenerateMock<IPermission>());
            }
            PermissionFactory.Current.Expect(m => m.Create("CreateStudentNameString", TestData.Students[0])).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());
            PermissionFactory.Current.Expect(m => m.Create("ViewStudentDetail", TestData.Students[0])).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());

            var actual = (string[])Target.DataSelector.Compile().Invoke(TestData.Students[0]);

            Assert.AreEqual(expected, actual[2]);
        }

        [TestMethod]
        public void GivenUserIsDataAdmin_AndCreateServiceOfferingStringBuilt_WhenInvokeDataSelector_ThenDataIsReturned()
        {
            IEnumerable<string> serviceOfferings = TestData.StudentAssignedOfferings.Where(s => s.StudentId == TestData.Students[0].Id).
                        Select(a => string.Format("Y|{0}|{1}|", a.Id, a.ServiceOffering.Name));
            var expected = BuildListString(serviceOfferings);
            Target = new StudentClientDataTable(MockContext.Request, CreateUser(true), StudentProperties);
            foreach (var request in TestData.Students[0].ServiceRequests)
            {
                PermissionFactory.Current.Expect(m => m.Create("CreateServiceRequestString", request)).Return(MockRepository.GenerateMock<IPermission>());
            }
            PermissionFactory.Current.Expect(m => m.Create("CreateStudentNameString", TestData.Students[0])).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());
            PermissionFactory.Current.Expect(m => m.Create("ViewStudentDetail", TestData.Students[0])).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());

            var actual = (string[])Target.DataSelector.Compile().Invoke(TestData.Students[0]);

            Assert.AreEqual(expected, actual[5]);
        }

        [TestMethod]
        public void GivenUserIsDataAdmin_AndCreateServiceOfferingStringBuilt_WhenInvokeDataSelector_ThenInactiveNotReturned()
        {
            IEnumerable<string> serviceOfferings = TestData.StudentAssignedOfferings.Where(s => s.StudentId == TestData.Students[2].Id && s.IsActive).
                        Select(a => string.Format("Y|{0}|{1}|", a.Id, a.ServiceOffering.Name));
            var expected = BuildListString(serviceOfferings);
            Target = new StudentClientDataTable(MockContext.Request, CreateUser(true), StudentProperties);
            PermissionFactory.Current.Expect(m => m.Create("CreateStudentNameString", TestData.Students[2])).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());
            PermissionFactory.Current.Expect(m => m.Create("ViewStudentDetail", TestData.Students[2])).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());

            var actual = (string[])Target.DataSelector.Compile().Invoke(TestData.Students[2]);

            Assert.AreEqual(expected, actual[5]);
        }

        [TestMethod]
        public void GivenUserIsNotDataAdmin_AndCreateServiceOfferingStringBuilt_WhenInvokeDataSelector_ThenDataIsReturned()
        {
            Target = new StudentClientDataTable(MockContext.Request, CreateUser(false), StudentProperties);
            foreach (var request in TestData.Students[0].ServiceRequests)
            {
                PermissionFactory.Current.Expect(m => m.Create("CreateServiceRequestString", request)).Return(MockRepository.GenerateMock<IPermission>());
            }
            PermissionFactory.Current.Expect(m => m.Create("CreateStudentNameString", TestData.Students[0])).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());
            PermissionFactory.Current.Expect(m => m.Create("ViewStudentDetail", TestData.Students[0])).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());

            var actual = (string[])Target.DataSelector.Compile().Invoke(TestData.Students[0]);

            Assert.AreEqual(string.Empty, actual[5]);
        }

        [TestMethod]
        public void GivenUserIsNotDataAdmin_AndDoesNotHavePermission_WhenInvokeDataSelector_ThenStudentSISIdIsBlank()
        {
            var user = CreateUser(false);
            user.Identity.User.UserRoles.Add(new UserRole
            { 
                Role = TestData.Roles.Where(r => r.Name.Equals(SecurityRoles.Provider)).SingleOrDefault()
            });
            Target = new StudentClientDataTable(MockContext.Request, user, StudentProperties);
            IViewStudentDetailPermission permission = MockRepository.GenerateMock<IViewStudentDetailPermission>();
            permission.Expect(p => p.GrantAccess(user)).Throw(new EntityAccessUnauthorizedException());
            PermissionFactory.Current.Expect(m => m.Create("ViewStudentDetail", TestData.Students[1])).Return(permission);
            PermissionFactory.Current.Expect(m => m.Create("CreateStudentNameString", TestData.Students[1])).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());

            var actual = (string[])Target.DataSelector.Compile().Invoke(TestData.Students[1]);

            Assert.AreEqual(string.Empty, actual[1]);
        }

        [TestMethod]
        public void GivenUserIsNotDataAdmin_AndDoNotHaveAnyServiceOfferingsCreateServiceOfferingStringBuilt_WhenInvokeDataSelector_ThenDataIsReturned()
        {
            Target = new StudentClientDataTable(MockContext.Request, CreateUser(false), StudentProperties);
            PermissionFactory.Current.Expect(m => m.Create("CreateStudentNameString", TestData.Students[1])).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());
            PermissionFactory.Current.Expect(m => m.Create("ViewStudentDetail", TestData.Students[1])).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());

            var actual = (string[])Target.DataSelector.Compile().Invoke(TestData.Students[1]);

            Assert.AreEqual(string.Empty, actual[5]);
        }

        [TestMethod]
        public void GivenUserIsDataAdmin_AndServiceRequestHasNoSubject_WhenInvokeDataSelector_ThenServiceRequestStringIsReturnedWithBlankSubject()
        {
            IEnumerable<string> priorities = TestData.ServiceRequests.Where(r => r.Id == 1).Select(r => string.Format("Y|{0}|{1}|{2}|{3}|{4}|", r.Id, r.ServiceType.Name, r.Priority.Id.ToString(), r.Subject.Name.Equals(Subject.DefaultName) ? "" : r.Subject.Name, r.FulfillmentDetails.OrderByDescending(f => f.CreateTime).First().FulfillmentStatusId));
            var expected = BuildListString(priorities);
            Target = new StudentClientDataTable(MockContext.Request, CreateUser(true), StudentProperties);
            foreach (var request in TestData.Students[0].ServiceRequests)
            {
                PermissionFactory.Current.Expect(m => m.Create("CreateServiceRequestString", request)).Return(MockRepository.GenerateMock<IPermission>());
            }
            PermissionFactory.Current.Expect(m => m.Create("CreateStudentNameString", TestData.Students[0])).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());
            PermissionFactory.Current.Expect(m => m.Create("ViewStudentDetail", TestData.Students[0])).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());

            var actual = (string[])Target.DataSelector.Compile().Invoke(TestData.Students[0]);

            Assert.AreEqual(expected, actual[0]);
        }

        [TestMethod]
        public void GivenUserIsDataAdmin_WhenInvokeDataSelector_ThenServiceRequestStringIsReturned()
        {
            IEnumerable<string> priorities = TestData.ServiceRequests.Where(r => r.Id == 1).Select(r => string.Format("Y|{0}|{1}|{2}|{3}|{4}|", r.Id, r.ServiceType.Name, r.Priority.Id.ToString(), r.Subject.Name.Equals(Subject.DefaultName) ? "" : r.Subject.Name, r.FulfillmentDetails.OrderByDescending(f => f.CreateTime).First().FulfillmentStatusId));
            var expected = BuildListString(priorities);
            Target = new StudentClientDataTable(MockContext.Request, CreateUser(true), StudentProperties);
            foreach (var request in TestData.Students[0].ServiceRequests)
            {
                PermissionFactory.Current.Expect(m => m.Create("CreateServiceRequestString", request)).Return(MockRepository.GenerateMock<IPermission>());
            }
            PermissionFactory.Current.Expect(m => m.Create("CreateStudentNameString", TestData.Students[0])).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());
            PermissionFactory.Current.Expect(m => m.Create("ViewStudentDetail", TestData.Students[0])).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());

            var actual = (string[])Target.DataSelector.Compile().Invoke(TestData.Students[0]);

            Assert.AreEqual(expected, actual[0]);
        }

        [TestMethod]
        public void GivenUserIsSiteCoordinator_WhenInvokeDataSelector_ThenServiceRequestStringIsReturned()
        {
            var student = TestData.Students[0];
            student.School.UserRoles = new List<UserRole> { new UserRole { UserId = 1 } };
            IEnumerable<string> priorities = TestData.ServiceRequests.Where(r => r.Id == 1).Select(r => string.Format("Y|{0}|{1}|{2}|{3}|{4}|", r.Id, r.ServiceType.Name, r.Priority.Id.ToString(), r.Subject.Name.Equals(Subject.DefaultName) ? "" : r.Subject.Name, r.FulfillmentDetails.OrderByDescending(f => f.CreateTime).First().FulfillmentStatusId));
            var expected = BuildListString(priorities);
            var associatedSchoolIds = new List<int> { TestData.Students[0].School.Id };
            var user = CreateUser(false, associatedSchoolIds);
            user.Identity.User.UserRoles.Single().Schools = new List<School> { TestData.Students[0].School };
            Target = new StudentClientDataTable(MockContext.Request, user, StudentProperties);
            foreach (var request in student.ServiceRequests)
            {
                PermissionFactory.Current.Expect(m => m.Create("CreateServiceRequestString", request)).Return(MockRepository.GenerateMock<IPermission>());
            }
            PermissionFactory.Current.Expect(m => m.Create("CreateStudentNameString", student)).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());
            PermissionFactory.Current.Expect(m => m.Create("ViewStudentDetail", TestData.Students[0])).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());

            var actual = (string[])Target.DataSelector.Compile().Invoke(student);

            Assert.AreEqual(expected, actual[0]);
        }

        [TestMethod]
        public void GivenUserIsProvider_AndUserNotAssignedToProviders_WhenInvokeDataSelector_ThenNoServiceRequestStringIsReturned()
        {
            var user = CreateUser(false);
            user.Identity.User.UserRoles.Add(new UserRole
            { 
                Role = TestData.Roles.Where(r => r.Name.Equals(SecurityRoles.Provider)).SingleOrDefault()
            });
            var student = TestData.Students[0];
            student.ApprovedProviders = TestData.Providers;
            Target = new StudentClientDataTable(MockContext.Request, user, StudentProperties);
            foreach (var request in student.ServiceRequests)
            {
                IPermission permission = MockRepository.GenerateMock<IPermission>();
                permission.Expect(p => p.GrantAccess(user)).Throw(new EntityAccessUnauthorizedException());
                PermissionFactory.Current.Expect(m => m.Create("CreateServiceRequestString", request)).Return(permission);
            }
            PermissionFactory.Current.Expect(m => m.Create("CreateStudentNameString", student)).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());
            PermissionFactory.Current.Expect(m => m.Create("ViewStudentDetail", TestData.Students[0])).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());

            var actual = (string[])Target.DataSelector.Compile().Invoke(student);

            Assert.AreEqual(string.Empty, actual[0]);
        }

        [TestMethod]
        public void GivenUserIsProvider_AndUserAssignedAllProviders_AndStudentApprovedAllProviders_AndStudentHasAssignedOffering_WhenInvokeDataSelector_ThenServiceRequestStringIsReturned()
        {
            IEnumerable<string> priorities = TestData.ServiceRequests.Where(r => r.Id == 1).Select(r => string.Format("N|{0}|{1}|{2}|{3}|{4}|", r.Id, r.ServiceType.Name, r.Priority.Id.ToString(), r.Subject.Name.Equals(Subject.DefaultName) ? "" : r.Subject.Name, r.FulfillmentDetails.OrderByDescending(f => f.CreateTime).First().FulfillmentStatusId));
            var expected = BuildListString(priorities);
            var user = CreateUser(false);
            user.Identity.User.UserRoles.Add(new UserRole
            {
                Role = TestData.Roles.Where(r => r.Name.Equals(SecurityRoles.Provider)).SingleOrDefault(),
                Providers = TestData.Providers
            });
            var student = TestData.Students[0];
            student.ApprovedProviders = TestData.Providers;
            Target = new StudentClientDataTable(MockContext.Request, user, StudentProperties);
            foreach (var request in student.ServiceRequests)
            {
                PermissionFactory.Current.Expect(m => m.Create("CreateServiceRequestString", request)).Return(MockRepository.GenerateMock<IPermission>());
            }
            PermissionFactory.Current.Expect(m => m.Create("CreateStudentNameString", student)).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());
            PermissionFactory.Current.Expect(m => m.Create("ViewStudentDetail", TestData.Students[0])).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());

            var actual = (string[])Target.DataSelector.Compile().Invoke(student);

            Assert.AreEqual(expected, actual[0]);
        }

        [TestMethod]
        public void GivenUserIsProvider_AndUserAssignedAllProviders_AndStudentApprovedAllProviders_AndStudentHasNoAssignedOfferings_WhenInvokeDataSelector_ThenServiceRequestStringIsReturned()
        {
            var user = CreateUser(false);
            user.Identity.User.UserRoles.Add(new UserRole
            {
                Role = TestData.Roles.Where(r => r.Name.Equals(SecurityRoles.Provider)).SingleOrDefault(),
                Providers = TestData.Providers
            });
            var student = TestData.Students[0];
            student.ApprovedProviders = TestData.Providers;
            student.StudentAssignedOfferings = new List<StudentAssignedOffering>();
            Target = new StudentClientDataTable(MockContext.Request, user, StudentProperties);
            foreach (var request in student.ServiceRequests)
            {
                IPermission permission = MockRepository.GenerateMock<IPermission>();
                permission.Expect(p => p.GrantAccess(user)).Throw(new EntityAccessUnauthorizedException());
                PermissionFactory.Current.Expect(m => m.Create("CreateServiceRequestString", request)).Return(permission);
            }
            PermissionFactory.Current.Expect(m => m.Create("CreateStudentNameString", student)).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());
            PermissionFactory.Current.Expect(m => m.Create("ViewStudentDetail", TestData.Students[0])).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());

            var actual = (string[])Target.DataSelector.Compile().Invoke(student);

            Assert.AreEqual(string.Empty, actual[0]);
        }

        [TestMethod]
        public void GivenUserIsProvider_AndUserAssignedAllProviders_AndStudentApprovedNoProviders_WhenInvokeDataSelector_ThenServiceRequestStringIsReturned()
        {
            var user = CreateUser(false);
            user.Identity.User.UserRoles.Add(new UserRole
            {
                Role = TestData.Roles.Where(r => r.Name.Equals(SecurityRoles.Provider)).SingleOrDefault(),
                Providers = TestData.Providers
            });
            var student = TestData.Students[2];
            Target = new StudentClientDataTable(MockContext.Request, user, StudentProperties);
            PermissionFactory.Current.Expect(m => m.Create("CreateStudentNameString", student)).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());
            PermissionFactory.Current.Expect(m => m.Create("ViewStudentDetail", TestData.Students[2])).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());

            var actual = (string[])Target.DataSelector.Compile().Invoke(student);

            Assert.AreEqual(string.Empty, actual[0]);
        }

        [TestMethod]
        public void GivenUserIsSiteCoordinatorWithInvalidSchool_WhenInvokeDataSelector_ThenServiceRequestStringIsReturned()
        {
            var associatedSchoolIds = new List<int> { 758937498 };
            var user = CreateUser(false, associatedSchoolIds);
            Target = new StudentClientDataTable(MockContext.Request, user, StudentProperties);
            foreach (var request in TestData.Students[0].ServiceRequests)
            {
                IPermission permission = MockRepository.GenerateMock<IPermission>();
                permission.Expect(p => p.GrantAccess(user)).Throw(new EntityAccessUnauthorizedException());
                PermissionFactory.Current.Expect(m => m.Create("CreateServiceRequestString", request)).Return(permission);
            }
            PermissionFactory.Current.Expect(m => m.Create("CreateStudentNameString", TestData.Students[0])).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());
            PermissionFactory.Current.Expect(m => m.Create("ViewStudentDetail", TestData.Students[0])).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());

            var actual = (string[])Target.DataSelector.Compile().Invoke(TestData.Students[0]);

            Assert.AreEqual(string.Empty, actual[0]);
        }

        [TestMethod]
        public void GivenUserIsNotDataAdmin_WhenInvokeDataSelector_ThenServiceRequestStringIsReturned()
        {
            Target = new StudentClientDataTable(MockContext.Request, CreateUser(false), StudentProperties);
            foreach (var request in TestData.Students[0].ServiceRequests)
            {
                PermissionFactory.Current.Expect(m => m.Create("CreateServiceRequestString", request)).Return(MockRepository.GenerateMock<IPermission>());
            }
            PermissionFactory.Current.Expect(m => m.Create("CreateStudentNameString", TestData.Students[0])).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());
            PermissionFactory.Current.Expect(m => m.Create("ViewStudentDetail", TestData.Students[0])).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());

            var actual = (string[])Target.DataSelector.Compile().Invoke(TestData.Students[0]);

            Assert.AreEqual(string.Empty, actual[0]);
        }

        [TestMethod]
        public void GivenUserIsDataAdmin_WhenInvokeDataSelector_ThenServiceRequestIsViewable()
        {
            var student = TestData.Students[0];
            var priorities = new List<string>();
            foreach(ServiceRequest request in student.ServiceRequests)
            {
                PermissionFactory.Current.Expect(m => m.Create("CreateServiceRequestString", request)).Return(MockRepository.GenerateMock<IPermission>());
                priorities.Add(string.Format(CultureInfo.CurrentCulture, "Y|{0}|{1}|{2}|{3}|{4}|", request.Id, request.ServiceType.Name, request.Priority.Id, request.Subject.Name.Equals(Subject.DefaultName) ? "" : request.Subject.Name, request.FulfillmentDetails.OrderByDescending(f => f.CreateTime).First().FulfillmentStatusId));
            }
            var expected = BuildListString(priorities);
            Target = new StudentClientDataTable(MockContext.Request, CreateUser(true), StudentProperties);
            PermissionFactory.Current.Expect(m => m.Create("CreateStudentNameString", student)).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());
            PermissionFactory.Current.Expect(m => m.Create("ViewStudentDetail", TestData.Students[0])).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());

            var actual = (string[])Target.DataSelector.Compile().Invoke(student);

            Assert.AreEqual(expected, actual[0]);
        }

        [TestMethod]
        public void GivenUserIsSiteCoordinator_AndUserIsAssignedToSchool_WhenInvokeDataSelector_ThenServiceRequestIsViewable()
        {
            var student = TestData.Students[0];
            student.School.UserRoles = new List<UserRole> { new UserRole{ UserId = 1 }};
            var priorities = new List<string>();
            foreach (ServiceRequest request in student.ServiceRequests)
            {
                priorities.Add(string.Format(CultureInfo.CurrentCulture, "Y|{0}|{1}|{2}|{3}|{4}|", request.Id, request.ServiceType.Name, request.Priority.Id, request.Subject.Name.Equals(Subject.DefaultName) ? "" : request.Subject.Name, request.FulfillmentDetails.OrderByDescending(f => f.CreateTime).First().FulfillmentStatusId));
            }
            var expected = BuildListString(priorities);
            var user = CreateUser(false, new List<int> { TestData.Students[0].SchoolId });
            user.Identity.User.UserRoles.Single().Schools = new List<School> { TestData.Students[0].School };
            Target = new StudentClientDataTable(MockContext.Request, user, StudentProperties);
            foreach (var request in student.ServiceRequests)
            {
                PermissionFactory.Current.Expect(m => m.Create("CreateServiceRequestString", request)).Return(MockRepository.GenerateMock<IPermission>());
            }
            PermissionFactory.Current.Expect(m => m.Create("CreateStudentNameString", student)).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());
            PermissionFactory.Current.Expect(m => m.Create("ViewStudentDetail", TestData.Students[0])).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());

            var actual = (string[])Target.DataSelector.Compile().Invoke(student);

            Assert.AreEqual(expected, actual[0]);
        }

        [TestMethod]
        public void GivenUserIsSiteCoordinator_AndUserIsNotAssignedToSchool_WhenInvokeDataSelector_ThenServiceRequestIsNotViewable()
        {
            var student = TestData.Students[0];
            var expected = "";
            var user = CreateUser(false, new List<int>());
            Target = new StudentClientDataTable(MockContext.Request, user, StudentProperties);
            foreach (var request in student.ServiceRequests)
            {
                IPermission permission = MockRepository.GenerateMock<IPermission>();
                permission.Expect(p => p.GrantAccess(user)).Throw(new EntityAccessUnauthorizedException());
                PermissionFactory.Current.Expect(m => m.Create("CreateServiceRequestString", request)).Return(permission);
            }
            PermissionFactory.Current.Expect(m => m.Create("CreateStudentNameString", student)).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());
            PermissionFactory.Current.Expect(m => m.Create("ViewStudentDetail", TestData.Students[0])).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());

            var actual = (string[])Target.DataSelector.Compile().Invoke(student);

            Assert.AreEqual(expected, actual[0]);
        }

        [TestMethod]
        public void GivenUserIsAnAssignedProvider_WhenInvokeDataSelector_ThenServiceRequestIsViewable()
        {
            var student = TestData.Students[1];
            student.ServiceRequests = TestData.ServiceRequests;
            var priorities = new List<string>();
            foreach (ServiceRequest request in student.ServiceRequests)
            {
                priorities.Add(string.Format(CultureInfo.CurrentCulture, "Y|{0}|{1}|{2}|{3}|{4}|", request.Id, request.ServiceType.Name, request.Priority.Id, request.Subject.Name.Equals(Subject.DefaultName) ? "" : request.Subject.Name, request.FulfillmentDetails.OrderByDescending(f => f.CreateTime).First().FulfillmentStatusId));
            }
            var expected = BuildListString(priorities);
            var user = CreateUser(false);
            user.Identity.User.UserRoles = TestData.UserRoles;
            Target = new StudentClientDataTable(MockContext.Request, user, StudentProperties);
            foreach (var request in student.ServiceRequests)
            {
                PermissionFactory.Current.Expect(m => m.Create("CreateServiceRequestString", request)).Return(MockRepository.GenerateMock<IPermission>());
            }
            PermissionFactory.Current.Expect(m => m.Create("CreateStudentNameString", student)).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());
            PermissionFactory.Current.Expect(m => m.Create("ViewStudentDetail", TestData.Students[1])).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());

            var actual = (string[])Target.DataSelector.Compile().Invoke(student);

            Assert.AreEqual(expected, actual[0]);
        }

        [TestMethod]
        public void GivenUserIsAnUnassignedProvider_WhenInvokeDataSelector_ThenServiceRequestIsNotViewable()
        {
            var student = TestData.Students[1];
            var expected = "";
            var user = CreateUser(false);
            user.Identity.User.UserRoles = TestData.UserRoles;
            Target = new StudentClientDataTable(MockContext.Request, user, StudentProperties);
            PermissionFactory.Current.Expect(m => m.Create("CreateStudentNameString", student)).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());
            PermissionFactory.Current.Expect(m => m.Create("ViewStudentDetail", TestData.Students[1])).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());

            var actual = (string[])Target.DataSelector.Compile().Invoke(student);

            Assert.AreEqual(expected, actual[0]);
        }

        [TestMethod]
        public void GivenUserIsDataAdmin_WhenInvokeDataSelector_ThenServiceOfferingIsViewable()
        {
            var student = TestData.Students[0];
            student.StudentAssignedOfferings = TestData.StudentAssignedOfferings;
            var serviceOfferings = new List<string>();
            foreach (StudentAssignedOffering offering in student.StudentAssignedOfferings.Where(s => s.IsActive))
            {
                serviceOfferings.Add(string.Format(CultureInfo.CurrentCulture, "Y|{0}|{1}|", offering.Id, offering.ServiceOffering.Name));
            }
            var expected = BuildListString(serviceOfferings);
            Target = new StudentClientDataTable(MockContext.Request, CreateUser(true), StudentProperties);
            foreach (var request in student.ServiceRequests)
            {
                PermissionFactory.Current.Expect(m => m.Create("CreateServiceRequestString", request)).Return(MockRepository.GenerateMock<IPermission>());
            }
            PermissionFactory.Current.Expect(m => m.Create("CreateStudentNameString", student)).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());
            PermissionFactory.Current.Expect(m => m.Create("ViewStudentDetail", TestData.Students[0])).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());

            var actual = (string[])Target.DataSelector.Compile().Invoke(student);

            Assert.AreEqual(expected, actual[5]);
        }

        [TestMethod]
        public void GivenUserIsAnAssignedSiteCoordinator_WhenInvokeDataSelector_ThenServiceOfferingIsViewable()
        {
            var student = TestData.Students[0];
            student.StudentAssignedOfferings = TestData.StudentAssignedOfferings;
            var serviceOfferings = new List<string>();
            foreach (StudentAssignedOffering offering in student.StudentAssignedOfferings.Where(s => s.IsActive))
            {
                serviceOfferings.Add(string.Format(CultureInfo.CurrentCulture, "Y|{0}|{1}|", offering.Id, offering.ServiceOffering.Name));
            }
            var expected = BuildListString(serviceOfferings);
            Target = new StudentClientDataTable(MockContext.Request, CreateUser(false, new List<int> { TestData.Students[0].SchoolId }), StudentProperties);
            foreach (var request in student.ServiceRequests)
            {
                PermissionFactory.Current.Expect(m => m.Create("CreateServiceRequestString", request)).Return(MockRepository.GenerateMock<IPermission>());
            }
            PermissionFactory.Current.Expect(m => m.Create("CreateStudentNameString", student)).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());
            PermissionFactory.Current.Expect(m => m.Create("ViewStudentDetail", TestData.Students[0])).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());

            var actual = (string[])Target.DataSelector.Compile().Invoke(student);

            Assert.AreEqual(expected, actual[5]);
        }

        [TestMethod]
        public void GivenUserIsAnUnassignedSiteCoordinator_WhenInvokeDataSelector_ThenServiceOfferingIsNotViewable()
        {
            var student = TestData.Students[0];
            var expected = "";
            Target = new StudentClientDataTable(MockContext.Request, CreateUser(false, new List<int>()), StudentProperties);
            foreach (var request in student.ServiceRequests)
            {
                PermissionFactory.Current.Expect(m => m.Create("CreateServiceRequestString", request)).Return(MockRepository.GenerateMock<IPermission>());
            }
            PermissionFactory.Current.Expect(m => m.Create("CreateStudentNameString", student)).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());
            PermissionFactory.Current.Expect(m => m.Create("ViewStudentDetail", TestData.Students[0])).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());

            var actual = (string[])Target.DataSelector.Compile().Invoke(student);

            Assert.AreEqual(expected, actual[5]);
        }

        [TestMethod]
        public void GivenUserIsAnAssignedProvider_WhenInvokeDataSelector_ThenServiceOfferingIsViewable()
        {
            var student = TestData.Students[1];
            student.StudentAssignedOfferings = TestData.StudentAssignedOfferings;
            student.ServiceRequests = TestData.ServiceRequests;
            var serviceOfferings = new List<string>();
            foreach (StudentAssignedOffering offering in student.StudentAssignedOfferings.Where(s => s.IsActive))
            {
                serviceOfferings.Add(string.Format(CultureInfo.CurrentCulture, "Y|{0}|{1}|", offering.Id, offering.ServiceOffering.Name));
            }
            var expected = BuildListString(serviceOfferings);
            var user = CreateUser(false);
            user.Identity.User.UserRoles = TestData.UserRoles;
            Target = new StudentClientDataTable(MockContext.Request, user, StudentProperties);
            foreach (var request in student.ServiceRequests)
            {
                PermissionFactory.Current.Expect(m => m.Create("CreateServiceRequestString", request)).Return(MockRepository.GenerateMock<IPermission>());
            }
            PermissionFactory.Current.Expect(m => m.Create("CreateStudentNameString", student)).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());
            PermissionFactory.Current.Expect(m => m.Create("ViewStudentDetail", TestData.Students[1])).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());

            var actual = (string[])Target.DataSelector.Compile().Invoke(student);

            Assert.AreEqual(expected, actual[5]);
        }

        [TestMethod]
        public void GivenUserIsAnUnassignedProvider_WhenInvokeDataSelector_ThenServiceOfferingIsNotViewable()
        {
            var student = TestData.Students[1];
            var expected = "";
            var user = CreateUser(false);
            user.Identity.User.UserRoles = TestData.UserRoles;
            Target = new StudentClientDataTable(MockContext.Request, user, StudentProperties);
            PermissionFactory.Current.Expect(m => m.Create("CreateStudentNameString", student)).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());
            PermissionFactory.Current.Expect(m => m.Create("ViewStudentDetail", TestData.Students[1])).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());

            var actual = (string[])Target.DataSelector.Compile().Invoke(student);

            Assert.AreEqual(expected, actual[5]);
        }

        [TestMethod]
        public void GivenUserIsDataAdmin_WhenInvokeDataSelector_ThenCheckboxIsViewable()
        {
            var student = TestData.Students[0];
            var expected = student.Id.ToString(CultureInfo.CurrentCulture);
            Target = new StudentClientDataTable(MockContext.Request, CreateUser(true), StudentProperties);
            foreach (var request in student.ServiceRequests)
            {
                PermissionFactory.Current.Expect(m => m.Create("CreateServiceRequestString", request)).Return(MockRepository.GenerateMock<IPermission>());
            }
            PermissionFactory.Current.Expect(m => m.Create("CreateStudentNameString", student)).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());
            PermissionFactory.Current.Expect(m => m.Create("ViewStudentDetail", TestData.Students[0])).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());

            var actual = (string[])Target.DataSelector.Compile().Invoke(student);

            Assert.AreEqual(expected, actual[6]);
        }

        [TestMethod]
        public void GivenUserIsAnAssignedSiteCoordinator_WhenInvokeDataSelector_ThenCheckboxIsViewable()
        {
            var student = TestData.Students[0];
            var expected = student.Id.ToString(CultureInfo.CurrentCulture);
            Target = new StudentClientDataTable(MockContext.Request, CreateUser(false, new List<int> { TestData.Students[0].SchoolId }), StudentProperties);
            foreach (var request in student.ServiceRequests)
            {
                PermissionFactory.Current.Expect(m => m.Create("CreateServiceRequestString", request)).Return(MockRepository.GenerateMock<IPermission>());
            }
            PermissionFactory.Current.Expect(m => m.Create("CreateStudentNameString", student)).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());
            PermissionFactory.Current.Expect(m => m.Create("ViewStudentDetail", TestData.Students[0])).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());

            var actual = (string[])Target.DataSelector.Compile().Invoke(student);

            Assert.AreEqual(expected, actual[6]);
        }

        [TestMethod]
        public void GivenUserIsAnUnassignedSiteCoordinator_WhenInvokeDataSelector_ThenCheckboxIsNotViewable()
        {
            var student = TestData.Students[0];
            var expected = "";
            Target = new StudentClientDataTable(MockContext.Request, CreateUser(false, new List<int>()), StudentProperties);
            foreach (var request in student.ServiceRequests)
            {
                PermissionFactory.Current.Expect(m => m.Create("CreateServiceRequestString", request)).Return(MockRepository.GenerateMock<IPermission>());
            }
            PermissionFactory.Current.Expect(m => m.Create("CreateStudentNameString", student)).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());
            PermissionFactory.Current.Expect(m => m.Create("ViewStudentDetail", TestData.Students[0])).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());

            var actual = (string[])Target.DataSelector.Compile().Invoke(student);

            Assert.AreEqual(expected, actual[6]);
        }

        [TestMethod]
        public void GivenUserIsAnAssignedProvider_WhenInvokeDataSelector_ThenCheckboxIsViewable()
        {
            var student = TestData.Students[1];
            var expected = student.Id.ToString(CultureInfo.CurrentCulture);
            var user = CreateUser(false);
            user.Identity.User.UserRoles = TestData.UserRoles;
            Target = new StudentClientDataTable(MockContext.Request, user, StudentProperties);
            PermissionFactory.Current.Expect(m => m.Create("CreateStudentNameString", student)).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());
            PermissionFactory.Current.Expect(m => m.Create("ViewStudentDetail", TestData.Students[1])).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());

            var actual = (string[])Target.DataSelector.Compile().Invoke(student);

            Assert.AreEqual(expected, actual[6]);
        }

        [TestMethod]
        public void GivenUserIsAnUnassignedProvider_WhenInvokeDataSelector_ThenCheckboxIsNotViewable()
        {
            var student = TestData.Students[1];
            var expected = student.Id.ToString(CultureInfo.CurrentCulture);
            var user = CreateUser(false);
            user.Identity.User.UserRoles = TestData.UserRoles;
            Target = new StudentClientDataTable(MockContext.Request, user, StudentProperties);
            PermissionFactory.Current.Expect(m => m.Create("CreateStudentNameString", student)).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());
            PermissionFactory.Current.Expect(m => m.Create("ViewStudentDetail", TestData.Students[1])).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());

            var actual = (string[])Target.DataSelector.Compile().Invoke(student);

            Assert.AreEqual(expected, actual[6]);
        }

        [TestMethod]
        public void GivenUserIsAProviderNotAssociated_WhenInvokeDataSelector_ThenServiceOfferingIsNotViewable()
        {
            var student = TestData.Students[1];
            var expected = string.Empty;
            var user = CreateUser(false);
            var userRole = TestData.UserRoles.Where(ur => ur.Role.Name.Equals(SecurityRoles.Provider)).First();
            userRole.Providers = new List<Provider>();
            user.Identity.User.UserRoles = new List<UserRole> { userRole };
            student.StudentAssignedOfferings = TestData.StudentAssignedOfferings;
            Target = new StudentClientDataTable(MockContext.Request, user, StudentProperties);
            PermissionFactory.Current.Expect(m => m.Create("CreateStudentNameString", student)).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());
            PermissionFactory.Current.Expect(m => m.Create("ViewStudentDetail", TestData.Students[1])).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());

            var actual = (string[])Target.DataSelector.Compile().Invoke(student);

            Assert.AreEqual(expected, actual[5]);
        }

        [TestMethod]
        public void GivenUserIsAProviderThatUploadedData_WhenInvokeDataSelector_ThenStudentHasId()
        {
            var student = TestData.Students[1];
            var expected = "2";
            var user = CreateUser(false);
            var userRole = TestData.UserRoles.Where(ur => ur.Role.Name.Equals(SecurityRoles.Provider)).First();
            userRole.Providers = new List<Provider>();
            user.Identity.User.UserRoles = new List<UserRole> { userRole };
            student.CustomFieldValues = TestData.CustomFieldValues;
            Target = new StudentClientDataTable(MockContext.Request, user, StudentProperties);
            PermissionFactory.Current.Expect(m => m.Create("CreateStudentNameString", student)).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());
            PermissionFactory.Current.Expect(m => m.Create("ViewStudentDetail", TestData.Students[1])).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());

            var actualStudent = (string[])Target.DataSelector.Compile().Invoke(student);

            string[] actual = actualStudent[2].Split('|');
            Assert.AreEqual(expected, actual[1]);
        }

        private void MockRequestParameter(string paramName, string paramValue)
        {
            if (paramValue != null)
            {
                MockContext.Request.Expect(m => m[paramName]).Return(paramValue);
            }
        }

        private void PrepareDataTableRequestParameters(string sortColumn, string sortDirection, string firstName, string lastName, string id, string schools, string grades, string priorities, string requestStatuses, string serviceTypes, string subjects)
        {
            MockContext.Request.Expect(m => m["iSortCol_0"]).Return(sortColumn); //sorting by ID
            MockContext.Request.Expect(m => m["sSortDir_0"]).Return(sortDirection);
            MockRequestParameter("firstName", firstName);
            MockRequestParameter("lastName", lastName);
            MockRequestParameter("ID", id);
            MockRequestParameter("schools", schools);
            MockRequestParameter("grades", grades);
            MockRequestParameter("priorities", priorities);
            MockRequestParameter("requestStatuses", requestStatuses);
            MockRequestParameter("serviceTypes", serviceTypes);
            MockRequestParameter("subjects", subjects);
        }

        private static string BuildListString(IEnumerable<string> listItems)
        {
            if (listItems.Any())
            {
                string listString = string.Join(string.Empty, listItems);
                return listString.Remove(listString.Length - 1);
            }
            return string.Empty;
        }
    }
}
