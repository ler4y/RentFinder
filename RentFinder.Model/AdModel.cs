﻿using System.Collections.Generic;

namespace RentFinder.Model
{
    public class AdModel: IAddModel
    {
        public AdModel()
        {
            PhoneNumbers = new List<string>();
        }

        public string TempId { get; set; }
        public uint AdId { get; set; }
        public string Address { get; set; }
        public string Area => Address.Split(',')[2];
        public string Title => $"{Area} {Price} комнат {Rooms}";
        public string Link { get; set; }
        public List<string> PhoneNumbers { get; private set; }
        public double Price { get; set; }
        public bool IsPrivate { get; set; }
        public int Rooms { get; set; }
    }
}
