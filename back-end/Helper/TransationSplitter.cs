using back_end.Data;
using back_end.Models;

namespace back_end.Helper
{
    public static class TransactionSplitter
    {
        public static async Task Split(Transaction t, Group group, User payer, AppDbContext db)
        {
            if (t.SType != SplitType.Equal && (t.SplitValues == null || t.SplitValues.Length != group.Members.Count))
                return;

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

            foreach (var (indebted, amount) in charges)
            {
                if (indebted.Id == payer.Id) continue;
                await AddAmount(amount, group, payer, indebted, db);
            }
        }

        private static async Task AddAmount(decimal amount, Group group, User payer, User indebted, AppDbContext db)
        {
            var existingTracker = group.DebtTrackers
            .FirstOrDefault(dt => dt.FromUserId == payer.Id && dt.ToUserId == indebted.Id);
            existingTracker ??= await MakeNewTrackerAsync(group, payer, indebted, db);

            existingTracker.Amount += amount;
        }
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
