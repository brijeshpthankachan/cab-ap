namespace CSMS.Areas.User.Controllers;

[Area("User")]
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

    public IActionResult Index()
    {
        return View();
    }


    [HttpPost]
    public async Task<IActionResult> Index(string location, string cabtype)
    {

        var userID = await _userManager.GetUserAsync(User);
        
        try
        {

            var driverInfo = (from c in _db.Cabs
                join a in _db.ApplicationUsers on c.ApplicationUserID equals a.Id
                where c.IsOnRoad == false && c.CabLocation == location.Trim() && c.CabType == cabtype
                select new RegisterDriverViewModel
                {
                    ApplicationUserID = a.Id,
                    CabName = c.CabName,
                    CabType = c.CabType,
                    PhoneNumber = a.PhoneNumber,
                    RcNumber = c.RcNumber,
                    FirstName = a.FirstName,
                    LastName = a.LastName,
                    Email = a.Email,
                    CurrentUserID = userID.Id


                }).ToList();

            return driverInfo == null ? View() : View(driverInfo);

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return View();
        }
  

    }



    //----------------------------------------------------------------------
    [HttpGet]
    public async Task<IActionResult> ViewProfile()
    {
        var userObject = await _userManager.GetUserAsync(User);

        return View(new RegisterUserViewModel
        {
            PhoneNumber = userObject.PhoneNumber,
            FirstName = userObject.FirstName,
            LastName = userObject.LastName,
            Email = userObject.Email,
            UserId = userObject.Id
        });
    }

    [HttpPost]
    public async Task<IActionResult> ViewProfile(RegisterUserViewModel model)
    {
        var userObject = await _userManager.FindByIdAsync(model.UserId);


        userObject.FirstName = model.FirstName;
        userObject.LastName = model.LastName;
        userObject.Email = model.Email;
        userObject.PhoneNumber = model.PhoneNumber;

        await _userManager.UpdateAsync(userObject);

        return RedirectToAction("Index", "Home", new { Area = "User" });
    }
    //----------------------------------------------------------------------

 

    //----------------------------------------------------------------------

    [HttpGet]
    public async Task<IActionResult> BookCab(string id)
    {
        var currentUser = _userManager.GetUserAsync(User).Result;

        var driver = await _db.Cabs.Include(m => m.ApplicationUsers).Where(m => m.ApplicationUserID == id).FirstAsync();


        var objectForBookingView = new RegisterDriverViewModel
        {
            FirstName = driver.ApplicationUsers.FirstName+ " " + driver.ApplicationUsers.LastName,
            CurrentUserID = currentUser.Id,
            PhoneNumber = driver.ApplicationUsers.PhoneNumber,
            Email = driver.ApplicationUsers.Email,
            CabName = driver.CabName,
            CabType = driver.CabType,
            ApplicationUserID = driver.ApplicationUserID
            

        };

        return View(objectForBookingView);
    }


    [HttpPost]
    public async Task<IActionResult> BookCab(RegisterDriverViewModel model)
    {



        try
        {
            await _db.Bookings.AddAsync(new Booking
            {

                ApplicationUserId = model.ApplicationUserID,
                UserId = model.CurrentUserID,
                ApplicationUsers = await _userManager.FindByIdAsync(model.ApplicationUserID),
                Source = model.Source,
                Destination = model.Destination,
                BookingTime = DateTime.UtcNow,
                BookingStatus = "Pending",
                Fair = 0.0

            });
            await _db.SaveChangesAsync();

            return RedirectToAction("Index", "Home", new { Area = "User" });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return View(model);
        }


    }

    //----------------------------------------------------------------------

    [HttpGet]
    public IActionResult ViewBookings()
    {
        var bookingList =_db.Bookings.Include(m=>m.ApplicationUsers).Where(m=>m.UserId == _userManager.GetUserAsync(User).Result.Id);
        return View(bookingList);
    }

    
    //----------------------------------------------------------------------

}