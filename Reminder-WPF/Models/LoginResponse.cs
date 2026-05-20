using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reminder_WPF.Models
{
    internal class LoginResponse
    {
        public required string token {  get; set; }
        public required string expiration { get; set; }
    }
}
