﻿using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPM.Repositories.DataContext
{
   
    public interface IFrameworkEntities : IDisposable, IObjectContextAdapter
    {
    }
}
