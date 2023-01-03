
namespace CSMS.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {


        private readonly ApplicationDbContext _db;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ApplicationDbContext db, UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }
        //---------------------------------------------------------------------------------------------

        public IActionResult Index()
        {
            return View();
        }
        //---------------------------------------------------------------------------------------------

        [HttpGet]
        public IActionResult ViewProfile()
        {
            var adminUser = _userManager.GetUserAsync(User).Result;
            return View(new RegisterUserViewModel
            {
                FirstName = adminUser.FirstName,
                LastName = adminUser.LastName,
                Email = adminUser.Email,
                PhoneNumber = adminUser.PhoneNumber
            });
        }


        [HttpPost]
        public async Task<IActionResult> ViewProfile(RegisterUserViewModel model)
        {
            var userObject = await _userManager.FindByIdAsync(_userManager.GetUserAsync(User).Result.Id);


            userObject.FirstName = model.FirstName;
            userObject.LastName = model.LastName;
            userObject.Email = model.Email;
            userObject.PhoneNumber = model.PhoneNumber;

            await _userManager.UpdateAsync(userObject);

            return RedirectToAction("Index","Home",new {Area = "Admin"});
        }


        //---------------------------------------------------------------------------------------------

        [HttpGet]
        public async Task<IActionResult> ViewUsers()
        {
            var users=await _userManager.GetUsersInRoleAsync("User");
            
            return View(users);
        }
        //---------------------------------------------------------------------------------------------

        public async Task<IActionResult> ViewDrivers()
        {
            var users = await _userManager.GetUsersInRoleAsync("Driver");

            return View(users);
        }

        //---------------------------------------------------------------------------------------------
        [HttpGet]
        public IActionResult ViewReports()
        {
            var bookings = _db.Bookings.ToList();
            var bookingList = bookings.Select(i => new RegisterDriverViewModel
                {
                    CustomerName = _userManager.FindByIdAsync(i.UserId).Result.FirstName,
                    FirstName = _userManager.FindByIdAsync(i.ApplicationUserId).Result.FirstName,
                    BookingDate = i.BookingTime,
                    BookingStatus = i.BookingStatus,
                    Source = i.Source,
                    Destination = i.Destination,
                    Fair = i.Fair
                })
                .ToList();

            return View(bookingList);
        }
        //---------------------------------------------------------------------------------------------

    }
}
