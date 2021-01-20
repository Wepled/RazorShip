using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.ViewModels
{
    public class ShipVM
    {
        public ShipVM(Ship ship, int maxCount)
        {
            Ship = ship;
            MaxCount = maxCount;
        }
        public Ship Ship { get; set; }
        public int MaxCount { get; set; }
    }
}
