namespace InventoryManager.Network;

/// <summary>An error whose message is already written for the end user (shown as-is in the UI).</summary>
public sealed class NetworkException : Exception
{
    public NetworkException(string message, Exception? inner = null) : base(message, inner) { }
}
