using AmazonBBS.Common;
using AmazonBBS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.BLL
{
    public class CacheBLL
    {
        public static CacheBLL Instance
        {
            get { return SingleHepler<CacheBLL>.Instance; }
        }

        #region 首页标签固定个数
        public void Set_TagFixedNumber(int number)
        {
            CSharpCacheHelper.Set(APPConst.TagFixedNumber, number, APPConst.ExpriseTime.Month1);
        }

        public int Get_TagFixedNumber()
        {
            var val = CSharpCacheHelper.Get(APPConst.TagFixedNumber);
            if (val == null)
            {
                var config = ConfigHelper.AppSettings(APPConst.TagFixedNumber).ToInt32();
                Set_TagFixedNumber(config);
                return config;
            }
            else
            {
                return Convert.ToInt32(val);
            }
        }
        #endregion

        #region 首页标签随机展示个数
        public void Set_TagRandomNumber(int number)
        {
            CSharpCacheHelper.Set(APPConst.TagRandomNumber, number, APPConst.ExpriseTime.Month1);
        }

        public int Get_TagRandomNumber()
        {
            var val = CSharpCacheHelper.Get(APPConst.TagRandomNumber);
            if (val == null)
            {
                var config = ConfigHelper.AppSettings(APPConst.TagRandomNumber).ToInt32();
                Set_TagRandomNumber(config);
                return config;
            }
            else
            {
                return Convert.ToInt32(val);
            }
        }
        #endregion

        #region 首页右侧公告显示条数
        public void Set_NewShowCount(int number)
        {
            CSharpCacheHelper.Set(APPConst.NewShowCount, number, APPConst.ExpriseTime.Month1);
        }

        public int Get_NewShowCount()
        {
            var val = CSharpCacheHelper.Get(APPConst.NewShowCount);
            if (val == null)
            {
                var config = ConfigHelper.AppSettings(APPConst.NewShowCount).ToInt32();
                Set_NewShowCount(config);
                return config;
            }
            else
            {
                return Convert.ToInt32(val);
            }
        }
        #endregion

        #region 鼠标点击提示语
        public void Add_ClickMsg(ClickMsg model)
        {
            var list = ClickMsgBLL.Instance.FindALL();
            list.Add(model);
            CSharpCacheHelper.Set(APPConst.ClickMsgs, list, APPConst.ExpriseTime.Week1);
        }

        public void Delete_ClickMsg(ClickMsg model)
        {
            var list = ClickMsgBLL.Instance.FindALL();
            CSharpCacheHelper.Set(APPConst.ClickMsgs, list.Where(a => { return a.ClickMsgId == model.ClickMsgId; }).ToList(), APPConst.ExpriseTime.Week1);
        }

        public void Update_ClickMsg(ClickMsg model)
        {
            var list = ClickMsgBLL.Instance.FindALL();
            var newlist = list.Where(a => { return a.ClickMsgId != model.ClickMsgId; }).ToList();
            newlist.Add(model);
            CSharpCacheHelper.Set(APPConst.ClickMsgs, newlist, APPConst.ExpriseTime.Week1);
        }
        #endregion
    }
}
