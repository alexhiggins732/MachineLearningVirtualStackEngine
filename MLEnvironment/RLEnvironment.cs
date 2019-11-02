using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLEnvironment
{
    public class RLEnvironment
    {
        public MLActionSpace ActionSpace { get; private set; }
        public MLAgent agent { get; private set; }
        public dynamic Target { get; private set; }
        public RLEnvironment(MLActionSpace space)
        {
            this.ActionSpace = space;
            var agent = new MLAgent(this);
        }

        internal void Reset()
        {


        }

        internal void SetTarget(dynamic target)
        {
            this.Target = target;
        }


        internal EnviromentStep Step()
        {
            var observeration = new EnviromentStep();
     
            observeration.Action = agent.NextAction();
            observeration.Info = new MLStepInfo();
            return observeration;
        }
    }
    public class EnviromentStep
    {
        public MLAction Action { get; set; }
        public MLObserveration Observeration { get; set; }
        public MLReward Reward { get; set; }
        public MLStepInfo Info { get; set; }
    }

    public class MLStepInfo : Dictionary<string, dynamic>
    {
    }

    public class MLReward
    {
    }

    public class MLObserveration
    {

    }
    public class MLAgent
    {
        private RLEnvironment mLEnvironment;

        public MLAgent(RLEnvironment mLEnvironment)
        {
            this.mLEnvironment = mLEnvironment;
        }

        internal MLAction NextAction()
        {
            throw new NotImplementedException();
        }

        internal void Train()
        {
            // take action based on previously learned knowledge;
            //  if no previous learned knownledge make it random. can choose, random, genetic, neural, etc.
            // take and observation;
            // update learning table;
        }
    }
}
