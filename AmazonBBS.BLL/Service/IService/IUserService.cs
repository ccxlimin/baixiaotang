using AmazonBBS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.BLL
{
    public interface IUserService
    {
        UserSet GetUserSet(long uid);

        List<SignUserStudyInfo> GetSignUserStudyInfo(int skip, int take);

        /// <summary>
        /// 计算用户积分等级
        /// </summary>
        bool CalcUserLevel(long userId);
    }
}
