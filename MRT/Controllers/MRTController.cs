using Microsoft.AspNetCore.Mvc;
using MRT.Models;
using System.Data;
using System.Data.SqlClient;

namespace MRT.Controllers
{
    public class MRTController : Controller
    {
        private readonly IConfiguration configuration;
        public MRTController(IConfiguration config)
        {
            this.configuration = config;
        }

        public class MultipleUserViewModel
        {
            public User LoggedInUser { get; set; }
            public IList<User> UserList { get; set; }
        }
        public class SingleUserViewModel
        {
            public User LoggedInUser { get; set; }
            public User TargetUser { get; set; }
        }

        public class BookingViewModel
        {
            public User User { get; set; }
            public Booking Booking { get; set; }
        }

        // GET: MRTController
        // Login
        public ActionResult Index()
        {
            return View();
        }

        // Sign Up
        public ActionResult SignUp()
        {
            return View();
        }

        // POST: SignUp
        [HttpPost]
        public ActionResult SignUp(User user)
        {
            IList<User> userList = GetUserList();

            // Check if the email already exists
            if (userList.Any(u => u.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase)))
            {
                // Set an error message in ViewBag
                ViewBag.ErrorMessage = "Email already exists. Please use a different email address.";

                // Redirect to the SignUp view
                return View("SignUp", user);
            }

            //if (ModelState.IsValid)
            //{
            SqlConnection conn = new SqlConnection(configuration.GetConnectionString("MRTConnStr"));
            SqlCommand cmd = new SqlCommand("spInsertUser", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Email", user.Email);
            cmd.Parameters.AddWithValue("@Name", user.Name);
            cmd.Parameters.AddWithValue("@IdentityCard", user.IdentityCard);
            cmd.Parameters.AddWithValue("@Password", user.Password);
            cmd.Parameters.AddWithValue("@Type", user.Type);

            try
            {
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes
                Console.WriteLine(ex.Message);

                // Set an error message in ViewBag
                ViewBag.ErrorMessage = "An error occurred while processing your request. Please try again later.";

                // Redirect to the SignUp view
                return View("SignUp", user);
            }
            finally
            {
                conn.Close();
            }

            ViewBag.SuccessMessage = "Register Success. Please log in.";

            return View("Index");
            //}
            //else
            //{
            //    // Log the exception for debugging purposes
            //    Console.WriteLine("Invalid Model State");

            //    // Set an error message in ViewBag
            //    ViewBag.ErrorMessage = "Invalid Model State";
            //    return View("SignUp");
            //}
        }

        // Get all user
        IList<User> GetUserList()
        {
            IList<User> userList = new List<User>();
            SqlConnection conn = new SqlConnection(configuration.GetConnectionString("MRTConnStr"));

            string sql = @"SELECT * FROM Users";

            SqlCommand cmd = new SqlCommand(sql, conn);

            try
            {
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    userList.Add(new User()
                    {
                        UserId = reader.GetInt32(0),
                        Email = reader.GetString(1),
                        Name = reader.GetString(2),
                        IdentityCard = reader.GetString(3),
                        IsAdmin = reader.IsDBNull(4) ? false : Convert.ToBoolean(reader.GetValue(4)),
                        Type = reader.IsDBNull(5) ? null : reader.GetString(5),
                        Password = reader.GetString(6)
                    });
                }
            }
            catch
            {
                RedirectToAction("Error");

            }
            finally
            {
                conn.Close();
            }

            return userList;
        }

        // Get single user by id
        public User? GetUserById(int userId)
        {
            IList<User> userList = GetUserList();

            if (userList == null)
            {
                return null;
            }

            var user = userList.FirstOrDefault(x => x.UserId == userId);

            if (user == null)
            {
                return null;
            }

            return user;
        }


        // Login
        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            IList<User> userList = GetUserList();

            if (userList == null)
            {
                ViewBag.ErrorMessage = "No user registered.";
                return View("Index");
            }

            var user = userList.FirstOrDefault(x => x.Email == email && x.Password == password);

            if (user != null)
            {
                return View("Home", user);
                //return RedirectToAction("Home", user.UserId);
            }
            else
            {
                ViewBag.ErrorMessage = "Invalid email or password.";
                return View("Index");
            }
        }

        // Home
        public ActionResult Home(int userId)
        {
            User? user = GetUserById(userId);

            if (user == null)
            {
                ViewBag.ErrorMessage = "Please log in.";
                return View("Index");
            }

            return View(user);
        }

        // Go to profile view
        [HttpGet]
        public ActionResult Profile(int userId)
        {
            User? user = GetUserById(userId);

            if (user == null)
            {
                ViewBag.ErrorMessage = "Please log in.";
                return View("Index");
            }

            return View(user);
        }

        // Update Profile
        [HttpPost]
        public ActionResult Profile(int userId, User user)
        {
            // Get user that want to update
            User? targetUser = GetUserById(userId);

            if (targetUser == null)
            {
                ViewBag.ErrorMessage = "Please log in.";
                return View("Index");
            }

            IList<User> userList = GetUserList();

            // Check if there is changes on email
            if (!targetUser.Email.Equals(user.Email))
            {
                // Check if the email already exists
                if (userList.Any(u => u.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase)))
                {
                    // Set an error message in ViewBag
                    ViewBag.ErrorMessage = "Email already exists. Please use a different email address.";

                    // Redirect to the SignUp view
                    return View("Profile", user);
                }
            }

            SqlConnection conn = new SqlConnection(configuration.GetConnectionString("MRTConnStr"));
            SqlCommand cmd = new SqlCommand("spUpdateUser", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@Email", user.Email);
            cmd.Parameters.AddWithValue("@Name", user.Name);
            cmd.Parameters.AddWithValue("@IdentityCard", user.IdentityCard);
            cmd.Parameters.AddWithValue("@Password", user.Password);
            cmd.Parameters.AddWithValue("@Type", user.Type);

            // try
            // {
            conn.Open();
            cmd.ExecuteNonQuery();
            // }
            // catch
            // {
            // RedirectToAction("Error");
            // }
            // finally
            // {
            conn.Close();
            // }

            ViewBag.SuccessMessage = "Profile updated!";

            return View("Profile", user);
        }

        // Manage User - View all user
        public ActionResult ManageUsers(int userId)
        {
            User? user = GetUserById(userId);

            if (user == null)
            {
                ViewBag.ErrorMessage = "Please log in.";
                return View("Index");
            }

            IList<User> userList = GetUserList();

            MultipleUserViewModel viewModel = new MultipleUserViewModel
            {
                LoggedInUser = user,
                UserList = userList
            };

            return View(viewModel);
        }

        // View User - View profile invidually
        [HttpGet]
        public ActionResult ViewUser(int loggedInUserId, int userId)
        {
            User? loggedInUser = GetUserById(loggedInUserId);

            if (loggedInUser == null)
            {
                ViewBag.ErrorMessage = "Please log in.";
                return View("Index");
            }

            User? user = GetUserById(userId);

            if (user == null)
            {
                ViewBag.ErrorMessage = "Please log in.";
                return View("ManageUsers", loggedInUser);
            }

            SingleUserViewModel viewModel = new SingleUserViewModel
            {
                LoggedInUser = loggedInUser,
                TargetUser = user
            };

            return View(viewModel);
        }

        // Add Booking view
        [HttpGet]
        public ActionResult AddBooking(int userId)
        {
            User? user = GetUserById(userId);

            if (user == null)
            {
                ViewBag.ErrorMessage = "Please log in.";
                return View("Index");
            }

            // Create a new instance of Booking
            Booking newBooking = new Booking();

            // Pass both User and Booking to the view
            BookingViewModel viewModel = new BookingViewModel
            {
                User = user,
                Booking = newBooking
            };

            return View(viewModel);
        }

        // Insert Booking
        [HttpPost]
        public ActionResult AddBooking(Booking booking)
        {
            User? user = GetUserById(booking.UserId);

            SqlConnection conn = new SqlConnection(configuration.GetConnectionString("MRTConnStr"));
            SqlCommand cmd = new SqlCommand("spInsertBooking", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@UserId", booking.UserId);
            cmd.Parameters.AddWithValue("@StationFrom", booking.StationFrom);
            cmd.Parameters.AddWithValue("@StationTo", booking.StationTo);
            cmd.Parameters.AddWithValue("@IsOneWay", booking.IsOneWay);
            cmd.Parameters.AddWithValue("@ListPrice", booking.ListPrice);
            cmd.Parameters.AddWithValue("@DiscountPercentage", booking.DiscountPercentage);
            cmd.Parameters.AddWithValue("@TotalPrice", booking.TotalPrice);

            // try
            // {
            conn.Open();
            cmd.ExecuteNonQuery();
            // }
            // catch
            // {
            // RedirectToAction("Error");
            // }
            // finally
            // {
            conn.Close();
            // }

            ViewBag.SuccessMessage = "Booking success!";

            return View("Home", user);
        }
    }
}
