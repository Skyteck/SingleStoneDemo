using LiteDB;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace SingleStone.Controllers
{
    public class ContactsController : ApiController
    {
        string dbConnection = @"C:\Temp\MyData.db";
        public ContactsController() { }

        public ContactsController(List<ContactModel> contacts)
        {
            dbConnection = @"C:\Temp\MyDataTest.db";
            using (var db = new LiteDatabase(dbConnection))
            {
                var col = db.GetCollection<ContactModel>("Contacts");
                col.DeleteAll();
                foreach (ContactModel cm in contacts)
                {
                    col.Insert(cm);
                }
            }
        }
        // GET api/contacts
        /// <summary>
        /// Returns all contacts in the database
        /// </summary>
        /// <returns></returns>
        public IHttpActionResult Get()
        {
            using (var db = new LiteDatabase(dbConnection))
            {
                //get a reference to the collection
                var collection = db.GetCollection<ContactModel>("Contacts");
                //return all contacts
                return Ok(collection.FindAll().ToList<ContactModel>());
            }

        }

        // GET api/contacts/5
        /// <summary>
        /// returns the contact with the given ID
        /// </summary>
        /// <param name="id">The ID of the contact to get the information for</param>
        /// <returns></returns>
        public IHttpActionResult Get(int id)
        {
            using (var db = new LiteDatabase(dbConnection))
            {
                // Get a reference to the collection
                var collection = db.GetCollection<ContactModel>("Contacts");
                //search for the contact using the ID
                var contact = collection.FindById(id);

                //if the contact was found, return it
                if (contact != null)
                {
                    return Ok(collection.FindById(id));

                }
                else
                {
                    //contact was not found
                    return NotFound();
                }
            }
        }

        // GET api/contacts/call-list
        /// <summary>
        /// Returns a list of all contacts name and home phone number if they have a home phone number. Sorted by last name, then first name
        /// </summary>
        /// <returns></returns>
        [Route("~/api/contacts/call-list")]
        [HttpGet]
        public IHttpActionResult CallList()
        {
            using (var db = new LiteDatabase(dbConnection))
            {
                //get a reference to the collection
                var col = db.GetCollection<ContactModel>("Contacts");

                //get all contacts
                List<ContactModel> AllContacts = col.FindAll().ToList();

                //create a list to store the contacts to return
                List<CallListResultsModel> ContactsWithHomePhones = new List<CallListResultsModel>();

                //get the contacts that have home phone numbers
                IEnumerable<ContactModel> FilteredContacts = AllContacts.Where(x => x.phone.Any(y => y.type == "home"));

                //loop through all contacts
                foreach (ContactModel c in FilteredContacts)
                {
                    //loop through the contacts phone numbers
                    foreach (Phone p in c.phone)
                    {
                        //if the phone number is a home type add the contact and the number to the results
                        if (p.type == "home")
                        {
                            ContactsWithHomePhones.Add(new CallListResultsModel { name = c.name, phone = p.number });
                        }
                    }
                }
                //order by last name then first name
                ContactsWithHomePhones.OrderBy(x => x.name.last).ThenBy(x => x.name.first);
                return Ok(ContactsWithHomePhones);
            }
        }

        // POST api/contacts
        /// <summary>
        /// Adds the contact sent to the database
        /// </summary>
        /// <param name="contact">The new contact to be added</param>
        /// <returns></returns>
        public IHttpActionResult Post([FromBody] ContactModel contact)
        {
            using (var db = new LiteDatabase(dbConnection))
            {
                //get a reference to the collection
                var Contacts = db.GetCollection<ContactModel>("Contacts");
                //insert the new contact
                Contacts.Insert(contact);
            }
            return Ok();
        }

        // PUT api/contacts/5
        /// <summary>
        /// Updates an existing contact in the database
        /// </summary>
        /// <param name="id">The ID of the contact to update</param>
        /// <param name="contact">the contact model with the data to update the exist contact to</param>
        /// <returns></returns>
        public IHttpActionResult Put(int id, [FromBody] ContactModel contact)
        {
            using (var db = new LiteDatabase(dbConnection))
            {
                //get the reference to the collection
                var Contacts = db.GetCollection<ContactModel>("Contacts");

                //find the contact to update
                ContactModel toUpdate = Contacts.FindById(id);

                //make sure the contact was found
                if (toUpdate == null)
                {
                    return NotFound();
                }

                //update the contact
                toUpdate.address = contact.address;
                toUpdate.email = contact.email;
                toUpdate.name = contact.name;
                toUpdate.phone = contact.phone;
                //save the update to the DB
                Contacts.Update(toUpdate);

            }
            return Ok();
        }

        // DELETE api/contacts/5
        /// <summary>
        /// Deletes the contact from the database with the given id
        /// </summary>
        /// <param name="id">the ID of the contatc to delete</param>
        /// <returns></returns>
        public IHttpActionResult Delete(int id)
        {
            using (var db = new LiteDatabase(dbConnection))
            {
                //Get a reference to the contacts collection
                var Contacts = db.GetCollection<ContactModel>("Contacts");

                //make sure the contact we're deleting currently exists
                if (Contacts.FindById(id) != null)
                {
                    //if it does exist, delete it from the DB
                    Contacts.Delete(id);
                    return Ok();
                }
                else
                {
                    //the contact wasn't found.
                    return NotFound();
                }
            }
        }
    }
}
