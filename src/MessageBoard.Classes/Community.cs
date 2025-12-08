namespace Message_Board;

public class Community
{
    private string Name {get; set;}
    private string Description {get; set;}
    private int MemberCount {get; set;}
    private bool IsPrivate {get; set; }
    private static int _maxModerators = 10;
    
    private List<Moderator> _moderatorList;
    
    private static List<Community> _communityList = new List<Community>();

    private List<Member> _memberList { get; }

    private List<PastActions> _actionsList;

    public Community(
        string name,
        string description = "",
        bool isPrivate = false
        )
    {
        if (name.Trim() == null || description.Trim() == null)
        {
            throw new Exception("name cannot be empty");
        }

        this.Name = name;
        this.Description = description;
        this.MemberCount = 0;
        this.IsPrivate = isPrivate;
        this._moderatorList = new List<Moderator>(_maxModerators);

        this._memberList = new List<Member>();
        
        this._actionsList = new List<PastActions>();
        
        _communityList.Add(this);
    }
    
    public string GetName() => this.Name;

    public List<Member> GetMembers()
    {
        return this._memberList;
    }

    public List<PastActions> GetActions()
    {
        return this._actionsList;
    }

    public List<Moderator> GetModerators()
    {
        return this._moderatorList;
    }

    public void SetDesc(string desc)
    {
        if (desc == null || desc.Trim() == "")
        {
            throw new Exception("description cannot be null");
        }

        this.Description = desc;
    }

    public Community CreateCommunity(        
        string name,
        string description = "",
        bool privateFlag = false
        )
    {
        return new Community(name, description, privateFlag);
    }

    public void AddModerator(Moderator moderator)
    {
        this._moderatorList.Add(moderator);
    }

    public static void RemoveCommunity(Community removedcommunity)
    {
        removedcommunity = null;
    }

    public void Subscribe(Member member)
    {
        this._memberList.Add(member);
    }
    

}