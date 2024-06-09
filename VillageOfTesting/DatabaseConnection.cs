using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VillageOfTesting.Interfaces;

namespace VillageOfTesting
{
    public class DatabaseConnection : IDatabasCoannection
    {
        // This class exists to be mocked using Moq.
        // It should not be changed.

        public List<string> GetTownNames()
        {
            return new List<string>() { "These", "are", "placeholders", "to", "make", "sure", "it", "works" };
        }

        public Village LoadVillage(string choice)
        {
            return null;
        }

        public virtual bool SaveVillage(Village village, string choice)
        {
            return false;
        }
    }
}
