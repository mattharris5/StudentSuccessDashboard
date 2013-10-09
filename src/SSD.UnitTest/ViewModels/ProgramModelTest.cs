using Microsoft.VisualStudio.TestTools.UnitTesting;
using SSD.Domain;
using System;

namespace SSD.ViewModels
{
    [TestClass]
    public class ProgramModelTest
    {
        private ProgramModel Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            Target = new ProgramModel();
        }

        [TestMethod]
        public void GivenNullModel_WhenCopyTo_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => Target.CopyTo(null));
        }

        [TestMethod]
        public void GivenTargetHasContactInfo_AndBlankEntity_WhenCopyTo_ThenEntityHasContactInfo()
        {
            Program copy = new Program();
            Target.ContactEmail = "bob@bob.bob";
            Target.ContactName = "Bob";
            Target.ContactPhone = "123-456-7890";

            Target.CopyTo(copy);

            Assert.AreEqual(Target.ContactEmail, copy.ContactInfo.Email);
            Assert.AreEqual(Target.ContactName, copy.ContactInfo.Name);
            Assert.AreEqual(Target.ContactPhone, copy.ContactInfo.Phone);
        }

        [TestMethod]
        public void GivenTargetHasAttributeData_AndBlankEntity_WhenCopyTo_ThenEntityHasAttributeData()
        {
            Program copy = new Program();
            Target.Name = "Tutoring With Clowns";
            Target.StartDate = new DateTime(2001, 4, 6);
            Target.EndDate = new DateTime(2001, 10, 11);
            Target.Purpose = "Clowns are scary and will motivate kids to do well in school.";

            Target.CopyTo(copy);

            Assert.AreEqual(Target.Name, copy.Name);
            Assert.AreEqual(Target.StartDate, copy.StartDate);
            Assert.AreEqual(Target.EndDate, copy.EndDate);
            Assert.AreEqual(Target.Purpose, copy.Purpose);
        }

        [TestMethod]
        public void GivenNullModel_WhenCopyFrom_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => Target.CopyFrom(null));
        }

        [TestMethod]
        public void GivenBlankTarget_AndEntityHasContactInfo_WhenCopyFrom_ThenEntityHasContactInfo()
        {
            Program copy = new Program
            {
                ContactInfo = new Contact
                {
                    Email = "jim@jim.jim",
                    Name = "Jim",
                    Phone = "321-654-0987"
                }
            };

            Target.CopyFrom(copy);

            Assert.AreEqual(copy.ContactInfo.Email, Target.ContactEmail);
            Assert.AreEqual(copy.ContactInfo.Name, Target.ContactName);
            Assert.AreEqual(copy.ContactInfo.Phone, Target.ContactPhone);
        }

        [TestMethod]
        public void GivenTargetHasAttributeData_AndBlankEntity_WhenCopyFrom_ThenEntityHasAttributeData()
        {
            Program copy = new Program
            {
                Name = "Mortal Combat",
                StartDate = new DateTime(2001, 4, 6),
                EndDate = new DateTime(2001, 10, 11),
                Purpose = "Don't worry, it is just a video game.  It teaches important life lessons....really."
            };

            Target.CopyFrom(copy);

            Assert.AreEqual(copy.Name, Target.Name);
            Assert.AreEqual(copy.StartDate, Target.StartDate);
            Assert.AreEqual(copy.EndDate, Target.EndDate);
            Assert.AreEqual(copy.Purpose, Target.Purpose);
        }
    }
}
