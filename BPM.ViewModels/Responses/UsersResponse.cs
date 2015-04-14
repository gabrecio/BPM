using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BPM.ViewModels.Responses
{
    public class UsersResponse
    {
        public UsersResponse()
        {
            users = new List<UserViewModel>();
        }

        public List<UserViewModel> users { get; set; }
        public int TotalUsers { get; set; }
    }
}