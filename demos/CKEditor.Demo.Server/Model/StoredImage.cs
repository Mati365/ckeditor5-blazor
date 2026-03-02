namespace CKEditor.Demo.Server.Model;

/// <summary>Immutable value representing a single stored image.</summary>
public sealed record StoredImage(string FileName, string MimeType, byte[] Data);
