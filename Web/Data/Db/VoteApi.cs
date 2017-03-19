using System;

namespace Data
{
    public class VoteApi : DbApi
    {
        public static int Vote(int contentId, int userId, int direction)
        {
            if (contentId == 0)
                throw new ArgumentException(nameof(contentId));

            if (userId == 0)
                throw new ArgumentException(nameof(userId));

            if (Math.Abs(direction) != 1)
                throw new Exception("Invalid direction value");

            var exists = (int)Execute($"SELECT COUNT(*) FROM [dbo].[Vote] WHERE [ContentId] = {contentId} AND [UserId] = {userId}") == 1;

            if (exists)
            {
                Execute($"UPDATE [Vote] SET [Direction] = {direction} WHERE [ContentId] = {contentId} AND [UserId] = {userId}");
            }
            else
            {
                Execute($@"INSERT INTO [dbo].[Vote]
                   ([ContentId]
                   ,[UserId]
                   ,[Direction])
                VALUES
                   ({contentId}, {userId}, {direction})");
            }

            var score = Select(contentId);
            ContentApi.UpdateScore(contentId, score);
            return score;
        }

        public static int Select(int contentId)
        {
            var result = Execute($"SELECT SUM(Direction) FROM [Vote] WHERE [ContentId] = {contentId}");
            if (result == DBNull.Value)
                return 0;
            return (int)result;
        }
    }
}
