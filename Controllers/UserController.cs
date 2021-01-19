using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EuroCollection.Models;
using EuroCollection.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EuroCollection.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        public UserController(UserService userService)
        {
            _userService = userService;
        }
        [HttpGet]
        public ActionResult<List<User>> Getusers() =>
            _userService.Get();

        // GET: api/Users/5
        [HttpGet("{id:length(24)}", Name = "GetUser")]
        public ActionResult<User> GetUser(string id)
        {
            var user = _userService.Get(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }
        // GET: api/Users/GetOwnData/id
        [HttpGet("GetOwnData/{id:length(24)}")]
        public ActionResult<User> GetOwnData(string id)
        {
            var user = _userService.Get(id);

            if (user == null)
            {
                return NotFound();
            }
            user.Password = null;
            return Ok(user);
        }
        // PUT: api/Users/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [Authorize(Roles = Role.Admin)]
        [HttpPatch("{id:length(24)}")]
        public IActionResult PutUser(string id, User user)
        {
            var userToSearch = _userService.Get(id);

            if (userToSearch == null)
            {
                return NotFound();
            }
            userToSearch.Role = user.Role;
            _userService.Update(id, userToSearch);

            return NoContent();
        }

        [HttpPatch("UpdateOwnData/{id:length(24)}")]
        public IActionResult UpdateOwnDate(string id, UserUpdate user)
        {
            var userToSearch = _userService.Get(id);

            if (userToSearch == null)
            {
                return NotFound();
            }
            if (user.OldPassword != null)
            {
                string hashedPasword = ComputeSHA(user.OldPassword);
                if (userToSearch.Password.Equals(hashedPasword))
                {
                    userToSearch.Email = user.Email;
                    userToSearch.UserName = user.UserName;
                    hashedPasword = ComputeSHA(user.NewPassword);
                    userToSearch.Password = hashedPasword;
                    _userService.Update(id, userToSearch);
                    return NoContent();
                }
                else return BadRequest();
            }
            else
            {
                userToSearch.Email = user.Email;
                userToSearch.UserName = user.UserName;
                _userService.Update(id, userToSearch);
                return NoContent();

            }


        }
        // POST: api/Users
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [AllowAnonymous]
        [HttpPost]
        public ActionResult<User> PostUser(User user)
        {

            _userService.Create(user);

            return CreatedAtRoute("GetUser", new { id = user.Id.ToString() }, user);
        }

        // DELETE: api/Users/5
        [Authorize(Roles = Role.Admin)]
        [HttpDelete("{id:length(24)}")]
        public ActionResult<User> DeleteUser(string id)
        {
            var user = _userService.Get(id);
            if (user == null)
            {
                return NotFound();
            }

            _userService.Remove(user.Id);
            return NoContent();
            //return user;
        }

        /*  private bool UserExists(int id)
         {
             return _userService.Users.Any(e => e.Id == id);
         }*/
        [AllowAnonymous]
        [HttpPost("signup")]
        public IActionResult SignUp([FromBody]AuthenticateModel model)
        {
            bool isEmail = Regex.IsMatch(model.Email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
            if (!isEmail)
            {
                return BadRequest(new { message = "InvalidEmail" });
            }
            string hashedPasword = ComputeSHA(model.Password);

            var user = _userService.SignUp(model.Username, hashedPasword, model.Email);
            if (user == null)
            {
                return BadRequest(new { message = "Username exists" });
            }

            return Ok(user);
        }
        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Authenticate([FromBody]AuthenticateModel model)
        {

            string hashedPasword = ComputeSHA(model.Password);
            var user = _userService.Authenticate(model.Email, hashedPasword);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(user);
        }
        private static string ComputeSHA(string pass)
        {
            string hash = pass;
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(hash));
                hash = "";
                foreach (var x in bytes)
                {
                    hash += string.Format("{0:x2}", x);
                }
            }
            return hash;
        }
    }
}
