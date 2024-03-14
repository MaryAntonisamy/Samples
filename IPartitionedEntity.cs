
public interface IPartitionedEntity
{
    string Id { get; set; }
    DateTime NextEventDate { get; }
}
    