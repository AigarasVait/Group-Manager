using back_end.Models;

namespace back_end.Helper
{
    public static class BalanceCalculator
    {
        public static decimal Personal(Group group, int? userId, int? otherId)
        {
            if (group == null || userId == null || otherId == null)
            {
                throw new ArgumentNullException("Group or Users cannot be null.");
            }

            decimal totalOwed = group.DebtTrackers
                .Where(dt => dt.FromUserId == userId && dt.ToUserId == otherId)
                .Sum(dt => dt.Amount);

            decimal totalDebt = group.DebtTrackers
                .Where(dt => dt.ToUserId == userId && dt.FromUserId == otherId)
                .Sum(dt => dt.Amount);

            return totalOwed - totalDebt;
        }

        public static decimal Total(Group group, int userId)
        {
            decimal Total = 0;
            foreach (var member in group.Members)
            {
                if (member.Id == userId) continue;
                Total += Personal(group, userId, member.Id);
            }
            return Total;
        }
    }
}