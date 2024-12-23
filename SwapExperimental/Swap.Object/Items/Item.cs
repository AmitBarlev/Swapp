using Newtonsoft.Json;
using Swap.Object.Converters;
using Swap.Object.GeneralObjects;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Cryptography;

namespace Swap.Object.Items
{
    [JsonConverter(typeof(ItemJsonConverter))]
    [DataContract]
    public class Item 
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int IdCustomer { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Genre { get; private set; }
        [DataMember]
        public string Description { get; private set; }
        [DataMember]
        public ItemCondition Condition { get; private set; }
        [DataMember]
        public int Views { get; private set; }
        [DataMember]
        public DateTime UploadDate { get; set; }
        [DataMember]
        public ICollection<Image> ImagesOfItem { get; set; }
        [DataMember]
        public ICollection<Trade> Trades { get; set; }

        public User Owner { get; set; }


        public enum ItemCondition
        {
            New = 0,
            NewOther = 1,
            OpenBox = 2,
            Mint = 3,
            Used = 4,
            PartsOrNotWorking = 5
        }

        public virtual void Update(Item item, string folderPath)
        {
            Name = null == item.Name ? Name : item.Name; 
            Genre = item.Genre == null ? Genre : item.Genre;
            Description = item.Description == null ? Description : item.Description;
            Condition = item.Condition;
            ImagesOfItem = item.ImagesOfItem;
            SavePictures(folderPath);
        }

        public void SavePictures(string picturePath)
        {
            if (null == ImagesOfItem  || null == picturePath)
                return;

            foreach (Image imageToSave in ImagesOfItem)
            {
                if (null == imageToSave)
                    continue;

                imageToSave.SetForeignKeys(this, Owner);
                imageToSave.SavePicture(picturePath);
            }
        }

        public void LoadPictures()
        {
            if (null == ImagesOfItem)
                return;

            foreach (Image imageToLoad in ImagesOfItem)
            {
                if (null == imageToLoad)
                    continue;

                imageToLoad.LoadPicture();
            }
        }

        public void DeletePictures()
        {
            if (null == ImagesOfItem)
                return;

            foreach (Image image in ImagesOfItem)
            {
                image.DeletePicutre();
            }
        }

        public void IncView() => Views++;
    }
}
