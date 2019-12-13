using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ActivityCenter.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
namespace ActivityCenter.Controllers
{
    public class HomeController : Controller
    {
        public MyContext dbContext;
        public HomeController(MyContext context){
            dbContext = context;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

       
         //RegisterUser
        [HttpPost("register")]
        public IActionResult RegisterUser(RegLoginUser newUser){
            if(ModelState.IsValid){
                if(dbContext.Users.Any(u => u.Email == newUser.RegUser.Email)){
                    ModelState.AddModelError("RegUser.Email","Email Address should be unique");
                    return View("Index",newUser);
                }
                else{
                    PasswordHasher<User> hasher = new PasswordHasher<User>();
                    newUser.RegUser.Password = hasher.HashPassword(newUser.RegUser,newUser.RegUser.Password);
                    dbContext.Add(newUser.RegUser);
                    dbContext.SaveChanges();
                    //Log the user by adding to Session
                    // User userInDb = dbContext.Users.FirstOrDefault(u => u.Email == newUser.RegUser.Email);

                    // int? userID = HttpContext.Session.GetInt32("LoggedUser");
                    // if(userID == null){
                    HttpContext.Session.SetInt32("LoggedUser",newUser.RegUser.UserId);
                    // }

                    return RedirectToAction("Home");
 
                }
            }
            return View("Index",newUser);
        }

        [HttpGet("success")]
        public IActionResult Success(){
            int? logged_id = HttpContext.Session.GetInt32("LoggedUser");
            if(logged_id == null){
                
                return View("Index");
            }
            User logged = dbContext.Users.FirstOrDefault(u => u.UserId == logged_id);
            return View("account",logged);
        }
        [HttpGet("login")]
        public IActionResult Login(){
            return View();
        }
        [HttpGet("logOut")]
        public IActionResult LogOut(){
            int? logged_id = HttpContext.Session.GetInt32("LoggedUser");
            if(logged_id == null){
                
                return RedirectToAction("Index");
            }
            HttpContext.Session.Remove("LoggedUser");
            return RedirectToAction("Index");
        }

        [HttpPost("loginUser")]
        public IActionResult LoginUser(RegLoginUser user){
            if(ModelState.IsValid){
                User userInDb = dbContext.Users.FirstOrDefault(u => u.Email == user.LoginUser.Email);
                if(userInDb == null){
                    ModelState.AddModelError("LoginUser.Email","Invalid Email Addreess");
                    return View("Index",user);
                }
                PasswordHasher<LoginUser> hasher = new PasswordHasher<LoginUser>();
                PasswordVerificationResult result = hasher.VerifyHashedPassword(user.LoginUser,userInDb.Password,user.LoginUser.Password); 
                if (result == 0){
                    ModelState.AddModelError("LoginUser.Password","Passowrd doesn't match the given Email Addess");
                    return View("Index",user);
                }else{
                    // int? userID = HttpContext.Session.GetInt32("LoggedUser");
                    // if(userID == null){
                    HttpContext.Session.SetInt32("LoggedUser",userInDb.UserId);
                    // }
                    return RedirectToAction("Home");
                }
            }
            return View("Index",user);
        }
        [HttpGet("home")]
        public IActionResult Home(){
            int? logged_id = HttpContext.Session.GetInt32("LoggedUser");
            if(logged_id == null){
                
                return RedirectToAction("Index");
            }
            HomeView hv = new HomeView();
            hv.loggedUser = dbContext.Users.Include(u => u.createdPlan).Include(u => u.ActivitiesToAttend).ThenInclude(ac => ac.JoiningActivity).FirstOrDefault(u => u.UserId == (int)logged_id);
            hv.AllHappenings = dbContext.Happenings.Where(h => h.HappeningDate > DateTime.Now).Include(h => h.CreatedBy).Include(h => h.ParticipantsList).ThenInclude(pa => pa.Participant).ToList();
            // hv.AllHappenings.Where(h => h.HappeningDate > DateTime.Now);
            return View("home",hv);
        }
        [HttpGet("new")]
        public IActionResult NewHappening(){
            int? logged_id = HttpContext.Session.GetInt32("LoggedUser");
            if(logged_id == null){
                
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpPost("Add")]
        public IActionResult NewActivity(Happening newHappening){
            int? logged_id = HttpContext.Session.GetInt32("LoggedUser");
            if(logged_id == null){
                
                return RedirectToAction("Index");
            }
            if(ModelState.IsValid){
                newHappening.CreatedById = (int)logged_id;
                newHappening.HappeningDate = newHappening.HappeningDate.Date + newHappening.StartTime.TimeOfDay;
                if (newHappening.DurationMetric == "Days"){
                    TimeSpan ts =new TimeSpan(newHappening.Duration,0,0,0);
                    newHappening.EndTime = newHappening.HappeningDate + ts;
                }else if(newHappening.DurationMetric == "Hours"){
                    TimeSpan ts =new TimeSpan(newHappening.Duration,0,0);
                    newHappening.EndTime = newHappening.HappeningDate + ts;
                }else if(newHappening.DurationMetric == "Mins"){
                    TimeSpan ts =new TimeSpan(0,newHappening.Duration,0);
                    newHappening.EndTime = newHappening.HappeningDate + ts;
                }

                dbContext.Happenings.Add(newHappening);
                dbContext.SaveChanges();
                ActivityDetail ad = new ActivityDetail();
                ad.Happening = dbContext.Happenings
                                        .Include(h => h.CreatedBy)
                                        .Include(h =>  h.ParticipantsList)
                                        .ThenInclude(p => p.Participant)
                                        .FirstOrDefault(h => h.HappeningId == newHappening.HappeningId);
                
                if(ad.Happening !=  null){
                    ad.loggedUser = dbContext.Users.Include(u => u.createdPlan).Include(u =>u.ActivitiesToAttend).ThenInclude(p =>p.JoiningActivity).FirstOrDefault(u => u.UserId ==(int)logged_id);
                    return View("activityDetail",ad);
                }
                return RedirectToAction("home");
                // return RedirectToAction("Home");
                //return RedirectToAction("ActivityDetail",newHappening.HappeningId);
            }
            return View("newHappening",newHappening);
        }
        [HttpGet("activity/{hapId}")]
        public IActionResult ActivityDetail(int hapId){
            int? logged_id = HttpContext.Session.GetInt32("LoggedUser");
            if(logged_id == null){
                
                return RedirectToAction("Index");
            }
            ActivityDetail ad = new ActivityDetail();
            ad.Happening = dbContext.Happenings
                                        .Include(h => h.CreatedBy)
                                        .Include(h =>  h.ParticipantsList)
                                        .ThenInclude(p => p.Participant)
                                        .FirstOrDefault(h => h.HappeningId == hapId);
            if(ad.Happening !=  null){
                ad.loggedUser = dbContext.Users.Include(u => u.createdPlan).Include(u =>u.ActivitiesToAttend).ThenInclude(p =>p.JoiningActivity).FirstOrDefault(u => u.UserId ==(int)logged_id);
                return View("activityDetail",ad);
            }

            return RedirectToAction("Home");
        }

        ///activity/delete/hapId")]
        [HttpGet("activity/delete/{hapId}")]
        public IActionResult DeleteActivity(int hapId){
            int? logged_id = HttpContext.Session.GetInt32("LoggedUser");
            if(logged_id == null){
                
                return RedirectToAction("Index");
            }
            Happening remHap = dbContext.Happenings.FirstOrDefault(h => h.HappeningId == hapId && h.CreatedById == (int)logged_id);
            if(remHap != null){
                dbContext.Happenings.Remove(remHap);
                dbContext.SaveChanges();
                return RedirectToAction("Home");
            }
            return RedirectToAction("Home");
        }

        ///activity/leave/@part.ParticipationId
        [HttpGet("activity/leave/{partId}")]
        public IActionResult LeaveParticipation(int partId){
            int? logged_id = HttpContext.Session.GetInt32("LoggedUser");
            if(logged_id == null){
                
                return RedirectToAction("Index");
            }
            if(!dbContext.Participations.Any(p => p.ParticipationId == partId && p.ParticipantId == (int)logged_id)){
                return RedirectToAction("Home");
            }
            Participation remPart = dbContext.Participations.FirstOrDefault(p =>p.ParticipationId == partId);
            if(remPart != null){
                dbContext.Remove(remPart);
                dbContext.SaveChanges();
            }
            
            return RedirectToAction("Home");
        }

        ///activity/join/@item.HappeningId"
        [HttpGet("activity/join/{hapId}")]
        public IActionResult JoinHappening(int hapId){
            int? logged_id = HttpContext.Session.GetInt32("LoggedUser");
            if(logged_id == null){
                
                return RedirectToAction("Index");
            }
            if (dbContext.Participations.Any(p => p.ParticipantId == (int)logged_id && p.JoiningActivityId == hapId)){
                return RedirectToAction("Home");
            }
            if (!dbContext.Happenings.Any(h=> h.HappeningId == hapId)){
                return RedirectToAction("Home");
            }

            Participation newP = new Participation();
            newP.ParticipantId = (int)logged_id;
            newP.JoiningActivityId = hapId;
            dbContext.Participations.Add(newP);
            dbContext.SaveChanges();
            return RedirectToAction("Home");
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
