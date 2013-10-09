using Microsoft.VisualStudio.TestTools.UnitTesting;
using SSD.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SSD.ViewModels
{
    [TestClass]
    public class StudentDetailModelTest
    {
        private StudentDetailModel Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            Target = new StudentDetailModel();
        }

        [TestMethod]
        public void GivenStudentEntity_WhenCopyFrom_ThenIdIsSet()
        {
            Student entity = new Student { Id = 1, StudentKey = "2", StudentSISId = "3", School = new School() };

            Target.CopyFrom(entity);

            Assert.AreEqual(1, Target.Id);
        }

        [TestMethod]
        public void GivenStudentEntity_WhenCopyFrom_ThenSISIdIsSet()
        {
            Student entity = new Student { Id = 1, StudentKey = "2", StudentSISId = "3", School = new School() };

            Target.CopyFrom(entity);

            Assert.AreEqual("3", Target.SISId);
        }

        [TestMethod]
        public void GivenStudentEntity_WhenCopyFrom_ThenCustomFieldsSet()
        {
            Student entity = new Student
            {
                Id = 1,
                StudentKey = "2",
                StudentSISId = "3",
                School = new School(),
                CustomFieldValues = new List<CustomFieldValue>
                {
                    new CustomFieldValue
                    {
                        CustomDataOrigin = new CustomDataOrigin { Source = "College Board" },
                        CustomField = new PublicField { Name = "ACT" },
                        Value = "27"
                    }
                }
            };

            Target.CopyFrom(entity);

            Assert.AreEqual(1, Target.CustomData.Count());
        }

        [TestMethod]
        public void GivenStudentEntity_AndSomePrivateHealthFields_WhenCopyFrom_ThenCustomFieldsSet()
        {
            Student entity = new Student
            {
                Id = 1,
                StudentKey = "2",
                StudentSISId = "3",
                School = new School(),
                CustomFieldValues = new List<CustomFieldValue>
                {
                    new CustomFieldValue
                    {
                        CustomDataOrigin = new CustomDataOrigin { Source = "College Board" },
                        CustomField = new PublicField { Name = "ACT" },
                        Value = "27"
                    },
                    new CustomFieldValue
                    {
                        CustomDataOrigin = new CustomDataOrigin { Source = "HULK" },
                        CustomField = new PrivateHealthField { Name = "PHI" },
                        Value = "1234"
                    }
                }
            };

            Target.CopyFrom(entity);

            Assert.AreEqual(1, Target.CustomData.Count());
            Assert.AreEqual(1, Target.CustomPrivateData.Count());
        }

        [TestMethod]
        public void GivenStudentEntity_WhenCopyFrom_ThenNoStudentAssignedOfferingsAreDeleted()
        {
            Student entity = new Student
            {
                Id = 1,
                StudentKey = "2",
                StudentSISId = "3",
                School = new School(),
                StudentAssignedOfferings = new List<StudentAssignedOffering>
                {
                    new StudentAssignedOffering
                    {
                        CreatingUserId = 1,
                        IsActive = true,
                        StudentId = 1
                    },
                    new StudentAssignedOffering
                    {
                        CreatingUserId = 2,
                        IsActive = false,
                        StudentId = 1
                    }
                }
            };

            Target.CopyFrom(entity);

            Assert.AreEqual(1, Target.StudentAssignedOfferings.Count());
        }

        [TestMethod]
        public void WhenCopyTo_ThenThrowException()
        {
            Student entity = new Student { Id = 1, StudentKey = "2", StudentSISId = "3", School = new School() };

            Target.ExpectException<NotSupportedException>(() => Target.CopyTo(entity));
        }
    }
}
