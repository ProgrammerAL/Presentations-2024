using Microsoft.Playwright.MSTest;

namespace UiTests;

[TestClass]
public class FeedbackTests : PageTest
{
    [TestMethod]
    public async Task WhenCommentsProvided_SubmitSuccessful()
    {
        var url = $"{TestContext.Properties["baseUrl"]}/comments";
        await Page.GotoAsync(url);

        await Page.GetByTestId("comments-area").First.FillAsync($"Comment from WhenCommentsProvided_SubmitSuccessful - {DateTime.UtcNow}");
        await Page.ClickAsync("Submit");

        await Expect(Page.GetByTestId("comment-confirmation")).ToBeVisibleAsync();
    }
}