using CapitalProject.Models;

namespace CapitalProject.DTOs
{
    public class QuestionDto
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public QuestionType Type { get; set; }
        public List<string> Options { get; set; }
    }
}
