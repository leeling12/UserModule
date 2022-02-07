#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UserModule.Context;
using UserModule.Models;
using PagedList;

namespace UserModule.Controllers
{
    public class UserController : Controller
    {
        private MVCContext _context;
        public const string SessionKeyUserID = "_UserID";

        public UserController(MVCContext context)
        {
            _context = context;
        }

        //--------------------------------------------------------------------
        //------------------------ User Listing ------------------------------
        //--------------------------------------------------------------------
        public async Task<IActionResult> Index()
        {
        //public IActionResult Index(int? page, int? pageSize)

                /*   int pageIndex = 1;
                   pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                   //Ddefault size is 5 otherwise take pageSize value  
                   int defaSize = (pageSize ?? 5);

                   ViewBag.psize = defaSize;

                   //Dropdownlist code for PageSize selection  
                   //In View Attach this  
                   ViewBag.PageSize = new List<SelectListItem>()
           {
               new SelectListItem() { Value="5", Text= "5" },
               new SelectListItem() { Value="10", Text= "10" },
               new SelectListItem() { Value="15", Text= "15" },
            };

                   //Create a instance of our DataContext  
                //   InvoiceModel _dbContext = new InvoiceModel();
                   IPagedList<User> userlist = null;*/

                //Alloting nos. of records as per pagesize and page index.  
                //  involst = _dbContext.tblInvoices.OrderBy(x => x.InvoiceID).ToPagedList(pageIndex, defaSize);

                // return View(involst);
                var mVCContext = _context.Users.Include(u => u.Position).Include(u => u.Status).Include(u => u.Team).OrderBy(u => u.Username);
            return View(await mVCContext.ToListAsync());
        }

        //--------------------------------------------------------------------
        //----------------------------- Login --------------------------------
        //--------------------------------------------------------------------

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(User input)
        {
            // Get IP Address
            // Retreive server/local IP address
            var feature = HttpContext.Features.Get<IHttpConnectionFeature>();
            string LocalIPAddr = feature?.LocalIpAddress?.ToString();
            //HttpContext.Connection.RemoteIpAddress?.ToString();


            //       if (ModelState.IsValid)
            //   {
            User validUsername = _context.Users
                                   .Where(u => u.Username == input.Username)
                                   .FirstOrDefault();
            User user = _context.Users
                                       .Where(u => u.Username == input.Username && u.Password == input.Password)
                                       .FirstOrDefault();

            //Check is it different ip address
            if (user != null && user.StatusRefId == 1 && !(user.SecurityPhrase.Equals(LocalIPAddr)))
            {
                //force current user logged out
                user.StatusRefId = 2;
                _context.Entry(validUsername).State = EntityState.Modified;
                _context.SaveChanges();

                //login
                HttpContext.Session.SetInt32(SessionKeyUserID, user.EmployeeId);
                HttpContext.Session.SetString("_CurrentUser", user.Username);


                //active user
                user.StatusRefId = 1;
                _context.Entry(user).State = EntityState.Modified;
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            else if (user != null && user.StatusRefId != 3)
            {
                // Check whether the user is exist & suspend or not
                // Store userID inside session
                HttpContext.Session.SetInt32(SessionKeyUserID, user.EmployeeId);
                HttpContext.Session.SetString("_CurrentUser", user.Username);


                //active user
                user.StatusRefId = 1;
                _context.Entry(user).State = EntityState.Modified;
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            else if (user != null && user.StatusRefId == 3)
            {
                ViewBag.ErrorMsg = "Too many failed login attempts. Account has been locked.";
                return View();
            }
            else if (validUsername != null)
            {
                int _failToLogin = HttpContext.Session.GetInt32("failToLogin") ?? 10;
                // 1st time invalid
                if (_failToLogin == 1)
                {
                    validUsername.StatusRefId = 3;
                    _context.Entry(validUsername).State = EntityState.Modified;
                    _context.SaveChanges();
                    ViewBag.ErrorMsg = "Too many failed login attempts. Account has been locked.";
                    return View();
                }
                else
                {
                    _failToLogin--;
                    HttpContext.Session.SetInt32("failToLogin", _failToLogin);
                    ViewBag.ErrorMsg = "Incorrect password, " + _failToLogin + " more times to suspend";
                    return View();
                }


            }
            else
            {
                ViewBag.ErrorMsg = "Incorrect username or password";
                return View();
            }
            return View();
        }

        //--------------------------------------------------------------------
        //---------------------------- Logout --------------------------------
        //--------------------------------------------------------------------
      
        public IActionResult Logout()
        {
            //Retrieve current user id from session & set status to disable
            var currentuserid = HttpContext.Session.GetInt32(SessionKeyUserID);
            User currentuser = _context.Users
                                   .Where(u => u.EmployeeId == currentuserid)
                                   .FirstOrDefault();
            currentuser.StatusRefId = 2;
            _context.Entry(currentuser).State = EntityState.Modified;
            _context.SaveChanges();

            //Clear session & back to login page
            HttpContext.Session.Clear();
            return RedirectToAction("Index","Home");
        }

        //--------------------------------------------------------------------
        //---------------------------- Details -------------------------------
        //--------------------------------------------------------------------
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Position)
                .Include(u => u.Status)
                .Include(u => u.Team)
                .FirstOrDefaultAsync(m => m.EmployeeId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        //--------------------------------------------------------------------
        //---------------------------- Create --------------------------------
        //--------------------------------------------------------------------
        public IActionResult Create()
        {
            ViewData["PositionRefId"] = new SelectList(_context.Positions, "PositionId", "PositionName");
            ViewData["StatusRefId"] = new SelectList(_context.Statuses, "StatusId", "CurrentStatus");
            ViewData["TeamRefId"] = new SelectList(_context.Teams, "TeamId", "TeamName");
            return View();
        }

        // POST: User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmployeeId,Username,Fullname,Password,ConfirmPassword,Email,JoinDate,PositionRefId,TeamRefId,StatusRefId,SecurityPhrase")] User user)
        {
            // if (ModelState.IsValid)
            //  {


            // if username exist
            if (UsernameExists(user.Username))
            {
                //display error message
                ViewBag.UsernameExist = "This username has already been registered. Please enter a different username.";
                ViewData["PositionRefId"] = new SelectList(_context.Positions, "PositionId", "PositionName");
                ViewData["StatusRefId"] = new SelectList(_context.Statuses, "StatusId", "CurrentStatus");
                ViewData["TeamRefId"] = new SelectList(_context.Teams, "TeamId", "TeamName");
                return View(user);
            }
            else
            {
                //update database
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
                //   }
                ViewData["PositionRefId"] = new SelectList(_context.Positions, "PositionId", "PositionName", user.PositionRefId);
                ViewData["StatusRefId"] = new SelectList(_context.Statuses, "StatusId", "CurrentStatus", user.StatusRefId);
                ViewData["TeamRefId"] = new SelectList(_context.Teams, "TeamId", "TeamName", user.TeamRefId);
                return View(user);
            }
           
        }

        //--------------------------------------------------------------------
        //------------------------------ Edit --------------------------------
        //--------------------------------------------------------------------
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            ViewData["PositionRefId"] = new SelectList(_context.Positions, "PositionId", "PositionName", user.PositionRefId);
            ViewData["StatusRefId"] = new SelectList(_context.Statuses, "StatusId", "CurrentStatus", user.StatusRefId);
            ViewData["TeamRefId"] = new SelectList(_context.Teams, "TeamId", "TeamName", user.TeamRefId);
            return View(user);
        }

        // POST: User/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EmployeeId,Username,Fullname,Password,ConfirmPassword,Email,JoinDate,PositionRefId,TeamRefId,StatusRefId,SecurityPhrase")] User user)
        {
            if (id != user.EmployeeId)
            {
                return NotFound();
            }

            //if (ModelState.IsValid)
            //  {
            try
            {
                _context.Update(user);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(user.EmployeeId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
            //  }
            ViewData["PositionRefId"] = new SelectList(_context.Positions, "PositionId", "PositionName", user.PositionRefId);
            ViewData["StatusRefId"] = new SelectList(_context.Statuses, "StatusId", "CurrentStatus", user.StatusRefId);
            ViewData["TeamRefId"] = new SelectList(_context.Teams, "TeamId", "TeamName", user.TeamRefId);
            return View(user);
        }

        //--------------------------------------------------------------------
        //---------------------------- Delete --------------------------------
        //--------------------------------------------------------------------
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Position)
                .Include(u => u.Status)
                .Include(u => u.Team)
                .FirstOrDefaultAsync(m => m.EmployeeId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.EmployeeId == id);
        }

        private bool UsernameExists(string name)
        {
            return _context.Users.Any(e => e.Username == name);
        }
    }
}
