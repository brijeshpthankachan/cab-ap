
namespace CSMS.Areas.Driver.Controllers
{
    [Area("Driver")]
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


        //---------------------------------------------------------------------------------------------------

        [HttpGet]
        public IActionResult Index()
        {
            var driverId = _userManager.GetUserAsync(User).Result.Id;
            
            var bookingDetails = (from b in _db.Bookings
                join a in _db.ApplicationUsers on b.UserId equals a.Id
                where b.ApplicationUserId == driverId && b.BookingStatus == "Pending"
                select new RegisterDriverViewModel()
                {
                    FirstName = a.FirstName + " " + a.LastName,
                    PhoneNumber = a.PhoneNumber,
                    Email = a.Email,
                    Source = b.Source,
                    Destination = b.Destination,
                    BookingDate = b.BookingTime,
                    BookingStatus = b.BookingStatus,
                    Fair = b.Fair,
                    BookingId = b.Id
                }).ToList();

            return View(bookingDetails);
        }



        [HttpGet]
        public async Task<IActionResult> SetBookingStatus(int bookingId, int bookingStatusId)
        {
            Console.WriteLine("hi");
            var bookingObject= await _db.Bookings.FindAsync(bookingId);
            if (bookingStatusId == 1)
            {
                bookingObject.BookingStatus = "Payment Pending";
            }
            else
            {
                bookingObject.BookingStatus = "Rejected";
            }
            await _db.SaveChangesAsync();

            return RedirectToAction("Index", "Home", new { Area = "Driver" });

        }



        //---------------------------------------------------------------------------------------------------

        [HttpGet]
        public async Task<IActionResult> ViewProfile()
        {
            var userObject = await _userManager.GetUserAsync(User);
            var userId = userObject.Id;

            var userCabObject =  await _db.Cabs.Include(m => m.ApplicationUsers).Where(m=>m.ApplicationUserID == userId).FirstAsync();


            return View(new RegisterDriverViewModel
            {
                PhoneNumber = userObject.PhoneNumber,
                FirstName = userObject.FirstName,
                LastName = userObject.LastName,
                Email = userObject.Email,
                ApplicationUserID = userObject.Id,
                CabName = userCabObject.CabName,
                CabLocation = userCabObject.CabLocation,
                CabType = userCabObject.CabType,
                LicenseNumber = userCabObject.LicenseNumber,
                RcNumber = userCabObject.RcNumber
            });
        }

        [HttpPost]
        public async Task<IActionResult> ViewProfile(RegisterDriverViewModel model)
        {
            var userObject = await _userManager.FindByIdAsync(model.ApplicationUserID);
            var cabObject = await _db.Cabs.FirstAsync(m=>m.ApplicationUserID == userObject.Id);


            userObject.FirstName = model.FirstName;
            userObject.LastName = model.LastName;
            userObject.Email = model.Email;
            userObject.PhoneNumber = model.PhoneNumber;

            cabObject.CabType = model.CabType;
            cabObject.LicenseNumber = model.CabName;
            cabObject.CabLocation = model.CabLocation;

            await _db.SaveChangesAsync();
              await _userManager.UpdateAsync(userObject);



            return RedirectToAction("Index", "Home", new { Area = "Driver" });
        }


        public IActionResult Payment()
        {
            return View();
        }

    }
}
