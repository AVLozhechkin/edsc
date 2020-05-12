using MongoDB.Driver.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EDSc.Common.Utils
{
    public interface IImgToDbWriter<T>
    {
        string SaveToDb(T img);
    }
}
