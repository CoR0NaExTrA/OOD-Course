using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beverages;
public interface IBeverage
{
    string GetDescription();
    double GetCost();
}
