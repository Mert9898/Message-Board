using System;

namespace MessageBoard.Domain
{
    [Serializable]
    public class Votes
    {
        private int _score;
        public int Score
        {
            get => _score;
            set => _score = value;
        }

        public Votes()
        {
            _score = 0;
        }

        public void VotePos()
        {
            _score++;
        }

        public void VoteNeg()
        {
            _score--;
        }
    }
}
