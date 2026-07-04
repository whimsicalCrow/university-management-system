namespace University.Web.Services;

using System.Collections.Concurrent;

public interface IThesisInterestService
{
    Task<IReadOnlyList<PersistedTopicInterest>> GetInterestsAsync(Guid? topicId = null, CancellationToken cancellationToken = default);

    Task<bool> RecordInterestAsync(
        Guid topicId,
        string topicTitle,
        string studentEmail,
        string professorEmail,
        CancellationToken cancellationToken = default);

    Task<bool> HasInterestAsync(Guid topicId, string studentEmail, CancellationToken cancellationToken = default);

    Task<bool> RemoveInterestAsync(Guid topicId, string studentEmail, CancellationToken cancellationToken = default);

    /// <summary>Removes all recorded interests for the given topic (e.g. when the topic is deleted).</summary>
    Task ClearTopicInterestsAsync(Guid topicId, CancellationToken cancellationToken = default);
}

public sealed record PersistedTopicInterest(
    Guid TopicId,
    string TopicTitle,
    string StudentEmail,
    string ProfessorEmail,
    DateTime ExpressedOn);

public sealed class ThesisInterestService : IThesisInterestService
{
    private readonly ConcurrentDictionary<Guid, ConcurrentDictionary<string, PersistedTopicInterest>> _interests = new();

    public Task<IReadOnlyList<PersistedTopicInterest>> GetInterestsAsync(Guid? topicId = null, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<PersistedTopicInterest> interests = topicId.HasValue
            ? _interests.TryGetValue(topicId.Value, out var topicInterests)
                ? topicInterests.Values.OrderByDescending(i => i.ExpressedOn).ToList()
                : Array.Empty<PersistedTopicInterest>()
            : _interests.Values
                .SelectMany(topicInterests => topicInterests.Values)
                .OrderByDescending(i => i.ExpressedOn)
                .ToList();

        return Task.FromResult(interests);
    }

    public Task<bool> RecordInterestAsync(
        Guid topicId,
        string topicTitle,
        string studentEmail,
        string professorEmail,
        CancellationToken cancellationToken = default)
    {
        if (topicId == Guid.Empty || string.IsNullOrWhiteSpace(topicTitle) || string.IsNullOrWhiteSpace(studentEmail))
        {
            return Task.FromResult(false);
        }

        var topicInterests = _interests.GetOrAdd(topicId, _ => new ConcurrentDictionary<string, PersistedTopicInterest>(StringComparer.OrdinalIgnoreCase));
        var normalizedStudentEmail = NormalizeEmail(studentEmail);

        topicInterests[normalizedStudentEmail] = new PersistedTopicInterest(
            topicId,
            topicTitle.Trim(),
            studentEmail.Trim(),
            professorEmail.Trim(),
            DateTime.UtcNow);

        return Task.FromResult(true);
    }

    public Task<bool> HasInterestAsync(Guid topicId, string studentEmail, CancellationToken cancellationToken = default)
    {
        if (topicId == Guid.Empty || string.IsNullOrWhiteSpace(studentEmail))
        {
            return Task.FromResult(false);
        }

        return Task.FromResult(
            _interests.TryGetValue(topicId, out var topicInterests) &&
            topicInterests.ContainsKey(NormalizeEmail(studentEmail)));
    }

    public Task<bool> RemoveInterestAsync(Guid topicId, string studentEmail, CancellationToken cancellationToken = default)
    {
        if (topicId == Guid.Empty || string.IsNullOrWhiteSpace(studentEmail))
        {
            return Task.FromResult(false);
        }

        if (!_interests.TryGetValue(topicId, out var topicInterests))
        {
            return Task.FromResult(false);
        }

        var removed = topicInterests.TryRemove(NormalizeEmail(studentEmail), out _);
        if (topicInterests.IsEmpty)
        {
            _interests.TryRemove(topicId, out _);
        }

        return Task.FromResult(removed);
    }

    public Task ClearTopicInterestsAsync(Guid topicId, CancellationToken cancellationToken = default)
    {
        _interests.TryRemove(topicId, out _);
        return Task.CompletedTask;
    }

    private static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();
}