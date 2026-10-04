using System;
using System.Collections.Generic;
using System.Text;
using Catalog.Domain.Common;

namespace Catalog.Domain.Entity
{
    public class Topic : BaseEntity
    {
        public Guid DisciplineId { get; private set; }
        public Discipline Discipline { get; private set; }

        public string Name { get; private set; }

        public string Description { get; private set; }

        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

        private readonly List<Question> _questions = new();
        public IReadOnlyCollection<Question> Questions => _questions.AsReadOnly();

        private Topic()
        {

        }
            
        public Topic(Guid disciplineId, string name, string description)
        {
            if (disciplineId == Guid.Empty)
            {
                throw new ArgumentException("DisciplineId cannot be empty", nameof(disciplineId));
            }
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name cannot be null or whitespace", nameof(name));
            }
            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentException("Description cannot be null or whitespace", nameof(description));
            }
            DisciplineId = disciplineId;
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

        public void UpdateDisciplineId(Guid disciplineId)
        {
            if (disciplineId == Guid.Empty)
            {
                throw new ArgumentException("DisciplineId cannot be empty", nameof(disciplineId));
            }
            DisciplineId = disciplineId;
        }

        public void UpdateTimeStamp()
        {
            this.UpdatedAt = DateTime.UtcNow;
        }

        public void AddQuestion(Question question)
        {
            if (question == null)
                throw new ArgumentNullException(nameof(question));

            _questions.Add(question);
            UpdateTimeStamp();
        }

        public void RemoveQuestion(Question question)
        {
            if (question == null)
                throw new ArgumentNullException(nameof(question));

            _questions.Remove(question);
            UpdateTimeStamp();
        }
    }
}
