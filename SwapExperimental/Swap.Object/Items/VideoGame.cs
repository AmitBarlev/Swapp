using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Swap.Object.Items
{
    [DataContract]
    public class VideoGame : Item
    {
        [DataMember]
        public GamingPlatform Platform { get; set; }

        internal VideoGame() : base()
        {
        }

        public override void Update(Item videoGame, string folderPath)
        {
            base.Update(videoGame,folderPath);
            Platform = ((VideoGame)videoGame).Platform;
        }

        public enum GamingPlatform : int
        {
            PC = 0,
            Playstation4 = 1,
            XboxOne = 2,
            Switch = 3,
            N3DS = 4,
            Xbox360 = 5,
            Playstation3 = 6,
            Other = 7
        }

    }
}
