namespace PolicyNotesService.Models
{
    public class PolicyNote
    {
        public int Id { get; set; }                  
        public string Policynumber { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
    }

}
