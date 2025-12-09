using System;

namespace MessageBoard.Domain
{
    [Serializable]
    public class TextPost : Post
    {
        private string _text;
        public string Text
        {
            get => _text;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Text content cannot be empty.");
                _text = value;
            }
        }

        public TextPost() : base()
        {
        }

        public TextPost(int postId, string title, bool isMature, string text)
            : base(postId, title, isMature)
        {
            Text = text;
        }
    }
}
