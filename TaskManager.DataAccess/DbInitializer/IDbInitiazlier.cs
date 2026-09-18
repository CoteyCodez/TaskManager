using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManager.Data.DbInitializer
{
    public interface IDbInitializer
    {
        Task InitializeAsync();
    }
}
