namespace CKEditor.Blazor.Cloud.Bundle;

/// <summary>
/// Represents a bundle of cloud assets (JavaScript and CSS).
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="AssetsBundle"/> class.
/// </remarks>
/// <param name="js">The JavaScript assets.</param>
/// <param name="css">The CSS asset URLs.</param>
public class AssetsBundle(IReadOnlyList<JSAsset> js, IReadOnlyList<string> css)
{
    /// <summary>
    /// The JavaScript assets in this bundle.
    /// </summary>
    public List<JSAsset> Js { get; } = [.. js];

    /// <summary>
    /// The CSS asset URLs in this bundle.
    /// </summary>
    public List<string> Css { get; } = [.. css];

    /// <summary>
    /// Creates a new bundle by merging this bundle with another one.
    /// </summary>
    /// <param name="other">The bundle to merge.</param>
    /// <returns>The merged bundle.</returns>
    public AssetsBundle Merge(AssetsBundle other)
    {
        var js = new List<JSAsset>(Js);
        js.AddRange(other.Js);

        var css = new List<string>(Css);
        css.AddRange(other.Css);

        return new AssetsBundle(js, css);
    }
}
