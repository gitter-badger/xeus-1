using System;
using System.Diagnostics;
using Omnix.Base;
using Omnix.Collections;
using Omnix.Cryptography;
using Omnix.Network;
using Xeus.Messages;
using Xeus.Messages.Reports;

namespace Xeus.Core.Exchange.Internal
{
    internal sealed class SessionStatus
    {
        public SessionStatus()
        {
            var random = RandomProvider.GetThreadRandom();

            this.DecrementAtHopLimitMaximum = (random.Next(0, int.MaxValue) % 2 == 0);
            this.DecrementAtHopLimitMinimum = (random.Next(0, int.MaxValue) % 2 == 0);
        }

        public ProtocolVersion Version { get; set; }
        public SessionType Type { get; set; }
        public int ThreadId { get; set; }

        public byte[] Id { get; set; }
        public OmniAddress Address { get; set; }

        public PriorityManager Priority { get; } = new PriorityManager(new TimeSpan(0, 10, 0));

        public SendInfo Send { get; } = new SendInfo();
        public ReceiveInfo Receive { get; } = new ReceiveInfo();

        public bool DecrementAtHopLimitMaximum { get; }
        public bool DecrementAtHopLimitMinimum { get; }

        public DateTime CreationTime { get; } = DateTime.UtcNow;

        public const int MaxLocationCount = 256;
        public const int MaxWantBroadcastClueCount = 256;
        public const int MaxWantUnicastClueCount = 256;
        public const int MaxWantMulticastClueCount = 256;
        public const int MaxBroadcastClueCount = 256;
        public const int MaxUnicastClueCount = 256;
        public const int MaxMulticastClueCount = 256;
        public const int MaxPublishBlockCount = 256;
        public const int MaxWantBlockCount = 256;

        public void Update()
        {
            this.Priority.Update();
            this.Send.Update();
            this.Receive.Update();
        }

        public sealed class SendInfo
        {
            internal SendInfo() { }

            public bool IsInitialized { get; set; }

            public StopwatchInfo Stopwatch { get; } = new StopwatchInfo();
            public FiltersInfo Anchor { get; } = new FiltersInfo();
            public QueueInfo Queue { get; } = new QueueInfo();

            internal void Update()
            {
                this.Anchor.Update();
            }

            public sealed class StopwatchInfo
            {
                internal StopwatchInfo()
                {
                    this.AddressesResultStopwatch.Start();
                    this.BlockResultStopwatch.Start();
                    this.BroadcastClueResultStopwatch.Start();
                    this.UnicastClueResultStopwatch.Start();
                    this.MulticastClueResultStopwatch.Start();
                }

                public Stopwatch AddressesResultStopwatch { get; } = new Stopwatch();
                public Stopwatch BlockResultStopwatch { get; } = new Stopwatch();
                public Stopwatch BroadcastClueResultStopwatch { get; } = new Stopwatch();
                public Stopwatch UnicastClueResultStopwatch { get; } = new Stopwatch();
                public Stopwatch MulticastClueResultStopwatch { get; } = new Stopwatch();
            }

            public sealed class FiltersInfo
            {
                public VolatileBloomFilter<BroadcastClue> BroadcastCluesFilter { get; } = new VolatileBloomFilter<BroadcastClue>(MaxBroadcastClueCount * 2 * 3, 0.0001, 3, (n) => n.GetHashCode(), new TimeSpan(0, 3, 0), new TimeSpan(0, 30, 0));
                public VolatileBloomFilter<UnicastClue> UnicastCluesFilter { get; } = new VolatileBloomFilter<UnicastClue>(MaxUnicastClueCount * 2 * 3, 0.0001, 3, (n) => n.GetHashCode(), new TimeSpan(0, 3, 0), new TimeSpan(0, 30, 0));
                public VolatileBloomFilter<MulticastClue> MulticastCluesFilter { get; } = new VolatileBloomFilter<MulticastClue>(MaxMulticastClueCount * 2 * 3, 0.0001, 3, (n) => n.GetHashCode(), new TimeSpan(0, 3, 0), new TimeSpan(3, 0, 0));

                public VolatileBloomFilter<OmniHash> PublishBlocksFilter { get; } = new VolatileBloomFilter<OmniHash>(MaxPublishBlockCount * 2 * 3, 0.0001, 3, (n) => n.GetHashCode(), new TimeSpan(0, 3, 0), new TimeSpan(0, 30, 0));
                public VolatileBloomFilter<OmniHash> WantBlocksFilter { get; } = new VolatileBloomFilter<OmniHash>(MaxWantBlockCount * 10, 0.0001, 3, (n) => n.GetHashCode(), new TimeSpan(0, 10, 0), new TimeSpan(0, 30, 0));

                internal void Update()
                {
                    this.BlockRequestFilter.Update();
                    this.BlockLinkFilter.Update();
                }
            }

            public sealed class QueuesInfo
            {
                internal QueuesInfo() { }

                public LockedHashDictionary<OmniSignature, RelayOption> WantBroadcastClueMap { get; } = new LockedHashDictionary<OmniSignature, RelayOption>();
                public LockedHashDictionary<OmniSignature, RelayOption> WantUnicastClueMap { get; } = new LockedHashDictionary<OmniSignature, RelayOption>();
                public LockedHashDictionary<Channel, RelayOption> WantMulticastClueMap { get; } = new LockedHashDictionary<Channel, RelayOption>();

                public LockedHashDictionary<OmniHash, RelayOption> PublishBlocksMap { get; } = new LockedHashDictionary<OmniHash, RelayOption>();
                public LockedHashDictionary<OmniHash, RelayOption> WantBlocksMap { get; } = new LockedHashDictionary<OmniHash, RelayOption>();
                public LockedHashDictionary<OmniHash, RelayOption> DiffuseBlocksMap { get; } = new LockedHashDictionary<OmniHash, RelayOption>();
                public LockedHashDictionary<OmniHash, RelayOption> BlocksMap { get; } = new LockedHashDictionary<OmniHash, RelayOption>();
            }
        }

        public sealed class ReceiveInfo
        {
            internal ReceiveInfo() { }

            public bool IsInitialized { get; set; }

            public StopwatchesInfo Stopwatches { get; } = new StopwatchesInfo();
            public QueuesInfo Queues { get; } = new QueuesInfo();

            internal void Update()
            {
                this.Queues.Update();
            }

            public sealed class StopwatchesInfo
            {
                internal StopwatchesInfo()
                {
                    this.ReceiveBlockStopwatch.Start();
                }

                public Stopwatch ReceiveBlockStopwatch { get; } = new Stopwatch();
            }

            public sealed class QueuesInfo
            {
                internal QueuesInfo() { }

                public VolatileHashDictionary<OmniSignature, RelayOption> WantBroadcastClueMap { get; } = new VolatileHashDictionary<OmniSignature, RelayOption>(new TimeSpan(0, 3, 0));
                public VolatileHashDictionary<OmniSignature, RelayOption> WantUnicastClueMap { get; } = new VolatileHashDictionary<OmniSignature, RelayOption>(new TimeSpan(0, 3, 0));
                public VolatileHashDictionary<Channel, RelayOption> WantMulticastClueMap { get; } =new VolatileHashDictionary<Channel, RelayOption>(new TimeSpan(0, 3, 0));

                public VolatileHashDictionary<OmniHash, RelayOption> PublishBlocksMap { get; } = new VolatileHashDictionary<OmniHash, RelayOption>(new TimeSpan(0, 10, 0));
                public VolatileHashDictionary<OmniHash, RelayOption> WantBlocksMap { get; } = new VolatileHashDictionary<OmniHash, RelayOption>(new TimeSpan(0, 10, 0));

                internal void Update()
                {
                    this.WantBroadcastClueMap.Update();
                    this.WantUnicastClueMap.Update();
                    this.WantMulticastClueMap.Update();

                    this.PublishBlocksMap.Update();
                    this.WantBlocksMap.Update();
                }
            }
        }
    }
}
