using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reminder_WPF.Models
{
    internal class LoginResponse
    {
        public string token {  get; set; }
        public string expiration { get; set; }
    }
}
