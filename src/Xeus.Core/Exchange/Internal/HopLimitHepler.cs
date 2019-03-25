using System;
using System.Collections.Generic;
using System.Text;
using Omnix.Base;

namespace Xeus.Core.Exchange.Internal.Helpers
{
    internal static class HopLimitHepler
    {
        private const int MaxHopLimit = 16;
        private static readonly int _myInitHopLimit;

        static HopLimitHepler()
        {
            var random = RandomProvider.GetThreadRandom();

            if (random.Next(0, int.MaxValue) % 2 == 0)
            {
                _myInitHopLimit = MaxHopLimit;
            }
            else
            {
                _myInitHopLimit = MaxHopLimit - 1;
            }
        }

        public static int CreateNew()
        {
            return _myInitHopLimit;
        }

        public static int Decrement(int current, bool decrementAtHopLimitMaximum, bool decrementAtHopLimitMinimum)
        {
            if (current > MaxHopLimit)
            {
                current = MaxHopLimit;
            }

            if (current <= 0)
            {
                return 0;
            }

            if (current == MaxHopLimit)
            {
                if (decrementAtHopLimitMaximum)
                {
                    return current - 1;
                }

                return current;
            }

            if (current == 1)
            {
                if (decrementAtHopLimitMinimum)
                {
                    return current - 1;
                }

                return current;
            }

            return current - 1;
        }
    }
}
