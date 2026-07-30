using System;
using System.Collections.Generic;
using System.Text;

namespace MyHr.Service.Interface
{
    public interface IPositionService
    {
        Task<String?> GetPositionNameById(String positionId);
    }
}
