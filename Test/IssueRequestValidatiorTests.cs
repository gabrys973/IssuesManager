using Core.Models;
using Core.Validators;
using FluentValidation.TestHelper;

namespace Test;

public class IssueRequestValidatiorTests
{
    private readonly IssueRequestValidator _validator;
    private static readonly Random _random = new();

    public IssueRequestValidatiorTests()
    {
        _validator = new IssueRequestValidator();
    }

    [Test]
    public void IssueRequestValidator_CorrectRequest_Success()
    {
        var request = new IssueRequest("title", "description");

        var validationResult = _validator.TestValidate(request);

        Assert.IsNotNull(validationResult);
        Assert.True(validationResult.IsValid);
        Assert.That(validationResult.Errors, Is.Empty);
    }

    [Test]
    public void IssueRequestValidator_TitleNull_Failed()
    {
        var request = new IssueRequest(null, "description");

        var validationResult = _validator.TestValidate(request);

        Assert.IsNotNull(validationResult);
        Assert.False(validationResult.IsValid);
        Assert.That(validationResult.Errors, Is.Not.Empty);

        validationResult.ShouldHaveValidationErrorFor(x => x.Title);
        validationResult.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Test]
    public void IssueRequestValidator_TitleTooLong_Failed()
    {
        var request = new IssueRequest(RandomString(300), "description");

        var validationResult = _validator.TestValidate(request);

        Assert.IsNotNull(validationResult);
        Assert.False(validationResult.IsValid);
        Assert.That(validationResult.Errors, Is.Not.Empty);
        validationResult.ShouldHaveValidationErrorFor(x => x.Title);
        validationResult.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Test]
    public void IssueRequestValidator_DescriptionTooLong_Failed()
    {
        var request = new IssueRequest("title", RandomString(65635));

        var validationResult = _validator.TestValidate(request);

        Assert.IsNotNull(validationResult);
        Assert.False(validationResult.IsValid);
        Assert.That(validationResult.Errors, Is.Not.Empty);
        validationResult.ShouldHaveValidationErrorFor(x => x.Description);
        validationResult.ShouldNotHaveValidationErrorFor(x => x.Title);
    }

    private string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[_random.Next(s.Length)]).ToArray());
    }
}