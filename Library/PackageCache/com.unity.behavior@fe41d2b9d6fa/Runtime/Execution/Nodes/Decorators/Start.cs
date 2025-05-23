using System;
using Unity.Properties;
using UnityEngine;

namespace Unity.Behavior
{
    /// <summary>
    /// The root of a behaviour graph. Starts the behavior Graph.
    /// </summary>
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "On Start", 
        description: "The root of a behaviour graph.", 
        category: "Events", 
        id: "jf0ecb9b9f44492eb96f0442454949ao")]
    internal partial class Start : Modifier
    {
        /// <summary>
        /// If true, the graph will restart when all nodes completes.
        /// </summary>
        [SerializeReference] public bool Repeat;
        public bool AllowMultipleRepeatsPerTick = false;

        private int m_CurrentFrame;
        [CreateProperty] private int m_FrameDelta;

        /// <inheritdoc cref="OnStart" />
        protected override Status OnStart()
        {
            m_CurrentFrame = Time.frameCount;
            if (Child == null)
            {
                return Status.Success;
            }
                    
            var status = StartNode(Child);
            if (status == Status.Failure || status == Status.Success)
            {
                if (!Repeat)
                {
                    return status;
                }                
                return Status.Running;
            }
                
            return Status.Waiting;
        }

        /// <inheritdoc cref="OnUpdate" />
        protected override Status OnUpdate()
        {
            if (!AllowMultipleRepeatsPerTick && m_CurrentFrame == Time.frameCount)
            {
                return Status.Running;
            }
            m_CurrentFrame = Time.frameCount;
            Status status = Child.CurrentStatus;
            if (status == Status.Failure || status == Status.Success)
            {
                if (!Repeat)
                {
                    return status;
                }

                var newStatus = StartNode(Child);
                if (newStatus == Status.Failure || newStatus == Status.Success)
                {
                    return Status.Running;
                }
            }
            return Status.Waiting;
        }

        protected override void OnDeserialize()
        {
            m_CurrentFrame = Time.frameCount + m_FrameDelta;
        }

        protected override void OnSerialize()
        {
            m_FrameDelta = Time.frameCount - m_CurrentFrame; 
        }
    }
}
