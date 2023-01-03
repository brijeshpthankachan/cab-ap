

namespace CSMS.Areas.Accounts.Controllers;

[Area("Accounts")]
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

    //-----------------------------------------------------------------------------------

    [Route("/")]
    public IActionResult Index()
    {
        return View();
    }


    [Route("/")]
    [HttpPost]
    public async Task<IActionResult> Index(LoginViewModel model)
    {
        //if (!ModelState.IsValid) return View(model);

        Console.WriteLine("hi");
        var user = await _userManager.FindByEmailAsync(model.Email);

        if (user == null)
        {
            ModelState.AddModelError("", "Invalid Details");
            return View(model);
        }

        var res = await _signInManager.PasswordSignInAsync(user, model.Password, true, true);
        var role = _userManager.GetRolesAsync(user);

        if (res.Succeeded)
        {
            Console.WriteLine("hello");

            if (role.Result.Contains("Admin"))
            {
                Console.WriteLine("Admin here");
                return RedirectToAction("Index", "Home", new { Area = "Admin" });
            }
            if (role.Result.Contains("User"))
            {
                Console.WriteLine("User is here");
                return RedirectToAction("Index", "Home", new { Area = "User" });
            }
               
            return RedirectToAction("Index", "Home", new { Area = "Driver" });
        }

        return View(model);
    }

    //-----------------------------------------------------------------------------------


    [HttpGet]
    public IActionResult RegisterUser()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> RegisterUser(RegisterUserViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = new ApplicationUser
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            PhoneNumber = model.PhoneNumber,
            UserName = Guid.NewGuid().ToString().Replace("-", "").ToLower()
        };

        var res = await _userManager.CreateAsync(user, model.Password);
        if (res.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "User");
            return Redirect("/");
        }


        ModelState.AddModelError("", "An Error Has Occured While Creating New User");
        return View(model);
    }

    //-----------------------------------------------------------------------------------

    [HttpGet]
    public IActionResult RegisterDriver()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> RegisterDriver(RegisterDriverViewModel model)
    {
        var driver = new ApplicationUser
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            PhoneNumber = model.PhoneNumber,
            UserName = Guid.NewGuid().ToString().Replace("-", "")
        };

        var res = await _userManager.CreateAsync(driver, model.Password);

        if (res.Succeeded)
        {
            await _userManager.AddToRoleAsync(driver, "Driver");

            await _db.SaveChangesAsync();


//Adding Cab Details

            var userObject = await _userManager.FindByEmailAsync(model.Email);
            var driverId = userObject.Id;

            await _db.Cabs.AddAsync(new Cab
            {
                CabName = model.CabName,
                CabType = model.CabType,
                LicenseNumber = model.LicenseNumber,
                RcNumber = model.RcNumber,
                ApplicationUserID = driverId,
                ApplicationUsers = userObject,
                CabLocation = model.CabLocation,
                IsOnRoad = false
            });

            await _db.SaveChangesAsync();
            return Redirect("/");
        }

        ModelState.AddModelError("", "An error has occured");
        return View(model);
    }

    //-----------------------------------------------------------------------------------

    public async Task<IActionResult> GenerateData()
    {
        await _roleManager.CreateAsync(new IdentityRole { Name = "Admin" });
        await _roleManager.CreateAsync(new IdentityRole { Name = "User" });
        await _roleManager.CreateAsync(new IdentityRole { Name = "Driver" });

        var users = await _userManager.GetUsersInRoleAsync("Admin");
        if (users.Count != 0) return Ok("Data Generated");
        var adminUser = new ApplicationUser
        {
            FirstName = "Admin",
            LastName = "User",
            Email = "admin@123.com",
            UserName = "admin"
        };
        var res = await _userManager.CreateAsync(adminUser, "Pass@123");
        await _userManager.AddToRoleAsync(adminUser, "admin");

        return Ok("Data Generated");
    }
}