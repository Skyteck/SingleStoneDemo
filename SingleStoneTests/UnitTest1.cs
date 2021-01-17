using Microsoft.VisualStudio.TestTools.UnitTesting;
using SingleStone;
using SingleStone.Controllers;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Results;

namespace SingleStoneTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void GetAllContacts_ShouldReturnAllContacts()
        {
            //create the list of test contacts
            List<ContactModel> testContacts = GetTestContacts();
            //create the controller and populate the test DB
            ContactsController controller = new ContactsController(testContacts);

            //call the controller to get all contacts
            var AllContacts = controller.Get() as OkNegotiatedContentResult<List<ContactModel>>;
            Assert.IsNotNull(AllContacts);

            
            Assert.AreEqual(testContacts.Count, AllContacts.Content.Count);
        }

        [TestMethod]
        public void CallListShouldReturnHomeOnly()
        {
            //create the test contacts
            var testContacts = GetTestContacts();
            var controller = new ContactsController(testContacts);

            //call the controller method we're testing
            IHttpActionResult result = controller.CallList();
            OkNegotiatedContentResult<List<CallListResultsModel>> response = result as OkNegotiatedContentResult<List<CallListResultsModel>>;
            //make sure we got a response
            Assert.IsNotNull(response);

            List<CallListResultsModel> cms = response.Content;
            Assert.AreEqual(3, cms.Count);
            Assert.AreEqual("terry", cms[1].name.first);
            Assert.AreEqual("6017418965", cms[2].phone);
        }

        [TestMethod]
        public void PostShouldMake5Contacts()
        {
            //create the test contacts
            var testContacts = GetTestContacts();
            var controller = new ContactsController(testContacts);

            //create the new contact to post
            Phone newPhone = new Phone { number = "8749658521", type = "work" };
            ContactModel newContact = new ContactModel
            {
                address = new Address { city = "Testville", state = "Mississippi", street = "123 test road", zip = "11111" },
                email = "testSHallPass@testing.com",
                name = new Name { first = "TestFirst", middle = "testMiddle", last = "TestLast" },
                phone = new List<Phone>(),
            };
            newContact.phone.Add(newPhone);

            //get the current number of contacts
            var AllContacts = controller.Get() as OkNegotiatedContentResult<List<ContactModel>>;
            Assert.IsNotNull(AllContacts);

            Assert.AreEqual(4, AllContacts.Content.Count);
            Assert.AreEqual("Andy", AllContacts.Content[3].name.first);

            //post the new contact
            IHttpActionResult result = controller.Post(newContact);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkResult));

            //get the new list of contacts to compare
            AllContacts = controller.Get() as OkNegotiatedContentResult<List<ContactModel>>;
            Assert.IsNotNull(AllContacts);

            Assert.AreEqual(5, AllContacts.Content.Count);
            Assert.AreEqual("TestFirst", AllContacts.Content[4].name.first);
        }

        [TestMethod]
        public void PutShouldUpdate2ndContact()
        {
            //create the test contacts
            var testContacts = GetTestContacts();
            var controller = new ContactsController(testContacts);

            //create the contact to update
            ContactModel newContact = new ContactModel
            {
                id = 2,
                address = new Address { city = "Brandon", state = "Mississippi", street = "123 3333 road", zip = "39207" },
                email = "Test@SHallPass.com",
                name = new Name { first = "terry", middle = "the", last = "crews" },
                phone = new List<Phone>(),
            };

            //update the contact
            IHttpActionResult result = controller.Put(newContact.id, newContact);

            //make sure we got the appropriate response
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkResult));

            //get the contacts and make sure things were updated
            var AllContacts = controller.Get() as OkNegotiatedContentResult<List<ContactModel>>;
            Assert.IsNotNull(AllContacts);

            Assert.AreEqual(4, AllContacts.Content.Count);
            Assert.AreEqual("Test@SHallPass.com", AllContacts.Content[1].email);
        }

        [TestMethod]
        public void PutShouldFailUpdating()
        {
            //create the test contacts
            var testContacts = GetTestContacts();
            var controller = new ContactsController(testContacts);

            //create the contact that will fail updating
            ContactModel newContact = new ContactModel
            {
                id = 20,
                address = new Address { city = "Brandon", state = "Mississippi", street = "123 3333 road", zip = "39207" },
                email = "Test@SHallPass.com",
                name = new Name { first = "terry", middle = "the", last = "crews" },
                phone = new List<Phone>(),
            };

            //try to update the not existing contact
            IHttpActionResult result = controller.Put(newContact.id, newContact);

            //make sure we got a response and the contact was not found
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));

            //get the contacts and make sure nothing was updated
            var AllContacts = controller.Get() as OkNegotiatedContentResult<List<ContactModel>>;
            Assert.IsNotNull(AllContacts);

            Assert.AreEqual(4, AllContacts.Content.Count);
            Assert.AreEqual("richard.ellzy@hotmail.com", AllContacts.Content[1].email);
        }

        [TestMethod]
        public void DeleteShouldReturn3()
        {
            //create the test contacts
            var testContacts = GetTestContacts();
            var controller = new ContactsController(testContacts);

            //call the method to delete the 2nd contact
            IHttpActionResult result = controller.Delete(1);

            //check for response
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkResult));

            //make sure the contact was deleted
            var AllContacts = controller.Get() as OkNegotiatedContentResult<List<ContactModel>>;
            Assert.IsNotNull(AllContacts);

            Assert.AreEqual(3, AllContacts.Content.Count);
        }

        [TestMethod]
        public void DeleteShouldFail()
        {
            //create the test list of contacts
            var testContacts = GetTestContacts();
            var controller = new ContactsController(testContacts);

            //try to delete a non-existing contact
            IHttpActionResult result = controller.Delete(10);

            //make sure we got a response and the contact wasn't found
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));

            //test to make sure we still have 4 contacts
            var AllContacts = controller.Get() as OkNegotiatedContentResult<List<ContactModel>>;
            Assert.IsNotNull(AllContacts);

            Assert.AreEqual(4, AllContacts.Content.Count);
        }

        private List<SingleStone.ContactModel> GetTestContacts()
        {
            //create the list to hold the test contacts
            var testContacts = new List<ContactModel>();

            //create the test contacts and add them to the list
            Phone newPhone = new Phone { number = "6018261234", type = "home" };
            ContactModel newContact = new ContactModel
            {
                id = 1,
                address = new Address { city = "Hattiesburg", state = "Mississippi", street = "123 foot road", zip = "39208" },
                email = "richard.ellzy@gggg.com",
                name = new Name { first = "Bob", middle = "Andy", last = "hungry" },
                phone = new List<Phone>(),
            };
            newContact.phone.Add(newPhone);
            testContacts.Add(newContact);

            newContact = new ContactModel
            {
                id = 2,
                address = new Address { city = "Brandon", state = "Mississippi", street = "123 3333 road", zip = "39207" },
                email = "richard.ellzy@hotmail.com",
                name = new Name { first = "terry", middle = "the", last = "crews" },
                phone = new List<Phone>(),
            };
            newPhone = new Phone();
            newPhone.type = "mobile";
            newPhone.number = "6019876541";
            newContact.phone.Add(newPhone);

            newPhone = new Phone();
            newPhone.type = "work";
            newPhone.number = "6018527412";
            newContact.phone.Add(newPhone);

            newPhone = new Phone();
            newPhone.type = "home";
            newPhone.number = "6019638523";
            newContact.phone.Add(newPhone);

            testContacts.Add(newContact);

            newContact = new ContactModel
            {
                id = 3,
                address = new Address { city = "Pearl", state = "Mississippi", street = "67 street", zip = "39201" },
                email = "bobBob@hotmail.com",
                name = new Name { first = "Bob", middle = "Bobber", last = "Bobbest" },
                phone = new List<Phone>(),
            };
            newPhone = new Phone();
            newPhone.type = "home";
            newPhone.number = "6017418965";
            newContact.phone.Add(newPhone);
            testContacts.Add(newContact);


            newContact = new ContactModel
            {
                id = 4,
                address = new Address { city = "jackson", state = "Mississippi", street = "12 front street", zip = "39201" },
                email = "TestSpam@ggg.com",
                name = new Name { first = "Andy", middle = "Nitro", last = "Cortez" },
                phone = new List<Phone>(),
            };
            newPhone = new Phone();
            newPhone.type = "mobile";
            newPhone.number = "6018749654";
            newContact.phone.Add(newPhone);

            newPhone = new Phone();
            newPhone.type = "mobile";
            newPhone.number = "6019867412";
            newContact.phone.Add(newPhone);
            testContacts.Add(newContact);

            return testContacts;
        }
    }
}
