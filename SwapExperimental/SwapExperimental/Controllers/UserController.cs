using GeoCoordinatePortable;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Swap.Object.ChatObjects;
using Swap.Object.GeneralObjects;
using Swap.Object.Tools;
using Swap.WebApi.Repository;
using Swap.WebApi.Services;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Swap.Controllers
{
    public class UserController : Controller
    {
        private Database _database;
        private IConfiguration _configuration = null;

        private readonly string _authServerAddress = null;
        private readonly string _pictureFolder = null;
        private const string _idKey = "id";

        public UserController(Database database, IConfiguration configuration)
        {
            string addresses = "Addresses";
            _database = database;
            _configuration = configuration;
            _authServerAddress = configuration.GetSection(addresses).GetValue<string>("AuthServer");
            _pictureFolder = configuration.GetSection(addresses).GetValue<string>("PicturePath");
        }

        [HttpPost]
        public IActionResult Signup([FromBody] User newUser)
        {
            if (null == newUser?.Email || null != _database.UserTable.Get(user => user.Email == newUser.Email))
                return BadRequest();

            newUser.Password = FormatTools.ComputePassword(newUser.Email + newUser.Password);
            newUser.SignUpDate = DateTime.Now;
            _database.UserTable.Add(newUser);

            using (ApiHttpClient http = new ApiHttpClient(_authServerAddress, _configuration))
            {
                newUser.SetToken(http.GetAccessToken(newUser));
                _database.UserTable.Update(newUser);
                http.SendPushNotification(newUser, "Signup");
            }

            return Ok(new { newUser.Id, newUser.Token });
        }

        [HttpPost]
        public IActionResult Login([FromBody] User userDetails)
        {
            const string profilePictureIdentifer = "-profile";
            User userToLogin = _database.UserTable.Get(user => user.Email == userDetails.Email &&
            user.Password == FormatTools.ComputePassword(userDetails.Email + userDetails.Password));

            if (null == userToLogin)
                return BadRequest();
            else
            {
                using (ApiHttpClient http = new ApiHttpClient(_authServerAddress))
                {
                    userToLogin.SetToken(http.GetAccessToken(userToLogin));
                    _database.UserTable.Update(userToLogin);
                }
            }

            if (userToLogin.Images != null && userToLogin.Images.Any(i => i.Path.Contains(profilePictureIdentifer)))
            {
                Image profilePicture = userToLogin.Images.Where(i => i.Path.Contains(profilePictureIdentifer)).FirstOrDefault();
                profilePicture?.LoadPicture();
                userToLogin.Images = new Image[] { profilePicture };
            }
            userToLogin.ClearSensitiveInformation();
            return Ok(userToLogin);
        }

        [Authorize]
        [HttpPost]
        public IActionResult UpdateProfile([FromBody] User newUserDetails)
        {
            User userToUpdate = _database.UserTable.Get(newUserDetails.Id);
            if (null == userToUpdate)
                return BadRequest();

            userToUpdate.Update(newUserDetails);
            Image currentProfileImageOfUserToUpdate = userToUpdate.GetProfileImage();
            if (null != newUserDetails.Images)
            {
                if (null != currentProfileImageOfUserToUpdate)
                {
                    currentProfileImageOfUserToUpdate.DeletePicutre();
                    _database.ImageTable.Remove(currentProfileImageOfUserToUpdate);
                }
                Image newProfilePicture = newUserDetails.Images?.First();
                newProfilePicture.SavePicture(_pictureFolder, true);
                userToUpdate.Images.Add(newProfilePicture);
            }
            _database.UserTable.Update(userToUpdate);
            return Ok();
        }

        [Authorize]
        [HttpPost]
        public IActionResult UpdateCoordinate([FromBody] User user)
        {
            User userToUpdate = _database.UserTable.Get(u => u.Id == user.Id);
            if (null == userToUpdate)
                return BadRequest();

            userToUpdate.SetCoordinate(user.Latitude, user.Longitude);
            _database.UserTable.Update(userToUpdate);
            return Ok();
        }

        [Authorize]
        [HttpGet]
        public IActionResult Logout()
        {
            if (!Request.TryGetToken(out string token))
                return BadRequest();

            User userToLogout = _database.UserTable.Get(user => user.Token == token);
            if (null == userToLogout)
                return BadRequest();

            userToLogout.ClearToken();
            _database.UserTable.Update(userToLogout);
            return Ok();
        }

        [HttpGet]
        public IActionResult GetUser()
        {
            if (!Request.TryGetValue(_idKey, out int id))
                return BadRequest();

            User user = _database.UserTable.Get(u => u.Id == id);
            if (null == user)
                return BadRequest();

            Image profilePicture = user.GetProfileImage();
            profilePicture?.LoadPicture();
            if (null != profilePicture)
                user.Images = new Image[] { profilePicture };
            else
                user.Images = null;
            user.ClearSensitiveInformation();
            user.ClearToken();
            return Ok(user);
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetDistance()
        {
            NameValueCollection parameters = HttpUtility.ParseQueryString(Request.QueryString.Value);
            if (!parameters.TryGetValue(_idKey, out int userId) || !parameters.TryGetValue("id2", out int userIdToCheckDistanceFrom))
                return BadRequest();

            User user = _database.UserTable.Get(u => u.Id == userId);
            User ownerOfItem = _database.UserTable.Get(u => u.Id == userIdToCheckDistanceFrom);
            if (null == user || null == ownerOfItem)
                return BadRequest();

            GeoCoordinate coordinateOfItemOwner = new GeoCoordinate(ownerOfItem.Latitude, ownerOfItem.Longitude);
            GeoCoordinate coordinateOfUser = new GeoCoordinate(user.Latitude, user.Longitude);
            return Ok(coordinateOfUser.GetDistanceTo(coordinateOfItemOwner));
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetMessages()
        {
            NameValueCollection parameters = HttpUtility.ParseQueryString(Request.QueryString.Value);
            if (!parameters.TryGetValue(_idKey, out int userId) || !parameters.TryGetValue("timeStamp", out string time) ||
                !long.TryParse(time, out long ticks))
                return BadRequest();

            DateTime dateTime = new DateTime(ticks);
            IEnumerable<Chat> chatsOfUser = _database.UserToGroupTable.GetAll(u => u.UserId == userId).Select(u => u.Chat);
            IEnumerable<InstantMessage> messagesOfUser = chatsOfUser.SelectMany(c => c.Messages).Where(m => dateTime < m.DateTime);

            return Ok(messagesOfUser);
        }
    }
}
