using BlogDoFT.Libs.ResultPattern;

namespace BlogDoFT.Libs.DomainNotifications.Extensions.Tests;

public class DomainNotificationExtensionsTests
{
    private readonly DomainNotificationBag _notifications = new();
    private readonly Failure _expectedFailure;
    private readonly Faker _faker = BogusFixtures.Get();

    public DomainNotificationExtensionsTests()
    {
        _expectedFailure = new Failure(Code: _faker.Random.Word(), Message: _faker.Random.Words());
    }

    [Fact]
    public void Should_AddFailure_When_ResultIsAFailure()
    {
        // Given
        var result = Result.AsFailure(_expectedFailure);

        // When
        _notifications.Add(result);

        // Then
        _notifications.ToEnumerable().ShouldContain(m => m.Code == _expectedFailure.Code && m.Message == _expectedFailure.Message);
    }

    [Fact]
    public void Should_AddFailure_When_ResultIsATypedResultFailure()
    {
        // Given
        Result<StubClass> result = _expectedFailure;

        // When
        _notifications.Add(result);

        // Then
        _notifications.ToEnumerable().ShouldContain(m => m.Code == _expectedFailure.Code && m.Message == _expectedFailure.Message);
    }

    [Fact]
    public void Should_NotAddFailure_When_ResultIsASuccess()
    {
        // Given
        var result = Result.AsSuccess();

        // When
        _notifications.Add(result);

        // Then
        _notifications.IsEmpty().ShouldBeTrue();
    }

    [Fact]
    public void Should_NotAddFailure_When_TypedResultIsASuccess()
    {
        // Given
        Result<StubClass> result = new StubClass();

        // When
        _notifications.Add(result);

        // Then
        _notifications.IsEmpty().ShouldBeTrue();
    }

    [Fact]
    public void Should_AddFailureToNotification_When_FailureIsDiffFromNone()
    {
        // When
        _notifications.Add(_expectedFailure);

        // Then
        _notifications.IsEmpty().ShouldBeFalse();
        _notifications.ToEnumerable().ShouldContain(m => m.Code == _expectedFailure.Code && m.Message == _expectedFailure.Message);
    }

    [Fact]
    public void Should_NotAddFailureToNotification_When_FailureIsNone()
    {
        // Given
        var failure = Failure.None;

        // When
        _notifications.Add(failure);

        // Then
        _notifications.IsEmpty().ShouldBeTrue();
    }

    [Fact]
    public void Should_AddMessage_When_ThereIsAnExceptionAndErrorCode()
    {
        // Given
        var expectedCode = _faker.Random.Word();
        var exception = new Exception(_faker.Random.Words(5));

        // When
        _notifications.Add(exception, expectedCode);

        // Then
        _notifications[0].ShouldBeEquivalentTo(new DomainNotification(exception.Message)
        {
            Code = expectedCode,
        });
    }
}
