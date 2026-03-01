using CKEditor.Blazor.Components;
using Microsoft.JSInterop;
using Microsoft.JSInterop.Infrastructure;
using Moq;

namespace CKEditor.Blazor.Tests.Components;

public class CKE5ComponentJsInteropTests
{
    private readonly Mock<IJSRuntime> _jsRuntimeMock = new();
    private readonly Mock<IJSObjectReference> _jsModuleMock = new();
    private readonly Mock<IJSObjectReference> _jsInteropMock = new();

    private void SetupInitialize(string factoryFunctionName = "createEditorBlazorInterop")
    {
        _jsRuntimeMock
            .Setup(r => r.InvokeAsync<IJSObjectReference>(
                "import", It.IsAny<object?[]>()))
            .ReturnsAsync(_jsModuleMock.Object);

        _jsModuleMock
            .Setup(m => m.InvokeAsync<IJSObjectReference>(
                factoryFunctionName, It.IsAny<object?[]>()))
            .ReturnsAsync(_jsInteropMock.Object);
    }

    [Fact]
    public void IsInitializing_IsTrueByDefault()
    {
        var interop = new CKE5ComponentJsInterop();

        Assert.True(interop.IsInitializing);
    }

    [Fact]
    public async Task InitializeAsync_SetsIsInitializingToFalse()
    {
        SetupInitialize();
        var interop = new CKE5ComponentJsInterop();

        await interop.InitializeAsync(_jsRuntimeMock.Object, "createEditorBlazorInterop", default, null);

        Assert.False(interop.IsInitializing);
    }

    [Fact]
    public async Task InitializeAsync_ImportsModule_WithCkeditor5BlazorIdentifier()
    {
        SetupInitialize();
        var interop = new CKE5ComponentJsInterop();

        await interop.InitializeAsync(_jsRuntimeMock.Object, "createEditorBlazorInterop", default, null);

        _jsRuntimeMock.Verify(r => r.InvokeAsync<IJSObjectReference>(
            "import",
            It.Is<object?[]>(args => args.Length == 1 && (string)args[0]! == "ckeditor5-blazor")),
            Times.Once);
    }

    [Fact]
    public async Task InitializeAsync_InvokesFactoryFunction_WithElementAndDotNetHelper()
    {
        const string factoryFunction = "createEditableBlazorInterop";
        SetupInitialize(factoryFunction);
        var interop = new CKE5ComponentJsInterop();
        var dotNetHelper = new object();

        await interop.InitializeAsync(_jsRuntimeMock.Object, factoryFunction, default, dotNetHelper);

        _jsModuleMock.Verify(m => m.InvokeAsync<IJSObjectReference>(
            factoryFunction,
            It.Is<object?[]>(args => args.Length == 2 && args[1] == dotNetHelper)),
            Times.Once);
    }

    [Fact]
    public async Task InitializeAsync_InvokesFactoryFunction_WithNullDotNetHelper()
    {
        SetupInitialize();
        var interop = new CKE5ComponentJsInterop();

        await interop.InitializeAsync(_jsRuntimeMock.Object, "createEditorBlazorInterop", default, null);

        _jsModuleMock.Verify(m => m.InvokeAsync<IJSObjectReference>(
            "createEditorBlazorInterop",
            It.Is<object?[]>(args => args.Length == 2 && args[1] == null)),
            Times.Once);
    }

    [Fact]
    public async Task InvokeVoidAsync_ThrowsInvalidOperationException_WhenNotInitialized()
    {
        var interop = new CKE5ComponentJsInterop();

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            interop.InvokeVoidAsync("someMethod"));

        Assert.Contains("someMethod", ex.Message);
    }

    [Fact]
    public async Task InvokeVoidAsync_DelegatesToJsInterop_WhenInitialized()
    {
        SetupInitialize();
        var interop = new CKE5ComponentJsInterop();
        await interop.InitializeAsync(_jsRuntimeMock.Object, "createEditorBlazorInterop", default, null);

        _jsInteropMock
            .Setup(i => i.InvokeAsync<IJSVoidResult>("setValue", It.IsAny<object?[]>()))
            .ReturnsAsync(Mock.Of<IJSVoidResult>());

        await interop.InvokeVoidAsync("setValue", "arg1", 42);

        _jsInteropMock.Verify(i => i.InvokeAsync<IJSVoidResult>(
            "setValue",
            It.Is<object?[]>(args => args.Length == 2 && (string)args[0]! == "arg1" && (int)args[1]! == 42)),
            Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_ThrowsInvalidOperationException_WhenNotInitialized()
    {
        var interop = new CKE5ComponentJsInterop();

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            interop.InvokeAsync<string>("getValue"));

        Assert.Contains("getValue", ex.Message);
    }

    [Fact]
    public async Task InvokeAsync_ReturnsValueFromJsInterop_WhenInitialized()
    {
        SetupInitialize();
        var interop = new CKE5ComponentJsInterop();
        await interop.InitializeAsync(_jsRuntimeMock.Object, "createEditorBlazorInterop", default, null);

        _jsInteropMock
            .Setup(i => i.InvokeAsync<string>("getData", It.IsAny<object?[]>()))
            .ReturnsAsync("<p>Hello</p>");

        var result = await interop.InvokeAsync<string>("getData");

        Assert.Equal("<p>Hello</p>", result);
    }

    [Fact]
    public async Task DisposeAsync_DoesNotThrow_WhenNotInitialized()
    {
        var interop = new CKE5ComponentJsInterop();

        await interop.DisposeAsync();
    }

    [Fact]
    public async Task DisposeAsync_CallsUnmountOnInterop_WhenInitialized()
    {
        SetupInitialize();
        var interop = new CKE5ComponentJsInterop();
        await interop.InitializeAsync(_jsRuntimeMock.Object, "createEditorBlazorInterop", default, null);

        _jsInteropMock
            .Setup(i => i.InvokeAsync<IJSVoidResult>("unmount", It.IsAny<object?[]>()))
            .ReturnsAsync(Mock.Of<IJSVoidResult>());
        _jsInteropMock.Setup(i => i.DisposeAsync()).Returns(ValueTask.CompletedTask);
        _jsModuleMock.Setup(m => m.DisposeAsync()).Returns(ValueTask.CompletedTask);

        await interop.DisposeAsync();

        _jsInteropMock.Verify(i => i.InvokeAsync<IJSVoidResult>(
            "unmount", It.IsAny<object?[]>()), Times.Once);
    }

    [Fact]
    public async Task DisposeAsync_DisposesInteropAndModule_WhenInitialized()
    {
        SetupInitialize();
        var interop = new CKE5ComponentJsInterop();
        await interop.InitializeAsync(_jsRuntimeMock.Object, "createEditorBlazorInterop", default, null);

        _jsInteropMock
            .Setup(i => i.InvokeAsync<IJSVoidResult>("unmount", It.IsAny<object?[]>()))
            .ReturnsAsync(Mock.Of<IJSVoidResult>());
        _jsInteropMock.Setup(i => i.DisposeAsync()).Returns(ValueTask.CompletedTask);
        _jsModuleMock.Setup(m => m.DisposeAsync()).Returns(ValueTask.CompletedTask);

        await interop.DisposeAsync();

        _jsInteropMock.Verify(i => i.DisposeAsync(), Times.Once);
        _jsModuleMock.Verify(m => m.DisposeAsync(), Times.Once);
    }

    [Fact]
    public async Task DisposeAsync_SwallowsJSDisconnectedException()
    {
        SetupInitialize();
        var interop = new CKE5ComponentJsInterop();
        await interop.InitializeAsync(_jsRuntimeMock.Object, "createEditorBlazorInterop", default, null);

        _jsInteropMock
            .Setup(i => i.InvokeAsync<IJSVoidResult>("unmount", It.IsAny<object?[]>()))
            .ThrowsAsync(new JSDisconnectedException("disconnected"));

        await interop.DisposeAsync();
    }

    [Fact]
    public async Task DisposeAsync_SwallowsTaskCanceledException()
    {
        SetupInitialize();
        var interop = new CKE5ComponentJsInterop();
        await interop.InitializeAsync(_jsRuntimeMock.Object, "createEditorBlazorInterop", default, null);

        _jsInteropMock
            .Setup(i => i.InvokeAsync<IJSVoidResult>("unmount", It.IsAny<object?[]>()))
            .ThrowsAsync(new TaskCanceledException());

        await interop.DisposeAsync();
    }
}
