namespace Message_Board;

public class Moderator
{
    private Member _member;
    private DateTime _dateOfAssingment;
    private Community _moderatedCommunity;

    public Moderator(Member member, Community moderatedCommunity)
    {
        if (moderatedCommunity.GetMembers().Find(m => m.GetId() == member.GetId()) == null)
        {
            throw new Exception("Member "+  member.GetName() +" not found in community " + moderatedCommunity.GetName());
        }

        this._dateOfAssingment = DateTime.Now;
        this._member = member;
        this._moderatedCommunity = moderatedCommunity;
        moderatedCommunity.AddModerator(this);
        moderatedCommunity.GetActions().Add(new PastActions(Action.UserGivenModerator, "user " + member.GetName() + " is now Moderator"));
    }
}