using AmazonBBS.Common;
using AmazonBBS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonBBS.BLL
{
    public class UserService : IUserService
    {
        private readonly AmazonBBSDBContext _amazonBBSDBContext;

        public UserService(AmazonBBSDBContext amazonBBSDBContext)
        {
            _amazonBBSDBContext = amazonBBSDBContext;
        }

        public bool CalcUserLevel(long userId)
        {
            bool ok = false;
            //计算积分等级
            List<BBSEnum> levels = BBSEnumBLL.Instance.Query(3, true);
            var coinSource = CoinSourceEnum.Sign.GetHashCode();

            //select UserID, LevelName,
            // (select count(1) from ScoreCoinLog where UserID = a.UserID and CoinSource = 3) SignCount
            // from userext a where a.UserID = @uid

            //SignCountAndLevel userext = UserExtBLL.Instance.GetSignAndLevelByUserID(Tran, UserID);
            UserExt userExt = _amazonBBSDBContext.UserExt.FirstOrDefault(a => a.UserID == userId);
            int signCount = _amazonBBSDBContext.ScoreCoinLog.Count(t => t.UserID == userId && t.CoinSource == coinSource);
            //string sqlTemp = "update UserExt set LevelName={0} where UserID={1};";
            //string empty = "null";
            //StringBuilder sb = new StringBuilder();
            var tempLevel = levels.LastOrDefault(item => { return item.SortIndex <= signCount; });
            if (tempLevel == null)
            {
                if (userExt.LevelName.HasValue)
                {
                    //sb.Append(sqlTemp.FormatWith(empty, userext.UserID.ToString()));
                    userExt.LevelName = null;
                }
            }
            else
            {
                if (tempLevel.BBSEnumId != userExt.LevelName)
                {
                    //sb.Append(sqlTemp.FormatWith(tempLevel.BBSEnumId.ToString(), userext.UserID.ToString()));
                    userExt.LevelName = tempLevel.BBSEnumId;
                }
            }
            ok = true;
            //if (sb.ToString().IsNotNullOrEmpty())
            //{
            //    if (SqlHelper.ExecuteSql(Tran, System.Data.CommandType.Text, sb.ToString(), null) > 0)
            //    {
            //        ok = true;
            //    }
            //}
            //else
            //{
            //    ok = true;
            //}
            return ok;
        }

        #region 获取签到
        /// <summary>
        /// 获取签到
        /// </summary>
        /// <returns></returns>
        public List<SignUserStudyInfo> GetSignUserStudyInfo(int skip, int take)
        {
            var date = DateTime.Now;
            var monthFrom = date.AddDays(-date.Day + 1).Date;

            var source = CoinSourceEnum.Sign.GetHashCode();

            //获取今日签到的 标签会员
            var studyUsers = _amazonBBSDBContext.UserExt.Where(a => a.OnlyLevelName.HasValue)
                .Join(_amazonBBSDBContext.ScoreCoinLog.Where(a => a.CoinSource == source && a.CoinTime >= date.Date && a.CoinTime <= date).OrderByDescending(a => a.CoinTime), a => a.UserID, b => b.UserID, (a, b) => b)
                .ToList()
                .Skip(skip).Take(take).ToList();

            List<SignUserStudyInfo> studies = studyUsers.Select(a => new SignUserStudyInfo
            {
                UserName = a.UserName,
                Uid = a.UserID,
                SignCountMonth = _amazonBBSDBContext.ScoreCoinLog.Count(sign => sign.CoinTime >= monthFrom && sign.CoinTime <= date && sign.UserID == a.UserID && sign.CoinSource == source),
                SignCountTotal = _amazonBBSDBContext.ScoreCoinLog.Count(sign => sign.CoinSource == source && sign.UserID == a.UserID),
                CurrentStudyInfo = _amazonBBSDBContext.UserStudy.Where(s => s.UserID == a.UserID && !s.IsStudyed).OrderByDescending(s => s.CreateTime).FirstOrDefault(),
                //StudyClassName = _amazonBBSDBContext.UserStudy.Where(s => s.UserID == a.UserID).OrderByDescending(s => s.CreateTime).DefaultIfEmpty()
                //.Join(_amazonBBSDBContext.StudyClass.Where(s => s.IsDelete == 0), study => study.StudyClassId, _class => _class.StudyClassId, (study, _class) => _class)
                //.Join(_amazonBBSDBContext.StudyClass.Where(s => s.IsDelete == 0), s1 => s1.StudyUnitId, s2 => s2.StudyUnitId, (s1, s2) => s2).ToList()
                //StudyClasses = _amazonBBSDBContext.UserStudy.Where(s => s.UserID == a.UserID).OrderByDescending(s => s.CreateTime).DefaultIfEmpty()
                //.Join(_amazonBBSDBContext.StudyClass.Where(s => s.IsDelete == 0), s1 => s1.StudyUnitId, s2 => s2.StudyUnitId, (s1, s2) => s2).OrderBy(c => c.SortIndex).Select(c => c.StudyClassId).ToList()
            }).ToList();

            var allStudyClass = _amazonBBSDBContext.StudyClass.Where(a => a.IsDelete == 0).GroupBy(a => a.StudyUnitId).ToList();

            studies.ForEach(a =>
            {
                //处理进度
                if (a.CurrentStudyInfo == null)
                {
                    //判断是否有学完的课程
                    var studyFinish = _amazonBBSDBContext.UserStudy.Where(finish => finish.UserID == a.Uid && finish.IsStudyed).ToList();
                    if (studyFinish.Count > 0)
                    {
                        //var _studyFinishInfo = studyFinish.FirstOrDefault();
                        var _studyFinishClassIds = studyFinish.Select(finish => finish.StudyUnitId).ToList();
                        var classes = allStudyClass.FirstOrDefault(cs => _studyFinishClassIds.Contains(cs.Key)).OrderBy(cs => cs.SortIndex).Select(cs => cs).ToList();
                        var lastClassId = classes.LastOrDefault().StudyClassId;
                        var classids = classes.Select(cs => cs.StudyClassId).ToList();
                        var _index = classids.IndexOf(lastClassId);
                        a.StudyRate = ((decimal)(_index + 1) / classids.Count).ToString("0.00%");
                        a.CurrentStudyInfo = studyFinish.FirstOrDefault(finish => finish.StudyClassId == lastClassId);
                        a.CurrentStudyClassInfo = classes[_index];
                    }
                    else
                    {
                        a.StudyRate = "0.00%";
                    }
                }
                else
                {
                    var classes = allStudyClass.FirstOrDefault(cs => cs.Key == a.CurrentStudyInfo.StudyUnitId).OrderBy(cs => cs.SortIndex).Select(cs => cs).ToList();
                    var classids = classes.Select(cs => cs.StudyClassId).ToList();
                    //a.StudyRate = (a.StudyClassName.Select(s => s.StudyClassId).ToList().IndexOf(a.CurrentStudyInfo.StudyClassId) / a.StudyClassName.Count).ToString("0.00%");
                    var _index = classids.IndexOf(a.CurrentStudyInfo.StudyClassId);
                    a.StudyRate = ((decimal)_index / classids.Count).ToString("0.00%");
                    a.CurrentStudyClassInfo = classes[_index];
                }
            });
            //.Join(_amazonBBSDBContext.UserStudy.OrderByDescending(a => a.CreateTime).DefaultIfEmpty(), a => a.UserID, b => b.UserID, (a, b) => new { a, b }).ToList();
            return studies;
        }
        #endregion

        #region 获取用户设置
        public UserSet GetUserSet(long uid)
        {
            if (uid == 0)
            {
                return new UserSet();
            }
            var set = _amazonBBSDBContext.UserSet.FirstOrDefault(a => a.UserId == uid && a.IsDelete == 0);
            if (set == null)
            {
                set = new UserSet
                {
                    IsDelete = 0,
                    UserId = uid,
                    CreateTime = DateTime.Now,
                    ShowOrHideBaseInfo = true
                };
                _amazonBBSDBContext.UserSet.Add(set);
                _amazonBBSDBContext.SaveChanges();
            }
            return set;
        }
        #endregion



    }
}