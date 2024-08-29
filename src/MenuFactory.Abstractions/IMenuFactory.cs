using System.Collections;

namespace MenuFactory.Abstractions;

public interface IMenuFactory
{
    IEnumerable Items { get; }

    /// <summary>
    /// Add a new menu group from a menu model.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="groupId">The ID of the group to add.</param>
    /// <param name="source">An instance of <typeparamref name="T"/> for non-static methods.</param>
    void AddMenuGroup<T>(string? groupId = null, object? source = null);

    /// <inheritdoc cref="AddMenuGroup{T}(string?, object?)"/>
    void AddMenuGroup<T>(T source) => AddMenuGroup<T>(typeof(T).Name, source);

    /// <summary>
    /// Remove an existing menu group.
    /// </summary>
    /// <param name="groupId">The ID of the group to remove.</param>
    /// <returns><see langword="true"/> is the group was found, <see langword="false"/> is the <paramref name="groupId"/> could not be found.</returns>
    bool RemoveMenuGroup(string groupId);

    /// <inheritdoc cref="RemoveMenuGroup(string)"/>
    bool RemoveMenuGroup<T>(string? groupId = null) => RemoveMenuGroup(groupId ?? typeof(T).Name);
}
