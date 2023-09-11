using System;
using System.Linq;
 
namespace AppinnoNew
{

    partial class DataAccessDataContext
    {
        #region BestIdeaCompetition
        public enum BestIdeaCompetitionStatus
        {
            idle = 0, sending, voting, done
        }
        public IQueryable<bestIdeaCompetitionsTbl> getSendingBestIdeaCompetition()
        {
            var dt = new DateTime();
            dt = DateTime.Now;
            return this.bestIdeaCompetitionsTbls.Where(c => c.endDate >= dt && c.startDate < dt && c.isBlock == false);
        }
        public IQueryable<bestIdeaCompetitionsTbl> getVotingBestIdeaCompetition()
        {
            var dt = new DateTime();
            dt = DateTime.Now;
            return this.bestIdeaCompetitionsTbls.Where(c => c.resultVoteDate >= dt && c.endDate < dt && c.isBlock == false);
        }
        public IQueryable<bestIdeaCompetitionsTbl> getDoneBestIdeaCompetition()
        {
            var dt = new DateTime();
            dt = DateTime.Now;
            return this.bestIdeaCompetitionsTbls.Where(c => c.resultVoteDate <= dt && c.isBlock == false);
        }
        public BestIdeaCompetitionStatus getBestIdeaCompetitionStatus(long BestIdeaCompetitionID)
        {
            var dt = new DateTime();
            dt = DateTime.Now;

            var item = bestIdeaCompetitionsTbls.Single(c => c.ID == BestIdeaCompetitionID);
            if (dt <= item.startDate) return BestIdeaCompetitionStatus.idle;
            else if (item.startDate < dt && dt <= item.endDate) return BestIdeaCompetitionStatus.sending;
            else if (item.endDate < dt && dt <= item.resultVoteDate) return BestIdeaCompetitionStatus.voting;
            else return BestIdeaCompetitionStatus.done;
        }
        #endregion
         
        #region CreativityCompetition
        public enum CreativityCompetitionStatus
        {
            idle = 0, sending, pending, done
        }
        public IQueryable<creativityCompetitionTbl> getSendingCreativityCompetition()
        {
            var dt = new DateTime();
            dt = DateTime.Now;
            return creativityCompetitionTbls.Where(c => dt.Date >= c.startDate.Date && dt.Date <= c.endDate.Date && c.isDone == false && c.isBlock == false);
        }
        public IQueryable<creativityCompetitionTbl> getPendingCreativityCompetition()
        {
            var dt = new DateTime();
            dt = DateTime.Now;
            return creativityCompetitionTbls.Where(c => dt.Date > c.endDate.Date && c.isDone == false && c.isBlock == false);
        }
        public IQueryable<creativityCompetitionTbl> getDoneCreativityCompetition()
        {
            var dt = new DateTime();
            dt = DateTime.Now;
            return creativityCompetitionTbls.Where(c => dt.Date > c.endDate.Date && c.isDone == true && c.isBlock == false);
        }
        public CreativityCompetitionStatus getCreativityCompetitionStatus(long CreativityCompetitionID)
        {
            var dt = new DateTime();
            dt = DateTime.Now;

            var item = creativityCompetitionTbls.Single(c => c.ID == CreativityCompetitionID);
            if (dt <= item.startDate) return CreativityCompetitionStatus.idle;
            else if (item.startDate <= dt && dt <= item.endDate) return CreativityCompetitionStatus.sending;
            else if (item.endDate < dt && item.isDone == false) return CreativityCompetitionStatus.pending;
            else return CreativityCompetitionStatus.done;
        }
        #endregion
        
        #region MyIran
        public enum MyIranStatus
        {
            idle = 0, sending, pending, done
        }
        public IQueryable<myIranTbl> getSendingMyIran()
        {
            var dt = new DateTime();
            dt = DateTime.Now;
            return myIranTbls.Where(c => dt.Date >= c.startDate.Date && dt.Date <= c.endDate.Date && c.isDone == false && c.isBlock == false);
        }
        public IQueryable<myIranTbl> getPendingMyIran()
        {
            var dt = new DateTime();
            dt = DateTime.Now;
            return myIranTbls.Where(c => dt.Date > c.endDate.Date && c.isDone == false && c.isBlock == false);
        }
        public IQueryable<myIranTbl> getDoneMyIran()
        {
            var dt = new DateTime();
            dt = DateTime.Now;
            return myIranTbls.Where(c => dt.Date > c.endDate.Date && c.isDone == true && c.isBlock == false);
        }
        public MyIranStatus getMyIranStatus(long MyIranID)
        {
            var dt = new DateTime();
            dt = DateTime.Now;

            var item = myIranTbls.Single(c => c.ID == MyIranID);
            if (dt <= item.startDate) return MyIranStatus.idle;
            else if (item.startDate <= dt && dt <= item.endDate) return MyIranStatus.sending;
            else if (item.endDate < dt && item.isDone == false) return MyIranStatus.pending;
            else return MyIranStatus.done;
        }
        #endregion
    }
}