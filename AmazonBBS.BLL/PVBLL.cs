using AmazonBBS.Common;
using AmazonBBS.DAL;
using AmazonBBS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.BLL
{
    public class PVBLL
    {
        PVDAL dal = new PVDAL();

        public static PVBLL Instance
        {
            get { return SingleHepler<PVBLL>.Instance; }
        }

        /// <summary>
        /// 记录浏览量
        /// </summary>
        /// <returns></returns>
        public bool RecordPVCount(PVTableEnum pvEnum, long id)
        {
            return dal.RecordPVCount(pvEnum.ToString(), id);
        }

        public bool EditPVCount(PVTableEnum pvEnum, long id, long number)
        {
            return dal.EditPVCount(pvEnum.ToString(), id, number);
        }
    }
}
