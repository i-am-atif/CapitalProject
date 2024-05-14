namespace CapitalProject.Models
{
    public class Question
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public QuestionType Type { get; set; }
        public List<Answer> Options { get; set; } // For Dropdown & MultipleChoice
    }
}
