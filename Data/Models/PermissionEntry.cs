namespace Rosalyn.Data.Models
{
    public class PermissionEntry
    {
        public int Id { get; set; }
        public ulong ServerId { get; set; }
        public ulong ForeignId { get; set; }
        public string Permission { get; set; }
    }
}