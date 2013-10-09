using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SSD.Domain
{
    public class TestData
    {
        private List<CustomDataOrigin> _CustomDataOrigin;
        private List<CustomFieldCategory> _CustomFieldCategories;
        private List<CustomFieldType> _CustomFieldTypes;
        private List<CustomField> _CustomFields;
        private List<CustomFieldValue> _CustomFieldValues;
        private List<Category> _Categories;
        private List<School> _Schools;
        private List<Student> _Students;
        private List<ServiceType> _ServiceTypes;
        private List<ServiceOffering> _ServiceOfferings;
        private List<ServiceRequest> _ServiceRequests;
        private List<Priority> _Priorities;
        private List<Subject> _Subjects;
        private List<Provider> _Providers;
        private List<Program> _Programs;
        private List<User> _Users;
        private List<Role> _Roles;
        private List<UserRole> _UserRoles;
        private List<Property> _Properties;
        private List<StudentAssignedOffering> _StudentAssignedOfferings;
        private List<ServiceAttendance> _ServiceAttendances;
        private List<FulfillmentStatus> _FulfillmentStatuses;
        private List<ServiceRequestFulfillment> _ServiceRequestFulfillments;
        private List<UserAccessChangeEvent> _UserAccessChangeEvents;
        private List<EulaAgreement> _Eulas;
        private List<EulaAcceptance> _EulaAcceptances;
        private List<PrivateHealthDataViewEvent> _PrivateHealthDataViewEvents;
        private List<LoginEvent> _LoginEvents;

        public List<CustomDataOrigin> CustomDataOrigins
        {
            get { return _CustomDataOrigin ?? (_CustomDataOrigin = GetCustomDataOrigins()); }
        }

        public List<CustomFieldCategory> CustomFieldCategories
        {
            get { return _CustomFieldCategories ?? (_CustomFieldCategories = GetCustomFieldCategories()); }
        }

        public List<CustomFieldType> CustomFieldTypes
        {
            get { return _CustomFieldTypes ?? (_CustomFieldTypes = GetCustomFieldTypes()); }
        }

        public List<CustomFieldValue> CustomFieldValues
        {
            get { return _CustomFieldValues ?? (_CustomFieldValues = GetCustomFieldValues()); }
        }

        public List<CustomField> CustomFields
        {
            get { return _CustomFields ?? (_CustomFields = GetCustomFields()); }
        }

        public List<Category> Categories
        {
            get { return _Categories ?? (_Categories = GetCategories()); }
        }
        public List<School> Schools
        {
            get { return _Schools ?? (_Schools = GetSchools()); }
        }
        public List<Student> Students
        {
            get
            {
                if (_Students == null)
                {
                    _Students = GetStudents();
                    _Students[0].ServiceRequests = new List<ServiceRequest> { ServiceRequests[0] };
                    _Students[0].StudentAssignedOfferings = new List<StudentAssignedOffering> { StudentAssignedOfferings[2] };
                    _Students[0].CustomFieldValues = CustomFieldValues.Where(c => c.StudentId == _Students[0].Id).ToList();
                    _Students[2].StudentAssignedOfferings = new List<StudentAssignedOffering> { StudentAssignedOfferings[3], StudentAssignedOfferings[5] };
                    _Students[0].ApprovedProviders.Add(Providers[1]);
                    Providers[1].ApprovingStudents.Add(_Students[0]);
                    _Students[1].ApprovedProviders.Add(Providers[0]);
                    Providers[0].ApprovingStudents.Add(_Students[1]);
                    _Students[3].ApprovedProviders.Add(Providers[1]);
                    Providers[1].ApprovingStudents.Add(_Students[3]);
                    _Students[4].ApprovedProviders.Add(Providers[0]);
                    Providers[0].ApprovingStudents.Add(_Students[4]);
                    _Students[5].ServiceRequests = new List<ServiceRequest> { ServiceRequests[1] };
                }
                return _Students;
            }
        }
        public List<ServiceType> ServiceTypes
        {
            get
            {
                if (_ServiceTypes == null)
                {
                    _ServiceTypes = GetServiceTypes();
                    foreach (ServiceType s in _ServiceTypes)
                    {
                        s.ServiceOfferings = ServiceOfferings.Where(o => o.ServiceTypeId == s.Id).ToList();
                    }
                }
                return _ServiceTypes;
            }
        }
        public List<ServiceOffering> ServiceOfferings
        {
            get
            {
                if (_ServiceOfferings == null)
                {
                    _ServiceOfferings = GetServiceOfferings();
                    foreach (ServiceOffering offering in _ServiceOfferings)
                    {
                        offering.Provider = Providers.Single(p => p.Id == offering.ProviderId);
                        offering.ServiceType = ServiceTypes.Single(s => s.Id == offering.ServiceTypeId);
                        offering.Program = Programs.Single(p => p.Id == offering.ProgramId);
                    }
                }
                return _ServiceOfferings;
            }
        }
        public List<ServiceRequest> ServiceRequests
        {
            get { return _ServiceRequests ?? (_ServiceRequests = GetServiceRequests()); }
        }
        public List<Priority> Priorities
        {
            get { return _Priorities ?? (_Priorities = GetPriorities()); }
        }
        public List<Subject> Subjects
        {
            get { return _Subjects ?? (_Subjects = GetSubjects()); }
        }
        public List<Provider> Providers
        {
            get 
            {
                if (_Providers == null)
                {
                    _Providers = GetProviders();
                    foreach (Provider p in _Providers)
                    {
                        p.ServiceOfferings = ServiceOfferings.Where(s => s.ProviderId == p.Id).ToList();
                    }
                }
                return _Providers;
            }
        }
        public List<Program> Programs
        {
            get
            {
                if (_Programs == null)
                {
                    _Programs = GetPrograms();
                    foreach (Program p in _Programs)
                    {
                        p.ServiceOfferings = ServiceOfferings.Where(s => s.ProgramId == p.Id).ToList();
                    }
                }
                return _Programs;
            }
        }
        public List<User> Users
        {
            get
            {
                if (_Users == null)
                {
                    _Users = GetUsers();
                    _Users[0].UserRoles = UserRoles.Take(1).ToList();
                    _Users[1].UserRoles = UserRoles.Skip(1).Take(1).ToList();
                    _Users[2].UserRoles = UserRoles.Skip(2).Take(1).ToList();
                    _Users[3].UserRoles = UserRoles.Skip(3).Take(3).ToList();
                }
                return _Users;
            }
        }
        public List<Role> Roles
        {
            get
            {
                if (_Roles == null)
                {
                    _Roles = GetRoles();
                    _Roles[0].UserRoles = UserRoles.Skip(2).Take(1).Concat(UserRoles.Skip(3).Take(3)).ToList();
                    _Roles[1].UserRoles = UserRoles.Skip(1).Take(1).Concat(UserRoles.Skip(3).Take(3)).ToList();
                    _Roles[2].UserRoles = UserRoles.Take(1).Concat(UserRoles.Skip(3).Take(3)).ToList();
                }
                return _Roles;
            }
        }
        public List<UserRole> UserRoles
        {
            get
            {
                if (_UserRoles == null)
                {
                    _UserRoles = GetUserRoles();
                    foreach (Provider p in Providers)
                    {
                        p.UserRoles = _UserRoles.Where(u => u.Providers.Contains(p)).ToList();
                    }
                }
                return _UserRoles;
            }
        }
        public List<Property> Properties
        {
            get { return _Properties ?? (_Properties = GetProperties()); }
        }
        public List<StudentAssignedOffering> StudentAssignedOfferings
        {
            get { return _StudentAssignedOfferings ?? (_StudentAssignedOfferings = GetStudentAssignedOfferings()); }
        }
        public List<ServiceAttendance> ServiceAttendances
        {
            get { return _ServiceAttendances ?? (_ServiceAttendances = GetServiceAttendances()); }
        }
        public List<FulfillmentStatus> FulfillmentStatuses
        {
            get { return _FulfillmentStatuses ?? (_FulfillmentStatuses = GetFulfillmentStatuses()); }
        }
        public List<ServiceRequestFulfillment> ServiceRequestFulfillments
        {
            get { return _ServiceRequestFulfillments ?? (_ServiceRequestFulfillments = GetServiceRequestFulfillments()); }
        }
        public List<UserAccessChangeEvent> UserAccessChangeEvents
        {
            get { return _UserAccessChangeEvents ?? (_UserAccessChangeEvents = GetUserAccessChangeEvents()); }
        }

        public List<EulaAgreement> Eulas
        {
            get { return _Eulas ?? (_Eulas = GetEulas()); }
        }
        public List<EulaAcceptance> EulaAcceptances
        {
            get { return _EulaAcceptances ?? (_EulaAcceptances = GetEulaAcceptances()); }
        }

        public List<PrivateHealthDataViewEvent> PrivateHealthDataViewEvents
        {
            get { return _PrivateHealthDataViewEvents ?? (_PrivateHealthDataViewEvents = GetPrivateHealthDataViewEvents()); }
        }

        public List<LoginEvent> LoginEvents
        {
            get { return _LoginEvents ?? (_LoginEvents = GetLoginEvents()); }
        }

        private string GetMemberName<T>(Expression<Func<T>> expr)
        {
            return ((MemberExpression)expr.Body).Member.Name;
        }

        private List<CustomDataOrigin> GetCustomDataOrigins()
        {
            return new List<CustomDataOrigin>
            {
                new CustomDataOrigin
                {
                    Id = 1,
                    FileName = "testfile.csv",
                    CreateTime = new DateTime(2012, 12, 21),
                    AzureBlobKey = "testingkey", 
                    CreatingUser = Users[0],
                    CreatingUserId = Users[0].Id,
                    Source = "College Board",
                    WasManualEntry = false
                },
                new CustomDataOrigin
                {
                    CreateTime = new DateTime(2011, 10, 10),
                    AzureBlobKey = "testingkey2", 
                    CreatingUserId = Users[1].Id,
                    CreatingUser = Users[1],
                    Source = "Checkers",
                    WasManualEntry = true
                }
            };
        }

        private List<CustomFieldCategory> GetCustomFieldCategories()
        {
            return new List<CustomFieldCategory>
            {
                new CustomFieldCategory { Id = 1, Name = "Attendance" },
                new CustomFieldCategory { Id = 2, Name = "Behavior" },
                new CustomFieldCategory { Id = 3, Name = "Student Details" },
                new CustomFieldCategory { Id = 4, Name = "Tests" }
            };
        }

        private List<CustomField> GetCustomFields()
        {
            return new List<CustomField>
            {
                new PublicField
                {
                    Id = 1,
                    CustomFieldTypeId = CustomFieldTypes[0].Id,
                    CustomFieldType = CustomFieldTypes[0],
                    Categories = new List<CustomFieldCategory> { CustomFieldCategories[0], CustomFieldCategories[1] },
                    Name = "Tardies",
                    CreatingUser = Users[0],
                    CreatingUserId = Users[0].Id,
                    CreateTime = new DateTime(2004, 2, 4)
                },
                new PublicField
                {
                    Id = 2,
                    CustomFieldTypeId = CustomFieldTypes[0].Id,
                    CustomFieldType = CustomFieldTypes[0],
                    Categories = new List<CustomFieldCategory> { CustomFieldCategories[3] },
                    Name = "ACT",
                    CreatingUser = Users[0],
                    CreatingUserId = Users[0].Id,
                    CreateTime = new DateTime(2004, 3, 4)
                },
                new PublicField
                {
                    Id = 3,
                    CustomFieldTypeId = CustomFieldTypes[0].Id,
                    CustomFieldType = CustomFieldTypes[0],
                    Categories = new List<CustomFieldCategory> { CustomFieldCategories[3] },
                    Name = "SAT",
                    CreatingUser = Users[0],
                    CreatingUserId = Users[0].Id,
                    CreateTime = new DateTime(2004, 4, 4)
                },
                new PublicField
                {
                    Id = 4,
                    CustomFieldTypeId = CustomFieldTypes[1].Id,
                    CustomFieldType = CustomFieldTypes[1],
                    Categories = new List<CustomFieldCategory> { CustomFieldCategories[2] },
                    Name = "Guardian",
                    CreatingUser = Users[0],
                    CreatingUserId = Users[0].Id,
                    CreateTime = new DateTime(2004, 5, 4)
                },
                new PublicField
                {
                    Id = 5,
                    CustomFieldTypeId = CustomFieldTypes[3].Id,
                    CustomFieldType = CustomFieldTypes[3],
                    Categories = new List<CustomFieldCategory> { CustomFieldCategories[3] },
                    Name = "Test Dates",
                    CreatingUser = Users[0], 
                    CreatingUserId = Users[0].Id,
                    CreateTime = new DateTime(2004, 6, 4)
                },
                new PublicField
                {
                    Id = 6,
                    CustomFieldTypeId = CustomFieldTypes[0].Id,
                    CustomFieldType = CustomFieldTypes[0],
                    Categories = new List<CustomFieldCategory> { CustomFieldCategories[3] },
                    Name = "ACT Math",
                    CreatingUser = Users[0], 
                    CreatingUserId = Users[0].Id,
                    CreateTime = new DateTime(2004, 7, 4)
                },
                new PublicField
                {
                    Id = 7,
                    CustomFieldTypeId = CustomFieldTypes[1].Id,
                    CustomFieldType = CustomFieldTypes[1],
                    Categories = new List<CustomFieldCategory> { CustomFieldCategories[2] },
                    Name = "Nationality",
                    CreatingUser = Users[0], 
                    CreatingUserId = Users[0].Id,
                    CreateTime = new DateTime(2004, 8, 4)
                },
                new PrivateHealthField
                {
                    Id = 8,
                    CustomFieldTypeId = CustomFieldTypes[1].Id,
                    CustomFieldType = CustomFieldTypes[1],
                    Categories = new List<CustomFieldCategory> { CustomFieldCategories[2] },
                    Name = "PHI",
                    CreatingUser = Users[0], 
                    CreatingUserId = Users[0].Id,
                    CreateTime = new DateTime(2004, 8, 4)
                }
            };
        }

        private List<CustomFieldType> GetCustomFieldTypes()
        {
            return new List<CustomFieldType>
            {
                new CustomFieldType { Id = 1, Name = "Integer" },
                new CustomFieldType { Id = 2, Name = "Text" }, 
                new CustomFieldType { Id = 3, Name = "Rich Text" },
                new CustomFieldType { Id = 4, Name = "Date" }, 
                new CustomFieldType { Id = 5, Name = "XML" },
                new CustomFieldType { Id = 6, Name = "HTML" },
                new CustomFieldType { Id = 6, Name = "Decimal" }
            };
        }

        private List<CustomFieldValue> GetCustomFieldValues()
        {
            return new List<CustomFieldValue>
            {
                new CustomFieldValue
                {
                    Id = 1,
                    CustomFieldId = CustomFields[0].Id,
                    CustomField = CustomFields[0],
                    CustomDataOriginId = CustomDataOrigins[0].Id,
                    CustomDataOrigin = CustomDataOrigins[0],
                    StudentId = Students[0].Id,
                    Student = Students[0],
                    Value = "1200"
                },
                new CustomFieldValue
                {
                    Id = 2,
                    CustomFieldId = CustomFields[1].Id,
                    CustomField = CustomFields[1],
                    CustomDataOriginId = CustomDataOrigins[0].Id,
                    CustomDataOrigin = CustomDataOrigins[0],
                    StudentId = Students[0].Id,
                    Student = Students[0],
                    Value = "1201"
                },
                new CustomFieldValue
                {
                    Id = 3,
                    CustomFieldId = CustomFields[2].Id,
                    CustomField = CustomFields[2],
                    CustomDataOrigin = CustomDataOrigins[0],
                    CustomDataOriginId = CustomDataOrigins[0].Id,
                    StudentId = Students[0].Id,
                    Student = Students[0],
                    Value = "1202"
                }
            };
        }

        private List<Category> GetCategories()
        {
            return new List<Category>
            {
                new Category { Id = 1, Name = "Basic Needs" },
                new Category { Id = 2, Name = "Consumer Services"},
                new Category { Id = 3, Name = "Criminal Justice and Legal Services"},
                new Category { Id = 4, Name = "Education"},
                new Category { Id = 5, Name = "Environmental Quality"},
                new Category { Id = 6, Name = "Health Care"},
                new Category { Id = 7, Name = "Income Support and Employment"},
                new Category { Id = 8, Name = "Individual and Family Life"},
                new Category { Id = 9, Name = "Mental Health Care and Counseling"},
                new Category { Id = 10, Name = "Organizational/Community Services"},
                new Category { Id = 11, Name = "Support Groups"},
                new Category { Id = 12, Name = "Target Populations" }
            };
        }

        private List<School> GetSchools()
        {
            return new List<School>
            {
                new School { Id = 1, Name = "Wyoming High School" },
                new School { Id = 2, Name = "Bombay Education Center" },
                new School { Id = 3, Name = "Evergreen High School" },
                new School { Id = 4, Name = "Columbus High School" }
            };
        }

        private List<Student> GetStudents()
        {
            return new List<Student>
            {
                new Student { Id = 1, StudentKey = "3", StudentSISId = "10", LastName = "Ovadia", MiddleName = "Zev", FirstName = "Micah", Grade = 9, DateOfBirth = new DateTime(2012, 9, 11), Parents = "Meir, Sheila", School = Schools[0], SchoolId = Schools[0].Id },
                new Student { Id = 2, StudentKey = "6", StudentSISId = "20", LastName = "Jain", MiddleName = "Quizzno", FirstName = "Nidhi", Grade = 10, DateOfBirth = new DateTime(2012, 9, 11), Parents = "Apu", School = Schools[1], SchoolId = Schools[1].Id },
                new Student { Id = 3, StudentKey = "9", StudentSISId = "30", LastName = "Glecker", MiddleName = "Azure", FirstName = "Mark", Grade = 11, DateOfBirth = new DateTime(2012, 9, 11), Parents = "Marilyn", School = Schools[2], SchoolId = Schools[2].Id },
                new Student { Id = 4, StudentKey = "12", StudentSISId = "40", LastName = "Martin", MiddleName = "Columbus", FirstName = "Nick", Grade = 12, DateOfBirth = new DateTime(2012, 9, 11), Parents = "Sam, Jenny", School = Schools[3], SchoolId = Schools[3].Id },
                new Student { Id = 5, StudentKey = "15", StudentSISId = "50", LastName = "glecker", MiddleName = "Azure", FirstName = "mark", Grade = 11, DateOfBirth = new DateTime(2012, 9, 11), Parents = "Marilyn", School = Schools[2], SchoolId = Schools[2].Id },
                new Student { Id = 5, StudentKey = "18", StudentSISId = "60", LastName = "Deitz", MiddleName = "Newbie", FirstName = "Alec", Grade = 3, DateOfBirth = new DateTime(2011, 2, 1), Parents = "Janet and George", School = Schools[3], SchoolId = Schools[3].Id },
            };
        }

        private List<ServiceType> GetServiceTypes()
        {
            return new List<ServiceType>
            {
                new ServiceType
                {
                    Id = 1,
                    Name = "Provide College Access",
                    Description = "Provide College Access description",
                    Categories = new List<Category> { Categories[0] },
                    IsActive = true
                },
                new ServiceType
                {
                    Id = 2,
                    Name = "Mentoring",
                    Description = "Mentoring description",
                    Categories = new List<Category> { Categories[3] },
                    IsActive = true
                },
                new ServiceType
                {
                    Id = 3,
                    Name = "Tutoring",
                    Description = "Tutoring Description",
                    Categories = Categories,
                    IsActive = true
                },
                new ServiceType
                {
                    Id = 4,
                    Name = "After School",
                    Description = "After School Description",
                    Categories = new List<Category> { Categories[0], Categories[3] },
                    IsActive = true
                },
                new ServiceType
                {
                    Id = 5,
                    Name = "Before School",
                    Description = "Before School Description",
                    Categories = new List<Category> { Categories[3] },
                    IsPrivate = true,
                    IsActive = true
                },
                new ServiceType
                {
                    Id = 6,
                    Name = "Inactive Type",
                    Description = "Inactive Description",
                    Categories = new List<Category> { Categories[0] },
                    IsPrivate = false, 
                    IsActive = false
                }
            };
        }

        private List<ServiceOffering> GetServiceOfferings()
        {
            List<ServiceOffering> serviceOfferings = new List<ServiceOffering>
            {
                new ServiceOffering 
                { 
                    Id = 1, 
                    ServiceTypeId = 1,
                    ProviderId = 1,
                    ProgramId = 1,
                    IsActive = true
                },
                new ServiceOffering
                {
                    Id = 2,
                    ServiceTypeId = 4,
                    ProviderId = 2,
                    ProgramId = 2,
                    IsActive = true
                },
                new ServiceOffering
                {
                    Id = 3,
                    ServiceTypeId = 4,
                    ProviderId = 1,
                    ProgramId = 2,
                    IsActive = true
                },
                new ServiceOffering
                {
                    Id = 4,
                    ServiceTypeId = 1,
                    ProviderId = 1,
                    ProgramId = 3,
                    IsActive = true
                },
                new ServiceOffering
                {
                    Id = 5,
                    ServiceTypeId = 2,
                    ProviderId = 1,
                    ProgramId = 3,
                    IsActive = true
                },
                new ServiceOffering
                {
                    Id = 6,
                    ServiceTypeId = 1,
                    ProviderId = 1,
                    ProgramId = 4,
                    IsActive = true
                },
                new ServiceOffering
                {
                    Id = 7,
                    ServiceTypeId = 2,
                    ProviderId = 1,
                    ProgramId = 4,
                    IsActive = true
                },
                new ServiceOffering
                {
                    Id = 8,
                    ServiceTypeId = 1,
                    ProviderId = 2,
                    ProgramId = 4,
                    IsActive = true
                },
                new ServiceOffering
                {
                    Id = 9,
                    ServiceTypeId = 2,
                    ProviderId = 2,
                    ProgramId = 4,
                    IsActive = true
                },
                new ServiceOffering
                {
                    Id = 10,
                    ProviderId = 1,
                    ServiceTypeId = 1,
                    ProgramId = 5,
                    IsActive = false
                },
                new ServiceOffering
                {
                    Id = 11,
                    ProviderId = 1,
                    ServiceTypeId = 2,
                    ProgramId = 5,
                    IsActive = false
                },
                new ServiceOffering
                {
                    Id = 12,
                    ProviderId = 1,
                    ServiceTypeId = 3,
                    ProgramId = 5,
                    IsActive = false
                },
                new ServiceOffering
                {
                    Id = 13,
                    ProviderId = 2,
                    ServiceTypeId = 3,
                    ProgramId = 5,
                    IsActive = false
                },
                new ServiceOffering
                {
                    Id = 14,
                    ProviderId = 2,
                    ServiceTypeId = 2,
                    ProgramId = 5,
                    IsActive = false
                },
                new ServiceOffering
                {
                    Id = 15,
                    ProviderId = 2,
                    ServiceTypeId = 3,
                    ProgramId = 5,
                    IsActive = false
                }
            };
            return serviceOfferings;
        }

        private List<ServiceRequest> GetServiceRequests()
        {
            return new List<ServiceRequest>
            {
                new ServiceRequest
                {
                    Id = 1,
                    Student = Students[0],
                    StudentId = Students[0].Id,
                    ServiceType = ServiceTypes[0],
                    ServiceTypeId = ServiceTypes[0].Id,
                    Priority = Priorities[0],
                    PriorityId = Priorities[0].Id,
                    Subject = Subjects[2],
                    SubjectId = Subjects[2].Id,
                    CreateTime = DateTime.Now,
                    CreatingUser = Users[0],
                    FulfillmentDetails = new List<ServiceRequestFulfillment>
                    { 
                        new ServiceRequestFulfillment{ Id = 1, ServiceRequestId = 1, FulfillmentStatus = FulfillmentStatuses[0], FulfillmentStatusId = FulfillmentStatuses[0].Id, CreateTime = DateTime.Now.AddDays(-3), CreatingUserId = 1, Notes = "Just created" },
                        new ServiceRequestFulfillment{ Id = 2, ServiceRequestId = 1, FulfillmentStatus = FulfillmentStatuses[1], FulfillmentStatusId = FulfillmentStatuses[1].Id, CreateTime = DateTime.Now.AddDays(-2), CreatingUserId = 1, Notes = "Made open" }
                    }
                },
                new ServiceRequest
                {
                    Id = 2,
                    Student = Students[5],
                    StudentId = Students[5].Id,
                    ServiceType = ServiceTypes[0],
                    ServiceTypeId = ServiceTypes[0].Id,
                    Priority = Priorities[0],
                    PriorityId = Priorities[0].Id,
                    Subject = Subjects[0],
                    SubjectId = Subjects[0].Id,
                    CreateTime = DateTime.Now,
                    CreatingUser = Users[0],
                    FulfillmentDetails = new List<ServiceRequestFulfillment>
                    { 
                        new ServiceRequestFulfillment{ Id = 1, ServiceRequestId = 2, FulfillmentStatus = FulfillmentStatuses[0], FulfillmentStatusId = FulfillmentStatuses[0].Id, CreateTime = DateTime.Now.AddDays(-3), CreatingUserId = 1, Notes = "Just created" },
                        new ServiceRequestFulfillment{ Id = 2, ServiceRequestId = 2, FulfillmentStatus = FulfillmentStatuses[2], FulfillmentStatusId = FulfillmentStatuses[2].Id, CreateTime = DateTime.Now.AddDays(-2), CreatingUserId = 1, Notes = "Made open" }
                    }
                }
            };
        }

        private List<Priority> GetPriorities()
        {
            return new List<Priority>
            {
                new Priority { Id = 1, Name = "None" },
                new Priority { Id = 2, Name = "Low" },
                new Priority { Id = 3, Name = "Medium" },
                new Priority { Id = 4, Name = "High" },
            };
        }

        private List<Subject> GetSubjects()
        {
            return new List<Subject>
            {
                new Subject { Id = 1, Name = Subject.DefaultName },
                new Subject { Id = 2, Name = "Math" },
                new Subject { Id = 3, Name = "Reading" },
                new Subject { Id = 4, Name = "Writing" }
            };
        }

        private List<Provider> GetProviders()
        {
            return new List<Provider>
            {
                new Provider
                {
                    Id = 1, Name = "YMCA", Website = "www.cincinnatiymca.org",
                    Address = new Address { Street = "22 Acacia Avenue", City = "Cincinnati", State="OH", Zip = "45202" },  
                    Contact = new Contact { Email="ymca@ymca.com", Phone = "(513) 555-1212" },
                    IsActive = true
                },
                new Provider
                {
                    Id = 2, Name = "Jimbo's Math Shop", Website = "www.mathletes.com",
                    Address = new Address { Street = "314 Pi Ave", City = "CircleVille", State="OH", Zip = "45202" },  
                    Contact = new Contact { Email="mole@pi.org", Phone = "(555) 555-5555" },
                    IsActive = true
                },
                new Provider
                {
                    Id = 3, Name = "Big Brothers, Big Sisters", IsActive = true
                },
                new Provider
                {
                    Id = 4, Name = "The Math Hut", IsActive = false
                }
            };
        }

        private List<Program> GetPrograms()
        {
            List<Program> programs = new List<Program>
            {
                new Program
                {
                    Id = 1,
                    Name = "Test Program 1",
                    IsActive = true,
                    Schools = Schools
                },
                new Program
                {
                    Id = 2,
                    Name = "Test Program 2",
                    IsActive = true
                },
                new Program
                {
                    Id = 3,
                    Name = "Test Program 3 - After School Swimming",
                    IsActive = true
                },
                new Program
                {
                    Id = 4,
                    Name = "Test Program 4 - After School Wrestling",
                    IsActive = true
                },
                new Program
                {
                    Id = 5,
                    Name = "Test Program 5 - The Avengers",
                    IsActive = false
                }
            };
            return programs;
        }

        private List<User> GetUsers()
        {
            return new List<User>
            {
                new User
                {
                    Id = 1,
                    Active = true,
                    DisplayName = "JonG",
                    FirstName = "Jon",
                    LastName = "Gear",
                    EmailAddress = "jgear@gear.com",
                    UserKey = "testkey",
                    Comments = "Comment Mc. Commentson",
                    EulaAcceptances = new List<EulaAcceptance> 
                    { 
                        new EulaAcceptance
                        { 
                            Id = 1, 
                            EulaAgreementId = 1,
                            EulaAgreement = new EulaAgreement 
                            { 
                                Id = 1, 
                                EulaText = "EULA Agreement, EULA Agreement, EULA Agreement, EULA Agreement, EULA Agreement, EULA Agreement, EULA Agreement, EULA Agreement, ATM MACHINE!", 
                                CreateTime = DateTime.Now.AddDays(-10),
                                CreatingUser = new User{ DisplayName = "blah" }
                            },
                        } 
                    }
                },
                new User
                {
                    Id = 2,
                    Active = false,
                    DisplayName = "NickM",
                    FirstName = "Nick",
                    LastName = "Martin",
                    EmailAddress = "nmartin@martin.com",
                    UserKey = "testkey2",
                    Comments = "Sir Comment, Duke of CommentVille"
                },
                new User
                {
                    Id = 3,
                    Active = true,
                    DisplayName = "NickM2",
                    FirstName = "Nick",
                    LastName = "Martin",
                    EmailAddress = "nmartin@martin.com",
                    UserKey = "testkey3",
                    Comments = "Supreme Chancellor of Linguistics"
                },
                new User
                {
                    Id = 4,
                    Active = true,
                    DisplayName = "Invalid",
                    FirstName = "Invalid",
                    LastName = "Invalid",
                    EmailAddress = "Invalid@user.com",
                    UserKey = "user has too many roles",
                    Comments = "user cannot be multiple roles - invalid!"
                },
                new User
                {
                    Id = 5,
                    Active = false,
                    DisplayName = "No Roles",
                    FirstName = "No",
                    LastName = "Roles",
                    EmailAddress = "no@roles.com",
                    UserKey = "no roles",
                    Comments = "this user has no roles - blank activation canvas!"
                }
            };
        }
        
        private List<Role> GetRoles()
        {
            return new List<Role>
            {
                new Role { Id = 1, Name = SecurityRoles.Provider },
                new Role { Id = 2, Name = SecurityRoles.SiteCoordinator, },
                new Role { Id = 3, Name = SecurityRoles.DataAdmin }
            };
        }

        private List<UserRole> GetUserRoles()
        {
            return new List<UserRole>
            {
                new UserRole
                {
                    Id = 1,
                    User = Users[0],
                    UserId = 1,
                    Role = Roles[2],
                    RoleId = 3
                },
                new UserRole
                {
                    Id = 2,
                    User = Users[1],
                    UserId = 2,
                    Role = Roles[1],
                    RoleId = 2,
                    Schools = Schools.Where(s => s.Id == 1 || s.Id == 3).ToList()
                },
                new UserRole
                {
                    Id = 3,
                    User = Users[2],
                    UserId = 3,
                    Role = Roles[0],
                    RoleId = 1,
                    Providers = Providers.Where(p => p.Id == 1 || p.Id == 3).ToList()
                },
                new UserRole
                {
                    Id = 4,
                    User = Users[3],
                    UserId = 4,
                    Role = Roles[0],
                    RoleId = 1,
                    Providers = Providers
                },
                new UserRole
                {
                    Id = 5,
                    User = Users[3],
                    UserId = 4,
                    Role = Roles[1],
                    RoleId = 2,
                    Schools = Schools
                },
                new UserRole
                {
                    Id = 6,
                    User = Users[3],
                    UserId = 4,
                    Role = Roles[2],
                    RoleId = 3
                }
            };
        }

        private List<Property> GetProperties()
        {
            string[] nonDirectoryLevelProperties = 
            {
                GetMemberName(() => (new Student()).StudentKey),
                GetMemberName(() => (new Student()).ServiceRequests),
                GetMemberName(() => (new Student()).StudentAssignedOfferings)
            };

            var properties = typeof(Student).Assembly.GetTypes().
                SelectMany(p => p.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.FlattenHierarchy)).
                Select(p => new Property
                {
                    EntityName = p.DeclaringType.FullName,
                    Name = p.Name,
                    IsProtected = nonDirectoryLevelProperties.Contains(p.Name)
                }).ToList();

            var propertyList = new List<Property>();
            for (int i = 0; i < properties.Count(); i++)
            {
                properties[i].Id = i + 1;
                propertyList.Add(properties[i]);
            }
            return propertyList;
        }

        private List<StudentAssignedOffering> GetStudentAssignedOfferings()
        {
            return new List<StudentAssignedOffering>
            {
                new StudentAssignedOffering
                {
                    Id = 1,
                    ServiceOffering = ServiceOfferings[0],
                    ServiceOfferingId = ServiceOfferings[0].Id,
                    StartDate = new DateTime(2007, 10, 1),
                    EndDate = new DateTime(2007, 10, 15),
                    Notes = "blahdwiwh",
                    Student = Students[1],
                    StudentId = Students[1].Id,
                    CreatingUser = Users[1],
                    CreatingUserId = Users[1].Id,
                    CreateTime = new DateTime(2005, 2, 4),
                    IsActive = true
                },
                new StudentAssignedOffering
                {
                    Id = 2,
                    ServiceOffering = ServiceOfferings[0],
                    ServiceOfferingId = ServiceOfferings[0].Id,
                    StartDate = new DateTime(2007, 10, 1),
                    EndDate = new DateTime(2007, 10, 15),
                    Notes = "blahdwiwh",
                    Student = Students[3],
                    StudentId = Students[3].Id,
                    CreatingUser = Users[1],
                    CreatingUserId = Users[1].Id,
                    CreateTime = new DateTime(2005, 3, 4),
                    IsActive = true
                },
                new StudentAssignedOffering
                {
                    Id = 3,
                    ServiceOffering = ServiceOfferings[1],
                    ServiceOfferingId = ServiceOfferings[1].Id,
                    StartDate = new DateTime(2004, 10, 1),
                    EndDate = new DateTime(2008, 10, 15),
                    Notes = "jo9jef892jf2",
                    Student = Students[0],
                    StudentId = Students[0].Id,
                    CreatingUser = Users[0],
                    CreatingUserId = Users[0].Id,
                    CreateTime = new DateTime(2005, 4, 4),
                    IsActive = true
                },
                new StudentAssignedOffering
                {
                    Id = 4,
                    ServiceOffering = ServiceOfferings[1],
                    ServiceOfferingId = ServiceOfferings[1].Id,
                    StartDate = new DateTime(2004, 10, 1),
                    EndDate = new DateTime(2008, 10, 15),
                    Notes = "jo9jef892jf2",
                    Student = Students[2],
                    StudentId = Students[2].Id,
                    CreatingUser = Users[0],
                    CreatingUserId = Users[0].Id,
                    CreateTime = new DateTime(2005, 5, 4),
                    IsActive = true
                },
                new StudentAssignedOffering
                {
                    Id = 5,
                    ServiceOffering = ServiceOfferings[1],
                    ServiceOfferingId = ServiceOfferings[1].Id,
                    StartDate = new DateTime(2000, 10, 1),
                    EndDate = new DateTime(2010, 10, 15),
                    Notes = "278ehf2e879h",
                    Student = Students[4],
                    StudentId = Students[4].Id,
                    CreatingUser = Users[2],
                    CreatingUserId = Users[2].Id,
                    CreateTime = new DateTime(2005, 6, 4),
                    IsActive = true
                },
                new StudentAssignedOffering
                {
                    Id = 6,
                    ServiceOffering = ServiceOfferings[1],
                    ServiceOfferingId = ServiceOfferings[1].Id,
                    StartDate = new DateTime(2000, 10, 1),
                    EndDate = new DateTime(2010, 10, 15),
                    Notes = "278ehf2e879h",
                    Student = Students[2],
                    StudentId = Students[2].Id,
                    CreatingUser = Users[2],
                    CreatingUserId = Users[2].Id,
                    CreateTime = new DateTime(2005, 7, 4),
                    IsActive = false
                }
            };
        }

        private List<ServiceAttendance> GetServiceAttendances()
        {
            return new List<ServiceAttendance>
            {
                new ServiceAttendance
                {
                    Id = 1,
                    StudentAssignedOfferingId = 1,
                    DateAttended = DateTime.Now,
                    Subject = Subjects[0],
                    Duration = 1,
                    CreatingUser = Users[0],
                    CreateTime = DateTime.Now
                },
                new ServiceAttendance
                {
                    Id = 2,
                    StudentAssignedOfferingId = 1,
                    DateAttended = DateTime.Now,
                    Subject = Subjects[1],
                    Duration = 2,
                    CreatingUser = Users[0],
                    CreateTime = DateTime.Now
                },
                new ServiceAttendance
                {
                    Id = 3,
                    StudentAssignedOfferingId = 2,
                    DateAttended = DateTime.Now,
                    Subject = Subjects[2],
                    Duration = 3,
                    CreatingUser = Users[0],
                    CreateTime = DateTime.Now
                }
            };
        }

        private List<FulfillmentStatus> GetFulfillmentStatuses()
        {
            return new List<FulfillmentStatus>
            {
                new FulfillmentStatus { Id = 1, Name = Statuses.Open },
                new FulfillmentStatus { Id = 2, Name = Statuses.Fulfilled },
                new FulfillmentStatus { Id = 3, Name = Statuses.Rejected }
            };
        }

        private List<ServiceRequestFulfillment> GetServiceRequestFulfillments()
        {
            return new List<ServiceRequestFulfillment>
            {
                new ServiceRequestFulfillment{ Id = 1, ServiceRequestId = ServiceRequests[0].Id, FulfillmentStatus = FulfillmentStatuses[0], FulfillmentStatusId = 1, CreateTime = DateTime.Now.AddDays(-3), CreatingUserId = 1, Notes = "Just created" },
                new ServiceRequestFulfillment{ Id = 2, ServiceRequestId = ServiceRequests[0].Id, FulfillmentStatus = FulfillmentStatuses[1], FulfillmentStatusId = 2, CreateTime = DateTime.Now.AddDays(-2), CreatingUserId = 1, Notes = "Made open" },
                new ServiceRequestFulfillment{ Id = 3, ServiceRequestId = ServiceRequests[0].Id, FulfillmentStatus = FulfillmentStatuses[2], FulfillmentStatusId = 3, CreateTime = DateTime.Now.AddDays(-1), CreatingUserId = 1, Notes = "Made rejected" },
                new ServiceRequestFulfillment{ Id = 4, ServiceRequestId = ServiceRequests[0].Id, FulfillmentStatus = FulfillmentStatuses[1], FulfillmentStatusId = 2, CreateTime = DateTime.Now, CreatingUserId = 1, Notes = "Made fulfileed" }
            };
        }

        private List<UserAccessChangeEvent> GetUserAccessChangeEvents()
        {
            return new List<UserAccessChangeEvent>
            {
                new UserAccessChangeEvent{ Id = 1, CreatingUser = Users[0], CreatingUserId = Users[0].Id, UserId = 1, CreateTime = DateTime.Now, AccessData = "blah" } 
            };
        }

        private List<EulaAgreement> GetEulas()
        {
            return new List<EulaAgreement>
            {
                new EulaAgreement { Id = 1, EulaText = "EULA Agreement, EULA Agreement, EULA Agreement, EULA Agreement, EULA Agreement, EULA Agreement, EULA Agreement, EULA Agreement, ATM MACHINE!", CreateTime = DateTime.Now.AddDays(-10), CreatingUser = Users[0], CreatingUserId = Users[0].Id },
                new EulaAgreement { Id = 2, EulaText = "EULA Agreement, EULA Agreement, EULA Agreement, EULA Agreement, EULA Agreement, EULA Agreement, EULA Agreement, EULA Agreement, ATM MACHINE!", CreateTime = DateTime.Now.AddDays(-5), CreatingUser = Users[0], CreatingUserId = Users[0].Id },
                new EulaAgreement { Id = 3, EulaText = "EULA Agreement, EULA Agreement, EULA Agreement, EULA Agreement, EULA Agreement, EULA Agreement, EULA Agreement, EULA Agreement, ATM MACHINE!", CreateTime = DateTime.Now.AddDays(-2), CreatingUser = Users[0], CreatingUserId = Users[0].Id }
            };
        }

        private List<EulaAcceptance> GetEulaAcceptances()
        {
            return new List<EulaAcceptance>
            {
                new EulaAcceptance { Id = 1, EulaAgreement = Eulas[2], CreatingUser = Users[0], CreatingUserId = Users[0].Id },
                new EulaAcceptance { Id = 2, EulaAgreement = Eulas[1], CreatingUser = Users[0], CreatingUserId = Users[0].Id },
                new EulaAcceptance { Id = 3, EulaAgreement = Eulas[0], CreatingUser = Users[0], CreatingUserId = Users[0].Id }
            };
        }

        private List<PrivateHealthDataViewEvent> GetPrivateHealthDataViewEvents()
        {
            return new List<PrivateHealthDataViewEvent>
            {
                new PrivateHealthDataViewEvent{ Id = 1, CreatingUser = Users[0], CreatingUserId = Users[0].Id, CreateTime = DateTime.Now, PhiValuesViewed = CustomFieldValues } 
            };
        }

        private List<LoginEvent> GetLoginEvents()
        {
            return new List<LoginEvent>
            {
                new LoginEvent { Id = 1, CreateTime = new DateTime(2012, 10, 20), CreatingUser = Users[0], CreatingUserId = Users[0].Id }
            };
        }
    }
}
