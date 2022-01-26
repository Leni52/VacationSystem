namespace WorkForceManagement.DAL.Entities
{
    public partial class TblFile : Base
    {
        public string Name { get; set; }
        public string ContentType { get; set; }
        public byte[] Data { get; set; }
    }
}
