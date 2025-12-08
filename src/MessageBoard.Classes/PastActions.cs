namespace Message_Board;

public enum Action
{
    UserBanned,
    UserUnbanned,
    UserGivenModerator,
    RemovedModeratorRole,
    PostRemoved
}

public class PastActions
{
    private Action Action {get; set;}
    private string Description {get; set;}
    private DateTime DateOfAction {get; set;}
    
    public PastActions(Action action, string description)
    {
        this.Action = action;
        this.Description = description;
        this.DateOfAction = DateTime.Now;
    }
    
    
}