using FluentAssertions;
using NUnit.Framework;
using Ubit_Slendermena_Client.Models;

namespace GameClient.Tests.Models
{
    [TestFixture]
    public class QuestionTests
    {
        [Test]
        public void Question_DefaultValues_AreSet()
        {
            var question = new Question();

            question.Id.Should().Be(0);
            question.CategoryId.Should().Be(0);
            question.Text.Should().BeEmpty();
            question.Price.Should().Be(0);
        }

        [Test]
        public void Question_Properties_SetCorrectly()
        {
            var question = new Question
            {
                Id = 1,
                CategoryId = 2,
                Text = "Какой самый большой океан?",
                Price = 400
            };

            question.Id.Should().Be(1);
            question.CategoryId.Should().Be(2);
            question.Text.Should().Be("Какой самый большой океан?");
            question.Price.Should().Be(400);
        }
    }
}