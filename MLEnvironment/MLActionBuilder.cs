using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLEnvironment
{

    public class MLAction
    {
        public MLAction(string name, dynamic value)
        {
            this.Name = name;
            this.Value = value;
        }

        public string Name { get; set; }
        public dynamic Value { get; set; }


        public override string ToString()
        {
            return $"{Name}: {Value}";
        }
        public static MLAction DefaultValue(dynamic value) => new MLAction(nameof(DefaultValue), value);
        public static MLAction MaxValue(dynamic value) => new MLAction(nameof(MaxValue), value);
        public static MLAction MinValue(dynamic value) => new MLAction(nameof(MinValue), value);

        public static List<MLAction> BuildActionRange(dynamic defaultValue, dynamic minValue, dynamic maxValue) =>
            new List<MLAction>() { DefaultValue(defaultValue), MinValue(minValue), MaxValue(maxValue) };
    }
    public class MLActionSpace
    {
        public List<MLAction> Actions = new List<MLAction>();

        private static Random rnd= new Random();
        public MLActionSpace(string actionSpaceName, IEnumerable<MLAction> actions)
        {
            this.ActionSpaceName = actionSpaceName;
            this.Actions = actions.ToList();
            var minValue = Actions.Min(x => x.Value);
            var maxValue = Actions.Max(x => x.Value);
            ValueRange = new MLActionRange($"{ActionSpaceName}_ValueRange", minValue, maxValue);
            var minIndex = 0;
            var maxIndex = Actions.Count;
            IndexRange = new MLActionRange($"{ActionSpaceName}_ValueRange", minIndex, maxIndex);
        }

        private void SetRangeValues()
        {
            ValueRange.MinValue = Actions.Min(x => x.Value);
            ValueRange.MaxValue = Actions.Max(x => x.Value);
            IndexRange.MinValue = Math.Min(0, Actions.Count - 1);
            IndexRange.MinValue = Math.Max(-1, Actions.Count - 1);

        }
        public string ActionSpaceName { get; set; }
        public int ActionCount => Actions.Count;
        public MLActionRange ValueRange { get; }
        public MLActionRange IndexRange { get; }
        public void Add(MLAction action)
        {
            Actions.Add(action);
            SetRangeValues();
        }
        public MLAction Sample()
        {
            var nextIndex = rnd.Next((int)IndexRange.MinValue, (int)IndexRange.MaxValue);
            return Actions[nextIndex];
        }

    }
    public class MLActionRange
    {
        public MLActionRange(string name, dynamic minValue, dynamic maxValue)
        {
            this.Name = name;
            MinValue = minValue;
            MaxValue = maxValue;
        }

        public string Name { get; set; }
        public dynamic MinValue { get; set; }
        public dynamic MaxValue { get; set; }
    }
    public class MLIntActionGenerator
    {
        public static MLActionSpace BuildIntActionSpace()
        {
            var actions = MLAction.BuildActionRange(0, int.MinValue, int.MaxValue);
            for (var i = 0; i < 32; i++)
            {
                actions.Add(new MLAction($"bit_{i}", 1 << i));
            }
            MLActionSpace MLIntActionSpace = null;
            MLIntActionSpace = new MLActionSpace(nameof(MLIntActionSpace), actions);
            return MLIntActionSpace;
        }
    }


}
