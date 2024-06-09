using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VillageOfTesting.Interfaces
{
    public interface IDatabasCoannection
    {
        List<string> GetTownNames();
        Village LoadVillage(string choice);
        bool SaveVillage(Village village, string choice);

       
    }
}
