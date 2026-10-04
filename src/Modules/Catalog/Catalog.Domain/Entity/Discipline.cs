using System;
using System.Collections.Generic;
using System.Text;
using Catalog.Domain.Common;

namespace Catalog.Domain.Entity
{
    public class Discipline : BaseEntity
    {
        public string Name { get; private set; }

        public string Description { get; private set; }

        private readonly List<Topic> _topics = new();

        public IReadOnlyCollection<Topic> Topics => _topics.AsReadOnly();

        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

        private Discipline()
        {

        }

        public Discipline(string name, string description)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name cannot be null or whitespace", nameof(name));
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentException("Description cannot be null or whitespace", nameof(description));
            }

            Name = name;
            Description = description;
        }

        public void UpdateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name cannot be null or whitespace", nameof(name));
            }

            Name = name;
        }

        public void UpdateDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentException("Description cannot be null or whitespace", nameof(description));
            }

            Description = description;
        }

        public void UpdateTimeStamp()
        {
            UpdatedAt = DateTime.UtcNow;

        }

        public void AddTopic(Topic topic)
        {
            if (topic == null)
                throw new ArgumentNullException(nameof(topic));

            _topics.Add(topic);
            UpdateTimeStamp();
        }

        public void RemoveTopic(Topic topic)
        {
            if (topic == null)
                throw new ArgumentNullException(nameof(topic));

            _topics.Remove(topic);
            UpdateTimeStamp();
        }

    }
}
