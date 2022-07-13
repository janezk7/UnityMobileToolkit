using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets._Scripts.Classes
{
    public class ApiResponse
    {
        public object Data;
        public string ErrorMessage;
        public bool Ok { get { return ErrorMessage == null; } }
    }
}
