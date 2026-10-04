using System;
using System.Collections.Generic;
using System.Text;
using Catalog.Domain.Common;

namespace Catalog.Domain.Entity
{
    public class Answer : BaseEntity
    {

        public Guid QuestionId { get; private set; }
        public Question Question { get; private set; }
        public string Text { get; private set; }
        public bool IsCorrect { get; private set; }

        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        public DateTime UpdateAt { get; private set; } = DateTime.UtcNow;

        private Answer()
        {
        }
        public Answer(Guid questionId, string text, bool isCorrect)
        {
            if (questionId == Guid.Empty)
            {
                throw new ArgumentException("QuestionId cannot be empty", nameof(questionId));
            }
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException("Text cannot be null or whitespace", nameof(text));
            }
            QuestionId = questionId;
            Text = text;
            IsCorrect = isCorrect;
        }
        public void UpdateText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException("Text cannot be null or whitespace", nameof(text));
            }
            Text = text;
        }
        public void UpdateIsCorrect(bool isCorrect)
        {
            IsCorrect = isCorrect;
        }

        public void UpdateTimeStamp()
        {
            this.UpdateAt = DateTime.UtcNow;
        }
    }
}
