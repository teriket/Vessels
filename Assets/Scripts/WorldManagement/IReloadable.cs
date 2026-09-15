using UnityEngine;

/// <summary>
/// Interface to manage reloading of objects that require blocking operations
/// </summary>
public interface IReloadable
{
    public void OnReload();
}
