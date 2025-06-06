﻿using FluentAssertions;
using GameClient.Models;
using NUnit.Framework;


namespace GameClient.Tests.Models
{
    [TestFixture]
    public class CategoryTests
    {
        [Test]
        public void Category_DefaultValues_SetCorrectly()
        {
            var category = new Category();

            category.Id.Should().Be(0);
            category.Name.Should().BeEmpty();
        }

        [Test]
        public void Category_WithName_SetCorrectly()
        {
            var category = new Category { Name = "История" };

            category.Name.Should().Be("История");
        }
    }
}

