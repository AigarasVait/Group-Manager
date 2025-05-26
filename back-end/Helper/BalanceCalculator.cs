using back_end.Models;

namespace back_end.Helper
{
    public static class BalanceCalculator
    {
        /// <summary>
        /// Calculates the net balance between two users in a group.
        /// Positive value means `userId` is owed money by `otherId`.
        /// Negative value means `userId` owes money to `otherId`.
        /// </summary>
        public static decimal Personal(Group group, int? userId, int? otherId)
        {
            if (group == null || userId == null || otherId == null)
                throw new ArgumentNullException("Group or Users cannot be null.");

            // Sum of what userId is owed by otherId
            decimal owedToUser = group.DebtTrackers
                .Where(dt => dt.FromUserId == userId && dt.ToUserId == otherId)
                .Sum(dt => dt.Amount);

            // Sum of what userId owes to otherId
            decimal owedByUser = group.DebtTrackers
                .Where(dt => dt.ToUserId == userId && dt.FromUserId == otherId)
                .Sum(dt => dt.Amount);

            // Net position between the two
            return owedToUser - owedByUser;
        }

        /// <summary>
        /// Calculates the user's total balance in the group.
        /// This is the net result across all members.
        /// </summary>
        public static decimal Total(Group group, int userId)
        {
            decimal total = 0;

            foreach (var member in group.Members)
            {
                if (member.Id == userId)
                    continue;

                // Add net balance between user and each member
                total += Personal(group, userId, member.Id);
            }

            return total;
        }
    }
}
