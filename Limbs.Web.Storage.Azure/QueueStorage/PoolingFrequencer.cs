using System;

namespace Limbs.Web.Storage.Azure.QueueStorage
{
    public class PoolingFrequencer : IPoolingFrequencer
    {
        private const int CeilingMultiplier = 30;
        private const int FloorMultiplier = 2;

        private readonly int _ceiling;
        private readonly int _floor;
        private readonly bool _useGradualDecrease;
        private int _current;

        public static PoolingFrequencer For(TimeSpan estimatedTimeToProcessMessageBlock, bool useGradualDecrease = true)
        {
            return new PoolingFrequencer(estimatedTimeToProcessMessageBlock, useGradualDecrease);
        }
        /// <summary>
        /// Create a new instance of <see cref="PoolingFrequencer"/>
        /// </summary>
        /// <param name="estimatedTimeToProcessMessageBlock">Estimated time to process a "MessageBlock"</param>
        /// <param name="useGradualDecrease">False to decrease to floor immediately.</param>
        /// <remarks>
        /// Uses <paramref name="estimatedTimeToProcessMessageBlock"/> to calculate the ceiling and the floor of the frequence.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">Where <paramref name="estimatedTimeToProcessMessageBlock"/> execeed one hour. </exception>
        public PoolingFrequencer(TimeSpan estimatedTimeToProcessMessageBlock, bool useGradualDecrease = true)
        {
            if (estimatedTimeToProcessMessageBlock.TotalHours > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(estimatedTimeToProcessMessageBlock), "Max value allowed is one hour.");
            }
            _useGradualDecrease = useGradualDecrease;
            var estimatedMilliseconds = Convert.ToInt32(estimatedTimeToProcessMessageBlock.TotalMilliseconds);
            _ceiling = estimatedMilliseconds * CeilingMultiplier;
            _floor = estimatedMilliseconds * FloorMultiplier;
            _current = _floor;
        }

        /// <summary>
        /// Create a new instance of <see cref="PoolingFrequencer"/>
        /// </summary>
        /// <param name="floor">minimum milliseconds</param>
        /// <param name="ceiling">max milliseconds</param>
        /// <param name="useGradualDecrease">False to decrease to <paramref name="floor"/> immediately.</param>
        public PoolingFrequencer(int floor, int ceiling, bool useGradualDecrease = true)
        {
            if (floor > ceiling)
            {
                throw new ArgumentOutOfRangeException(nameof(floor), "The floor should be less than ceiling.");
            }
            _useGradualDecrease = useGradualDecrease;
            _ceiling = ceiling;
            _floor = floor;
            _current = floor;
        }

        /// <inheritdoc />
        /// <summary>
        /// The current milliseconds.
        /// </summary>
        /// <remarks> it increase at each call.</remarks>
        public int Current
        {
            get
            {
                var actual = _current;
                if (_current < _ceiling)
                {
                    _current *= 2;
                }
                return actual;
            }
        }

        public void Decrease()
        {
            if (_useGradualDecrease && _current > _floor)
            {
                _current /= 2;
                return;
            }
            _current = _floor;
        }
    }
}
