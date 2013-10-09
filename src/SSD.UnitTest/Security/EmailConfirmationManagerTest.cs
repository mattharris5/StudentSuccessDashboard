using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Domain;
using SSD.Security.Net;
using System;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;

namespace SSD.Security
{
    [TestClass]
    public class EmailConfirmationManagerTest
    {
        private IMailer MockMailer { get; set; }
        private EmailConfirmationManager Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            MockMailer = MockRepository.GenerateMock<IMailer>();
            Target = new EmailConfirmationManager(MockMailer);
        }

        [TestMethod]
        public void GivenANullMailer_WhenIConstruct_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => new EmailConfirmationManager(null));
        }

        [TestMethod]
        public void GivenAValidUser_WhenIRequest_ThenUsersPendingEmailIsSentAMessage()
        {
            User expectedState = new User { PendingEmail = "bob@bob.bob", ConfirmationGuid = Guid.NewGuid(), DisplayName = "Bob Smith" };
            bool wasCalled = false;
            MockMailer.Expect(m => m.Send(Arg<MailMessage>.Is.NotNull)).Do(new Action<MailMessage>(e =>
            {
                AssertRecipient(expectedState, e);
                wasCalled = true;
            }));
            
            Target.Request(expectedState, new Uri("http://tempuri.org"));

            Assert.IsTrue(wasCalled);
        }

        [TestMethod]
        public void GivenAValidUser_WhenIRequest_ThenSentMessageContainsGuidBothHtmlAndPlainContentTypes()
        {
            User expectedState = new User { PendingEmail = "bob@bob.bob", ConfirmationGuid = Guid.NewGuid() };
            bool wasCalled = false;
            MockMailer.Expect(m => m.Send(Arg<MailMessage>.Is.NotNull)).Do(new Action<MailMessage>(e =>
            {
                AssertMailMessage(expectedState, e);
                wasCalled = true;
            }));
            
            Target.Request(expectedState, new Uri("http://tempuri.org"));

            Assert.IsTrue(wasCalled);
        }

        [TestMethod]
        public void GivenANullUser_WhenIRequest_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => Target.Request(null, new Uri("http://tempuri.org")));
        }

        [TestMethod]
        public void GivenANullConfirmationEndpoint_WhenIRequest_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => Target.Request(new User { PendingEmail = "bob@bob.bob", ConfirmationGuid = Guid.NewGuid() }, null));
        }

        [TestMethod]
        public void GivenAValidUser_WhenIProcess_ThenUserEntityUpdated()
        {
            string expectedEmailAddress = "bob@bob.bob";
            User user = new User { EmailAddress = "jim@jim.jim", PendingEmail = expectedEmailAddress, ConfirmationGuid = Guid.NewGuid() };

            Target.Process(user);

            Assert.AreEqual(expectedEmailAddress, user.EmailAddress);
            Assert.IsNull(user.PendingEmail);
            Assert.AreEqual(Guid.Empty, user.ConfirmationGuid);
        }

        [TestMethod]
        public void GivenANullUser_WhenIProcess_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => Target.Process(user: null));
        }

        private static bool AssertRecipient(User expectedState, MailMessage email)
        {
            Assert.AreEqual(expectedState.PendingEmail, email.To.Single().Address);
            Assert.AreEqual(expectedState.DisplayName, email.To.Single().DisplayName);
            return true;
        }

        private static bool AssertMailMessage(User expectedState, MailMessage email)
        {
            AlternateView plainView = email.AlternateViews.SingleOrDefault(v => v.ContentType.MediaType == MediaTypeNames.Text.Plain);
            AlternateView htmlView = email.AlternateViews.SingleOrDefault(v => v.ContentType.MediaType == MediaTypeNames.Text.Html);
            Assert.IsNotNull(plainView);
            Assert.IsNotNull(htmlView);
            Assert.IsNotNull(plainView.ContentStream);
            Assert.IsNotNull(htmlView.ContentStream);
            Assert.IsTrue(ExtractContent(plainView).Contains("http://tempuri.org?identifier=" + expectedState.ConfirmationGuid.ToString()));
            Assert.IsTrue(ExtractContent(htmlView).Contains("http://tempuri.org?identifier=" + expectedState.ConfirmationGuid.ToString()));
            Assert.AreEqual(2, email.AlternateViews.Count);
            return true;
        }

        private static string ExtractContent(AlternateView emailView)
        {
            var dataStream = emailView.ContentStream;
            byte[] byteBuffer = new byte[dataStream.Length];
            Encoding encoding = Encoding.GetEncoding(emailView.ContentType.CharSet);
            string blah = encoding.GetString(byteBuffer, 0, dataStream.Read(byteBuffer, 0, byteBuffer.Length));
            return blah;
        }
    }
}
