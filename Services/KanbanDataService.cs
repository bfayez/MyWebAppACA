using MyWebApp.Models;

namespace MyWebApp.Services;

public interface IKanbanDataService
{
    IReadOnlyList<KanbanItem> GetAllItems();
    IReadOnlyList<TeamMember> GetAllMembers();
    IEnumerable<KanbanItem> GetItemsByStatus(KanbanStatus status);
    TeamMember? GetMemberById(int? memberId);
    KanbanItem AddItem(string title, string? description, int? assignedToId);
    TeamMember AddMember(string name, string email);
    bool UpdateItemStatus(int itemId, KanbanStatus status);
    bool AssignItem(int itemId, int? memberId);
    bool DeleteItem(int itemId);
    bool RemoveMember(int memberId);
}

public class KanbanDataService : IKanbanDataService
{
    private static readonly List<KanbanItem> _kanbanItems = new();
    private static readonly List<TeamMember> _teamMembers = new();
    private static int _nextItemId = 1;
    private static int _nextMemberId = 1;
    private readonly ILogger<KanbanDataService> _logger;

    public KanbanDataService(ILogger<KanbanDataService> logger)
    {
        _logger = logger;
    }

    public IReadOnlyList<KanbanItem> GetAllItems() => _kanbanItems.AsReadOnly();
    
    public IReadOnlyList<TeamMember> GetAllMembers() => _teamMembers.AsReadOnly();

    public IEnumerable<KanbanItem> GetItemsByStatus(KanbanStatus status)
    {
        return _kanbanItems.Where(i => i.Status == status).OrderByDescending(i => i.CreatedAt);
    }

    public TeamMember? GetMemberById(int? memberId)
    {
        return memberId.HasValue ? _teamMembers.FirstOrDefault(m => m.Id == memberId.Value) : null;
    }

    public KanbanItem AddItem(string title, string? description, int? assignedToId)
    {
        var item = new KanbanItem
        {
            Id = _nextItemId++,
            Title = title,
            Description = description,
            Status = KanbanStatus.New,
            AssignedToId = assignedToId,
            CreatedAt = DateTime.UtcNow
        };

        _kanbanItems.Add(item);
        _logger.LogInformation("New Kanban item created: {Title} at {Time}", title, DateTime.UtcNow);
        
        return item;
    }

    public TeamMember AddMember(string name, string email)
    {
        var member = new TeamMember
        {
            Id = _nextMemberId++,
            Name = name,
            Email = email,
            CreatedAt = DateTime.UtcNow
        };

        _teamMembers.Add(member);
        _logger.LogInformation("New team member added: {Name} at {Time}", name, DateTime.UtcNow);
        
        return member;
    }

    public bool UpdateItemStatus(int itemId, KanbanStatus status)
    {
        var item = _kanbanItems.FirstOrDefault(i => i.Id == itemId);
        if (item != null)
        {
            var oldStatus = item.Status;
            item.Status = status;
            item.UpdatedAt = DateTime.UtcNow;
            
            _logger.LogInformation("Kanban item {Id} status changed from {OldStatus} to {NewStatus} at {Time}", 
                itemId, oldStatus, status, DateTime.UtcNow);
            return true;
        }
        return false;
    }

    public bool AssignItem(int itemId, int? memberId)
    {
        var item = _kanbanItems.FirstOrDefault(i => i.Id == itemId);
        if (item != null)
        {
            item.AssignedToId = memberId;
            item.UpdatedAt = DateTime.UtcNow;
            
            var memberName = memberId.HasValue 
                ? _teamMembers.FirstOrDefault(m => m.Id == memberId.Value)?.Name ?? "Unknown"
                : "Unassigned";
            
            _logger.LogInformation("Kanban item {Id} assigned to {Member} at {Time}", 
                itemId, memberName, DateTime.UtcNow);
            return true;
        }
        return false;
    }

    public bool DeleteItem(int itemId)
    {
        var item = _kanbanItems.FirstOrDefault(i => i.Id == itemId);
        if (item != null)
        {
            _kanbanItems.Remove(item);
            _logger.LogInformation("Kanban item {Id} deleted at {Time}", itemId, DateTime.UtcNow);
            return true;
        }
        return false;
    }

    public bool RemoveMember(int memberId)
    {
        var member = _teamMembers.FirstOrDefault(m => m.Id == memberId);
        if (member != null)
        {
            // Unassign this member from all items
            foreach (var item in _kanbanItems.Where(i => i.AssignedToId == memberId))
            {
                item.AssignedToId = null;
            }

            _teamMembers.Remove(member);
            _logger.LogInformation("Team member {Id} removed at {Time}", memberId, DateTime.UtcNow);
            return true;
        }
        return false;
    }
}