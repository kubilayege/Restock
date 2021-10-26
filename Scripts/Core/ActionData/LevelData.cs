namespace Core.ActionData
{
    public class LevelData : BaseActionData
    {
        public int TotalItemCount;
        
        public LevelData(int itemCount)
        {
            TotalItemCount = itemCount;
        }
    }
}