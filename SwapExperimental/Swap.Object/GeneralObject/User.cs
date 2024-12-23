using Swap.Object.ChatObjects;
using Swap.Object.Items;
using Swap.Object.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;

namespace Swap.Object.GeneralObjects
{
    [DataContract]
    public class User
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Email { get; private set; }
        [DataMember]
        public string Password { get; set; }
        [DataMember]
        public string FirstName { get; private set; }
        [DataMember]
        public string LastName { get; private set; }
        [DataMember]
        public string CellPhone { get; private set; }
        [DataMember]
        public string City { get; private set; }
        [DataMember]
        public string Token { get; private set; }
        [DataMember]
        public string FirebaseToken { get; private set; }
        [DataMember]
        public string ChatConnectionId { get; private set; }
        [DataMember]
        public double Latitude { get; private set; }
        [DataMember]
        public double Longitude { get; private set; }
        [DataMember]
        public DateTime SignUpDate { get; set; }

        [DataMember]
        [NotMapped]
        public string NewPassword { get; set; }

        [DataMember]
        public ICollection<Item> ItemsOfUser { get; set; }
        [DataMember]
        public ICollection<Image> Images { get; set; }
        [DataMember]
        public ICollection<Trade> Trades { get; set; }
        [DataMember]
        public ICollection<UserToGroup> Groups { get; set; }
        [DataMember]
        public ICollection<InstantMessage> Messages { get; set; }

        internal User() { }

        public void SetToken(string token) => Token = token;

        public void ClearSensitiveInformation()
        {
            ClearFirebaseToken();
            Password = null;
        }
        public void ClearToken() => Token = null;
        public void ClearFirebaseToken() => FirebaseToken = null;

        public void Update(User userDetailsToUpdate)
        {
            if (userDetailsToUpdate.NewPassword != null)
            {
                string passwordEnteredByUser = FormatTools.ComputePassword(userDetailsToUpdate.Email + userDetailsToUpdate.NewPassword);
                Password = passwordEnteredByUser != Password ? passwordEnteredByUser : Password;
            }
            FirstName = userDetailsToUpdate.FirstName == null ? FirstName : userDetailsToUpdate.FirstName;
            LastName = userDetailsToUpdate.LastName == null ? LastName : userDetailsToUpdate.LastName;
            CellPhone = userDetailsToUpdate.CellPhone == null ? CellPhone : userDetailsToUpdate.CellPhone;
            City = userDetailsToUpdate.City == null ? City : userDetailsToUpdate.City;
            FirebaseToken = userDetailsToUpdate.FirebaseToken ?? FirebaseToken;
            if (default(double) != userDetailsToUpdate.Longitude && default(double) != userDetailsToUpdate.Latitude)
                SetCoordinate(userDetailsToUpdate.Latitude, userDetailsToUpdate.Longitude);
        }

        public Image GetProfileImage() => Images.Where(i => i.Path.Contains("-profile")).FirstOrDefault();

        public void SetCoordinate(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public void SetConnectionId(string connectionId) => ChatConnectionId = connectionId;
    }
}
