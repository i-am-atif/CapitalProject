using CapitalProject.DTOs;
using CapitalProject.Models;
using CapitalProject.Services;
using Microsoft.AspNetCore.Mvc;

namespace CapitalProject.Controllers
{
    public class QuestionsController : ControllerBase
    {
        private readonly IQuestionService _questionService;

        public QuestionsController(IQuestionService questionService)
        {
            _questionService = questionService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateQuestion([FromBody] QuestionDto question)
        {
            var newQuestion = await _questionService.CreateQuestionAsync(question);
            return CreatedAtRoute("GetQuestion", new { id = newQuestion.Id }, newQuestion);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQuestion(string id, [FromBody] QuestionDto question)
        {
            if (id != question.Id)
            {
                return BadRequest();
            }

            await _questionService.UpdateQuestionAsync(id, question);
            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> GetQuestions()
        {
            var questions = await _questionService.GetQuestionsAsync();
            return Ok(questions);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetQuestionById(string id)
        {
            var question = await _questionService.GetQuestionByIdAsync(id);
            if (question == null)
            {
                return NotFound();
            }
            return Ok(question);
        }

        // New endpoint for submitting applications
        [HttpPost("applications")]
        public async Task<IActionResult> SubmitApplication([FromBody] ApplicationDto application)
        {
            // This example just logs the application data for demonstration
            Console.WriteLine("Received application:");
            Console.WriteLine($"  - Candidate Name: {application.CandidateName}");
            Console.WriteLine($"  - Answers:");
            foreach (var answer in application.Answers)
            {
                Console.WriteLine($"    - Question: {answer.QuestionId}");
                Console.WriteLine($"      - Answer: {answer.AnswerText}");
            }
            return Ok();
        }

        // New endpoint to get questions for rendering
        [HttpGet("questions/types")]
        public async Task<IActionResult> GetQuestionTypes()
        {
            var questionTypes = Enum.GetValues<QuestionType>();
            return Ok(questionTypes);
        }
    }
}
