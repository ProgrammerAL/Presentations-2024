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

        var commentsArea = Page.Locator("textarea[id='comments-area']");
        await commentsArea.FillAsync($"Comment from WhenCommentsProvided_SubmitSuccessful - {DateTime.UtcNow}");

        await Page.ClickAsync("button[id='submit-btn']");

        await Expect(Page.Locator("p[id='comment-confirmation']")).ToBeVisibleAsync();
    }
}