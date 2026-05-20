using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reminder_WPF.Models
{
    public class CredentialsModel
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required bool Save {  get; set; }  
    }
}
