namespace TodoList.Core.Entities;

public class IdentifyRequest
{
    public int Id { get; set; }
    public required string Code { get; set; }
}