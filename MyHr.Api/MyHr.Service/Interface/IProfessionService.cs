using System;
using System.Collections.Generic;
using System.Text;

namespace MyHr.Service.Interface
{
    public interface IProfessionService
    {
        Task<String?> GetProfessionNameById(String professionId);
    }
}
