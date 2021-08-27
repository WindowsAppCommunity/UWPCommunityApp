using UwpCommunityBackend.Models;
using Xunit;

namespace UwpCommunityBackend.Tests
{
    public class ProjectTests
    {
        [Fact]
        public void GetCategoryTitle_SingleWord()
        {
            Assert.Equal(
                "Business",
                Project.GetCategoryTitle(Project.ProjectCategory.Business)
            );
        }

        [Fact]
        public void GetCategoryTitle_MultiWord()
        {
            Assert.Equal(
                "Developer tools",
                Project.GetCategoryTitle(Project.ProjectCategory.DeveloperTools)
            );
        }

        [Fact]
        public void GetCategoryTitle_And()
        {
            Assert.Equal(
                "Navigation & maps",
                Project.GetCategoryTitle(Project.ProjectCategory.NavigationAndMaps)
            );
        }

        [Fact]
        public void GetCategoryFromTitle_SingleWord()
        {
            Assert.Equal(
                Project.ProjectCategory.Business,
                Project.GetCategoryFromTitle("Business")
            );
        }

        [Fact]
        public void GetCategoryFromTitle_MultiWord()
        {
            Assert.Equal(
                Project.ProjectCategory.DeveloperTools,
                Project.GetCategoryFromTitle("Developer tools")
            );
        }

        [Fact]
        public void GetCategoryFromTitle_And()
        {
            Assert.Equal(
                Project.ProjectCategory.NavigationAndMaps,
                Project.GetCategoryFromTitle("Navigation & maps")
            );
        }
    }
}
