using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BPM.ViewModels.Responses
{
    public class RolResponse
    {
        public RolResponse()
        {
            rols = new List<RoleViewModel>();
        }

        public List<RoleViewModel> rols { get; set; }
        public int TotalRoles { get; set; }
    }
}