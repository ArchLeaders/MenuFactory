﻿using Avalonia.Controls;
using Avalonia.Input;
using CommunityToolkit.Mvvm.Input;
using MenuFactory.Abstractions;
using MenuFactory.Abstractions.Attributes;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Reflection;
using System.Windows.Input;
using Icon = Projektanker.Icons.Avalonia.Icon;

namespace MenuFactory;

public class AvaloniaMenuFactory(InputElement? visualRoot, Func<string, string>? getTranslationResource) : IMenuFactory
{
    private const BindingFlags ALL_BINDING_FLAGS = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
    private const string MENU_ITEM_STYLE = "MenuFactory-MenuItem";
    private const string TOP_LEVEL_MENU_ITEM_STYLE = "MenuFactory-TopLevel";

    private static readonly Func<string, string> _defaultGetTranslationResource = static (str) => str;

    private readonly InputElement? _visualRoot = visualRoot;
    private readonly Func<string, string> _getTranslationResource = getTranslationResource ?? _defaultGetTranslationResource;
    private readonly Dictionary<string, List<MenuItem>> _groups = [];
    private readonly Dictionary<string, MenuItem> _cache = [];
    private readonly ObservableCollection<Control> _items = [];

    public IEnumerable Items => _items;

    public AvaloniaMenuFactory() : this(null, null)
    {
    }

    public AvaloniaMenuFactory(InputElement visualRoot) : this(visualRoot, null)
    {
    }

    public void AddMenuGroup<T>(string? groupId = null, object? source = null)
    {
        Type type = typeof(T);
        groupId ??= type.Name;

        IEnumerable<(MethodInfo, MenuAttribute)> attributes = type
            .GetMethods(ALL_BINDING_FLAGS)
            .Select(x => (MethodInfo: x, Attribute: x.GetCustomAttribute<MenuAttribute>()!))
            .Where(x => x.MethodInfo is not null && x.Attribute is not null);

        if (!_groups.TryGetValue(groupId, out List<MenuItem>? groupItems)) {
            _groups[groupId] = groupItems = [];
        }

        foreach (var (info, attribute) in attributes) {
            groupItems.Add(
                BuildMenuItemFromAttribute(type, info, attribute, source)
            );
        }
    }

    public bool RemoveMenuGroup(string groupId)
    {
        if (!_groups.TryGetValue(groupId, out List<MenuItem>? items)) {
            return false;
        }

        for (int i = 0; i < items.Count; i++) {
            ItemsControl item = items[i];

        RemoveFromParent:
            if (item is MenuItem menuItem && _visualRoot?.KeyBindings.FirstOrDefault(x => x.Command == menuItem.Command) is KeyBinding keyBinding) {
                _visualRoot.KeyBindings.Remove(keyBinding);
            }

            if (_items.Remove(item)) {
                continue;
            }

            if (item.Tag is ItemsControl parent && parent.ItemsSource is IList itemsSource) {
                itemsSource.Remove(item);

                if (parent.ItemCount == 0 && parent.Name is not null) {
                    _cache.Remove(parent.Name);
                    item = parent;
                    goto RemoveFromParent;
                }
            }
        }

        return _groups.Remove(groupId);
    }

    private MenuItem BuildMenuItemFromAttribute(Type type, MethodInfo info, MenuAttribute attribute, object? source)
    {
        int parameterCount = info.GetParameters().Length;
        if (info.GetParameters().Length > 2) {
            throw new Exception(
                $"The target menu method '{info.Name}' has too many parameters."
            );
        }

        if (info.ReturnType.IsAssignableTo(typeof(ValueTask<>))) {
            throw new Exception(
                $"The target menu method '{info.Name}' has an invalid return type (likely ValueTask<T>, use Task<T> instead)."
            );
        }

        AsyncRelayCommand<object> command = new(async (x) => {
            object? result = info.Invoke(source, parameterCount == 1 ? [x] : null);

            if (result is Task task) {
                await task;
                return;
            }

            if (result is ValueTask valueTask) {
                await valueTask;
                return;
            }
        });

        KeyGesture? inputGesture = string.IsNullOrEmpty(attribute.InputGesture)
            ? null : KeyGesture.Parse(attribute.InputGesture);

        MenuItem result = new() {
            Name = attribute.Name,
            Header = _getTranslationResource(attribute.Name),
            Classes = {
                MENU_ITEM_STYLE
            },
            Icon = new Icon {
                Value = attribute.Icon ?? string.Empty
            }
        };

        if (attribute.GetCollectionMethodName is not null && type.GetMethod(attribute.GetCollectionMethodName, ALL_BINDING_FLAGS)?.Invoke(source, null) is IEnumerable enumerable) {
            ObservableCollection<MenuItem> menuItems = [];
            result.ItemsSource = menuItems;
            BuildMenuItems(enumerable, menuItems, command);

            if (enumerable is INotifyCollectionChanged collection) {
                collection.CollectionChanged += (s, e) => {
                    if (e.NewItems is not null) {
                        BuildMenuItems(e.NewItems, menuItems, command);
                    }
                };
            }

            goto RegisterItem;
        }

        result.InputGesture = inputGesture;
        result.Command = command;
        result.CommandParameter = result;

        if (inputGesture is not null && _visualRoot is not null) {
            _visualRoot.KeyBindings.Add(new KeyBinding {
                Gesture = inputGesture,
                Command = command,
                CommandParameter = result
            });
        }

    RegisterItem:
        MenuItem? parent = BuildMenuItemPath(attribute.Path);
        IList targetItemCollection = parent switch {
            not null => (IList)parent.ItemsSource!,
            null => _items
        };

        if (attribute.IsSeparator) {
            targetItemCollection.Add(
                new Separator()
            );
        }

        result.Tag = parent;
        targetItemCollection.Add(result);

        return result;
    }

    private MenuItem? BuildMenuItemPath(string path)
    {
        IEnumerable<PathPart> parts = path
            .Split('/')
            .Select(x => new PathPart(x))
            .Reverse();

        MenuItem? deepestMenuItem = null;
        MenuItem? highestMenuItem = null;
        bool isHighestMenuItemSeparator = false;

        foreach (PathPart part in parts) {
            if (!_cache.TryGetValue(part.Name, out MenuItem? item)) {
                item = _cache[part.Name] = new() {
                    Name = part.Name,
                    Header = _getTranslationResource(part.Name),
                    Icon = new Icon {
                        Value = part.Icon ?? string.Empty
                    },
                    ItemsSource = new ObservableCollection<object>()
                };
            }

            if (highestMenuItem is not null && !item.Items.Contains(highestMenuItem) && item.ItemsSource is IList itemSource) {
                if (isHighestMenuItemSeparator) {
                    itemSource.Add(new Separator());
                }

                highestMenuItem.Tag = item;
                itemSource.Add(highestMenuItem);
            }

            isHighestMenuItemSeparator = part.IsSeparator;
            highestMenuItem = item;
            deepestMenuItem ??= item;
        }

        if (highestMenuItem is not null && !_items.Contains(highestMenuItem)) {
            _items.Add(highestMenuItem);
            highestMenuItem?.Classes.Add(TOP_LEVEL_MENU_ITEM_STYLE);
        }

        return deepestMenuItem;
    }

    private static void BuildMenuItems(IEnumerable source, ObservableCollection<MenuItem> menuItems, ICommand command)
    {
        foreach (object item in source) {
            if (item is MenuItem menuItem) {
                menuItem.Command = command;
                menuItem.CommandParameter = menuItem;
                menuItem.Classes.Add(MENU_ITEM_STYLE);
                menuItems.Add(menuItem);
                continue;
            }

            menuItems.Add(new MenuItem {
                Header = item.ToString(),
                Command = command,
                CommandParameter = item,
                Classes = {
                    MENU_ITEM_STYLE
                },
            });
        }
    }

    private class PathPart
    {
        public string Name { get; }
        public string? Icon { get; }
        public bool IsSeparator { get; } = false;

        public PathPart(string input)
        {
            ReadOnlySpan<char> inputSpan = input.AsSpan();

            int paramsStartIndex = inputSpan.IndexOf("$:");
            if (paramsStartIndex == -1) {
                Name = input;
                return;
            }

            Name = input[..paramsStartIndex];
            paramsStartIndex += 2;

            const string ICON_KEYWORD = "Icon";
            const string IS_SEPARATOR_KEYWORD = "IsSeparator";

        MoveNext:
            if (inputSpan.Length <= paramsStartIndex) {
                return;
            }

            int nextCommaIndex = inputSpan[paramsStartIndex..].IndexOf(',');
            int parameterValueEndIndex = nextCommaIndex switch {
                -1 => inputSpan.Length,
                _ => paramsStartIndex + nextCommaIndex
            };

            if (inputSpan[paramsStartIndex..(paramsStartIndex + ICON_KEYWORD.Length)] is ICON_KEYWORD) {
                Icon = input[(paramsStartIndex + ICON_KEYWORD.Length + 1)..parameterValueEndIndex];
                goto MoveNextSetup;
            }

            if (inputSpan[paramsStartIndex..(paramsStartIndex + IS_SEPARATOR_KEYWORD.Length)] is IS_SEPARATOR_KEYWORD) {
                IsSeparator = bool.Parse(
                    input[(paramsStartIndex + IS_SEPARATOR_KEYWORD.Length + 1)..parameterValueEndIndex]
                );
                goto MoveNextSetup;
            }

        MoveNextSetup:
            paramsStartIndex = parameterValueEndIndex + 1;
            goto MoveNext;
        }
    }
}
