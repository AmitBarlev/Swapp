using Swap.Object.Items;
using Swap.Object.Tools;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Runtime.Serialization;
using System.Security.Cryptography;

namespace Swap.Object.GeneralObjects
{
    [DataContract]
    public class Image
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Path { get; set; }
        [NotMapped]
        [DataMember]
        public string BytesOfImage{ get; set; }

        public int? UserId { get; set; }
        public User User { get; set; }
        public int? ItemId { get; set; }
        public Item Item { get; set; }

        internal Image()
        {
        }

        public void SetForeignKeys(Item item, User user)
        {
            User = user;
            Item = item;
        }

        public void SavePicture(string parentFolder, bool isProfilePicture = false)
        {
            string saveTo = System.IO.Path.Combine(parentFolder, FormatTools.ComputePictureName(BytesOfImage));
            saveTo = isProfilePicture ? saveTo + "-profile" : saveTo;
            using (FileStream imageFile = new FileStream(saveTo, FileMode.Create))
            {
                byte[] bytesOfImage = Convert.FromBase64String(BytesOfImage);
                imageFile.Write(bytesOfImage, 0, bytesOfImage.Length);
            }
            Path = saveTo;
        }

        public void LoadPicture()
        {
            using (FileStream currentPhoto = File.OpenRead(Path))
            {
                byte[] bytesOfImage = new byte[currentPhoto.Length];
                currentPhoto.Read(bytesOfImage, 0, bytesOfImage.Length);
                BytesOfImage = Convert.ToBase64String(bytesOfImage);
            }
        }

        public void DeletePicutre() => File.Delete(Path);
    }
}
