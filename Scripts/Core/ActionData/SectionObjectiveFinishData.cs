using System.Collections.Generic;

namespace Core.ActionData
{
    public class SectionObjectiveFinishData : BaseActionData
    {
        public int star;
        public int money;
        public SectionObjectiveFinishData(int _star, int _money)
        {
            star = _star;
            money = _money;
        }
    }
}