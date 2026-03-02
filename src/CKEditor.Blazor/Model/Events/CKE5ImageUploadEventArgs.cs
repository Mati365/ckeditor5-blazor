namespace CKEditor.Blazor.Model.Events;

/// <summary>
/// Event arguments carried by the <see cref="Components.CKE5Editor.OnImageUpload"/> callback
/// when the user inserts / uploads an image through the editor's file repository.
/// </summary>
/// <param name="FileName">
/// Original file name as reported by the browser (e.g. <c>photo.jpg</c>).
/// </param>
/// <param name="MimeType">
/// MIME type of the uploaded file (e.g. <c>image/jpeg</c>).
/// </param>
/// <param name="Payload">
/// Raw binary content of the file as a Base64-encoded string.
/// Decode with <see cref="Convert.FromBase64String"/> when you need raw bytes.
/// </param>
public sealed record CKE5ImageUploadEventArgs(string FileName, string MimeType, string Payload);
