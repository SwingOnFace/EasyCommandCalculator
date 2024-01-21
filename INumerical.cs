using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCommandCalculator
{
    public interface INumerical
    {
        public string GenerateString();
        public INumerical Simplify();
    }
}
