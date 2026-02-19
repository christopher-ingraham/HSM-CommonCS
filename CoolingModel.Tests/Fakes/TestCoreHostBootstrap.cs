using System.Reflection;
using HSM_CommonCS.Core;
using HSM_CommonCS.Database;
using HSM_CommonCS.Messaging;

namespace CoolingModel.Tests.Fakes;

/// <summary>
/// Bootstraps CoreHost with fake dependencies for testing.
///
/// CoreHost is a singleton with a private constructor that normally requires
/// Oracle + RabbitMQ connections. This helper uses reflection to inject fakes
/// so that service classes (which inherit from AppService and use CoreHost.Log
/// and CoreHost.Db) work in a test environment.
///
/// USAGE:
///   TestCoreHostBootstrap.Initialize(fakeLog, fakeDb, fakeBus);
///   // ... run tests ...
///   TestCoreHostBootstrap.Reset();
///
/// NOTE: Since CoreHost is a static singleton, tests using this MUST NOT run in parallel.
/// Use [Collection("CoreHost")] to serialize test classes that depend on CoreHost.
/// </summary>
public static class TestCoreHostBootstrap
{
    private static readonly object _lock = new();
    private static bool _initialized;

    /// <summary>
    /// Initialize CoreHost singleton with fake dependencies.
    /// </summary>
    public static void Initialize(
        ILog? log = null,
        IDbSessionFactory? db = null,
        IMessageBus? bus = null)
    {
        lock (_lock)
        {
            // Reset first if already initialized
            if (_initialized) Reset();

            var coreHostType = typeof(CoreHost);

            // Create an instance using reflection (bypassing private constructor)
            var instance = System.Runtime.Serialization.FormatterServices
                .GetUninitializedObject(coreHostType);

            // Set the private fields
            SetPrivateField(instance, "_log", log ?? new FakeLog());
            SetPrivateField(instance, "_db", db ?? new FakeDbSessionFactory());
            SetPrivateField(instance, "_bus", bus ?? new FakeMessageBus());
            SetPrivateField(instance, "_applicationName", "TEST");
            SetPrivateField(instance, "_disposed", false);

            // Set the static _instance field
            var instanceField = coreHostType.GetField("_instance",
                BindingFlags.NonPublic | BindingFlags.Static);
            instanceField!.SetValue(null, instance);

            _initialized = true;
        }
    }

    /// <summary>
    /// Reset the CoreHost singleton so it can be re-initialized.
    /// Call this in test cleanup / Dispose.
    /// </summary>
    public static void Reset()
    {
        lock (_lock)
        {
            var coreHostType = typeof(CoreHost);
            var instanceField = coreHostType.GetField("_instance",
                BindingFlags.NonPublic | BindingFlags.Static);
            instanceField!.SetValue(null, null);
            _initialized = false;
        }
    }

    private static void SetPrivateField(object obj, string fieldName, object? value)
    {
        var field = obj.GetType().GetField(fieldName,
            BindingFlags.NonPublic | BindingFlags.Instance);
        if (field == null)
            throw new InvalidOperationException(
                $"Field '{fieldName}' not found on {obj.GetType().Name}. " +
                "CoreHost internals may have changed - update this bootstrap.");
        field.SetValue(obj, value);
    }
}
