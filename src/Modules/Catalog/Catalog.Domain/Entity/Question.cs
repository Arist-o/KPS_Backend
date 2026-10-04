using System;
using System.Collections.Generic;
using System.Text;
using Catalog.Domain.Common;

namespace Catalog.Domain.Entity
{
    public class Question : BaseEntity
    {
        public Guid TopicId { get; private set; }

        public Topic Topic { get; private set; }

        public string Text { get; private set; }

        public int DifficultyLevel { get; private set; }

        public int Points { get; private set; }

        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

        private readonly List<Answer> _answers = new();

        public IReadOnlyCollection<Answer> Answers => _answers.AsReadOnly();

        private Question()
        {
        }

        public Question(Guid topicId, string text, int difficultyLevel, int points)
        {
            if (topicId == Guid.Empty)
            {
                throw new ArgumentException("TopicId cannot be empty", nameof(topicId));
            }
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException("Text cannot be null or whitespace", nameof(text));
            }
            if (difficultyLevel < 1 || difficultyLevel > 5)
            {
                throw new ArgumentOutOfRangeException(nameof(difficultyLevel), "Difficulty level must be between 1 and 5");
            }
            if (points < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(points), "Points cannot be negative");
            }
            TopicId = topicId;
            Text = text;
            DifficultyLevel = difficultyLevel;
            Points = points;
        }

        public void UpdateText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException("Text cannot be null or whitespace", nameof(text));
            }
            Text = text;
        }

        public void UpdateDifficultyLevel(int difficultyLevel)
        {
            if (difficultyLevel < 1 || difficultyLevel > 5)
            {
                throw new ArgumentOutOfRangeException(nameof(difficultyLevel), "Difficulty level must be between 1 and 5");
            }
            DifficultyLevel = difficultyLevel;
        }

        public void UpdatePoints(int points)
        {
            if (points < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(points), "Points cannot be negative");
            }
            Points = points;
        }

        public void UpdateTimeStamp()
        {
            this.UpdatedAt = DateTime.UtcNow;
        }
        public void AddAnswer(Answer answer)
        {
            if (answer == null)
                throw new ArgumentNullException(nameof(answer));

            _answers.Add(answer);
            UpdateTimeStamp();
        }

        public void RemoveAnswer(Answer answer)
        {
            if (answer == null)
                throw new ArgumentNullException(nameof(answer));

            _answers.Remove(answer);
            UpdateTimeStamp();
        }
    }
}
