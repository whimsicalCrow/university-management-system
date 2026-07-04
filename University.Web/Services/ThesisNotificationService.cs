namespace University.Web.Services;

using System.Collections.Concurrent;

public sealed record ThesisInterestNotification(
    Guid Id,
    string ProfessorEmail,
    string StudentEmail,
    string TopicTitle,
    DateTime At,
    bool IsRead);

public interface IThesisNotificationService
{
    /// <summary>Raised on any state change so subscribers can refresh counts.</summary>
    event Action? Changed;

    Task AddAsync(string professorEmail, string studentEmail, string topicTitle);
    Task<IReadOnlyList<ThesisInterestNotification>> GetForProfessorAsync(string professorEmail);
    Task<int> GetUnreadCountAsync(string professorEmail);
    Task MarkAllReadAsync(string professorEmail);
}

/// <summary>
/// Singleton in-memory notification store for thesis-interest events.
/// Each time a student expresses interest the professor receives a notification
/// that persists for the lifetime of the application process.
/// </summary>
public sealed class ThesisNotificationService : IThesisNotificationService
{
    // outer key: normalized professor email; inner key: notification id
    private readonly ConcurrentDictionary<string, ConcurrentDictionary<Guid, ThesisInterestNotification>>
        _store = new(StringComparer.OrdinalIgnoreCase);

    public event Action? Changed;

    public Task AddAsync(string professorEmail, string studentEmail, string topicTitle)
    {
        if (string.IsNullOrWhiteSpace(professorEmail) || string.IsNullOrWhiteSpace(studentEmail))
            return Task.CompletedTask;

        var key = Normalize(professorEmail);
        var bucket = _store.GetOrAdd(key, _ => new ConcurrentDictionary<Guid, ThesisInterestNotification>());

        var id = Guid.NewGuid();
        bucket[id] = new ThesisInterestNotification(
            id,
            professorEmail.Trim(),
            studentEmail.Trim(),
            topicTitle.Trim(),
            DateTime.UtcNow,
            IsRead: false);

        Changed?.Invoke();
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<ThesisInterestNotification>> GetForProfessorAsync(string professorEmail)
    {
        if (string.IsNullOrWhiteSpace(professorEmail))
            return Task.FromResult<IReadOnlyList<ThesisInterestNotification>>(Array.Empty<ThesisInterestNotification>());

        if (!_store.TryGetValue(Normalize(professorEmail), out var bucket))
            return Task.FromResult<IReadOnlyList<ThesisInterestNotification>>(Array.Empty<ThesisInterestNotification>());

        IReadOnlyList<ThesisInterestNotification> result = bucket.Values
            .OrderByDescending(n => n.At)
            .ToList();

        return Task.FromResult(result);
    }

    public Task<int> GetUnreadCountAsync(string professorEmail)
    {
        if (string.IsNullOrWhiteSpace(professorEmail))
            return Task.FromResult(0);

        if (!_store.TryGetValue(Normalize(professorEmail), out var bucket))
            return Task.FromResult(0);

        return Task.FromResult(bucket.Values.Count(n => !n.IsRead));
    }

    public Task MarkAllReadAsync(string professorEmail)
    {
        if (string.IsNullOrWhiteSpace(professorEmail))
            return Task.CompletedTask;

        if (!_store.TryGetValue(Normalize(professorEmail), out var bucket))
            return Task.CompletedTask;

        foreach (var id in bucket.Keys.ToList())
        {
            if (bucket.TryGetValue(id, out var n) && !n.IsRead)
                bucket[id] = n with { IsRead = true };
        }

        Changed?.Invoke();
        return Task.CompletedTask;
    }

    private static string Normalize(string value) => value.Trim().ToLowerInvariant();
}
