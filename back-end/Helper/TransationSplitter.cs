using back_end.Data;
using back_end.Models;

namespace back_end.Helper
{
    public static class TransactionSplitter
    {
        /// <summary>
        /// Splits a transaction amount among group members according to the split type and values.
        /// Updates the group's debt trackers accordingly.
        /// </summary>
        /// <param name="t">The transaction to split.</param>
        /// <param name="group">The group involved in the transaction.</param>
        /// <param name="payer">The user who paid.</param>
        /// <param name="db">The database context.</param>
        public static async Task Split(Transaction t, Group group, User payer, AppDbContext db)
        {
            // If not equal split, ensure split values match the number of group members
            if (t.SType != SplitType.Equal && (t.SplitValues == null || t.SplitValues.Length != group.Members.Count))
                return;

            // Calculate each member's charge based on the split type
            IEnumerable<(User member, decimal amount)> charges = t.SType switch
            {
                SplitType.Equal =>
                    group.Members
                         .Select(m => (m, Math.Round(t.Amount / group.Members.Count, 2))),

                SplitType.Percentage =>
                    group.Members
                         .Zip(t.SplitValues!, (m, pct) =>
                             (m, Math.Round(t.Amount * (pct / 100M), 2))),

                SplitType.Dynamic =>
                    group.Members
                         .Zip(t.SplitValues!, (m, amt) => (m, amt)),

                _ => Enumerable.Empty<(User, decimal)>()
            };

            // For each member charge, update debt trackers (skip payer)
            foreach (var (indebted, amount) in charges)
            {
                if (indebted.Id == payer.Id)
                    continue;

                await AddAmount(amount, group, payer, indebted, db);
            }
        }

        /// <summary>
        /// Adds an owed amount to the existing debt tracker or creates a new one if none exists.
        /// </summary>
        private static async Task AddAmount(decimal amount, Group group, User payer, User indebted, AppDbContext db)
        {
            // Find existing debt record from payer to indebted user
            var existingTracker = group.DebtTrackers
                .FirstOrDefault(dt => dt.FromUserId == payer.Id && dt.ToUserId == indebted.Id);

            // If not found, create a new debt tracker
            if (existingTracker == null)
                existingTracker = await MakeNewTrackerAsync(group, payer, indebted, db);

            existingTracker.Amount += amount;
        }

        /// <summary>
        /// Creates and saves a new debt tracker entry linking payer and indebted users in a group.
        /// </summary>
        private static async Task<DebtTracker> MakeNewTrackerAsync(Group group, User payer, User indebted, AppDbContext db)
        {
            var tracker = new DebtTracker
            {
                Group = group,
                GroupId = group.Id,
                FromUserId = payer.Id,
                FromUser = payer,
                ToUserId = indebted.Id,
                ToUser = indebted,
                Amount = 0.00m
            };

            db.DebtTrackers.Add(tracker);
            await db.SaveChangesAsync();

            return tracker;
        }
    }
}
