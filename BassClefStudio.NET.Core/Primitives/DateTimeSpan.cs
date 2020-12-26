using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.NET.Core.Primitives
{
    /// <summary>
    /// Represents a contiguous block of time between two <see cref="DateTimeOffset"/>s.
    /// </summary>
    public struct DateTimeSpan
    {
        /// <summary>
        /// Represents the beginning <see cref="DateTimeOffset"/> where this <see cref="DateTimeSpan"/> starts.
        /// </summary>
        public DateTimeOffset StartDate { get; }

        /// <summary>
        /// Represents the final <see cref="DateTimeOffset"/> where this <see cref="DateTimeSpan"/> ends.
        /// </summary>
        public DateTimeOffset EndDate { get; }

        /// <summary>
        /// Creates a new <see cref="DateTimeSpan"/> between two dates.
        /// </summary>
        /// <param name="start">The <see cref="DateTimeOffset"/> where this <see cref="DateTimeSpan"/> starts.</param>
        /// <param name="end">The <see cref="DateTimeOffset"/> where this <see cref="DateTimeSpan"/> ends.</param>
        public DateTimeSpan(DateTimeOffset start, DateTimeOffset end)
        {
            StartDate = start;
            EndDate = end;
            if(EndDate < StartDate)
            {
                throw new ArgumentException("DateTimeSpan must represent the distance between two dates where EndDate is greater than or equal to StartDate.");
            }
        }

        /// <summary>
        /// Creates a new <see cref="DateTimeSpan"/> between two dates.
        /// </summary>
        /// <param name="start">The <see cref="DateTime"/> where this <see cref="DateTimeSpan"/> starts.</param>
        /// <param name="end">The <see cref="DateTime"/> where this <see cref="DateTimeSpan"/> ends.</param>
        public DateTimeSpan(DateTime start, DateTime end) : this(new DateTimeOffset(start), new DateTimeOffset(end))
        { }

        /// <summary>
        /// Creates a new <see cref="DateTimeSpan"/> from a <see cref="DateTimeOffset"/> and a length.
        /// </summary>
        /// <param name="start">The <see cref="DateTimeOffset"/> where this <see cref="DateTimeSpan"/> starts.</param>
        /// <param name="duration">The <see cref="TimeSpan"/> length of this <see cref="DateTimeSpan"/>.</param>
        public DateTimeSpan(DateTimeOffset start, TimeSpan duration)
        {
            StartDate = start;
            EndDate = start + duration;
        }

        /// <summary>
        /// Creates a new <see cref="DateTimeSpan"/> from a <see cref="DateTime"/> and a length.
        /// </summary>
        /// <param name="start">The <see cref="DateTime"/> where this <see cref="DateTimeSpan"/> starts.</param>
        /// <param name="duration">The <see cref="TimeSpan"/> length of this <see cref="DateTimeSpan"/>.</param>
        public DateTimeSpan(DateTime start, TimeSpan duration) : this(new DateTimeOffset(start), duration)
        { }

        /// <summary>
        /// Gets the <see cref="TimeSpan"/> duration between the two events this <see cref="DateTimeSpan"/> encapsulates.
        /// </summary>
        public TimeSpan Duration => EndDate - StartDate;
    }
}
