//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.IdentityModel.Tokens;
//using movie_hospital_1.dataAccess;
//using movie_hospital_1.dataModel;

//namespace movie_hospital_1.Utilities.DBInitilizer
//{
//    public class DBInitializer : IDBInitializer
//    {
//        private readonly ApplicationDbContext _context;
//        private readonly ILogger<DBInitializer> _logger;
//        private  readonly RoleManager<IdentityRole> _roleManager;
//        private readonly UserManager<ApplicationUser> _userManager;


//        public DBInitializer(ApplicationDbContext context,ILogger<DBInitializer>logger,RoleManager<IdentityRole> roleManager,UserManager<ApplicationUser>userManager)
//        {
//            _context = context;
//            _logger = logger;
//            _roleManager = roleManager;
//            _userManager = userManager;
//        }

//        public RoleManager<IdentityRole> RoleManager { get; }

//        public void Initialize()
//        {
//            try
//            {
//                if (_context.Database.GetPendingMigrations().Any())
//                  _context.Database.Migrate();
//                if (_roleManager.Roles.IsNullOrEmpty())
//                {
//                    _roleManager.CreateAsync(new(SD.ROLE_SUPER_ADMIN)).GetAwaiter().GetResult();
//                    _roleManager.CreateAsync(new(SD.ROLE_ADMIN)).GetAwaiter().GetResult();
//                    _roleManager.CreateAsync(new(SD.ROLE_EMPLOYEE)).GetAwaiter().GetResult();
//                    _roleManager.CreateAsync(new(SD.ROLE_CUSTOMER)).GetAwaiter().GetResult();

//                    var result = _userManager.CreateAsync(new()
//                    {
//                        Email="superadmin@ecu.com",
//                        UserName="SuperAdmin",
//                        EmailConfirmed=true,
//                        firstName="super",
//                        lastName="admin",
//                    }, "SuperAdmin@123").GetAwaiter().GetResult();
//                    var user = _userManager.FindByNameAsync("SuperAdmin").GetAwaiter().GetResult();
//                  _userManager.AddToRoleAsync(user!, SD.ROLE_SUPER_ADMIN).GetAwaiter().GetResult();





//                    var resultAdmin = _userManager.CreateAsync(new()
//                    {
//                        Email = "admin@ecu.com",
//                        UserName = "Admin",
//                        EmailConfirmed = true,
//                        firstName = "Admin",
//                        lastName = "admin",
//                    }, "Admin@123").GetAwaiter().GetResult();
//                    var userAdmin = _userManager.FindByNameAsync("Admin").GetAwaiter().GetResult();
//                    _userManager.AddToRoleAsync(userAdmin!, SD.ROLE_ADMIN).GetAwaiter().GetResult();

//                }

//            }
//            catch (Exception ex)
//            {
//                _logger.LogError($"Error: {ex.Message}");
//            }
//        }
//    }
//}



using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using movie_hospital_1.dataAccess;
using movie_hospital_1.dataModel;

namespace movie_hospital_1.Utilities.DBInitilizer
{
    public class DBInitializer : IDBInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DBInitializer> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public DBInitializer(
            ApplicationDbContext context,
            ILogger<DBInitializer> logger,
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _logger = logger;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public void Initialize()
        {
            try
            {
                // تطبيق أي Migrations معلقة
                if (_context.Database.GetPendingMigrations().Any())
                    _context.Database.Migrate();

                // إنشاء الأدوار إذا مش موجودة
                if (!_roleManager.Roles.Any())
                {
                    _roleManager.CreateAsync(new IdentityRole(SD.ROLE_SUPER_ADMIN)).GetAwaiter().GetResult();
                    _roleManager.CreateAsync(new IdentityRole(SD.ROLE_ADMIN)).GetAwaiter().GetResult();
                    _roleManager.CreateAsync(new IdentityRole(SD.ROLE_EMPLOYEE)).GetAwaiter().GetResult();
                    _roleManager.CreateAsync(new IdentityRole(SD.ROLE_CUSTOMER)).GetAwaiter().GetResult();
                }

                // إنشاء SuperAdmin إذا مش موجود
                var superAdmin = _userManager.FindByNameAsync("SuperAdmin").GetAwaiter().GetResult();
                if (superAdmin == null)
                {
                    superAdmin = new ApplicationUser
                    {
                        UserName = "SuperAdmin",
                        Email = "superadmin@ecu.com",
                        EmailConfirmed = true,
                        firstName = "super",
                        lastName = "admin"
                    };
                    _userManager.CreateAsync(superAdmin, "SuperAdmin@123").GetAwaiter().GetResult();
                    _userManager.AddToRoleAsync(superAdmin, SD.ROLE_SUPER_ADMIN).GetAwaiter().GetResult();
                }

                // إنشاء Admin إذا مش موجود
                var admin = _userManager.FindByNameAsync("Admin").GetAwaiter().GetResult();
                if (admin == null)
                {
                    admin = new ApplicationUser
                    {
                        UserName = "Admin",
                        Email = "admin@ecu.com",
                        EmailConfirmed = true,
                        firstName = "Admin",
                        lastName = "admin"
                    };
                    _userManager.CreateAsync(admin, "Admin@123").GetAwaiter().GetResult();
                    _userManager.AddToRoleAsync(admin, SD.ROLE_ADMIN).GetAwaiter().GetResult();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during database initialization");
            }
        }
    }
}

